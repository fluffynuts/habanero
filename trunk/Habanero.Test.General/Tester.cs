using System;
using System.Collections;
using System.Data;
using Habanero.Bo.ClassDefinition;
using Habanero.Bo.CriteriaManager;
using Habanero.Bo;
using Habanero.DB;
using NUnit.Framework;

namespace Habanero.Test.General
{
    /// <summary>
    /// Summary description for Tester.
    /// </summary>
    [TestFixture]
    public class Tester : TestUsingDatabase
    {
        private BOPrimaryKey deleteContactPersonID;
        private BOPrimaryKey updateContactPersonID;
        private int dbInterval = 1;
        private ContactPerson mContactPersonUpdateConcurrency;
        private ContactPerson mContactPersonDeleteConcurrency;
        private ContactPerson mContactPBeginEditsConcurrency;
        private ContactPerson mCotanctPTestRefreshFromObjMan;
        private ContactPerson mContactPDeleted;
        private ContactPerson mContactPTestSave;

        public Tester()
        {
        }

        /// <summary>
        /// Used by Gui to step through the application. If the reason for failing a test is 
        /// not obvious.
        /// </summary>
        public static void RunTest()
        {
            Tester test = new Tester();
            test.CreateTestPack();
            //	test.TestCreateContactPerson();
            //			test.TestSaveContactPerson();
            //			test.TestEditTwoInstancesContactPerson();
            //			test.TestUpdateExingContactPerson();
            ////			test.TestDeleteContactPerson();
            //			test.TestObjectBeingRemovedFromCollection();
            //			test.TestOptimisticConcurrencyControl();
            //			test.RunTestPropRules();
            //			test.RunPropDefTester();
            //			test.TestMultipleUpdates();
            //			test.TestBeginEditsOnADirtyObject();
            //			test.TestAlwaysGetTheFreshestObject();
            //			test.TestForDuplicateNewObjectsSinglePropKeyNull();
            //			test.TestForDuplicateNewObjectsSinglePropKeyNull();
            test.TestUpdateExistingContactPerson();

            //			test.TestDirtyXml();
            //			test.TestTransactionSuccess();
            test.TestTransactionFail();

            //Run database connection tester
            TestDatabaseConnection dbtester = new TestDatabaseConnection();
            dbtester.SetUpDBCon();
            dbtester.TestExecuteSqlTransaction();
        }

        [TestFixtureSetUp]
        public void CreateTestPack()
        {
            this.SetupDBConnection();
            ContactPerson.DeleteAllContactPeople();

            createUpdatedContactPersonTestPack();

            createDeleteContactPersonTestPack();
            mContactPersonUpdateConcurrency = ContactPerson.GetNewContactPerson();
            mContactPersonUpdateConcurrency.Surname = "Update Concurrency";
            mContactPersonUpdateConcurrency.ApplyEdit();

            mContactPersonDeleteConcurrency = ContactPerson.GetNewContactPerson();
            mContactPersonDeleteConcurrency.Surname = "Delete Concurrency";
            mContactPersonDeleteConcurrency.ApplyEdit();

            mContactPBeginEditsConcurrency = ContactPerson.GetNewContactPerson();
            mContactPBeginEditsConcurrency.Surname = "BeginEdits Concurrency";
            mContactPBeginEditsConcurrency.ApplyEdit();

            mCotanctPTestRefreshFromObjMan = ContactPerson.GetNewContactPerson();
            mCotanctPTestRefreshFromObjMan.Surname = "FirstSurname";
            mCotanctPTestRefreshFromObjMan.ApplyEdit();

            CreateDeletedPersonTestPack();
            CreateSaveContactPersonTestPack();
            //waitForDB();
            //Ensure that a fresh object is loaded from DB
            ContactPerson.ClearContactPersonCol();
        }

        private void CreateSaveContactPersonTestPack()
        {
            mContactPTestSave = ContactPerson.GetNewContactPerson();
            mContactPTestSave.DateOfBirth = new DateTime(1980, 01, 22);
            mContactPTestSave.FirstName = "Brad";
            mContactPTestSave.Surname = "Vincent1";

            mContactPTestSave.ApplyEdit(); //save the object to the DB
        }

        private void CreateDeletedPersonTestPack()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();
            myContact.DateOfBirth = new DateTime(1980, 01, 22);
            myContact.FirstName = "Brad";
            myContact.Surname = "Vincent2";

            myContact.ApplyEdit(); //save the object to the DB
            //waitForDB();
            myContact.Delete();
            myContact.ApplyEdit();

            mContactPDeleted = myContact;
        }

        private void createUpdatedContactPersonTestPack()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();
            myContact.DateOfBirth = new DateTime(1969, 01, 29);
            myContact.FirstName = "FirstName";
            myContact.Surname = "Surname";
            myContact.ApplyEdit();
            updateContactPersonID = myContact.ID;
        }

        private void createDeleteContactPersonTestPack()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();
            myContact.FirstName = "To Be deleted";
            myContact.Surname = "To Be deleted";
            myContact.ApplyEdit();
            deleteContactPersonID = myContact.ID;
        }

//		private void waitForDB() {
//			Thread.Sleep(dbInterval);
//		}

        [Test]
        public void TestUpdateExistingContactPerson()
        {
            ContactPerson myContactPerson = ContactPerson.GetContactPerson(updateContactPersonID);
            myContactPerson.FirstName = "NewFirstName";
            myContactPerson.ApplyEdit();

            //waitForDB();
            ContactPerson.ClearContactPersonCol();
            //Reload the person and make sure that the changes have been made.
            ContactPerson myNewContactPerson = ContactPerson.GetContactPerson(updateContactPersonID);
            Assert.AreEqual("NewFirstName", myNewContactPerson.FirstName,
                            "The firstName was not updated");
        }

        [Test]
        public void TestCreateContactPerson()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();
            Assert.IsNotNull(myContact);
            myContact.DateOfBirth = new DateTime(1980, 01, 22);
            myContact.FirstName = "Brad";
            myContact.Surname = "Vincent3";

            Assert.AreEqual("Brad", myContact.FirstName);
            Assert.AreEqual(new DateTime(1980, 01, 22), myContact.DateOfBirth);
        }

        [Test]
        public void TestSaveContactPerson()
        {
            Assert.IsFalse(mContactPTestSave.IsNew); // this object is saved and thus no longer
            // new

            BOPrimaryKey id = mContactPTestSave.ID; //Save the objectsID so that it can be loaded from the Database
            Assert.AreEqual(id, mContactPTestSave.ID);

            ContactPerson mySecondContactPerson = ContactPerson.GetContactPerson(id);
            Assert.IsFalse(mContactPTestSave.IsNew); // this object is recovered from the DB
            // and is thus not new.
            Assert.AreEqual(mContactPTestSave.ID.ToString(), mySecondContactPerson.ID.ToString());
            Assert.AreEqual(mContactPTestSave.FirstName, mySecondContactPerson.FirstName);
            Assert.AreEqual(mContactPTestSave.DateOfBirth, mySecondContactPerson.DateOfBirth);

            //Add test to make certain that myContact person and contact person are not 
            // pointing at the same physical object

            mContactPTestSave.FirstName = "Change FirstName";
            Assert.IsFalse(mContactPTestSave.FirstName == mySecondContactPerson.FirstName);
        }

        [Test]
        public void TestDeleteFlagsSetContactPerson()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();
            Assert.IsTrue(myContact.IsNew); // this object is new
            myContact.DateOfBirth = new DateTime(1980, 01, 22);
            myContact.FirstName = "Brad";
            myContact.Surname = "Vincent4";

            myContact.ApplyEdit(); //save the object to the DB
            Assert.IsFalse(myContact.IsNew); // this object is saved and thus no longer
            // new
            Assert.IsFalse(myContact.IsDeleted);

            BOPrimaryKey id = myContact.ID; //Save the objectsID so that it can be loaded from the Database
            Assert.AreEqual(id, myContact.ID);
            //Put a loop in to take up some time due to MSAccess 
            myContact.Delete();
            Assert.IsTrue(myContact.IsDeleted);
            myContact.ApplyEdit();
            Assert.IsTrue(myContact.IsDeleted);
            Assert.IsTrue(myContact.IsNew);
        }

        [Test]
        [ExpectedException(typeof (BusinessObjectNotFoundException))]
        public void TestDeleteContactPerson()
        {
            ContactPerson mySecondContactPerson = ContactPerson.GetContactPerson(mContactPDeleted.ID);
        }

        [Test]
        public void TestEditTwoInstancesContactPerson()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();
            myContact.DateOfBirth = new DateTime(1980, 01, 22);
            myContact.FirstName = "Brad";
            myContact.Surname = "Vincent5";

            myContact.ApplyEdit(); //save the object to the DB

            BOPrimaryKey id = myContact.ID; //Save the objectsID so that it can be loaded from the Database
            Assert.AreEqual(id, myContact.ID);

            ContactPerson mySecondContactPerson = ContactPerson.GetContactPerson(id);

            Assert.AreEqual(myContact.ID,
                            mySecondContactPerson.ID);
            Assert.AreEqual(myContact.FirstName, mySecondContactPerson.FirstName);
            Assert.AreEqual(myContact.DateOfBirth, mySecondContactPerson.DateOfBirth);

            //Change the MyContact's Surname see if mySecondContactPerson is changed.
            //this should change since the second contact person was obtained from object manager and 
            // these should thus be the same instance.
            myContact.Surname = "New Surname";
            Assert.AreEqual(myContact.Surname, mySecondContactPerson.Surname);
        }

        [Test]
        public void TestObjectBeingRemovedFromCollection()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();
            BOPrimaryKey id = myContact.ID;
            myContact = null; // clear the person s.t. the GC can collect
            GC.Collect(); //Force the GC to collect
            //waitForDB(); // wait to give GC time to collect
            Hashtable contactPersonCol = ContactPerson.GetContactPersonCol();
            Assert.IsFalse(contactPersonCol.Contains(id), "Object has not been removed from the DB");
        }

//		[Test]
//		[ExpectedException(typeof (InvalidPropertyValueException))]
//		public void TestObjectSurnameTooLong() {
//			ContactPerson myContact = ContactPerson.GetNewContactPerson();
//			myContact.Surname = "MyPropertyIsTooLongByFarThisWill Cause and Error in Bus object";
//		}

        [Test]
        [ExpectedException(typeof (BusObjectInAnInvalidStateException))]
        public void TestObjectSurnameTooLong()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();
            myContact.Surname = "MyPropertyIsTooLongByFarThisWill Cause and Error in Bus object";
            myContact.ApplyEdit();
        }

        [Test]
        [ExpectedException(typeof (BusObjectInAnInvalidStateException))]
        public void TestObjectCompulsorySurnameNotSet()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();
            myContact.ApplyEdit();
        }

        [Test]
        public void TestCancelEdits()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();

            Assert.IsFalse(myContact.IsValid());
            myContact.Surname = "My Surname";
            Assert.IsTrue(myContact.IsValid());
            Assert.AreEqual("My Surname", myContact.Surname);
            myContact.CancelEdit();
            Assert.IsFalse(myContact.IsValid());
            Assert.IsTrue(myContact.Surname.Length == 0);
        }

        public void TestStateAfterApplyEdit()
        {
            ContactPerson myContact = ContactPerson.GetNewContactPerson();
            myContact.Surname = "Test Surname";
            myContact.ApplyEdit();
            Assert.IsFalse(myContact.IsNew, "BO is still IsNew after being saved.");
        }

        //TODO-Peter - check with Brett exactly what this should be doing - it's failing now
        // but I can't see how it could pass.
        //		[Test]
        //		[ExpectedException(typeof (BusObjOptimisticConcurrencyControlException))]
        //		public void TestOptimisticConcurrencyControl() {
        //			ContactPerson myContact = mContactPersonUpdateConcurrency;
        //			//Ensure that we have two physical instances of the same logical contact person
        ////			ContactPerson.ClearBusinessObjectBaseCol();//Ensure that a fresh object is loaded from DB
        //			ContactPerson myContact2 = ContactPerson.GetContactPerson(myContact.ID);
        //
        //			myContact.Surname = "New Surname"; //edit first object
        //			myContact2.Surname = "New Surname2"; //edit second object
        //			Assert.IsFalse(object.ReferenceEquals(myContact, myContact2));
        //			myContact.ApplyEdit(); //save first
        //			waitForDB();
        //			myContact2.ApplyEdit(); //save second
        //		}
        //
        [Test]
        //[Ignore("coupling threads")]
            public void TestMultipleUpdates()
        {
            mContactPersonUpdateConcurrency.Surname = "New Surname";
            mContactPersonUpdateConcurrency.ApplyEdit();
            //waitForDB();
            mContactPersonUpdateConcurrency.Surname = "New Surname 2";
            mContactPersonUpdateConcurrency.ApplyEdit();
            //waitForDB();
            mContactPersonUpdateConcurrency.Surname = "New Surname 3";
        }

        [Test]
        [ExpectedException(typeof (BusObjDeleteConcurrencyControlException))]
        //[Ignore("coupling threads")]
            public void TestDeleteObjectPriorToUpdatesConcurrencyControl()
        {
            ContactPerson myContact2 = ContactPerson.GetContactPerson(mContactPersonDeleteConcurrency.ID);
            mContactPersonDeleteConcurrency.Delete();
            myContact2.Surname = "New Surname 2";
            mContactPersonDeleteConcurrency.ApplyEdit();
            //waitForDB();
            myContact2.ApplyEdit();
        }

        [Test]
        [ExpectedException(typeof (BusObjBeginEditConcurrencyControlException))]
        //[Ignore("coupling threads")]
            public void TestBeginEditsOnADirtyObject()
        {
            ContactPerson.ClearContactPersonCol();
            //load second person from DB.
            mContactPBeginEditsConcurrency.Surname = "First Update to Surname";

            ContactPerson myContact2 = ContactPerson.GetContactPerson(mContactPBeginEditsConcurrency.ID);
            Assert.IsTrue(myContact2 != mContactPBeginEditsConcurrency, "two objects are the same");
            mContactPBeginEditsConcurrency.Surname = "First Update to Surname";

            mContactPBeginEditsConcurrency.ApplyEdit(); //save first object
            //Try edit second instance (should raise error)
            //waitForDB();
            myContact2.Surname = "Second Update To Surname";
        }

        /// <summary>
        /// Tests to ensure that if the object has been edited in the object manager by 
        /// another user the one we get back is always the latest.
        /// </summary>
        [Test]
        public void TestAlwaysGetTheFreshestObject()
        {
            //load second object from DB to ensure that it is now in the object manager
            ContactPerson myContact2 = ContactPerson.GetContactPerson(mCotanctPTestRefreshFromObjMan.ID);

            //Edit first object and save
            mCotanctPTestRefreshFromObjMan.Surname = "SecondSurname";
            mCotanctPTestRefreshFromObjMan.ApplyEdit(); //
            //get the third object from the object manager
            //waitForDB();
            //			System.Threading.Thread.Sleep(4000);
            ContactPerson myContact3 = ContactPerson.GetContactPerson(mCotanctPTestRefreshFromObjMan.ID);

            //The two surnames should be equal since the myContact3 was refreshed
            // when it was loaded.
            Assert.AreEqual(mCotanctPTestRefreshFromObjMan.Surname, myContact3.Surname);
            //Just to check the myContact2 should also match since it is physically the 
            // same object as myContact3
            Assert.AreEqual(mCotanctPTestRefreshFromObjMan.Surname, myContact2.Surname);
        }

        /// <summary>
        /// Tests to ensure that if the new object that is being saved to the database is always
        /// unique.
        /// </summary>
        [Test]
        [ExpectedException(typeof (BusObjDuplicateConcurrencyControlException))]
        public void TestForDuplicateNewObjects()
        {
            //create the first object
            ContactPerson myContact_1 = ContactPerson.GetNewContactPerson();

            //Edit first object and save
            myContact_1.Surname = "My Surname";
            myContact_1.SetPropertyValue("PK2Prop1", "PK2Prop1Value1");
            myContact_1.SetPropertyValue("PK2Prop2", "PK2Prop1Value2");
            myContact_1.ApplyEdit(); //
            //get the second new object from the object manager
            //waitForDB();
            ContactPerson myContact_2 = ContactPerson.GetNewContactPerson();
            //set this new object to have the same 
            // data as the already saved object
            myContact_2.Surname = "My Surname";
            myContact_2.SetPropertyValue("PK2Prop1", myContact_1.GetPropertyValue("PK2Prop1"));
            myContact_2.SetPropertyValue("PK2Prop2", myContact_1.GetPropertyValue("PK2Prop2"));
            myContact_2.ApplyEdit(); //Should raise an errors
        }

        /// <summary>
        /// Tests to ensure that if the new object that is being saved to the database is always
        /// unique.
        /// </summary>
        [Test]
        [ExpectedException(typeof (BusObjDuplicateConcurrencyControlException))]
        public void TestForDuplicateExistingObjects()
        {
            //create the first object
            ContactPerson myContact_1 = ContactPerson.GetNewContactPerson();

            //Edit first object and save
            myContact_1.Surname = "My Surname";
            myContact_1.SetPropertyValue("PK2Prop1", "PK2Prop1Value1");
            myContact_1.SetPropertyValue("PK2Prop2", "PK2Prop1Value2");
            myContact_1.ApplyEdit(); //
            //get the second new object from the object manager
            //waitForDB();
            ContactPerson myContact_2 = ContactPerson.GetNewContactPerson();
            myContact_2.SetPropertyValue("PK2Prop1", "PK2Prop1Value1  Two");
            myContact_2.Surname = "My Surname two";
            myContact_2.ApplyEdit();

            //set this new object to have the same 
            // data as the already saved object
            myContact_2.SetPropertyValue("PK2Prop1", myContact_1.GetPropertyValue("PK2Prop1"));
            myContact_2.SetPropertyValue("PK2Prop2", myContact_1.GetPropertyValue("PK2Prop2"));
            myContact_2.ApplyEdit(); //Should raise an errors
        }

        /// <summary>
        /// Tests to ensure that if the new object that is being saved to the database is always
        /// unique.
        /// </summary>
        [Test]
        [ExpectedException(typeof (BusObjDuplicateConcurrencyControlException))]
        public void TestForDuplicateNewObjectsSinglePropKey()
        {
            //create the first object
            ContactPerson myContact_1 = ContactPerson.GetNewContactPerson();

            //Edit first object and save
            myContact_1.Surname = "My Surname SinglePropKey 1";
            myContact_1.SetPropertyValue("PK3Prop", "PK3PropValue1");
            myContact_1.ApplyEdit(); //
            //get the second new object from the object manager
            //waitForDB();
            ContactPerson myContact_2 = ContactPerson.GetNewContactPerson();
            //set this new object to have the same 
            // data as the already saved object
            myContact_2.Surname = "My Surname SinglePropKey 22";
            myContact_2.SetPropertyValue("PK3Prop", myContact_1.GetPropertyValue("PK3Prop"));
            myContact_2.ApplyEdit(); //Should raise an errors
        }

        [Test]
        [ExpectedException(typeof (BusObjDuplicateConcurrencyControlException))]
        public void TestForDuplicateNewObjectsSinglePropKeyNull()
        {
            //create the first object
            ContactPerson myContact_1 = ContactPerson.GetNewContactPerson();

            //Edit first object and save
            myContact_1.Surname = "My Surname SinglePropKeyNull";
            myContact_1.SetPropertyValue("PK3Prop", null); // set the previous value to null
            myContact_1.ApplyEdit(); //
            //get the second new object from the object manager
            //waitForDB();
            ContactPerson myContact_2 = ContactPerson.GetNewContactPerson();
            //set this new object to have the same 
            // data as the already saved object
            myContact_2.Surname = "My Surname SinglePropKeyNull";
            myContact_2.SetPropertyValue("PK3Prop", myContact_1.GetPropertyValue("PK3Prop"));
            // set the previous value to null
            myContact_2.ApplyEdit(); //Should raise an errors
        }

        [Test]
        public void TestDirtyXml()
        {
            TransactionLog.DeleteAllTransactionLogs();

            ContactPerson myContact_1 = ContactPerson.GetNewContactPerson();
            //Edit first object and save
            myContact_1.Surname = "My Surname 1";
            //myContact_1.SetPropertyValue("PK3Prop", null); // set the previous value to null
            myContact_1.ApplyEdit(); //

            myContact_1.Surname = "My Surname New";

            Assert.AreEqual(
                "<ContactPerson ID=" + myContact_1.ID +
                "><Properties><Surname><PreviousValue>My Surname 1</PreviousValue><NewValue>My Surname New</NewValue></Surname><ContactPerson>",
                myContact_1.DirtyXML);

            myContact_1.ApplyEdit();

            myContact_1.Delete();
            myContact_1.ApplyEdit();

            BusinessObjectCollection<BusinessObject> myCol = TransactionLog.LoadBusinessObjCol("", "TransactionSequenceNo");
            Assert.AreEqual(myCol.Count, 3);

            TransactionLog myTransactionLog;
            int maxTransactionNo = 0;
            //TODO: hack until loading collections works better.

            //Get max transaction number
            for (int i = 0; i <= 2; i++)
            {
                myTransactionLog = (TransactionLog) myCol[i];
                if (maxTransactionNo < (int) myTransactionLog.GetPropertyValue("TransactionSequenceNo"))
                {
                    maxTransactionNo = (int) myTransactionLog.GetPropertyValue("TransactionSequenceNo");
                }
            }
            //Get latest transaction
            //TODO: this does not work at all the same transaction object is returned
            // each time regardless of whether i get item 0, 1 or 2.
            //this is possibly some bug with filling the collection.
            for (int i = 0; i <= 2; i++)
            {
                myTransactionLog = null;
                myTransactionLog = (TransactionLog) myCol[i];
                if ((int) myTransactionLog.GetPropertyValue("TransactionSequenceNo") == (maxTransactionNo - 2))
                {
                    Assert.AreEqual("Created", myTransactionLog.GetPropertyValue("CRUDAction"));
                }
                else if ((int) myTransactionLog.GetPropertyValue("TransactionSequenceNo") == (maxTransactionNo - 1))
                {
                    Assert.AreEqual("Updated", myTransactionLog.GetPropertyValue("CRUDAction"));
                }
                else if ((int) myTransactionLog.GetPropertyValue("TransactionSequenceNo") == (maxTransactionNo))
                {
                    Assert.AreEqual("Deleted", myTransactionLog.GetPropertyValue("CRUDAction"));
                }
                else
                {
                    Assert.AreEqual("", "1"); //force failure
                }
            }
        }

        [Test]
        public void TestTransactionSuccess()
        {
            ContactPerson myContact_1 = ContactPerson.GetNewContactPerson();
            //Edit first object and save
            myContact_1.Surname = "My Surname 1";
            //myContact_1.SetPropertyValue("PK3Prop", null); // set the previous value to null
            Transaction transact = new Transaction(DatabaseConnection.CurrentConnection);
            transact.AddTransactionObject(myContact_1);

            ContactPerson myContact_2 = ContactPerson.GetNewContactPerson();
            myContact_2.Surname = "My Surname 2";

            Assert.IsTrue(myContact_2.IsValid());

            transact.AddTransactionObject(myContact_2);

            transact.CommitTransaction();

            Assert.IsFalse(myContact_2.IsDirty);
            Assert.IsFalse(myContact_1.IsNew);
            Assert.IsFalse(myContact_1.IsDirty);
            Assert.IsFalse(myContact_2.IsNew);
            Assert.IsTrue(myContact_2.IsValid());

            //Ensure object loaded from DB.
            ContactPerson.ClearContactPersonCol();

            ContactPerson myContact_3 = ContactPerson.GetContactPerson(myContact_1.ID);

            Assert.AreEqual(myContact_1.ID, myContact_3.ID);
            Assert.AreEqual(myContact_1.Surname, myContact_3.Surname);

            ContactPerson myContact_4 = ContactPerson.GetContactPerson(myContact_2.ID);

            Assert.AreEqual(myContact_2.ID, myContact_4.ID);
            Assert.AreEqual(myContact_2.Surname, myContact_4.Surname);
        }

        [Test]
        public void TestTransactionFail()
        {
            ContactPerson myContact_1 = ContactPerson.GetNewContactPerson();
            //Edit first object and save
            myContact_1.Surname = "My Surname 1";
            //myContact_1.SetPropertyValue("PK3Prop", null); // set the previous value to null
            Transaction transact = new Transaction(DatabaseConnection.CurrentConnection);
            transact.AddTransactionObject(myContact_1);

            ContactPerson myContact_2 = ContactPerson.GetNewContactPerson();
            myContact_2.Surname = "My Surname 1"; //Should result in a duplicate error when try to persist
            //will result in the commit failing
            Assert.IsTrue(myContact_2.IsValid());
            transact.AddTransactionObject(myContact_2);
            bool errorRaised = false;
            try
            {
                transact.CommitTransaction();
            }
            catch (Exception ex) //todo:check type of error?
            {
                errorRaised = true;
            }

            Assert.IsTrue(errorRaised, "Error should have been raised");

            Assert.IsTrue(myContact_2.IsDirty);
            Assert.IsTrue(myContact_1.IsNew);
            Assert.IsTrue(myContact_1.IsDirty);
            Assert.IsTrue(myContact_2.IsNew);
            Assert.IsTrue(myContact_2.IsValid());
            Assert.IsTrue(myContact_2.IsValid());


            //Ensure object loaded from DB.
            ContactPerson.ClearContactPersonCol();
            errorRaised = false;
            try
            {
                ContactPerson myContact_3 = ContactPerson.GetContactPerson(myContact_1.ID);
            }
                //Expect this error since the object should not have been persisted to the DB.
            catch (BusinessObjectNotFoundException ex)
            {
                errorRaised = true;
            }
            Assert.IsTrue(errorRaised);


            //Test canceledits to transaction
            transact.CancelEdits();
            Assert.IsTrue(myContact_1.Surname.Length == 0);
            Assert.IsTrue(myContact_2.Surname.Length == 0);
            Assert.IsFalse(myContact_2.IsValid());
            Assert.IsFalse(myContact_2.IsValid());
        }

        #region tests

        [Test]
        public void TestActivatorCreate()
        {
            object contact = Activator.CreateInstance(typeof (ContactPerson), true);
        }

        #endregion tests

    }

    /// <summary>
    /// This is used only for testing reading transactions
    /// </summary>
    public class TransactionLog : BusinessObject
    {
        #region Constructors

        internal TransactionLog() : base()
        {
        }

        internal TransactionLog(BOPrimaryKey id) : base(id)
        {
        }

        public TransactionLog(ClassDef def) : base(def)
        {
        }

        private static ClassDef GetClassDef()
        {
            if (!ClassDef.IsDefined(typeof (TransactionLog)))
            {
                return CreateClassDef();
            }
            else
            {
                return ClassDef.ClassDefs[typeof (TransactionLog)];
            }
        }

        protected override ClassDef ConstructClassDef()
        {
            _classDef = GetClassDef();
            return _classDef;
        }

        protected override void ConstructClass(bool newObject)
        {
            base.ConstructClass(newObject);
            SetTransactionLog(new TransactionLogTable("TransactionLog",
                                                      "DateTimeUpdated",
                                                      "WindowsUser",
                                                      "LogonUser",
                                                      "MachineName",
                                                      "BusinessObjectTypeName",
                                                      "CRUDAction",
                                                      "DirtyXML"));
        }

        private static ClassDef CreateClassDef()
        {
            PropDefCol lPropDefCol = CreateBOPropDef();

            KeyDefCol keysCol = new KeyDefCol();

            PrimaryKeyDef primaryKey = new PrimaryKeyDef();
            primaryKey.IsObjectID = true;
            primaryKey.Add(lPropDefCol["TransactionSequenceNo"]);
            ClassDef lClassDef = new ClassDef(typeof (TransactionLog), primaryKey, lPropDefCol, keysCol, null);
			ClassDef.ClassDefs.Add(lClassDef);
            return lClassDef;
        }

        private static PropDefCol CreateBOPropDef()
        {
            PropDefCol lPropDefCol = new PropDefCol();
            PropDef propDef =
                new PropDef("TransactionSequenceNo", typeof (int), PropReadWriteRule.ReadWrite, null);
            lPropDefCol.Add(propDef);

            propDef = new PropDef("DateTimeUpdated", typeof (DateTime), PropReadWriteRule.ReadWrite, null);
            lPropDefCol.Add(propDef);

            propDef = new PropDef("WindowsUser", typeof (String), PropReadWriteRule.WriteOnce, null);
            lPropDefCol.Add(propDef);

            propDef = new PropDef("LogonUser", typeof (String), PropReadWriteRule.ReadOnly, null);
            lPropDefCol.Add(propDef);

            propDef = new PropDef("BusinessObjectTypeName", typeof (string), PropReadWriteRule.ReadOnly, null);
            lPropDefCol.Add(propDef);

            propDef = new PropDef("CRUDAction", typeof (string), PropReadWriteRule.ReadOnly, null);
            lPropDefCol.Add(propDef);

            propDef = new PropDef("DirtyXML", typeof (string), PropReadWriteRule.ReadOnly, null);
            lPropDefCol.Add(propDef);

            propDef = new PropDef("MachineName", typeof (string), PropReadWriteRule.ReadOnly, null);
            lPropDefCol.Add(propDef);
            return lPropDefCol;
        }

        public static TransactionLog GetNewTransactionLog()
        {
            TransactionLog myTransactionLog = new TransactionLog();
            AddToLoadedBusinessObjectCol(myTransactionLog);
            return myTransactionLog;
        }

        /// <summary>
        /// returns the TransactionLog identified by id.
        /// </summary>
        /// <remarks>
        /// If the Contact person is already leaded then an identical copy of it will be returned.
        /// </remarks>
        /// <param name="id">The object primary Key</param>
        /// <returns>The loaded business object</returns>
        /// <exception cref="Habanero.Bo.BusObjDeleteConcurrencyControlException">
        ///  if the object has been deleted already</exception>
        public static TransactionLog GetTransactionLog(BOPrimaryKey id)
        {
            TransactionLog myTransactionLog = (TransactionLog) TransactionLog.GetLoadedBusinessObject(id);
            if (myTransactionLog == null)
            {
                myTransactionLog = new TransactionLog(id);
                AddToLoadedBusinessObjectCol(myTransactionLog);
            }
            return myTransactionLog;
        }

        #endregion //Constructors

        #region ForTesting

        internal static void ClearTransactionLogCol()
        {
            BusinessObject.ClearLoadedBusinessObjectBaseCol();
        }

        internal static Hashtable GetTransactionLogCol()
        {
            return BusinessObject.GetLoadedBusinessObjectBaseCol();
        }

        internal static void DeleteAllTransactionLogs()
        {
            string sql = "DELETE FROM TransactionLog";
            DatabaseConnection.CurrentConnection.ExecuteRawSql(sql);
        }

        #endregion

        #region ForCollections //TODO: refactor this so that class construction occurs in its own 

        //class
        protected internal string GetObjectNewID()
        {
            return _primaryKey.GetObjectNewID();
        }

        protected internal static BusinessObjectCollection<BusinessObject> LoadBusinessObjCol()
        {
            return LoadBusinessObjCol("", "");
        }

        protected internal static BusinessObjectCollection<BusinessObject> LoadBusinessObjCol(string searchCriteria,
                                                                                  string orderByClause)
        {
            TransactionLog lTransactionLog = GetNewTransactionLog();
            SqlStatement statement = new SqlStatement(DatabaseConnection.CurrentConnection.GetConnection());
            statement.Statement.Append(lTransactionLog.SelectSqlWithNoSearchClause());
            if (searchCriteria.Length > 0)
            {
                statement.AppendCriteria("");
                SqlCriteriaCreator creator =
                    new SqlCriteriaCreator(Expression.CreateExpression(searchCriteria), lTransactionLog);
                creator.AppendCriteriaToStatement(statement);
            }
            BusinessObjectCollection<BusinessObject> bOCol = new BusinessObjectCollection<BusinessObject>(lTransactionLog.ClassDef);
            using (IDataReader dr = DatabaseConnection.CurrentConnection.LoadDataReader(statement, orderByClause))
            {
                try
                {
                    while (dr.Read())
                    {
                        lTransactionLog.LoadProperties(dr);
                        TransactionLog lTempPerson2;
                        lTempPerson2 = (TransactionLog) GetLoadedBusinessObject(lTransactionLog.GetObjectNewID());
                        if (lTempPerson2 == null)
                        {
                            bOCol.Add(lTransactionLog);
                        }
                        else
                        {
                            bOCol.Add(lTempPerson2);
                        }
                    }
                }
                finally
                {
                    if (dr != null & !(dr.IsClosed))
                    {
                        dr.Close();
                    }
                }
            }
            return bOCol;
        }

        #endregion
    }
}