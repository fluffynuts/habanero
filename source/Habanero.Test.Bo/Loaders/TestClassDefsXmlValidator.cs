using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Habanero.Base.Exceptions;
using Habanero.BO.Loaders;
using NUnit.Framework;

namespace Habanero.Test.BO.Loaders
{
    [TestFixture]
    public class TestClassDefsXmlValidator
    {

        [Test]
        public void TestValidateValidXml()
        {
            //---------------Set up test pack-------------------
            string xml =
                @"<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
					        <relationship name=""TestRelatedClass"" type=""single"" relatedClass=""TestRelatedClass"" relatedAssembly=""Habanero.Test.BO.Loaders"">
						        <relatedProperty property=""TestClassID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
						</class>
					</classes>";
            ClassDefsXmlValidator validator = new ClassDefsXmlValidator();

            //---------------Execute Test ----------------------
            XmlValidationResult validationResult = validator.ValidateClassDefsXml(xml);

            //---------------Test Result -----------------------

            Assert.IsTrue(validationResult.IsValid);

            //---------------Tear Down -------------------------          
        }

        //TODO andrew 03 Jan 2011: CF: Test fails because there is no DTD validation 
        [Test]
        public void TestValidateInvalidXml()
        {
            //---------------Set up test pack-------------------
            string xml =
                @"<classes bob=""Asdf"">
						<bob />
					</classes>";
            ClassDefsXmlValidator validator = new ClassDefsXmlValidator();

            //---------------Execute Test ----------------------
            XmlValidationResult validationResult = validator.ValidateClassDefsXml(xml);

            //---------------Test Result -----------------------

            Assert.IsFalse(validationResult.IsValid);
            //---------------Tear Down -------------------------          
        }

        [Test]
        public void TestValidateUIXml_Valid()
        {
            //---------------Set up test pack-------------------
            string xml =
                @"<ui name=""defTestName1"">
					<grid>
						<column heading=""testheading1"" property=""testpropname1""  />
						<column heading=""testheading2"" property=""testpropname2""  />
						<column heading=""testheading3"" property=""testpropname3""  />
					</grid>
					<form>
						<tab name=""testtab"">
							<columnLayout>
								<field label=""testlabel1"" property=""testpropname1"" type=""Button"" mapperType=""testmappertypename1"" />
								<field label=""testlabel2"" property=""testpropname2"" type=""Button"" mapperType=""testmappertypename2"" />
							</columnLayout>
						</tab>
					</form>
				</ui> 
";
            ClassDefsXmlValidator validator = new ClassDefsXmlValidator();

            //---------------Execute Test ----------------------
            XmlValidationResult validationResult = validator.ValidateClassDefsXml(xml);

            //---------------Test Result -----------------------

            Assert.IsTrue(validationResult.IsValid);

            //---------------Tear Down -------------------------          
        }

        //TODO andrew 03 Jan 2011: CF: Test fails because there is no DTD validation 
        [Test]
        public void TestValidateUIXml_Invalid()
        {
            //---------------Set up test pack-------------------
            string xml =
                @"<ui name=""defTestName1"">
						<bob />
					</ui> ";
            ClassDefsXmlValidator validator = new ClassDefsXmlValidator();

            //---------------Execute Test ----------------------
            XmlValidationResult validationResult = validator.ValidateClassDefsXml(xml);

            //---------------Test Result -----------------------

            Assert.IsFalse(validationResult.IsValid);
            //---------------Tear Down -------------------------          
        }
    }

    

 
}
