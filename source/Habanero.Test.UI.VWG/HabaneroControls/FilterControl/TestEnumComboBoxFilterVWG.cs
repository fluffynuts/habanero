using Habanero.Test.UI.Base.FilterController;
using Habanero.UI.Base;
using Habanero.UI.VWG;
using NUnit.Framework;

namespace Habanero.Test.UI.VWG.HabaneroControls
{
    [TestFixture]
    public class TestEnumComboBoxFilterVWG : TestEnumComboBoxFilter
    {
        protected override IControlFactory GetControlFactory() { return new ControlFactoryVWG(); }
    }
}