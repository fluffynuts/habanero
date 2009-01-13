using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Test.BO
{
    [TestFixture]
    public class TestBOPropLookupList_BusinessObjectLookupList_GuidID
    {
        private PropDef _propDef_guid;
        private IBusinessObjectCollection _collection;
        private MyBO _validBusinessObject;
        private MyBO _validBusinessObject_NotInList;
        private string _validLookupValue;

        [SetUp]
        public void Setup()
        {
            ClassDef.ClassDefs.Clear();
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.
            ClassDef.ClassDefs.Clear();
            MyBO.LoadClassDefsNoUIDef();
            BORegistry.DataAccessor = new DataAccessorInMemory();
            _propDef_guid = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null);
            _validBusinessObject = new MyBO {TestProp = "ValidValue"};
            _collection = new BusinessObjectCollection<MyBO> {_validBusinessObject};
            _validLookupValue = _validBusinessObject.ToString();

            _propDef_guid.LookupList = new BusinessObjectLookupListStub(typeof (MyBO), _collection);
            _validBusinessObject_NotInList = new MyBO {TestProp = "AnotherValue"};
        }

        private static string GuidToUpper(Guid guid)
        {
            return StringUtilities.GuidToUpper(guid);
        }

        [Test]
        public void Test_GetKey_FromLookupList_Sorted()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection);
            new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null)
                {LookupList = businessObjectLookupList};
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            Dictionary<string, string> lookupList = businessObjectLookupList.GetLookupList();

            //---------------Test Result -----------------------
            Assert.AreEqual(1, lookupList.Count, "There should be one item in the lookup list");
            Assert.IsTrue(lookupList.ContainsKey(_validLookupValue));
            string objectIDAsString = lookupList[_validLookupValue];
            Assert.AreEqual(GuidToUpper(_validBusinessObject.ID.GetAsGuid()), objectIDAsString);
        }

        [Test]
        public void Test_GetValue_FromKeyValueList_Sorted()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection, "TestProp");
            new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null)
                {LookupList = businessObjectLookupList};
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            Dictionary<string, string> idValueLookupList = businessObjectLookupList.GetIDValueLookupList();

            //---------------Test Result -----------------------
            Assert.AreEqual(1, idValueLookupList.Count, "There should be one item in the lookup list");
            string guidToUpper = GuidToUpper(_validBusinessObject.ID.GetAsGuid());
            Assert.IsTrue(idValueLookupList.ContainsKey(guidToUpper));
            string returnedValue = idValueLookupList[guidToUpper];
            Assert.AreEqual(_validLookupValue, returnedValue);
        }

        [Test]
        public void Test_GetKey_FromLookupList_Unsorted()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadClassDefsNoUIDef();
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null);
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof(MyBO), _collection);
            propDef.LookupList = businessObjectLookupList;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            Dictionary<string, string> lookupList = businessObjectLookupList.GetLookupList();

            //---------------Test Result -----------------------
            Assert.AreEqual(1, lookupList.Count, "There should be one item in the lookup list");
            Assert.IsTrue(lookupList.ContainsKey(_validLookupValue));
            string objectIDAsString = lookupList[_validLookupValue];
            Assert.AreEqual(GuidToUpper(_validBusinessObject.ID.GetAsGuid()), objectIDAsString);
        }

        [Test]
        public void Test_GetValue_FromKeyValueList_Unsorted()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadClassDefsNoUIDef();
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null);
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection, "");
            propDef.LookupList = businessObjectLookupList;
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            Dictionary<string, string> idValueLookupList = businessObjectLookupList.GetIDValueLookupList();

            //---------------Test Result -----------------------
            Assert.AreEqual(1, idValueLookupList.Count, "There should be one item in the lookup list");
            string guidToUpper = GuidToUpper(_validBusinessObject.ID.GetAsGuid());
            Assert.IsTrue(idValueLookupList.ContainsKey(guidToUpper));
            string returnedValue = idValueLookupList[guidToUpper];
            Assert.AreEqual(_validLookupValue, returnedValue);
        }

        [Test]
        public void Test_CompositeKey_ThrowsError()
        {
            //---------------Set up test pack-------------------
            ContactPersonTestBO.LoadClassDefWithCompositePrimaryKey();
            BusinessObjectCollection<ContactPersonTestBO> collection =
                new BusinessObjectCollection<ContactPersonTestBO>();
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (ContactPersonTestBO), collection);
            new PropDef("PropName", typeof (string), PropReadWriteRule.ReadWrite, null)
                {LookupList = businessObjectLookupList};
            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            try
            {
                businessObjectLookupList.GetLookupList();
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains("There is an application setup error", ex.Message);
                StringAssert.Contains
                    ("The lookup list cannot contain business objects 'ContactPersonTestBO' with a composite primary key.",
                     ex.DeveloperMessage);
            }
        }

        [Test]
        public void Test_SetLookupListForPropDef()
        {
            //---------------Set up test pack-------------------
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null);
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection);
            //---------------Assert Precondition----------------
            Assert.IsInstanceOfType(typeof (NullLookupList), propDef.LookupList);
            //---------------Execute Test ----------------------
            propDef.LookupList = businessObjectLookupList;
            //---------------Test Result -----------------------
            Assert.IsNotNull(propDef.LookupList);
            Assert.AreSame(propDef, businessObjectLookupList.PropDef);
        }

        [Test]
        public void Test_SimpleLookup_Create_SetsUpKeyLookupList()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            //---------------Assert Precondition----------------
            Assert.AreEqual(1, _collection.Count);
            //---------------Execute Test ----------------------
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection);
            businessObjectLookupList.PropDef = _propDef_guid;
            //---------------Assert Precondition----------------
            Assert.AreEqual(_collection.Count, businessObjectLookupList.GetLookupList().Count);
            Assert.IsNotNull(businessObjectLookupList.GetIDValueLookupList());
            Assert.AreEqual(_collection.Count, businessObjectLookupList.GetIDValueLookupList().Count);
        }

        [Test]
        public void Test_SimpleLookup_Create_SetsUpKeyLookupList_BO_PrimaryKeyNotSet()
        {
            //---------------Set up test pack-------------------
            OrganisationTestBO.LoadDefaultClassDef_WithSingleRelationship();
            OrganisationTestBO bo = new OrganisationTestBO {OrganisationID = null};
            BusinessObjectCollection<OrganisationTestBO> collectionBO =
                new BusinessObjectCollection<OrganisationTestBO> {bo};
            //---------------Assert Precondition----------------
            Assert.IsNull(bo.ID.GetAsValue());
            //---------------Execute Test ----------------------
            try
            {
                BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                    (typeof (OrganisationTestBO), collectionBO) {PropDef = _propDef_guid};
                businessObjectLookupList.GetIDValueLookupList();
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                string developerMessage = string.Format
                    ("A business object of '{0}' is being added to a lookup list for {1} it "
                     + "does not have a value for its primary key set", _propDef_guid.PropertyTypeName,
                     _propDef_guid.PropertyName);
                StringAssert.Contains(developerMessage, ex.DeveloperMessage);
            }
        }

        [Test]
        public void Test_SetLookupList_SetsUpKeyLookupList()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null);
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection) {PropDef = _propDef_guid};
            //---------------Assert Precondition----------------
            Assert.IsInstanceOfType(typeof (NullLookupList), propDef.LookupList);
            Assert.AreEqual(_collection.Count, businessObjectLookupList.GetLookupList().Count);
            //---------------Execute Test ----------------------
            propDef.LookupList = businessObjectLookupList;
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof (BusinessObjectLookupList), propDef.LookupList);
            Assert.AreSame(propDef, businessObjectLookupList.PropDef);
            Assert.AreEqual(_collection.Count, businessObjectLookupList.GetLookupList().Count);
            Assert.AreEqual(_collection.Count, businessObjectLookupList.GetIDValueLookupList().Count);
        }

        #region LookupListGuid

        [Test]
        public void Test_BusinessObjectLookupList_GetKey_Exists()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null);
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection);
            propDef.LookupList = businessObjectLookupList;
            Dictionary<string, string> list = businessObjectLookupList.GetLookupList();
            //---------------Assert Precondition----------------
            Assert.IsInstanceOfType(typeof (BusinessObjectLookupList), propDef.LookupList);
            Assert.AreSame(propDef, businessObjectLookupList.PropDef);
            //---------------Execute Test ----------------------
            string returnedKey;
            bool keyReturned = list.TryGetValue(_validLookupValue, out returnedKey);
            //---------------Test Result -----------------------
            Assert.IsTrue(keyReturned);
            Assert.AreEqual(GuidToUpper(_validBusinessObject.ID.GetAsGuid()), returnedKey);
        }

        [Test]
        public void Test_BusinessObjectLookupList_GetKey_NotExists()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null);
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection);
            propDef.LookupList = businessObjectLookupList;
            Dictionary<string, string> list = businessObjectLookupList.GetLookupList();
            //---------------Assert Precondition----------------
            Assert.IsInstanceOfType(typeof (BusinessObjectLookupList), propDef.LookupList);
            Assert.AreSame(propDef, businessObjectLookupList.PropDef);
            //---------------Execute Test ----------------------
            string returnedKey;
            bool keyReturned = list.TryGetValue("InvalidValue", out returnedKey);
            //---------------Test Result -----------------------
            Assert.IsFalse(keyReturned);
            Assert.IsNull(returnedKey);
        }

        [Test]
        public void Test_BusinessObjectLookupList_GetValue_Exists()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null);
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection);
            propDef.LookupList = businessObjectLookupList;
            Dictionary<string, string> list = businessObjectLookupList.GetIDValueLookupList();
            //---------------Assert Precondition----------------
            Assert.IsInstanceOfType(typeof (BusinessObjectLookupList), propDef.LookupList);
            Assert.AreSame(propDef, businessObjectLookupList.PropDef);
            //---------------Execute Test ----------------------
            string returnedValue;
            bool keyReturned = list.TryGetValue(GuidToUpper(_validBusinessObject.ID.GetAsGuid()), out returnedValue);
            //---------------Test Result -----------------------
            Assert.IsTrue(keyReturned);
            Assert.AreEqual(_validLookupValue, returnedValue);
        }

        [Test]
        public void Test_BusinessObjectLookupList_GetValue_NotExists()
        {
            //---------------Set up test pack-------------------
            PropDef propDef = GetPropDef_Guid_WithLookupList();
            BusinessObjectLookupList businessObjectLookupList = (BusinessObjectLookupList) propDef.LookupList;
            Dictionary<string, string> list = businessObjectLookupList.GetIDValueLookupList();
            //---------------Assert Precondition----------------
            Assert.IsInstanceOfType(typeof (BusinessObjectLookupList), propDef.LookupList);
            Assert.AreSame(propDef, businessObjectLookupList.PropDef);
            //---------------Execute Test ----------------------
            string returnedValue;
            bool keyReturned = list.TryGetValue(Guid.NewGuid().ToString(), out returnedValue);
            //---------------Test Result -----------------------
            Assert.IsFalse(keyReturned);
            Assert.IsNull(returnedValue);
        }

        private PropDef GetPropDef_Guid_WithLookupList()
        {
            MyBO.LoadDefaultClassDef();
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null);
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection);
            propDef.LookupList = businessObjectLookupList;
            return propDef;
        }

        private BOProp GetProp_String_WithLookupList()
        {
            MyBO.LoadDefaultClassDef();
            PropDef propDef = new PropDef("PropName", typeof (string), PropReadWriteRule.ReadWrite, null);
            BusinessObjectLookupList businessObjectLookupList = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection);
            propDef.LookupList = businessObjectLookupList;
            return new BOPropLookupList(propDef);
        }

        [Test]
        public void Test_BOPropLookupList_CreateWithLookupList()
        {
            //---------------Set up test pack-------------------
            PropDef def = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null)
                              {LookupList = new BusinessObjectLookupListStub(typeof (MyBO), _collection)};
            //---------------Assert Precondition----------------
            Assert.IsTrue(def.HasLookupList());
            //---------------Execute Test ----------------------
            BOPropLookupList boPropLookupList = new BOPropLookupList(def);
            //---------------Test Result -----------------------
            Assert.IsNotNull(boPropLookupList);
        }

        [Test]
        public void Test_BOPropLookupList_PropValueToDisplay_NullValue()
        {
            //---------------Set up test pack-------------------

            BOProp boProp = new BOPropLookupList(GetPropDef_Guid_WithLookupList());
            boProp.InitialiseProp(null);
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            object propertyValueToDisplay = boProp.PropertyValueToDisplay;
            //---------------Test Result -----------------------
            Assert.IsNull(propertyValueToDisplay);
        }

        [Test]
        public void Test_BOPropLookupList_InitialiseProp_ValidBusinessObject()
        {
            //---------------Set up test pack-------------------
            BOProp boProp = new BOPropLookupList(GetPropDef_Guid_WithLookupList());

            //---------------Assert Precondition----------------
            //---------------Execute Test ----------------------
            boProp.InitialiseProp(_validBusinessObject);
            //---------------Test Result -----------------------
            Assert.AreEqual(_validBusinessObject.MyBoID, boProp.Value);
            Assert.IsInstanceOfType(typeof (Guid), boProp.Value);
        }

        [Test]
        public void Test_BOPropLookupList_PropValueToDisplay_ValidBusinessObject()
        {
            //---------------Set up test pack-------------------
            BOProp boProp = new BOPropLookupList(GetPropDef_Guid_WithLookupList());
            boProp.InitialiseProp(_validBusinessObject);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boProp.Value);
            Assert.AreEqual(_validBusinessObject.MyBoID, boProp.Value);
            //---------------Execute Test ----------------------
            object propertyValueToDisplay = boProp.PropertyValueToDisplay;
            //---------------Test Result -----------------------
            Assert.AreEqual(_validBusinessObject.MyBoID, boProp.Value);
            Assert.AreEqual(_validLookupValue, propertyValueToDisplay);
        }

        [Test]
        public void Test_BOPropLookupList_PropValueToDisplay_ValidGuid()
        {
            //---------------Set up test pack-------------------
            BOProp boProp = new BOPropLookupList(GetPropDef_Guid_WithLookupList());
            boProp.InitialiseProp(_validBusinessObject.MyBoID);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boProp.Value);
            Assert.AreEqual(_validBusinessObject.MyBoID, boProp.Value);
            //---------------Execute Test ----------------------
            object propertyValueToDisplay = boProp.PropertyValueToDisplay;
            //---------------Test Result -----------------------
            Assert.AreEqual(_validBusinessObject.MyBoID, boProp.Value);
            Assert.AreEqual(_validLookupValue, propertyValueToDisplay);
        }

        [Test]
        public void Test_BOPropLookupList_PropValueToDisplay_InvalidGuid()
        {
            //GetPropertyValueToDisplay where the guid value is not 
            // in the lookup list (should return null)
            //---------------Set up test pack-------------------
            BOProp boProp = new BOPropLookupList(GetPropDef_Guid_WithLookupList());
            Guid guidNotInLookupList = Guid.NewGuid();
            boProp.InitialiseProp(guidNotInLookupList);
            //---------------Assert Precondition----------------
            Assert.IsNotNull(boProp.Value);
            Assert.AreEqual(guidNotInLookupList, boProp.Value);
            Assert.IsTrue(boProp.IsValid);
            //---------------Execute Test ----------------------
            object propertyValueToDisplay = boProp.PropertyValueToDisplay;
            //---------------Test Result -----------------------
            Assert.IsNull(propertyValueToDisplay);
        }

        [Test]
        public void Test_BOPropLookupList_PropValueToDisplay_ValidGuidStringLookUpList()
        {
            //---------------Set up test pack-------------------
            BOProp boProp = GetProp_String_WithLookupList();
            Guid guid = _validBusinessObject.ID.GetAsGuid();
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.InitialiseProp(guid);
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof (string), boProp.Value);
            Assert.AreEqual(guid.ToString("B").ToUpperInvariant(), boProp.Value);
            Assert.AreEqual(_validLookupValue, boProp.PropertyValueToDisplay);
        }


        [Test]
        public void Test_BOPropLookupList_PropValueToDisplay_ValidGuidStringLookUpList_SetTODisplayValue()
        {
            //---------------Set up test pack-------------------
            BOProp boProp = GetProp_String_WithLookupList();
            Guid guid = _validBusinessObject.ID.GetAsGuid();
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.InitialiseProp(_validLookupValue);
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof (string), boProp.Value);
            Assert.AreEqual(guid.ToString("B").ToUpperInvariant(), boProp.Value);
            Assert.AreEqual(_validLookupValue, boProp.PropertyValueToDisplay);
        }


        [Test, Ignore("This test was removed due to performance of parsing each value while filling the list")]
        public void Test_GetLookupList_LookupList_IncorrectIdentifierType()
        {
            //---------------Set up test pack------------------
            MyBO.LoadDefaultClassDef();
            PropDef propDef = new PropDef("PropName", typeof (int), PropReadWriteRule.ReadWrite, null);
            BusinessObjectLookupListStub businessObjectLookupListStub = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection);
            propDef.LookupList = businessObjectLookupListStub;
            //---------------Assert Precondition----------------
            Assert.IsInstanceOfType(typeof (BusinessObjectLookupList), propDef.LookupList);
            //---------------Execute Test ----------------------
            try
            {
                businessObjectLookupListStub.GetLookupList();
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains
                    ("There is an application setup error Please contact your system administrator", ex.Message);
                StringAssert.Contains
                    ("There is a class definition setup error the business object lookup list has lookup value items that are not of type",
                     ex.DeveloperMessage);
                StringAssert.Contains("Int32", ex.DeveloperMessage);
            }
        }

        [Test]
        public void Test_NoPropDefSet_ThrowsError()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BusinessObjectLookupListStub businessObjectLookupListStub = new BusinessObjectLookupListStub
                (typeof (MyBO), _collection);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            try
            {
                businessObjectLookupListStub.GetLookupList();
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (Exception ex)
            {
                StringAssert.Contains
                    ("There is an application setup error. There is no propdef set for the business object lookup list.",
                     ex.Message);
            }
            //---------------Test Result -----------------------
        }

        #region InitialiseProp

        [Test]
        public void Test_InialiseProp_ValidGuid()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.InitialiseProp(_validBusinessObject.ID.GetAsValue());
            //---------------Test Result -----------------------
            Assert.IsNotNull(boProp.Value);
            Assert.IsInstanceOfType(typeof (Guid), boProp.Value);
            Assert.AreEqual(_validBusinessObject.ID.GetAsValue(), boProp.Value);
            Assert.AreEqual(_validLookupValue, boProp.PropertyValueToDisplay);
        }

        [Test]
        public void Test_InialiseProp_ValidBusinessObject()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.InitialiseProp(_validBusinessObject);
            //---------------Test Result -----------------------
            Assert.IsNotNull(boProp.Value);
            Assert.IsInstanceOfType(typeof (Guid), boProp.Value);
            Assert.AreEqual(_validBusinessObject.ID.GetAsValue(), boProp.Value);
            Assert.AreEqual(_validLookupValue, boProp.PropertyValueToDisplay);
        }

        [Test]
        public void Test_InialiseProp_ValidGuidString_InList()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.InitialiseProp(_validBusinessObject.ID.GetAsValue().ToString());
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof (Guid), boProp.Value);
            Assert.AreEqual(_validBusinessObject.ID.GetAsValue(), boProp.Value);
            Assert.AreEqual(_validLookupValue, boProp.PropertyValueToDisplay);
            Assert.IsTrue(string.IsNullOrEmpty(boProp.IsValidMessage));
            Assert.IsTrue(boProp.IsValid);
        }

        //If try initialise a property with invalid property types is set to the property then
        //  An error is raised. Stating the error reason.
        //  The property value will be set to the previous property value.
        //  The property is not changed to be in an invalid state. The prop invalid reason is not set.
        [Test]
        public void Test_InitialiseProp_InvalidString()
        {
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            const string invalid = "Invalid";
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            try
            {
                boProp.InitialiseProp(invalid);
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroApplicationException ex)
            {
                StringAssert.Contains(boProp.PropertyName + " cannot be set to '" + invalid + "'", ex.Message);
                StringAssert.Contains("this value does not exist in the lookup list", ex.Message);
                Assert.AreEqual(null, boProp.Value);
                Assert.IsTrue(boProp.IsValid);
            }
        }


        [Test]
        public void Test_InialiseProp_EmptyGuid()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.InitialiseProp(Guid.Empty);
            //---------------Test Result -----------------------
            Assert.IsNull(boProp.Value);
        }

        [Test]
        public void Test_InitialiseProp_ValidDisplayValueString()
        {
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.InitialiseProp(_validLookupValue);
            //---------------Test Result -----------------------
            Assert.AreEqual(_validBusinessObject.ID.GetAsValue(), boProp.Value);
            Assert.AreEqual(_validLookupValue, boProp.PropertyValueToDisplay);
        }

        //Brett 12 Jan 2009: 
        //BO is being created for loading from DB then do not set defaults and do not validate when loading.
        //If BO is not being loaded from DB set defaults and do not validate these.
        //When saving run BO.Validate which will validate all rules and object will not save if not valid.
        //This will most closely simulate the current behaviour.
        //When saving a default for a simple lookup list you can store the default as either the string value or 
        //  the id value and it will be translated and stored as the appropriate value.
        //When saving a default for a business object lookup list you must store the Guid and can not store the 
        //   lookup value.
        [Test]
        public void Test_InitialiseProp_ValidGuid_NotInList()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.InitialiseProp(_validBusinessObject_NotInList.ID.GetAsValue());
            //---------------Test Result -----------------------
            Assert.IsNotNull(boProp.Value);
            Assert.IsInstanceOfType(typeof (Guid), boProp.Value);
            Assert.AreEqual(_validBusinessObject_NotInList.ID.GetAsValue(), boProp.Value);
            //            string errorMessage = String.Format("'{0}' invalid since '{1}' is not in the lookup list of available values.", boProp.DisplayName, boProp.Value);
            //            StringAssert.Contains(errorMessage, boProp.InvalidReason);
            Assert.AreEqual("", boProp.InvalidReason, "The business object is not placed in an invalid state");
            Assert.IsTrue(boProp.IsValid);
        }

        [Test]
        public void Test_InitialiseProp_ValidGuidString_NotInList()
        {
            //The business object property is loaded with the valid guid and is not placed in 
            //  an invalid state  when it is initialised (loaded) from the database due to the performance overhead 
            // and the potential for circular loading of lookup lists in the case of self referencing business objects
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.InitialiseProp(_validBusinessObject_NotInList.ID.GetAsValue().ToString());
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof (Guid), boProp.Value);
            Assert.AreEqual(_validBusinessObject_NotInList.ID.GetAsValue(), boProp.Value);
//            string errorMessage = String.Format("'{0}' invalid since '{1}' is not in the lookup list of available values.", boProp.DisplayName, boProp.Value);
//            StringAssert.Contains(errorMessage, boProp.InvalidReason);
            Assert.AreEqual("", boProp.InvalidReason, "The business object is not placed in an invalid state");
            Assert.IsTrue(boProp.IsValid);
        }

        #endregion

        #region SetValue

        //If an invalid property types is set to the property then
        //  An error is raised. Stating the error reason.
        //  The property value will be set to the previous property value.
        //  The property is not changed to be in an invalid state. The prop invalid reason is not set.
        [Test]
        public void Test_SetValue_InvalidString()
        {
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            const string invalid = "Invalid";
            object originalPropValue = _validBusinessObject.ID.GetAsGuid();
            boProp.Value = originalPropValue;
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.AreEqual(originalPropValue, boProp.Value);
            Assert.IsTrue(boProp.IsValid);
            //---------------Execute Test ----------------------
            try
            {
                boProp.Value = invalid;
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroApplicationException ex)
            {
                //You are trying to set the value for a lookup property PropName to 'Invalid' this value does not exist in the lookup list
                StringAssert.Contains(boProp.PropertyName + " cannot be set to '" + invalid + "'", ex.Message);
                StringAssert.Contains("this value does not exist in the lookup list", ex.Message);
                Assert.AreEqual(originalPropValue, boProp.Value);
                Assert.IsTrue(boProp.IsValid);
            }
        }

        //If an invalid Business Object types is set to the property 
        //  E.g. The Lookup is defined for MyBo and the business Object being set is ContactPerson
        //  Then an error is raised. Stating the error reason.
        //  The property value will be set to the previous property value.
        //  The property is not changed to be in an invalid state. The prop invalid reason is not set.
        [Test]
        public void Test_SetValue_InvalidBusinessObjectType()
        {
            MyBO.LoadDefaultClassDef();
            ContactPersonTestBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            ContactPersonTestBO invalidBOType = new ContactPersonTestBO {Surname = "Temp"};
            object originalPropValue = _validBusinessObject.ID.GetAsGuid();
            boProp.Value = originalPropValue;
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNotNull(boProp.Value);
            Assert.IsTrue(boProp.IsValid);
            //---------------Execute Test ----------------------
            try
            {
                boProp.Value = invalidBOType;
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                //You are trying to set the value for a lookup property PropName to 'Invalid' this value does not exist in the lookup list
                StringAssert.Contains
                    ("'PropName' cannot be set to a business object of type 'Habanero.Test.BO.ContactPersonTestBO' since the lookup list is defined for type 'Habanero.Test.MyBO'",
                     ex.Message);
                Assert.AreEqual(originalPropValue, boProp.Value);
                Assert.IsTrue(boProp.IsValid);
            }
        }

        [Test]
        public void Test_SetValue_BusinessObject_BaseBOType()
        {
            //If the lookup list is set to be a list of Circles then you cannot set the lookup value
            // to a Shape (unless that shape is a circle)
            Shape.GetClassDef();
            Circle.GetClassDef();
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null)
                                  {LookupList = new BusinessObjectLookupList(typeof (Circle))};
            BOProp boProp = new BOPropLookupList(propDef);
            Shape baseBOType = new Shape();
            object originalPropValue = _validBusinessObject.ID.GetAsGuid();
            boProp.Value = originalPropValue;
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNotNull(boProp.Value);
            //---------------Execute Test ----------------------
            try
            {
                boProp.Value = baseBOType;
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroDeveloperException ex)
            {
                //You are trying to set the value for a lookup property PropName to 'Invalid' this value does not exist in the lookup list
                StringAssert.Contains
                    ("'PropName' cannot be set to a business object of type 'Habanero.Test.Shape' since the lookup list is defined for type 'Habanero.Test.Circle'",
                     ex.Message);
                Assert.AreEqual(originalPropValue, boProp.Value);
            }
        }

        [Test]
        public void Test_SetValue_BusinessObject_ToStringNull_BONotInList()
        {
            ContactPersonTestBO.LoadDefaultClassDef();
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null)
                                  {LookupList = new BusinessObjectLookupList(typeof (ContactPersonTestBO))};
            BOProp boProp = new BOPropLookupList(propDef);
            boProp.InitialiseProp(_validBusinessObject.ID.GetAsGuid());
            ContactPersonTestBO boWithNullToString = new ContactPersonTestBO();
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsTrue(boProp.IsValid);
            //---------------Execute Test ----------------------
            boProp.Value = boWithNullToString;
            //---------------Test Result -----------------------
            Assert.AreEqual(boWithNullToString.ContactPersonID, boProp.Value);
            string expectedErrorMessage = String.Format
                ("{0}' invalid since '{1}' is not in the lookup list of available values.", boProp.DisplayName,
                 boProp.Value);
            StringAssert.Contains(expectedErrorMessage, boProp.IsValidMessage);
            Assert.IsFalse(boProp.IsValid);
        }

        //If Set inherited child to a list defined for the lookup for parent then no error.
        [Test]
        public void Test_SetValue_BusinessObject_InheritedChild_BONotInList()
        {
            PropDef propDef = new PropDef("PropName", typeof (Guid), PropReadWriteRule.ReadWrite, null)
                                  {LookupList = new BusinessObjectLookupList(typeof (Shape))};
            BOProp boProp = new BOPropLookupList(propDef);
            Circle inheritedBO = new Circle();

            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);

            //---------------Execute Test ----------------------
            boProp.Value = inheritedBO;
            //---------------Test Result -----------------------
            Assert.AreEqual(inheritedBO.ID.GetAsValue(), boProp.Value);
            string expectedErrorMessage = String.Format
                ("{0}' invalid since '{1}' is not in the lookup list of available values.", boProp.DisplayName,
                 boProp.Value);
            StringAssert.Contains(expectedErrorMessage, boProp.IsValidMessage);
            Assert.IsFalse(boProp.IsValid);
        }

        [Test]
        public void Test_SetValue_NullValue()
        {
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            object originalPropValue = Guid.NewGuid();
            boProp.Value = originalPropValue;
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNotNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.Value = null;
            //---------------Test Result -----------------------
            Assert.IsNull(boProp.Value);
            Assert.IsTrue(String.IsNullOrEmpty(boProp.IsValidMessage));
            Assert.IsTrue(String.IsNullOrEmpty(boProp.InvalidReason));
            Assert.IsTrue(boProp.IsValid);
        }

        [Test]
        public void Test_SetValue_ValidDisplayValueString()
        {
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            object originalPropValue = Guid.NewGuid();
            boProp.Value = originalPropValue;
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNotNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.Value = _validLookupValue;
            //---------------Test Result -----------------------
            Assert.AreEqual(_validBusinessObject.ID.GetAsValue(), boProp.Value);
            Assert.AreEqual(_validLookupValue, boProp.PropertyValueToDisplay);
        }

        [Test]
        public void Test_SetValue_ValidGuid_NotInList()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.Value = _validBusinessObject_NotInList.ID.GetAsValue();
            //---------------Test Result -----------------------
            Assert.IsNotNull(boProp.Value);
            Assert.IsInstanceOfType(typeof (Guid), boProp.Value);
            Assert.AreEqual(_validBusinessObject_NotInList.ID.GetAsValue(), boProp.Value);
            string expectedErrorMessage = String.Format
                ("{0}' invalid since '{1}' is not in the lookup list of available values.", boProp.DisplayName,
                 boProp.Value);
            StringAssert.Contains(expectedErrorMessage, boProp.InvalidReason);
            Assert.IsFalse(boProp.IsValid);
        }

        [Test]
        public void Test_SetValue_ValidGuidString_NotInList()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.Value = _validBusinessObject_NotInList.ID.GetAsValue().ToString();
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof (Guid), boProp.Value);
            Assert.AreEqual(_validBusinessObject_NotInList.ID.GetAsValue(), boProp.Value);
            string expectedErrorMessage = String.Format
                ("{0}' invalid since '{1}' is not in the lookup list of available values.", boProp.DisplayName,
                 boProp.Value);
            StringAssert.Contains(expectedErrorMessage, boProp.InvalidReason);
            Assert.IsFalse(boProp.IsValid);
        }

        [Test]
        public void Test_SetValue_ValidGuidString_InList()
        {
            //---------------Set up test pack-------------------
            MyBO.LoadDefaultClassDef();
            BOProp boProp = new BOPropLookupList(_propDef_guid);
            //---------------Assert Precondition----------------
            Assert.IsNull(boProp.Value);
            //---------------Execute Test ----------------------
            boProp.Value = _validBusinessObject.ID.GetAsValue().ToString();
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof (Guid), boProp.Value);
            Assert.AreEqual(_validBusinessObject.ID.GetAsValue(), boProp.Value);
            Assert.AreEqual("", boProp.InvalidReason);
            Assert.IsTrue(boProp.IsValid);
        }

        #endregion

        #region Business Object SetPropertyValue

        //If an invalid property types is set to the property then
        //  An error is raised. Stating the error reason.
        //  The property value will be set to the previous property value.
        //  The property is not changed to be in an invalid state. The prop invalid reason is not set.
        [Test]
        public void Test_BO_SetPropertyValue_InvalidString()
        {
            MyBO.LoadDefaultClassDef();
            IBusinessObject businessObject = GetBusinessObjectStub();
            BOProp boProp = (BOProp) businessObject.Props[_propDef_guid.PropertyName];
            const string invalid = "Invalid";
            object originalPropValue = _validBusinessObject.ID.GetAsGuid();
            businessObject.SetPropertyValue(_propDef_guid.PropertyName, originalPropValue);

            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNotNull(boProp.Value);
            Assert.AreEqual(originalPropValue, boProp.Value);
            Assert.IsInstanceOfType(typeof (BOPropLookupList), boProp);
            Assert.IsTrue(boProp.IsValid);
            //---------------Execute Test ----------------------
            try
            {
                businessObject.SetPropertyValue(boProp.PropertyName, invalid);
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (HabaneroApplicationException ex)
            {
                //You are trying to set the value for a lookup property PropName to 'Invalid' this value does not exist in the lookup list
                StringAssert.Contains(boProp.PropertyName + " cannot be set to '" + invalid + "'", ex.Message);
                StringAssert.Contains("this value does not exist in the lookup list", ex.Message);
                Assert.AreEqual(originalPropValue, boProp.Value);
                Assert.IsTrue(boProp.IsValid);
            }
        }

        [Test]
        public void Test_BO_SetPropertyValue_ValidGuidString()
        {
            MyBO.LoadDefaultClassDef();
            IBusinessObject businessObject = GetBusinessObjectStub();
            BOProp boProp = (BOProp) businessObject.Props[_propDef_guid.PropertyName];
            object originalPropValue = Guid.NewGuid();
            boProp.Value = originalPropValue;
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNotNull(boProp.Value);
            //---------------Execute Test ----------------------
            businessObject.SetPropertyValue(boProp.PropertyName, _validBusinessObject.ID.GetAsValue().ToString());
            //---------------Test Result -----------------------
            Assert.AreEqual(_validBusinessObject.ID.GetAsValue(), boProp.Value);
            Assert.AreEqual(_validLookupValue, boProp.PropertyValueToDisplay);
        }

        [Test]
        public void Test_BO_SetPropertyValue_ValidDisplayValueString()
        {
            MyBO.LoadDefaultClassDef();
            IBusinessObject businessObject = GetBusinessObjectStub();
            BOProp boProp = (BOProp) businessObject.Props[_propDef_guid.PropertyName];
            object originalPropValue = Guid.NewGuid();
            boProp.Value = originalPropValue;
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNotNull(boProp.Value);
            //---------------Execute Test ----------------------
            businessObject.SetPropertyValue(boProp.PropertyName, _validLookupValue);
            //---------------Test Result -----------------------
            Assert.AreEqual(_validBusinessObject.ID.GetAsValue(), boProp.Value);
            Assert.AreEqual(_validLookupValue, boProp.PropertyValueToDisplay);
        }

        [Test]
        public void Test_BO_SetPropertyValue_ValidGuidString_NotInList()
        {
            MyBO.LoadDefaultClassDef();
            IBusinessObject businessObject = GetBusinessObjectStub();
            BOProp boProp = (BOProp) businessObject.Props[_propDef_guid.PropertyName];
            object originalPropValue = Guid.NewGuid();
            boProp.Value = originalPropValue;
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNotNull(boProp.Value);
            //---------------Execute Test ----------------------
            businessObject.SetPropertyValue
                (boProp.PropertyName, _validBusinessObject_NotInList.ID.GetAsValue().ToString());
            //---------------Test Result -----------------------
            Assert.AreEqual(_validBusinessObject_NotInList.ID.GetAsValue(), boProp.Value);
            string expectedErrorMessage = String.Format
                ("{0}' invalid since '{1}' is not in the lookup list of available values.", boProp.DisplayName,
                 boProp.Value);
            StringAssert.Contains(expectedErrorMessage, boProp.InvalidReason);
            Assert.IsFalse(boProp.IsValid);
            StringAssert.Contains(expectedErrorMessage, businessObject.Status.IsValidMessage);
            Assert.IsFalse(businessObject.Status.IsValid());
        }

        private IBusinessObject GetBusinessObjectStub()
        {
            PropDefCol propDefCol = new PropDefCol {_propDef_guid};

            PrimaryKeyDef def = new PrimaryKeyDef {_propDef_guid};
            ClassDef classDef = new ClassDef(typeof (BusinessObjectStub), def, propDefCol, new KeyDefCol(), null);
            BusinessObjectStub businessObjectStub = new BusinessObjectStub(classDef);
            BOProp prop = new BOPropLookupList(_propDef_guid);
            businessObjectStub.Props.Remove(prop.PropertyName);
            businessObjectStub.Props.Add(prop);
            return businessObjectStub;
        }

        #endregion

        [Test]
        public void TestPropertyValueToDisplay_BusinessObjectLookupList_NotInList()
        {
            MyBO.LoadDefaultClassDef();
            IBusinessObject businessObject = GetBusinessObjectStub();
            BOProp boProp = (BOProp) businessObject.Props[_propDef_guid.PropertyName];
            MyBO bo1 = new MyBO {TestProp = "PropValue"};
            string expectedPropValueToDisplay = bo1.ToString();
            object expctedID = bo1.MyBoID;
            bo1.Save();
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNull(boProp.Value);
            Assert.IsFalse(bo1.Status.IsNew);
            //---------------Execute Test ----------------------
            boProp.Value = expctedID;
            //---------------Test Result -----------------------
            Assert.AreEqual(expctedID, boProp.Value);
            Assert.AreEqual(expectedPropValueToDisplay, boProp.PropertyValueToDisplay);
        }

        [Test]
        public void TestPropertyValueToDisplay_BusinessObjectLookupList_NotInList_NotInDB()
        {
            MyBO.LoadDefaultClassDef();
            IBusinessObject businessObject = GetBusinessObjectStub();
            BOProp boProp = (BOProp) businessObject.Props[_propDef_guid.PropertyName];
            MyBO bo1 = new MyBO {TestProp = "PropValue"};
            object expctedID = bo1.MyBoID;
            //---------------Assert Precondition----------------
            Assert.AreEqual(typeof (Guid), boProp.PropDef.PropertyType);
            Assert.IsNull(boProp.Value);
            Assert.IsTrue(bo1.Status.IsNew);
            //---------------Execute Test ----------------------
            boProp.Value = expctedID;
            //---------------Test Result -----------------------
            Assert.AreEqual(expctedID, boProp.Value);
            Assert.AreEqual(null, boProp.PropertyValueToDisplay);
        }

        #endregion

    }
}