using System;
using System.Data;
using System.Security;
using System.Security.Principal;
using Habanero.Base;
using Habanero.BO.SqlGeneration;
using Habanero.DB;

namespace Habanero.BO.ConcurrencyControl
{
    internal class PessimisticLockingDB : IConcurrencyControl
    {
        //private enum VerificationStage
        //{
        //    BeforeBeginEdit,
        //    BeforePersist
        //}

        private readonly BusinessObject _busObj;
        private readonly int _lockDurationInMinutes;
        private readonly BOProp _boPropDateLocked;
        private readonly BOProp _boPropUserLocked;
        private readonly BOProp _boPropMachineLocked;
        private readonly BOProp _boPropOperatingSystemUser;
        private readonly BOProp _boPropLocked;


        /// <summary>
        /// Constructor to initialise a new instance of pessimistic locking with the property details
        /// form implementing the pessimistic locking strategy.
        /// </summary>
        /// <param name="busObj">The business object on which to perform concurrency control</param>
        /// <param name="lockDurationInMinutes">The period of time that the lock will be maintained</param>
        /// <param name="boPropDateTimeLocked">The date that the object was locked</param>
        /// <param name="boPropUserLocked">The user that locked the object</param>
        /// <param name="boPropMachineLocked">The machine name on which the object was last updated</param>
        /// <param name="boPropOperatingSystemUser">The Windows logged on user who locked the object</param>
        /// <param name="boPropLocked">The property that determines whether the object is locked or not</param>
        public PessimisticLockingDB(BusinessObject busObj, int lockDurationInMinutes, BOProp boPropDateTimeLocked, BOProp boPropUserLocked, BOProp boPropMachineLocked, BOProp boPropOperatingSystemUser, BOProp boPropLocked)
        {
            _busObj = busObj;
            _lockDurationInMinutes = lockDurationInMinutes;
            _boPropDateLocked = boPropDateTimeLocked;
            _boPropUserLocked = boPropUserLocked;
            _boPropMachineLocked = boPropMachineLocked;
            _boPropOperatingSystemUser = boPropOperatingSystemUser;
            _boPropLocked = boPropLocked;
        }

        /// <summary>
        /// Checks concurrency before persisting an object to the database
        /// in order to prevent one of two conflicting copies from being lost.
        /// Throws an exception if the object has been locked by another
        /// process/user, as determined by a property locked
        /// The object is persisted by calling 
        /// <see cref= "UpdatePropertiesWithLatestConcurrencyInfoBeforePersisting"/>.
        /// </summary>
        /// <exception cref="BusObjDeleteConcurrencyControlException">Thrown if 
        /// the object has been deleted by another process/user</exception>
        /// <exception cref="BusObjOptimisticConcurrencyControlException">Thrown 
        /// if the object has been edited by another process/user</exception>
        public void CheckConcurrencyBeforePersisting()
        {
            if (IsLocked() && LockDurationExceeded())
            {
                throw new BusObjPessimisticConcurrencyControlException(
                        string.Format(
                        "The lock on the business object {0} has a duration of {1} minutes and has been exceeded for the object {2}"
                        ,_busObj.ClassName,this._lockDurationInMinutes,_busObj.ID));
            }
        }

        private bool LockDurationExceeded()
        {
            return !LockDurationValid(DateTimeLocked());
        }

        private DateTime DateTimeLocked()
        {
            return Convert.ToDateTime(this._boPropDateLocked.Value);
        }

        private bool IsLocked()
        {
            return Convert.ToBoolean(this._boPropLocked.Value);
        }

        /// <summary>
        /// Checks concurrency before the user begins editing an object, in
        /// order to avoid the user making changes to an object and then losing
        /// those changes when the committal process shows a concurrency
        /// failure
        /// </summary>
        /// <exception cref="BusObjBeginEditConcurrencyControlException">Thrown
        /// if the object has been edited in the database since it was last
        /// loaded by the object manager</exception>
        public void CheckConcurrencyBeforeBeginEditing()
        {
            if (!_busObj.State.IsNew) //you cannot have concurrency control issues on a new object 
            // all you can have is duplicate data issues.
            {
                using (IDataReader dr = BOLoader.Instance.LoadDataReader(_busObj, _busObj.GetDatabaseConnection(), null))
                {
                    // If this object no longer exists in the database
                    // then we have a concurrency conflict since it has been deleted by another process.
                    // If our objective was to delete it as well then no worries else throw error.
                    bool drHasData = dr.Read();
                    if (!(drHasData))
                    {
                        //The object you are trying to edit has been deleted by another user.
                        throw new BusObjDeleteConcurrencyControlException(_busObj.ClassName, 
                                _busObj.ID.ToString(), _busObj);
                    }
                    bool locked;
                    if (dr[_boPropLocked.DatabaseFieldName] == DBNull.Value)
                    {
                        locked = false;
                    }
                    else
                    {
                        locked = Convert.ToBoolean(dr[_boPropLocked.DatabaseFieldName]);
                    }
                    DateTime dateLocked = CastToDateTime(dr, this._boPropDateLocked.DatabaseFieldName);
                    if (locked && (LockDurationValid(dateLocked)))
                    {
                        string userLocked = (string)dr[this._boPropUserLocked.DatabaseFieldName];
                        string machineLocked = (string)dr[this._boPropMachineLocked.DatabaseFieldName];
                        throw new BusObjPessimisticConcurrencyControlException(_busObj.ClassName, userLocked,
                                                                               machineLocked, dateLocked,
                                                                               this._busObj.ID.ToString(),
                                                                               this._busObj);
                    }
                }
                _boPropLocked.Value = true;
                SetOperatingSystemUser();
                SetMachineName();
                SetUserName();
                SetDateTimeLocked();
                UpdateLockingToDB();
                return;
            }
        }

        private bool LockDurationValid(DateTime dateLocked)
        {
            return dateLocked > DateTime.Now.AddMinutes(-1 * this._lockDurationInMinutes);
        }

        private static DateTime CastToDateTime(IDataRecord dr, string fieldName)
        {
            if (dr[fieldName] == DBNull.Value)
            {
                return DateTime.MinValue;
            }
            return Convert.ToDateTime(dr[fieldName]);
        }

        private void UpdateLockingToDB()
        {
            ISqlStatementCollection sql = GetUpdateSql();
            if (sql == null) return;
            DatabaseConnection.CurrentConnection.ExecuteSql(sql);
        }

        private void SetDateTimeLocked()
        {
            if ((this._boPropDateLocked == null)) return;

            _boPropDateLocked.Value = DateTime.Now;
        }

        private void SetOperatingSystemUser()
        {
            if ((_boPropOperatingSystemUser == null)) return;

            try
            {
                _boPropOperatingSystemUser.Value = WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException)
            {
            }
        }

        private void SetMachineName()
        {
            if ((_boPropMachineLocked == null)) return;
            try
            {
                _boPropMachineLocked.Value = Environment.MachineName;
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void SetUserName()
        {
            if ((_boPropUserLocked == null)) return;
            try
            {
                //TODO Temp code possibly integrate with a system to get the user as
                // per custom code for now use the OS user.
                _boPropUserLocked.Value = WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException)
            {
            }
        }
        /// <summary>
        /// Returns an "update" sql statement list for updating this object
        /// </summary>
        /// <returns>Returns a collection of sql statements</returns>
        private SqlStatementCollection GetUpdateSql()
        {
            UpdateStatementGenerator gen = new UpdateStatementGenerator(_busObj, _busObj.GetDatabaseConnection());
            return gen.Generate();
        }

        /// <summary>
        /// Updates the version number, machine name, username and time edited.
        /// The version number is used to determine whether there is a 
        /// concurrency conflict, and the other properties are used for 
        /// reporting of concurrency conflicts if they occur.
        /// </summary>
        public void UpdatePropertiesWithLatestConcurrencyInfoBeforePersisting()
        {
        }

        #region IConcurrencyControl Members
        /// <summary>
        /// Does nothing since optimistic locking is used and no locks are applied
        /// </summary>
        public void ReleaseWriteLocks()
        {
            if (_boPropLocked != null && _boPropLocked.Value != null)
                if ((bool)_boPropLocked.Value)
                {
                    _boPropLocked.Value = false;
                    UpdateLockingToDB();               
                }
        }


        ///<summary>
        /// Makes any changes required to the concurrency control mechanism 
        /// to assert that the transaction has failed and thus been rolled back.
        ///</summary>
        public void UpdateAsTransactionRolledBack()
        {
            //if (_versionNumber.Value == null)
            //{
            //    _versionNumber.Value = 0;
            //    return;
            //}
            //_versionNumber.Value = (int)_versionNumber.Value - 1;
        }

        #endregion
    }
}