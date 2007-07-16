using Habanero.Bo;
using Habanero.Bo.ClassDefinition;
using NUnit.Framework;

namespace Habanero.Test.Bo
{
    [TestFixture]
    public class TestBoKey
    {
        private BOPropCol mBOPropCol1 = new BOPropCol();
        private KeyDef mKeyDef1 = new KeyDef();

        private BOPropCol mBOPropCol2 = new BOPropCol();
        private KeyDef mKeyDef2 = new KeyDef();

        [SetUp]
        public void init()
        {
            //Props for KeyDef 1
            PropDef lPropDef = new PropDef("PropName", typeof(string), PropReadWriteRule.ReadOnly, null);
            mBOPropCol1.Add(lPropDef.CreateBOProp(false));
            mKeyDef1.Add(lPropDef);

            lPropDef = new PropDef("PropName1", typeof(string), PropReadWriteRule.ReadOnly, null);
            mBOPropCol1.Add(lPropDef.CreateBOProp(false));
            mKeyDef1.Add(lPropDef);

            //Props for KeyDef 2

            lPropDef = new PropDef("PropName1", typeof(string), PropReadWriteRule.ReadOnly, null);
            mBOPropCol2.Add(lPropDef.CreateBOProp(false));
            mKeyDef2.Add(lPropDef);

            lPropDef = new PropDef("PropName", typeof(string), PropReadWriteRule.ReadOnly, null);
            mBOPropCol2.Add(lPropDef.CreateBOProp(false));
            mKeyDef2.Add(lPropDef);
        }

        [Test]
        public void TestBOKeyEqual()
        {
            //Set values for Key1
            BOKey lBOKey1 = mKeyDef1.CreateBOKey(mBOPropCol1);
            BOProp lProp = mBOPropCol1["PropName"];
            lProp.Value = "Prop Value";

            lProp = mBOPropCol1["PropName1"];
            lProp.Value = "Value 2";

            //Set values for Key2
            BOKey lBOKey2 = mKeyDef2.CreateBOKey(mBOPropCol2);
            lProp = mBOPropCol2["PropName"];
            lProp.Value = "Prop Value";

            lProp = mBOPropCol2["PropName1"];
            lProp.Value = "Value 2";

            //Assert.AreEqual(lBOKey1, lBOKey2);
            Assert.IsTrue(lBOKey1 == lBOKey2);

            Assert.AreEqual(lBOKey1.GetHashCode(), lBOKey2.GetHashCode());
        }

        public void TestSortedValues()
        {
            BOKey lBOKey1 = mKeyDef1.CreateBOKey(mBOPropCol2);
            BOProp lProp = mBOPropCol2["PropName"];
            Assert.AreSame(lProp, lBOKey1.SortedValues[0]);
        }
    }
}