using System.Windows.Forms;
using Habanero.Bo.Loaders;
using Habanero.Base;
using Habanero.Ui.Grid;
using NUnit.Framework;

namespace Habanero.Test.Bo.Loaders
{
    /// <summary>
    /// Summary description for TestXmlUIPropertyLoader.
    /// </summary>
    [TestFixture]
    public class TestXmlUIGridPropertyLoader
    {
        private XmlUIGridPropertyLoader loader;

        [SetUp]
        public void SetupTest()
        {
            loader = new XmlUIGridPropertyLoader();
        }

        [Test]
        public void TestSimpleUIProperty()
        {
            UIGridProperty uiProp =
                loader.LoadUIProperty(
                    @"<column heading=""testheading"" property=""testpropname"" type=""DataGridViewCheckBoxColumn"" width=""40"" />");
            Assert.AreEqual("testheading", uiProp.Heading);
            Assert.AreEqual("testpropname", uiProp.PropertyName);
            Assert.AreEqual(40, uiProp.Width);
            Assert.AreSame(typeof (DataGridViewCheckBoxColumn), uiProp.GridControlType);
        }

        [Test]
        public void TestDefaults()
        {
            UIGridProperty uiProp =
                loader.LoadUIProperty(@"<column heading=""testheading"" property=""testpropname"" />");
            Assert.AreSame(typeof (DataGridViewTextBoxColumn), uiProp.GridControlType);
            Assert.AreEqual(100, uiProp.Width);
        }

        [Test]
        public void TestAssemblyAttributeForSystem()
        {
            UIGridProperty uiProp =
                loader.LoadUIProperty(@"<column heading=""testheading"" property=""testpropname"" assembly=""System.Windows.Forms"" />");
            Assert.AreEqual(typeof(DataGridViewTextBoxColumn), uiProp.GridControlType);
        }

        //This test is breaking
//        [Test]
//        public void TestAssemblyAttributeForHabaneroTypes()
//        {
//            UIGridProperty uiProp =
//                loader.LoadUIProperty(@"<column heading=""testheading"" property=""testpropname"" type=""DataGridViewNumericUpDownColumn"" />");
//            Assert.AreEqual(typeof(DataGridViewNumericUpDownColumn), uiProp.GridControlType);
//        }

        [Test]
        public void TestCustomColumnType()
        {
            UIGridProperty uiProp =
                loader.LoadUIProperty(@"<column heading=""testheading"" property=""testpropname"" type=""MyBo"" assembly=""Habanero.Test"" />");
            Assert.AreEqual(typeof(MyBo), uiProp.GridControlType);
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestInvalidAssemblyAttribute()
        {
            loader.LoadUIProperty(@"<column heading=""testheading"" property=""testpropname"" assembly=""testx"" />");
        }

        [Test, ExpectedException(typeof(InvalidXmlDefinitionException))]
        public void TestInvalidColumnType()
        {
            loader.LoadUIProperty(@"<column heading=""testheading"" property=""testpropname"" type=""testx"" assembly=""System.Windows.Forms"" />");
        }
    }
}