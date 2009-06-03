//---------------------------------------------------------------------------------
// Copyright (C) 2008 Chillisoft Solutions
// 
// This file is part of the Habanero framework.
// 
//     Habanero is a free framework: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     The Habanero framework is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using System;
using Habanero.UI.Base;
using Habanero.UI.VWG;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Base
{
    /// <summary>
    /// This test class tests the base inherited methods of the Button class.
    /// </summary>
    [TestFixture]
    public class TestBaseMethodsWin_Button : TestBaseMethods.TestBaseMethodsWin
    {
        protected override IControlHabanero CreateControl()
        {
            return GetControlFactory().CreateButton();
        }
    }

    /// <summary>
    /// This test class tests the base inherited methods of the Button class.
    /// </summary>
    [TestFixture]
    public class TestBaseMethodsVWG_Button : TestBaseMethods.TestBaseMethodsVWG
    {
        protected override IControlHabanero CreateControl()
        {
            return GetControlFactory().CreateButton();
        }
    }

    /// <summary>
    /// This test class tests the Button class.
    /// </summary>
    public abstract class TestButton
    {
        protected abstract IControlFactory GetControlFactory();

        [TestFixture]
        public class TestButtonWin : TestButton
        {
            protected override IControlFactory GetControlFactory()
            {
                return new ControlFactoryWin();
            }
        }

        [TestFixture]
        public class TestButtonVWG : TestButton
        {
            protected override IControlFactory GetControlFactory()
            {
                return new ControlFactoryVWG();
            }
        }

        [Test]
        public void TestCreateButton()
        {
            //---------------Set up test pack-------------------
            //---------------Execute Test ----------------------
            IButton myButton = GetControlFactory().CreateButton();

            //---------------Test Result -----------------------
            Assert.IsNotNull(myButton);

            //---------------Tear Down -------------------------   
        }

        [Test]
        public void Test_PerformClick()
        {
            //---------------Set up test pack-------------------
            IButton button = this.GetControlFactory().CreateButton();
            bool clicked = false;
            button.Click += delegate(object sender, EventArgs e)
            {
                clicked = true;
            };
            //AddControlToForm(button);
            //-------------Assert Preconditions -------------
            Assert.IsFalse(clicked);
            //---------------Execute Test ----------------------
            button.PerformClick();
            //---------------Test Result -----------------------
            Assert.IsTrue(clicked);
        }
    }
}