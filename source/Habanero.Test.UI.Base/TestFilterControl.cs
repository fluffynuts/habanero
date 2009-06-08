using System.Collections;
using Habanero.Base;
using Habanero.UI.Base;
using Habanero.UI.WebGUI;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Base
{
    [TestFixture]
    public class TestFilterControl
    {
        [SetUp]
        public void SetupTest()
        {
            //Runs every time that any testmethod is executed
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            //Code that is executed before any test is run in this class. If multiple tests
            // are executed then it will still only be called once.
        }

        [TearDown]
        public void TearDownTest()
        {
            //runs every time any testmethod is complete
        }

        #region TextBoxFilter

        #region TestAddTextBox

        [Test]
        public void TestAddTextBoxGizmox()
        {
            TestAddTextBox(new GizmoxControlFactory());
        }

        [Test]
        public void TestAddTestBoxWinForms()
        {
            TestAddTextBox(new WinControlFactory());
        }

        public void TestAddTextBox(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterControl ctl = factory.CreateFilterControl();

            //---------------Execute Test ----------------------
            ITextBox myTextBox = ctl.AddStringFilterTextBox("", "");

            //---------------Test Result -----------------------
            Assert.IsNotNull(myTextBox);

            //---------------Tear Down -------------------------          
        }

        #endregion

        #region TestAddStringFilterTextBox

        [Test]
        public void TestAddStringFilterTextBoxWinForms()
        {
            TestAddStringFilterTextBox(new WinControlFactory());
        }

        [Test]
        public void TestAddStringFilterTextBoxGiz()
        {
            TestAddStringFilterTextBox(new GizmoxControlFactory());
        }

        public void TestAddStringFilterTextBox(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClause nullClause = new DataViewNullFilterClause();
            IFilterControl filterControl = factory.CreateFilterControl();
            //---------------Execute Test ----------------------
            ITextBox tb = filterControl.AddStringFilterTextBox("Test:", "TestColumn");
            tb.Text = "";
            //---------------Test Result -----------------------
            Assert.AreEqual(nullClause.GetFilterClauseString(), filterControl.GetFilterClause().GetFilterClauseString());

            //---------------Tear Down -------------------------          
        }

        #endregion


        [Test]
        public void TestGetTextBoxFilterClauseWinForms()
        {
            TestGetTextBoxFilterClause(new WinControlFactory());
        }

        [Test]
        public void TestGetTextBoxFilterClauseGiz()
        {
            TestGetTextBoxFilterClause(new GizmoxControlFactory());
        }

        public void TestGetTextBoxFilterClause(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClauseFactory itsFilterClauseFactory = new DataViewFilterClauseFactory();
            IFilterControl filterControl = factory.CreateFilterControl();
            ITextBox tb = filterControl.AddStringFilterTextBox("Test:", "TestColumn");

            //---------------Execute Test ----------------------
            tb.Text = "testvalue";
            string filterClauseString = filterControl.GetFilterClause().GetFilterClauseString();

            //---------------Test Result -----------------------
            IFilterClause clause =
                itsFilterClauseFactory.CreateStringFilterClause("TestColumn", FilterClauseOperator.OpLike, "testvalue");
            Assert.AreEqual(clause.GetFilterClauseString(), filterClauseString);

            //---------------Tear Down -------------------------          
        }

        [Test]
        public void TestTwoStringTextBoxFilterWinForms()
        {
            TestTwoStringTextBoxFilter(new WinControlFactory());
        }

        [Test]
        public void TestTwoStringTextBoxFilterGiz()
        {
            TestTwoStringTextBoxFilter(new GizmoxControlFactory());
        }

        public void TestTwoStringTextBoxFilter(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClauseFactory itsFilterClauseFactory = new DataViewFilterClauseFactory();
            IFilterControl filterControl = factory.CreateFilterControl();
            ITextBox tb = filterControl.AddStringFilterTextBox("Test:", "TestColumn");
            tb.Text = "testvalue";
            ITextBox tb2 = filterControl.AddStringFilterTextBox("Test:", "TestColumn2");
            tb2.Text = "testvalue2";

            //---------------Execute Test ----------------------
            string filterClauseString = filterControl.GetFilterClause().GetFilterClauseString();

            //---------------Test Result -----------------------
            IFilterClause clause1 =
                itsFilterClauseFactory.CreateStringFilterClause("TestColumn", FilterClauseOperator.OpLike, "testvalue");
            IFilterClause clause2 =
                itsFilterClauseFactory.CreateStringFilterClause("TestColumn2", FilterClauseOperator.OpLike, "testvalue2");
            IFilterClause fullClause =
                itsFilterClauseFactory.CreateCompositeFilterClause(clause1, FilterClauseCompositeOperator.OpAnd, clause2);
            Assert.AreEqual(fullClause.GetFilterClauseString(), filterClauseString);

            //---------------Tear Down -------------------------          
        }


        [Test]
        public void TestLabelAndTextBoxAreOnPanelWinForms()
        {
            TestLabelAndTextBoxAreOnPanel(new WinControlFactory(), 2);
        }

        [Test]
        public void TestLabelAndTextBoxAreOnPanelGiz()
        {
            TestLabelAndTextBoxAreOnPanel(new GizmoxControlFactory(), 4);
        }

        public void TestLabelAndTextBoxAreOnPanel(IControlFactory factory, int controlCount)
        {
            //---------------Set up test pack-------------------
            IFilterControl filterControl = factory.CreateFilterControl();

            //---------------Execute Test ----------------------
            ITextBox tb = filterControl.AddStringFilterTextBox("Test:", "TestColumn");

            //---------------Test Result -----------------------
            Assert.AreEqual(controlCount, filterControl.Controls.Count);
            Assert.Contains(tb, filterControl.Controls);
            //---------------Tear Down -------------------------          
        }

        #endregion

        //------------------------COMBO BOX----------------------------------------------------------

        #region ComboBoxFilter

        [Test]
        public void TestAddComboBoxGizmox()
        {
            TestAddComboBox(new GizmoxControlFactory());
        }

        [Test]
        public void TestAddComboBoxWinForms()
        {
            TestAddComboBox(new WinControlFactory());
        }

        public void TestAddComboBox(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            //IFilterClause nullClause = new DataViewNullFilterClause();
            IFilterControl filterControl = factory.CreateFilterControl();
            //---------------Execute Test ----------------------
            IComboBox cb = filterControl.AddStringFilterComboBox("t", "TestColumn", new ArrayList(), true);

            //---------------Test Result -----------------------
            Assert.IsNotNull(cb);

            //---------------Tear Down -------------------------          
        }


        [Test]
        public void TestAddStringFilterComboBoxGiz()
        {
            TestAddStringFilterComboBox(new GizmoxControlFactory());
        }
        [Test]
        public void TestAddStringFilterComboBoxWinForms()
        {
            TestAddStringFilterComboBox(new WinControlFactory());
        }
        public void TestAddStringFilterComboBox(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClause nullClause = new DataViewNullFilterClause();
            IFilterControl filterControl = factory.CreateFilterControl();
            //---------------Execute Test ----------------------
            filterControl.AddStringFilterComboBox("Test:", "TestColumn", new ArrayList(), true);
            //---------------Test Result -----------------------
            Assert.AreEqual(nullClause.GetFilterClauseString(), filterControl.GetFilterClause().GetFilterClauseString());

            //---------------Tear Down -------------------------          
        }

        [Test]
        public void TestGetComboBoxAddSelectedItemsGiz()
        {
            TestGetComboBoxAddSelectedItems(new GizmoxControlFactory());
        }
        public void TestGetComboBoxAddSelectedItemsWin()
        {
            TestGetComboBoxAddSelectedItems(new WinControlFactory());
        }
        public void TestGetComboBoxAddSelectedItems(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterControl filterControl = factory.CreateFilterControl();
            IList options = new ArrayList();
            options.Add("1");
            options.Add("2");
            //---------------Execute Test ----------------------
            IComboBox comboBox = filterControl.AddStringFilterComboBox("Test:", "TestColumn", options, true);
            //---------------Test Result -----------------------
            int numOfItemsInCollection = 2;
            int numItemsExpectedInComboBox = numOfItemsInCollection + 1;//one extra for the null selected item
            Assert.AreEqual(numItemsExpectedInComboBox, comboBox.Items.Count);
        }

        [Test]
        public void TestSelectItemWinForms()
        {
            TestSelectItem(new WinControlFactory());
        }
        [Test]
        public void TestSelectItemGiz()
        {
            TestSelectItem(new GizmoxControlFactory());
        }

        public void TestSelectItem(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterControl filterControl = factory.CreateFilterControl();
            IList options = new ArrayList();
            options.Add("1");
            options.Add("2");
            IComboBox comboBox = filterControl.AddStringFilterComboBox("Test:", "TestColumn", options, true);
            //---------------Execute Test ----------------------
            comboBox.SelectedIndex = 1;
            //---------------Test Result -----------------------
            Assert.AreEqual("1", comboBox.SelectedItem.ToString());
            //---------------Tear Down -------------------------          
        }
        [Test]
        public void TestGetComboBoxFilterClauseWinForms()
        {
            TestGetComboBoxFilterClause(new WinControlFactory());
        }

        [Test]
        public void TestGetComboBoxFilterClauseGiz()
        {
            TestGetComboBoxFilterClause(new GizmoxControlFactory());
        }

        public void TestGetComboBoxFilterClause(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClauseFactory filterClauseFactory = new DataViewFilterClauseFactory();
            IFilterControl filterControl = factory.CreateFilterControl();
            IComboBox comboBox = GetFilterComboBox_2Items(filterControl);

            //---------------Execute Test ----------------------
            comboBox.SelectedIndex = 1;
            string filterClauseString = filterControl.GetFilterClause().GetFilterClauseString();

            //---------------Test Result -----------------------
            IFilterClause clause =
                filterClauseFactory.CreateStringFilterClause("TestColumn", FilterClauseOperator.OpEquals, "1");
            Assert.AreEqual(clause.GetFilterClauseString(), filterClauseString);

            //---------------Tear Down -------------------------          
        }

        [Test]
        public void TestGetComboBoxFilterClauseNoSelectionWinForms()
        {
            TestGetComboBoxFilterClauseNoSelection(new WinControlFactory());
        }
        [Test]
        public void TestGetComboBoxFilterClauseNoSelectionGiz()
        {
            TestGetComboBoxFilterClauseNoSelection(new GizmoxControlFactory());
        }

        public void TestGetComboBoxFilterClauseNoSelection(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClauseFactory filterClauseFactory = new DataViewFilterClauseFactory();
            IFilterControl filterControl = factory.CreateFilterControl();
            IComboBox comboBox = GetFilterComboBox_2Items(filterControl);
            //---------------Execute Test ----------------------
            comboBox.SelectedIndex = -1;
            string filterClauseString = filterControl.GetFilterClause().GetFilterClauseString();
            //---------------Test Result -----------------------
            IFilterClause clause = filterClauseFactory.CreateNullFilterClause();
            Assert.AreEqual(clause.GetFilterClauseString(), filterClauseString);
            //---------------Tear Down -------------------------          
        }

        [Test]
        public void TestGetComboBoxFilterClause_SelectDeselectWinForms()
        {
            TestGetComboBoxFilterClause_SelectDeselect(new WinControlFactory());
        }
        [Test]
        public void TestGetComboBoxFilterClause_SelectDeselectGiz()
        {
            TestGetComboBoxFilterClause_SelectDeselect(new GizmoxControlFactory());
        }

        public void TestGetComboBoxFilterClause_SelectDeselect(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClauseFactory filterClauseFactory = new DataViewFilterClauseFactory();
            IFilterControl filterControl = factory.CreateFilterControl();
            IComboBox comboBox = GetFilterComboBox_2Items(filterControl);
            //---------------Execute Test ----------------------
            comboBox.SelectedIndex = 1;
            comboBox.SelectedIndex = -1;
            string filterClauseString = filterControl.GetFilterClause().GetFilterClauseString();
            //---------------Test Result -----------------------
            IFilterClause nullClause = filterClauseFactory.CreateNullFilterClause();
            Assert.AreEqual(nullClause.GetFilterClauseString(), filterClauseString);
            //---------------Tear Down -------------------------          
        }

        #endregion

        [Test]
        public void TestMultipleFiltersWinForms()
        {
            MultipleFilters(new WinControlFactory());
        }
        [Test]
        public void TestMultipleFiltersGiz()
        {
            MultipleFilters(new GizmoxControlFactory());
        }

        public void MultipleFilters(IControlFactory factory)
        {
            //---------------Set up test pack-------------------
            IFilterClauseFactory filterClauseFactory = new DataViewFilterClauseFactory();
            IFilterControl filterControl = factory.CreateFilterControl();
            ITextBox tb = filterControl.AddStringFilterTextBox("Test:", "TestColumn");
            tb.Text = "testvalue";
            IFilterClause clause =
                filterClauseFactory.CreateStringFilterClause("TestColumn", FilterClauseOperator.OpLike, "testvalue");

            ITextBox tb2 = filterControl.AddStringFilterTextBox("Test2:", "TestColumn2");
            tb2.Text = "testvalue2";
            //---------------Execute Test ----------------------

            string filterClause = filterControl.GetFilterClause().GetFilterClauseString();
            //---------------Test Result -----------------------
            IFilterClause clause2 =
                filterClauseFactory.CreateStringFilterClause("TestColumn2", FilterClauseOperator.OpLike, "testvalue2");

            IFilterClause compositeClause =
                filterClauseFactory.CreateCompositeFilterClause(clause, FilterClauseCompositeOperator.OpAnd, clause2);
            
            Assert.AreEqual(compositeClause.GetFilterClauseString(),
                            filterClause);
            //---------------Tear Down ------------------------- 
        }

        private static IComboBox GetFilterComboBox_2Items(IFilterControl filterControl)
        {
            IList options = new ArrayList();
            options.Add("1");
            options.Add("2");
            return filterControl.AddStringFilterComboBox("Test:", "TestColumn", options, true);
        }

//
//        [Test]
//        public void TestAddStringFilterTextBoxTextChanged()
//        {
//            itsIsFilterClauseChanged = false;
//            filterControl.SetAutomaticUpdate(true);
//            filterControl.FilterClauseChanged += FilterClauseChangedHandler;
//            TextBox tb = filterControl.AddStringFilterTextBox("Test:", "TestColumn");
//            Assert.IsTrue(itsIsFilterClauseChanged, "Adding a new control should make the filter clause change");
//            itsIsFilterClauseChanged = false;
//            tb.Text = "change";
//            Assert.IsTrue(itsIsFilterClauseChanged, "Changing the text should make the filter clause change");
//        }
//
//        private void FilterClauseChangedHandler(object sender, FilterControlEventArgs e)
//        {
//            itsIsFilterClauseChanged = true;
//        }
//
//
//        [Test]
//        public void TestAddStringFilterComboBoxTextChanged()
//        {
//            IList options = new ArrayList();
//            options.Add("1");
//            options.Add("2");
//            itsIsFilterClauseChanged = false;
//            filterControl.FilterClauseChanged += FilterClauseChangedHandler;
//            ComboBox cb = filterControl.AddStringFilterComboBox("Test:", "TestColumn", options, true);
//            Assert.IsTrue(itsIsFilterClauseChanged, "Adding a new control should make the filter clause change");
//            itsIsFilterClauseChanged = false;
//            cb.SelectedIndex = 0;
//            Assert.IsTrue(itsIsFilterClauseChanged, "Changing the selected item should make the filter clause change");
//        }
//
//        [Test]
//        public void TestAddBooleanFilterCheckBox()
//        {
//            CheckBox cb = filterControl.AddStringFilterCheckBox("Test?", "TestColumn", true);
//            IFilterClause clause =
//                itsFilterClauseFactory.CreateStringFilterClause("TestColumn", FilterClauseOperator.OpEquals, "true");
//            Assert.AreEqual(clause.GetFilterClauseString(), filterControl.GetFilterClause().GetFilterClauseString());
//            cb.Checked = false;
//            clause =
//                itsFilterClauseFactory.CreateStringFilterClause("TestColumn", FilterClauseOperator.OpEquals, "false");
//            Assert.AreEqual(clause.GetFilterClauseString(), filterControl.GetFilterClause().GetFilterClauseString());
//        }
//
//        [Test]
//        public void TestAddStringFilterDateTimeEditor()
//        {
//            DateTime testDate = DateTime.Now;
//            filterControl.AddStringFilterDateTimeEditor("test:", "testcolumn", testDate, true);
//            filterControl.AddStringFilterDateTimeEditor("test:", "testcolumn", testDate, false);
//            IFilterClause clause1 =
//                itsFilterClauseFactory.CreateStringFilterClause("testcolumn", FilterClauseOperator.OpGreaterThanOrEqualTo, testDate.ToString("yyyy/MM/dd"));
//            IFilterClause clause2 =
//                itsFilterClauseFactory.CreateStringFilterClause("testcolumn", FilterClauseOperator.OpLessThanOrEqualTo, testDate.ToString("yyyy/MM/dd"));
//            IFilterClause compClause =
//                itsFilterClauseFactory.CreateCompositeFilterClause(clause1, FilterClauseCompositeOperator.OpAnd, clause2);
//            Assert.AreEqual(compClause.GetFilterClauseString(), filterControl.GetFilterClause().GetFilterClauseString());
//        }
//
//        [Test]
//        public void TestAddDateFilterDateTimePicker()
//        {
//            DateTime testDate = DateTime.Now;
//            filterControl.AddDateFilterDateTimePicker("test:", "testcolumn", testDate, FilterClauseOperator.OpGreaterThan, true);
//            filterControl.AddDateFilterDateTimePicker("test:", "testcolumn", testDate, FilterClauseOperator.OpEquals, false);
//            IFilterClause clause1 = itsFilterClauseFactory.CreateDateFilterClause("testcolumn", FilterClauseOperator.OpGreaterThan, new DateTime(testDate.Year, testDate.Month, testDate.Day));
//            IFilterClause clause2 = itsFilterClauseFactory.CreateDateFilterClause("testcolumn", FilterClauseOperator.OpEquals, testDate);
//            IFilterClause compClause =
//                itsFilterClauseFactory.CreateCompositeFilterClause(clause1, FilterClauseCompositeOperator.OpAnd, clause2);
//            Assert.AreEqual(compClause.GetFilterClauseString(), filterControl.GetFilterClause().GetFilterClauseString());
//        }
//
//        [Test]
//        public void TestAddDateRangeFilterComboBoxInclusive()
//        {
//            DateTime testDate = new DateTime(2007, 1, 2, 3, 4, 5, 6);
//            DateRangeComboBox dr1 = filterControl.AddDateRangeFilterComboBox("test", "test", true, true);
//            dr1.UseFixedNowDate = true;
//            dr1.FixedNowDate = testDate;
//            dr1.SelectedItem = "Today";
//
//            IFilterClause clause1 = itsFilterClauseFactory.CreateDateFilterClause("test", FilterClauseOperator.OpGreaterThanOrEqualTo, new DateTime(2007, 1, 2, 0, 0, 0));
//            IFilterClause clause2 = itsFilterClauseFactory.CreateDateFilterClause("test", FilterClauseOperator.OpLessThanOrEqualTo, new DateTime(2007, 1, 2, 3, 4, 5));
//            //IFilterClause clause1 = itsFilterClauseFactory.CreateStringFilterClause("test", FilterClauseOperator.OpGreaterThanOrEqualTo, "2007/01/02 12:00:00 AM");
//            //IFilterClause clause2 = itsFilterClauseFactory.CreateStringFilterClause("test", FilterClauseOperator.OpLessThanOrEqualTo, "2007/01/02 03:04:05 AM");
//            IFilterClause compClause = itsFilterClauseFactory.CreateCompositeFilterClause(clause1, FilterClauseCompositeOperator.OpAnd, clause2);
//            Assert.AreEqual(compClause.GetFilterClauseString(), filterControl.GetFilterClause().GetFilterClauseString());
//        }
//
//        [Test]
//        public void TestAddDateRangeFilterComboBoxExclusive()
//        {
//            DateTime testDate = new DateTime(2007, 1, 2, 3, 4, 5, 6);
//            DateRangeComboBox dr1 = filterControl.AddDateRangeFilterComboBox("test", "test", false, false);
//            dr1.UseFixedNowDate = true;
//            dr1.FixedNowDate = testDate;
//            dr1.SelectedItem = "Today";
//
//            IFilterClause clause1 = itsFilterClauseFactory.CreateDateFilterClause("test", FilterClauseOperator.OpGreaterThan, new DateTime(2007, 1, 2, 0, 0, 0));
//            IFilterClause clause2 = itsFilterClauseFactory.CreateDateFilterClause("test", FilterClauseOperator.OpLessThan, new DateTime(2007, 1, 2, 3, 4, 5));
//            //IFilterClause clause1 = itsFilterClauseFactory.CreateStringFilterClause("test", FilterClauseOperator.OpGreaterThan, "2007/01/02 12:00:00 AM");
//            //IFilterClause clause2 = itsFilterClauseFactory.CreateStringFilterClause("test", FilterClauseOperator.OpLessThan, "2007/01/02 03:04:05 AM");
//            IFilterClause compClause = itsFilterClauseFactory.CreateCompositeFilterClause(clause1, FilterClauseCompositeOperator.OpAnd, clause2);
//            Assert.AreEqual(compClause.GetFilterClauseString(), filterControl.GetFilterClause().GetFilterClauseString());
//        }
//
//        [Test]
//        public void TestAddDateRangeFilterComboBoxCollectionOverload()
//        {
//            List<DateRangeComboBox.DateOptions> options = new List<DateRangeComboBox.DateOptions>();
//            options.Add(DateRangeComboBox.DateOptions.Today);
//            options.Add(DateRangeComboBox.DateOptions.Yesterday);
//            DateRangeComboBox testDRComboBox2 = filterControl.AddDateRangeFilterComboBox("test", "test", options, true, false);
//            Assert.AreEqual(2, testDRComboBox2.OptionsToDisplay.Count);
//        }
//
//        [Test]
//        public void TestMultipleFilters()
//        {
//            TextBox tb = filterControl.AddStringFilterTextBox("Test:", "TestColumn");
//            tb.Text = "testvalue";
//            IFilterClause clause =
//                itsFilterClauseFactory.CreateStringFilterClause("TestColumn", FilterClauseOperator.OpLike, "testvalue");
//
//            TextBox tb2 = filterControl.AddStringFilterTextBox("Test2:", "TestColumn2");
//            tb2.Text = "testvalue2";
//            IFilterClause clause2 =
//                itsFilterClauseFactory.CreateStringFilterClause("TestColumn2", FilterClauseOperator.OpLike, "testvalue2");
//
//            IFilterClause compositeClause =
//                itsFilterClauseFactory.CreateCompositeFilterClause(clause, FilterClauseCompositeOperator.OpAnd, clause2);
//            Assert.AreEqual(compositeClause.GetFilterClauseString(),
//                            filterControl.GetFilterClause().GetFilterClauseString());
//        }

//
    }

}