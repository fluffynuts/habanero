using System;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using NUnit.Framework;

namespace Habanero.Test.BO
{
    [TestFixture]
    public class TestConcurrencyControl_OptimisticLockingVersionNumberDB : TestUsingDatabase
    {
        [SetUp]
        public void SetupTest()
        {
            ClassDef.ClassDefs.Clear();
            //Runs every time that any testmethod is executed
            //base.SetupTest();
        }
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            base.SetupDBConnection();
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.
        }
        [TearDown]
        public  void TearDownTest()
        {
            //runs every time any testmethod is complete
            //DeleteObjects();
        }
        [Test]
        public void TestLockingVersionNumber_DoesNotCauseProblemsWithMultipleSaves()
        {
            //---------------Set up test pack-------------------
            //Create object in DB

            ContactPersonOptimisticLockingVersionNumberDB.LoadDefaultClassDef();
            ContactPersonOptimisticLockingVersionNumberDB contactPerson = new ContactPersonOptimisticLockingVersionNumberDB();
            contactPerson.Surname = Guid.NewGuid().ToString();
            AddObjectToDelete(contactPerson);
            contactPerson.Save();
            //---------------Execute Test ----------------------
            //Edit first object and persist to the database.
            contactPerson.Surname = Guid.NewGuid().ToString();
            contactPerson.Save();

        }
        [Test]
        public void TestLockingVersionNumber_OnBeginEdit()
        {
            //---------------Set up test pack-------------------
            //Create object in DB

            ContactPersonOptimisticLockingVersionNumberDB.LoadDefaultClassDef();
            ContactPersonOptimisticLockingVersionNumberDB contactPerson = new ContactPersonOptimisticLockingVersionNumberDB();
            contactPerson.Surname = Guid.NewGuid().ToString();
            AddObjectToDelete(contactPerson);
            contactPerson.Save();
            //Clear object manager
            ContactPersonOptimisticLockingVersionNumberDB.ClearLoadedBusinessObjectBaseCol();
            //Load second object from DB
            ContactPersonOptimisticLockingVersionNumberDB duplicateContactPerson =
                BOLoader.Instance.GetBusinessObjectByID<ContactPersonOptimisticLockingVersionNumberDB>(contactPerson.ContactPersonID);
            AddObjectToDelete(duplicateContactPerson);

            //---------------Execute Test ----------------------
            //Edit first object and persist to the database.
            contactPerson.Surname = Guid.NewGuid().ToString();
            contactPerson.Save();
            //Begin Edit on second object
            try
            {
                duplicateContactPerson.FirstName = Guid.NewGuid().ToString();
                Assert.Fail();
            }    
            //---------------Test Result -----------------------
            //Raise Exception that the object has been edited since 
            // the user last edited.
            catch(BusObjBeginEditConcurrencyControlException ex)
            {
                Assert.IsTrue(ex.Message.Contains("You cannot Edit 'ContactPersonOptimisticLockingVersionNumberDB', as another user has edited this record"));
            }
        }
        [Test]
        public void TestLockingVersionNumber_OnSave()
        {
            //---------------Set up test pack-------------------
            //Create object in DB

            ContactPersonOptimisticLockingVersionNumberDB contactPerson = CreateSavedCntactPersonOptimisticLockingVersionNumberDB();

            //Clear object manager
            ContactPersonOptimisticLockingVersionNumberDB.ClearLoadedBusinessObjectBaseCol();
            //Load second object from DB
            ContactPersonOptimisticLockingVersionNumberDB duplicateContactPerson =
                BOLoader.Instance.GetBusinessObjectByID<ContactPersonOptimisticLockingVersionNumberDB>(contactPerson.ContactPersonID);
            AddObjectToDelete(duplicateContactPerson);

            //---------------Execute Test ----------------------
            //Edit first object and persist to the database.
            contactPerson.Surname = Guid.NewGuid().ToString();
            //Begin Edit on second object
            duplicateContactPerson.FirstName = Guid.NewGuid().ToString();

            //Save first object
            contactPerson.Save();
            try
            {
                duplicateContactPerson.Save();
                Assert.Fail();
            }
            //---------------Test Result -----------------------
            //Raise Exception that the object has been edited since 
            // the user last edited.
            catch (BusObjOptimisticConcurrencyControlException ex)
            {
                Assert.IsTrue(ex.Message.Contains("You cannot save the changes to 'ContactPersonOptimisticLockingVersionNumberDB', as another user has edited this record"));
            }
        }
        [Test]
        [ExpectedException(typeof(BusObjDeleteConcurrencyControlException))]
        public void TestDeleteObjectPriorToUpdatesConcurrencyControl()
        {
            //----------SETUP TEST PACK--------------------------
            ContactPersonOptimisticLockingVersionNumberDB contactPersonDeleteConcurrency 
                    = CreateSavedCntactPersonOptimisticLockingVersionNumberDB();
            //Clear object manager
            ContactPersonOptimisticLockingVersionNumberDB.ClearLoadedBusinessObjectBaseCol();
            //Load second object from DB            

            ContactPersonOptimisticLockingVersionNumberDB contactPerson2 = BOLoader.Instance.GetBusinessObjectByID <ContactPersonOptimisticLockingVersionNumberDB>(contactPersonDeleteConcurrency.ID);
            //---------Run TEST ---------------------------------
            contactPersonDeleteConcurrency.Delete();
            contactPerson2.Surname = "New Surname 2";
            contactPersonDeleteConcurrency.Save();
            try
            {
                contactPerson2.Save();
                Assert.Fail();
            }
            //--------Check Result --------------------------------
            catch (BusObjDeleteConcurrencyControlException ex)
            {
                Assert.IsTrue(ex.Message.Contains("You cannot save the changes to 'ContactPersonOptimisticLockingVersionNumberDB', as another user has deleted the record"));
                throw;
            }
        }
        //Rollback failure must reset concurrency version number.
        [Test]
        public void TestRollBackVersionNumberOnError()
        {
            //---------------Set up test pack-------------------
            //Create object in DB
            ContactPersonOptimisticLockingVersionNumberDB contactPerson = CreateSavedCntactPersonOptimisticLockingVersionNumberDB();

            int versionNumber = contactPerson.VersionNumber;

            //---------------Execute Test ----------------------
            contactPerson.Surname = Guid.NewGuid().ToString();
            Assert.AreEqual(versionNumber, contactPerson.VersionNumber);
            try
            {
                TransactionCommitterStubDB trnCommitter = new TransactionCommitterStubDB();
                trnCommitter.AddBusinessObject(contactPerson);
                trnCommitter.AddTransaction(new StubDatabaseFailureTransaction());
                trnCommitter.CommitTransaction();
                Assert.Fail();
            }
            //---------------Test Result -----------------------
            //Raise Exception that the object has been edited since 
            // the user last edited.
            catch (NotImplementedException ex)
            {
                Assert.AreEqual(versionNumber, contactPerson.VersionNumber);
            }
        }
        private static ContactPersonOptimisticLockingVersionNumberDB CreateSavedCntactPersonOptimisticLockingVersionNumberDB()
        {
            ContactPersonOptimisticLockingVersionNumberDB.LoadDefaultClassDef();
            ContactPersonOptimisticLockingVersionNumberDB contactPersonOptimisticLockingVersionNumberDB;
            contactPersonOptimisticLockingVersionNumberDB = new ContactPersonOptimisticLockingVersionNumberDB();
            contactPersonOptimisticLockingVersionNumberDB.Surname = Guid.NewGuid().ToString();
            contactPersonOptimisticLockingVersionNumberDB.Save();
            AddObjectToDelete(contactPersonOptimisticLockingVersionNumberDB);
            return contactPersonOptimisticLockingVersionNumberDB;
        }
    }

    internal class ContactPersonOptimisticLockingVersionNumberDB : BusinessObject
    {
        public ContactPersonOptimisticLockingVersionNumberDB()
        {
            BOProp propDateLastUpdated = _boPropCol["DateLastUpdated"];
            BOProp propUserLastUpdated = _boPropCol["UserLastUpdated"];
            BOProp propMachineLastUpdated = _boPropCol["MachineLastUpdated"];
            BOProp propVersionNumber = _boPropCol["VersionNumber"];
            SetConcurrencyControl(new OptimisticLockingVersionNumberDB(this,propDateLastUpdated,
                                                         propUserLastUpdated, propMachineLastUpdated,
                                                         propVersionNumber));
        }
        public static ClassDef LoadDefaultClassDef()
        {
            XmlClassLoader itsLoader = new XmlClassLoader();
            ClassDef itsClassDef =
                itsLoader.LoadClass(
                    @"
				<class name=""ContactPersonOptimisticLockingVersionNumberDB"" assembly=""Habanero.Test.BO"" table=""contact_person"">
					<property  name=""ContactPersonID"" type=""Guid"" />
					<property  name=""Surname"" compulsory=""true"" />
                    <property  name=""FirstName"" />
					<property  name=""DateLastUpdated"" type=""DateTime"" />
					<property  name=""UserLastUpdated"" />
					<property  name=""VersionNumber"" type=""Int32""/>
					<property  name=""MachineLastUpdated"" />
					<primaryKey>
						<prop name=""ContactPersonID"" />
					</primaryKey>
			    </class>
			");
            ClassDef.ClassDefs.Add(itsClassDef);
            return itsClassDef;
        }
        public Guid ContactPersonID
        {
            get { return (Guid)GetPropertyValue("ContactPersonID"); }
            set { this.SetPropertyValue("ContactPersonID", value); }
        }

        public string Surname
        {
            get { return (string)GetPropertyValue("Surname"); }
            set { SetPropertyValue("Surname", value); }
        }
        public string FirstName
        {
            get { return (string)GetPropertyValue("FirstName"); }
            set { SetPropertyValue("FirstName", value); }
        }

        public DateTime? DateOfBirth
        {
            get { return (DateTime?)GetPropertyValue("DateOfBirth"); }
            set { SetPropertyValue("DateOfBirth", value); }
        }
        public int VersionNumber
        {
            get { return (int)GetPropertyValue("VersionNumber"); }
            set { SetPropertyValue("VersionNumber", value); }
        }
        public override string ToString()
        {
            return Surname;
        }
    }
}