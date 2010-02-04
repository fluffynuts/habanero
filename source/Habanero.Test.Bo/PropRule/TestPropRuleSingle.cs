using System;
using Habanero.Base;
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using NUnit.Framework;

namespace Habanero.Test.BO
{
    [TestFixture]
    public class TestPropRuleSingle
    {
        [Test]
        public void TestSingleRule()
        {
            PropRuleSingle rule = new PropRuleSingle("num", "TestMessage", 5.32f, 10.11111f);

            string errorMessage = "";

            //Test less than min
            Assert.IsFalse(rule.IsPropValueValid("Propname", 5.31116f, ref errorMessage));
            Assert.IsTrue(errorMessage.Length > 0);
            //Test valid data
            Assert.IsTrue(rule.IsPropValueValid("Propname", 6, ref errorMessage));
            Assert.IsFalse(errorMessage.Length > 0);
            //test greater than max
            Assert.IsFalse(rule.IsPropValueValid("Propname", 10.1111112f, ref errorMessage));
            Assert.IsTrue(errorMessage.Length > 0);

            rule = new PropRuleSingle("num", "TestMessage", 5.32f, 10.11111f);
            errorMessage = "";

            Assert.IsTrue(rule.IsPropValueValid("Propname", null, ref errorMessage));
            Assert.IsTrue(errorMessage.Length == 0);
            errorMessage = "";
            Assert.IsFalse(rule.IsPropValueValid("Propname", -53444.33222f, ref errorMessage));
            Assert.IsTrue(errorMessage.Length > 0);
        }
        protected virtual IDefClassFactory GetDefClassFactory()
        {
            return new DefClassFactory();
        }


        [Test]
        public void TestPropRuleSingle_MaxValue_ActualValueLT()
        {
            XmlRuleLoader loader = new XmlRuleLoader(new DtdLoader(), GetDefClassFactory());
            IPropRule rule = loader.LoadRule(typeof(Single).Name,
                @"<rule name=""TestSingle""  >
                            <add key=""min"" value=""12.22"" />
                            <add key=""max"" value=""15.51"" />
                        </rule>                          
");
            //-----------------Assert Preconditions ---------------------------
            Assert.AreEqual(12.22f, ((PropRuleSingle)rule).MinValue);
            Assert.AreEqual(15.51f, ((PropRuleSingle)rule).MaxValue);

            //---------------Execute ------------------------------------------
            string errorMessage = "";
            bool isValid = rule.IsPropValueValid("Propname", 13.1f, ref errorMessage);

            //--------------Verify Result -------------------------------------
            Assert.IsTrue(isValid);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [Test]
        public void TestPropRuleSingle_MaxValue_ActualValueEquals()
        {
            XmlRuleLoader loader = new XmlRuleLoader(new DtdLoader(), GetDefClassFactory());
            IPropRule rule = loader.LoadRule(typeof(Single).Name,
                @"<rule name=""TestSingle""  >
                            <add key=""min"" value=""12.22"" />
                            <add key=""max"" value=""15.51"" />
                        </rule>                          
");
            //-----------------Assert Preconditions ---------------------------
            Assert.AreEqual(12.22f, ((PropRuleSingle)rule).MinValue);
            Assert.AreEqual(15.51f, ((PropRuleSingle)rule).MaxValue);

            //---------------Execute ------------------------------------------
            string errorMessage = "";
            bool isValid = rule.IsPropValueValid("Propname", 15.51f, ref errorMessage);

            //--------------Verify Result -------------------------------------
            Assert.IsTrue(isValid);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
        [Test]
        public void TestPropRuleSingle_MaxValue_ActualValueGT()
        {
            XmlRuleLoader loader = new XmlRuleLoader(new DtdLoader(), GetDefClassFactory());
            IPropRule rule = loader.LoadRule(typeof(Single).Name,
                @"<rule name=""TestSingle""  >
                            <add key=""min"" value=""12.22"" />
                            <add key=""max"" value=""15.51"" />
                        </rule>                          
");
            //-----------------Assert Preconditions ---------------------------
            Assert.AreEqual(12.22f, ((PropRuleSingle)rule).MinValue);
            Assert.AreEqual(15.51f, ((PropRuleSingle)rule).MaxValue);

            //---------------Execute ------------------------------------------
            string errorMessage = "";
            bool isValid = rule.IsPropValueValid("Propname", 15.56f, ref errorMessage);

            //--------------Verify Result -------------------------------------
            Assert.IsFalse(isValid);
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
        }
    }
}