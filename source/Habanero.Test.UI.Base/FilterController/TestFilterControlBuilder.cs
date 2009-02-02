﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gizmox.WebGUI.Forms;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.UI.Base;
using Habanero.UI.VWG;
using Habanero.UI.Win;
using Habanero.Util;
using NUnit.Framework;

namespace Habanero.Test.UI.Base.FilterController
{
    [TestFixture]
    public class TestFilterControlBuilderVWG : TestFilterControlBuilder
    {
        protected override IControlFactory GetControlFactory() { return new ControlFactoryVWG();  }
    }

    [TestFixture]
    public class TestFilterControlBuilder
    {
        protected virtual IControlFactory GetControlFactory() { return new ControlFactoryWin(); }

        [Test]
        public void Test_BuildFilterControl_Simple()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            string propName = TestUtil.CreateRandomString();
            FilterDef filterDef = CreateFilterDef_1Property(propName);

            //---------------Execute Test ----------------------
            IFilterControl filterControl = builder.BuildFilterControl(filterDef);
            
            //---------------Test Result -----------------------
            Assert.IsNotNull(filterControl);
            Assert.AreEqual(FilterModes.Filter, filterControl.FilterMode);
            Assert.AreEqual(1, filterControl.FilterControls.Count);
            Assert.IsNotNull(filterControl.GetChildControl(propName));
            Assert.IsInstanceOfType(typeof(StringTextBoxFilter), filterControl.FilterControls[0]);

            //---------------Tear Down -------------------------          
        }


        [Test]
        public void Test_BuildFilterControl_TwoProperties_CheckPropNames()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            string testprop1 = TestUtil.CreateRandomString();
            string testprop2 = TestUtil.CreateRandomString();
            FilterDef filterDef = CreateFilterDef_2Properties(testprop1, testprop2);

            //---------------Execute Test ----------------------
            IFilterControl filterControl = builder.BuildFilterControl(filterDef);
            
            //---------------Test Result -----------------------
            Assert.AreEqual(2, filterControl.FilterControls.Count);
            Assert.AreEqual(testprop1, filterControl.FilterControls[0].PropertyName);
            Assert.AreEqual(testprop2, filterControl.FilterControls[1].PropertyName);
            
            //---------------Tear Down -------------------------          
        }


        [Test]
        public void Test_BuildFilterControl_TwoProperties_DifferentTypes()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            FilterDef filterDef = CreateFilterDef_2PropertiesWithType("StringTextBoxFilter", "BoolCheckBoxFilter");
            
            //---------------Execute Test ----------------------
            IFilterControl filterControl = builder.BuildFilterControl(filterDef);
            
            //---------------Test Result -----------------------
            Assert.AreEqual(2, filterControl.FilterControls.Count);
            Assert.IsInstanceOfType(typeof(StringTextBoxFilter), filterControl.FilterControls[0]);
            Assert.IsInstanceOfType(typeof(ITextBox), filterControl.FilterControls[0].Control);    
            Assert.IsInstanceOfType(typeof(BoolCheckBoxFilter), filterControl.FilterControls[1]);
            Assert.IsInstanceOfType(typeof(ICheckBox), filterControl.FilterControls[1].Control);
            
            //---------------Tear Down -------------------------          
        }


        [Test]  
        public void Test_BuildFilterControl_AlreadyConstructedFilterControl()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            string filterType = "Habanero.Test.UI.Base.FilterController.SimpleFilter";
            string filterTypeAssembly = "Habanero.Test.UI.Base";
            FilterDef filterDef = CreateFilterDef_1PropertyWithTypeAndAssembly(filterType, filterTypeAssembly);

            //---------------Execute Test ----------------------
            IFilterControl filterControl = GetControlFactory().CreateFilterControl();
            builder.BuildFilterControl(filterDef, filterControl);
            
            //---------------Test Result -----------------------
            Assert.AreEqual(1, filterControl.FilterControls.Count);
            Assert.IsInstanceOfType(typeof(SimpleFilter), filterControl.FilterControls[0]);

            //---------------Tear Down -------------------------          
        }

        [Test]
        public void Test_BuildFilterControl_PreviouslyBuiltFilterControl()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            FilterDef filterDef =CreateFilterDef_1Property();

            //---------------Execute Test ----------------------
            IFilterControl filterControl = GetControlFactory().CreateFilterControl();
            builder.BuildFilterControl(filterDef, filterControl);
            builder.BuildFilterControl(filterDef, filterControl);
            
            //---------------Test Result -----------------------
            Assert.AreEqual(1, filterControl.FilterControls.Count);

            //---------------Tear Down -------------------------          
        }

        [Test]
        public void Test_BuildFilterControl_FilterMode_FilterIsDefault()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            FilterDef filterDef = CreateFilterDef_1Property();
        
            //---------------Execute Test ----------------------
            IFilterControl filterControl = builder.BuildFilterControl(filterDef);

            //---------------Test Result -----------------------
            Assert.AreEqual(FilterModes.Filter, filterControl.FilterMode);
            
            //---------------Tear Down -------------------------          
        }

        [Test]
        public void Test_BuildFilterControl_FilterMode_Search()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            FilterDef filterDef = CreateFilterDef_1Property();
        
            //---------------Execute Test ----------------------
            filterDef.FilterMode = FilterModes.Search;
            IFilterControl filterControl = builder.BuildFilterControl(filterDef);

            //---------------Test Result -----------------------
            Assert.AreEqual(FilterModes.Search, filterControl.FilterMode);
            
            //---------------Tear Down -------------------------          
        }

        [Test]
        public void Test_BuildFilterControl_Layout_0Columns_UsesFlowLayout()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            FilterDef filterDef = CreateFilterDef_1Property();
          
            //---------------Execute Test ----------------------
            filterDef.Columns = 0;
            IFilterControl filterControl = builder.BuildFilterControl(filterDef);

            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof(FlowLayoutManager), filterControl.LayoutManager);

            //---------------Tear Down -------------------------          
        }

        [Test]
        public void Test_BuildFilterControl_Layout_1OrMoreColumns_UsesGridLayout()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            FilterDef filterDef = CreateFilterDef_1Property();
          
            //---------------Execute Test ----------------------
            filterDef.Columns = 3;
            IFilterControl filterControl = builder.BuildFilterControl(filterDef);

            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof(GridLayoutManager), filterControl.LayoutManager);
            GridLayoutManager layoutManager = (GridLayoutManager) filterControl.LayoutManager;
            Assert.AreEqual(6, layoutManager.Columns.Count);
            Assert.AreEqual(1, layoutManager.Rows.Count);

            //---------------Tear Down -------------------------          
        }

        [Test]
        public void Test_BuildFilterControl_Layout_1OrMoreColumns_UsesGridLayout_MoreThanOneRow()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            FilterDef filterDef = CreateFilterDef_3Properties();

            //---------------Execute Test ----------------------
            filterDef.Columns = 2;
            IFilterControl filterControl = builder.BuildFilterControl(filterDef);

            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof(GridLayoutManager), filterControl.LayoutManager);
            GridLayoutManager layoutManager = (GridLayoutManager) filterControl.LayoutManager;
            Assert.AreEqual(4, layoutManager.Columns.Count);
            Assert.AreEqual(2, layoutManager.Rows.Count);

            //---------------Tear Down -------------------------          
        }

        [Test]
        public void Test_BuildCustomFilter_FilterClauseOperator()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());

            const FilterClauseOperator op = FilterClauseOperator.OpLessThanOrEqualTo;
            FilterPropertyDef filterPropertyDef1 = CreateFilterPropertyDef(op);

            //---------------Execute Test ----------------------
            ICustomFilter customFilter = builder.BuildCustomFilter(filterPropertyDef1);

            //---------------Test Result -----------------------
            Assert.AreEqual(op, customFilter.FilterClauseOperator);

            //---------------Tear Down -------------------------          
        }


        [Test]
        public void Test_BuildCustomFilter_SetParametersViaReflection()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());

            Dictionary<string, string> parameters = new Dictionary<string, string> { { "IsChecked", "true" } };

            FilterPropertyDef filterPropertyDef =
                new FilterPropertyDef(TestUtil.CreateRandomString(), TestUtil.CreateRandomString(), "BoolCheckBoxFilter", "", FilterClauseOperator.OpEquals, parameters);
            //---------------Assert PreConditions---------------            
            //---------------Execute Test ----------------------
            ICustomFilter customFilter = builder.BuildCustomFilter(filterPropertyDef);
            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof(BoolCheckBoxFilter), customFilter);
            BoolCheckBoxFilter checkBoxFilter = (BoolCheckBoxFilter)customFilter;
            Assert.IsTrue(checkBoxFilter.IsChecked);
            //---------------Tear Down -------------------------          
        }

        [Test]
        public void Test_BuildCustomFilter_SetParametersViaReflection_InvalidProperty()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());
            string parameterName = TestUtil.CreateRandomString();
            Dictionary<string, string> parameters = new Dictionary<string, string> { { parameterName, TestUtil.CreateRandomString()} };

            string propertyName = TestUtil.CreateRandomString();
            const string filterType = "BoolCheckBoxFilter";
            FilterPropertyDef filterPropertyDef =
                new FilterPropertyDef(propertyName, TestUtil.CreateRandomString(), filterType, "", FilterClauseOperator.OpEquals, parameters);
            //---------------Execute Test ----------------------
            try
            {
                ICustomFilter customFilter = builder.BuildCustomFilter(filterPropertyDef);
                Assert.Fail("Error should have occured because a parameter didn't exist.");

                //---------------Test Result -----------------------
            }
            catch (HabaneroDeveloperException ex)
            {
                StringAssert.Contains(
                    string.Format("The property '{0}' was not found on a filter of type '{1}' for property '{2}'", 
                    parameterName, filterType, propertyName), ex.Message);
            }
        }

        [Test]
        public void Test_BuildCustomFilter_CustomAssembly()
        {
            //---------------Set up test pack-------------------
            FilterControlBuilder builder = new FilterControlBuilder(GetControlFactory());

            FilterPropertyDef filterPropertyDef1 =
                CreateFilterPropertyDefWithType("Habanero.Test.UI.Base.FilterController.SimpleFilter", "Habanero.Test.UI.Base");
            //---------------Execute Test ----------------------
            ICustomFilter customFilter = builder.BuildCustomFilter(filterPropertyDef1);

            //---------------Test Result -----------------------
            Assert.IsInstanceOfType(typeof(SimpleFilter), customFilter);
            //---------------Tear Down -------------------------          
        }


        private static FilterDef CreateFilterDef_1Property()
        {
            return CreateFilterDef_1Property(TestUtil.CreateRandomString());
         
        }
        
        private static FilterDef CreateFilterDef_1Property(string propName)
        {
            return new FilterDef(new List<FilterPropertyDef>() { CreateFilterPropertyDef(propName) });
        }

        private static FilterPropertyDef CreateFilterPropertyDef()
        {
            return CreateFilterPropertyDef(TestUtil.CreateRandomString());
        }

        private static FilterPropertyDef CreateFilterPropertyDef(string propName) {
            return CreateFilterPropertyDef(propName, FilterClauseOperator.OpEquals);
        }    

        private static FilterPropertyDef CreateFilterPropertyDef(FilterClauseOperator op)
        {
            return CreateFilterPropertyDef(TestUtil.CreateRandomString(), op);
        }

        private static FilterPropertyDef CreateFilterPropertyDef(string propName, FilterClauseOperator filterClauseOperator)
        {
            return CreateFilterPropertyDef(propName, "StringTextBoxFilter", "", filterClauseOperator);
        }         
        
        private static FilterPropertyDef CreateFilterPropertyDef(string propName, string filterType, string filterTypeAssembly, FilterClauseOperator filterClauseOperator)
        {
            return new FilterPropertyDef(propName, TestUtil.CreateRandomString(), filterType, filterTypeAssembly, filterClauseOperator, null);
        }  
        
        private static FilterDef CreateFilterDef_1PropertyWithTypeAndAssembly(string filterType, string filterTypeAssembly)
        {
            return new FilterDef(new List<FilterPropertyDef> { CreateFilterPropertyDefWithType(filterType, filterTypeAssembly) });
        }

        private static FilterPropertyDef CreateFilterPropertyDefWithType(string filterType, string filterTypeAssembly)
        {
            return CreateFilterPropertyDef(TestUtil.CreateRandomString(), filterType, filterTypeAssembly,
                                           FilterClauseOperator.OpEquals);
        }


        private static FilterDef CreateFilterDef_2Properties(string propName1, string propName2)
        {
            const string filterType = "StringTextBoxFilter";
            return CreateFilterDef_2Properties(propName1, filterType, propName2, filterType);
        }


        private static FilterDef CreateFilterDef_2PropertiesWithType(string filterType1, string filterType2)
        {
            return CreateFilterDef_2Properties(TestUtil.CreateRandomString(), filterType1, TestUtil.CreateRandomString(), filterType2);
        }

        private static FilterDef CreateFilterDef_2Properties(string propName1, string filterType1, string propName2, string filterType2) {
            return new FilterDef(new List<FilterPropertyDef>()
                                     {
                                         CreateFilterPropertyDef(propName1, filterType1, "", FilterClauseOperator.OpEquals), 
                                         CreateFilterPropertyDef(propName2, filterType2, "", FilterClauseOperator.OpEquals)
                                     });
        }
        
        private static FilterDef CreateFilterDef_3Properties()
        {
            FilterPropertyDef filterPropertyDef1 = CreateFilterPropertyDef();
            FilterPropertyDef filterPropertyDef2 = CreateFilterPropertyDef();
            FilterPropertyDef filterPropertyDef3 = CreateFilterPropertyDef();
            return new FilterDef(new List<FilterPropertyDef> { filterPropertyDef1, filterPropertyDef2, filterPropertyDef3 });
        }

    }
    
    internal class SimpleFilter : ICustomFilter
    {
        private IControlHabanero _textBox;
        public SimpleFilter(IControlFactory controlFactory, string propertyName, FilterClauseOperator filterClauseOperator)
        {
            _textBox = controlFactory.CreateTextBox();
        }
        public IControlHabanero Control { get { return _textBox; } }
        public IFilterClause GetFilterClause(IFilterClauseFactory filterClauseFactory) { throw new System.NotImplementedException(); }
        public void Clear() { throw new System.NotImplementedException(); }
        public event EventHandler ValueChanged;
        public string PropertyName { get { throw new System.NotImplementedException(); } }
        public FilterClauseOperator FilterClauseOperator { get { throw new System.NotImplementedException(); } }
    }
    

}