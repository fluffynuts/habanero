﻿using Habanero.Test.UI.Base;
using Habanero.UI.Base;
using Habanero.UI.Win;
using NUnit.Framework;

namespace Habanero.Test.UI.Win.Selectors
{
    /// <summary>
    /// This test class tests the ListBoxSelector class.
    /// </summary>
    [TestFixture]
    public class TestListBoxSelectorWin : TestListBoxSelector
    {
        protected override IControlFactory GetControlFactory()
        {
            ControlFactoryWin factory = new ControlFactoryWin();
            GlobalUIRegistry.ControlFactory = factory;
            return factory;
        }
    }
}