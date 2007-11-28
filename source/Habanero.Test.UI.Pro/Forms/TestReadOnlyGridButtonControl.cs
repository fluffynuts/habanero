using System;
using Habanero.BO.ClassDefinition;
using Habanero.BO;
using Habanero.Base;
using Habanero.Test;
using Habanero.UI.Forms;
using Habanero.UI.Grid;
using NMock;
using NUnit.Framework;
using BusinessObject=Habanero.BO.BusinessObject;

namespace Habanero.Test.UI.Forms
{
    /// <summary>
    /// Summary description for TestReadOnlyGridButtonControl.
    /// </summary>
    [TestFixture]
    public class TestReadOnlyGridButtonControl
    {
        private BusinessObject itsBo;
        private Mock itsGridMock;
        private IReadOnlyGrid itsGrid;
        private ReadOnlyGridButtonControl itsButtons;
        private Mock itsObjectEditorMock;
        private IObjectEditor itsEditor;
        private Mock itsObjectCreatorMock;
        private IObjectCreator itsCreator;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            ClassDef.ClassDefs.Clear();
            itsBo = MyBO.LoadDefaultClassDef().CreateNewBusinessObject();
        }

        [SetUp]
        public void SetupTest()
        {
            itsGridMock = new DynamicMock(typeof (IReadOnlyGrid));
            itsGrid = (IReadOnlyGrid)itsGridMock.MockInstance;
            itsButtons = new ReadOnlyGridButtonControl(itsGrid);
            itsObjectEditorMock = new DynamicMock(typeof (IObjectEditor));
            itsEditor = (IObjectEditor) itsObjectEditorMock.MockInstance;
            itsObjectCreatorMock = new DynamicMock(typeof (IObjectCreator));
            itsCreator = (IObjectCreator) itsObjectCreatorMock.MockInstance;
        }

        [Test]
        public void TestControlCreation()
        {
            Assert.AreEqual(2, itsButtons.Controls.Count);
            Assert.AreEqual("Add", itsButtons.Controls[0].Name);
            Assert.AreEqual("Edit", itsButtons.Controls[1].Name);
        }

        [Test]
        public void TestEditButtonClickSuccessfulEdit()
        {
            itsGridMock.ExpectAndReturn("SelectedBusinessObject", itsBo, new object[] {});
            itsGridMock.ExpectAndReturn("UIName", "default", new object[] { });
            itsObjectEditorMock.ExpectAndReturn("EditObject", true, new object[] { itsBo, "default" });
            //itsGridMock.Expect("RefreshRow", new object[] { itsBo }) ;
            itsButtons.ObjectEditor = itsEditor;

            itsButtons.ClickButton("Edit");
            itsObjectEditorMock.Verify();
            itsGridMock.Verify();
        }

        [Test]
        public void TestEditButtonClickUnsuccessfulEdit()
        {
            itsGridMock.ExpectAndReturn("SelectedBusinessObject", itsBo, new object[] {});
            itsGridMock.ExpectAndReturn("UIName", "default", new object[] { });
            itsObjectEditorMock.ExpectAndReturn("EditObject", false, new object[] { itsBo, "default" });
            //itsGridMock.ExpectNoCall("RefreshRow", new Type[] {typeof(object)});
            itsButtons.ObjectEditor = itsEditor;

            itsButtons.ClickButton("Edit");
            itsObjectEditorMock.Verify();
            itsGridMock.Verify();
        }

        [Test]
        public void TestEditButtonClickNothingSelected()
        {
            itsGridMock.ExpectAndReturn("SelectedBusinessObject", null, new object[] {});
            itsObjectEditorMock.ExpectNoCall("EditObject", new Type[] {typeof (object), typeof(string)});
            //itsGridMock.ExpectNoCall("RefreshRow", new Type[] {typeof(object)});
            itsButtons.ObjectEditor = itsEditor;

            itsButtons.ClickButton("Edit");
            itsObjectEditorMock.Verify();
            itsGridMock.Verify();
        }

        [Test]
        public void TestAddButtonClickSuccessfulAdd()
        {
            itsGridMock.ExpectAndReturn("UIName", "default", new object[]{} );
            itsObjectCreatorMock.ExpectAndReturn("CreateObject", itsBo, new object[] {itsEditor, null, "default"});
            itsGridMock.Expect("AddBusinessObject", new object[] {itsBo});
            itsButtons.ObjectCreator = itsCreator;
            itsButtons.ObjectEditor = itsEditor;

            itsButtons.ClickButton("Add");
            itsObjectCreatorMock.Verify();
            itsGridMock.Verify();
        }

        [Test]
        public void TestAddButtonClickUnsuccessfulAdd()
        {
            itsGridMock.ExpectAndReturn("UIName", "default", new object[] { });
            itsObjectCreatorMock.ExpectAndReturn("CreateObject", null, new object[] {itsEditor, null, "default"});
            itsGridMock.ExpectNoCall("AddBusinessObject", new Type[] {typeof (object)});
            itsButtons.ObjectCreator = itsCreator;
            itsButtons.ObjectEditor = itsEditor;

            itsButtons.ClickButton("Add");
            itsObjectCreatorMock.Verify();
            itsGridMock.Verify();
        }
    }
}