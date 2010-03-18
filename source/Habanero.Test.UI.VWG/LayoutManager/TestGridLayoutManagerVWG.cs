using Habanero.Test.UI.Base;
using Habanero.UI.Base;
using Habanero.UI.VWG;
using NUnit.Framework;

namespace Habanero.Test.UI.VWG.LayoutManager
{
    [TestFixture]
    public class TestGridLayoutManagerVWG : TestGridLayoutManager
    {
        protected override IControlFactory GetControlFactory()
        {
            return new ControlFactoryVWG();
        }
    }
}