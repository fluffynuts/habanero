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

using System.Xml;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using NUnit.Framework;

namespace Habanero.Test.BO.Loaders
{
    /// <summary>
    /// Summary description for TestXmlClassDefsLoader.
    /// </summary>
    [TestFixture]
    public class TestXmlClassDefsLoader
    {
        [TestFixtureSetUp]
        public void SetupFixture()
        {
            ClassDef.ClassDefs.Clear();
        }

        [Test]
        public void TestLoadClassDefs()
        {
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            ClassDefCol classDefList =
                loader.LoadClassDefs(
                    @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
						</class>
					</classes>
			");
            Assert.AreEqual(2, classDefList.Count);
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestClass"), "Class 'TestClass' should have been loaded.");
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestRelatedClass"), "Class 'TestRelatedClass' should have been loaded.");
        }

        [Test]
        public void TestLoadClassDefs_InheritedClassWithNoPrimaryKey()
        {
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            ClassDefCol classDefList =
                loader.LoadClassDefs(
                    @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
						</class>
						<class name=""TestClass2"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClass2ID"" />
                            <primaryKey>
                                <prop name=""TestClass2ID""/>
                            </primaryKey>
						</class>
						<class name=""TestClassInherited"" assembly=""Habanero.Test.BO.Loaders"" >							
                            <superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" />
						</class>
					</classes>
			");
            Assert.AreEqual(3, classDefList.Count);
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestClass"), "Class 'TestClass' should have been loaded.");
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestClass2"), "Class 'TestClass2' should have been loaded.");
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestClassInherited"), "Class 'TestClassInherited' should have been loaded.");
            ClassDef classDefTestClass = classDefList["Habanero.Test.BO.Loaders", "TestClass"];
            ClassDef classDefInherited = classDefList["Habanero.Test.BO.Loaders", "TestClassInherited"];
            Assert.IsNotNull(classDefTestClass);
            Assert.IsNotNull(classDefInherited.SuperClassDef);
            Assert.IsNull(classDefInherited.PrimaryKeyDef);
        }

        [Test]
        public void TestLoadClassDefs_KeyDefinedWithInheritedProperties()
        {
            //-------------Setup Test Pack ------------------
            const string xml = @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
							<property  name=""TestClassName"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
						</class>
						<class name=""TestClassInherited"" assembly=""Habanero.Test.BO.Loaders"" >							
                            <superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" />
                            <key>
                                <prop name=""TestClassName""/>
                            </key>
						</class>
					</classes>
			";
            //-------------Execute test ---------------------
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            ClassDefCol classDefList = loader.LoadClassDefs(xml);
            //-------------Test Result ----------------------
            Assert.AreEqual(2, classDefList.Count);
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestClass"), "Class 'TestClass' should have been loaded.");
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestClassInherited"), "Class 'TestClassInherited' should have been loaded.");
            ClassDef classDefTestClass = classDefList["Habanero.Test.BO.Loaders", "TestClass"];
            IPropDef propDef = classDefTestClass.PropDefcol["TestClassName"];
            ClassDef classDefInherited = classDefList["Habanero.Test.BO.Loaders", "TestClassInherited"];
            Assert.IsNotNull(classDefInherited.SuperClassDef);
            Assert.AreEqual(1, classDefInherited.KeysCol.Count);
            KeyDef keyDef = classDefInherited.KeysCol.GetKeyDefAtIndex(0);
            IPropDef keyDefPropDef = keyDef["TestClassName"];
            Assert.AreSame(propDef, keyDefPropDef, "The key's property should have been resolved to be the property of the superclass by the loader.");
        }

        [Test]
        public void TestLoadClassDefs_KeyDefinedWithNonExistantProperty()
        {
            //-------------Setup Test Pack ------------------
            const string xml = @"
				<classes>
					<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
						<property  name=""TestClassID"" />
						<property  name=""TestClassName"" />
                        <primaryKey>
                            <prop name=""TestClassID""/>
                        </primaryKey>
					</class>
					<class name=""TestClassInherited"" assembly=""Habanero.Test.BO.Loaders"" >							
                        <superClass class=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" />
                        <key>
                            <prop name=""DoesNotExist""/>
                        </key>
					</class>
				</classes>
			";
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            //-------------Execute test ---------------------
            try
            {
                loader.LoadClassDefs(xml);
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (InvalidXmlDefinitionException ex)
            {
                StringAssert.Contains("In a 'prop' element for the '' key of the 'TestClassInherited' class, the propery 'DoesNotExist' given in the 'name' attribute does not exist for the class or for any of it's superclasses. Either add the property definition or check the spelling and capitalisation of the specified property", ex.Message);
            }
        }
        [Test]
        public void Test_Invalid_Relationship_PropDefDoesNotExistOnOwningClassDef()
        {
            //----------------------Test Setup ----------------------
            const string classDefsString = @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
					        <relationship name=""TestRelatedClass"" type=""single"" relatedClass=""TestRelatedClass"" relatedAssembly=""Habanero.Test.BO.Loaders"">
						        <relatedProperty property=""PropDefDoesNonExist"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
						</class>
					</classes>
			";
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            //--------------------Execute Test-------------------------
            try
            {
                loader.LoadClassDefs(classDefsString);
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (InvalidXmlDefinitionException ex)
            {
                StringAssert.Contains("The property 'PropDefDoesNonExist' defined "
                          + "in the 'relatedProperty' element in its 'Property' " 
                          + "attribute, which specifies the property in the class " 
                          + "'TestClass' from which the relationship 'TestRelatedClass' will link is not defined in the " 
                          + "class 'TestClass'.", ex.Message);
            }
        }
        [Test]
        public void Test_Invalid_Relationship_PropDefForReverseRelationshipNotSameAsRelationship()
        {
            //----------------------Test Setup ----------------------
            const string classDefsString = @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
							<property  name=""OtherFieldID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
					        <relationship name=""TestRelatedClass"" type=""single"" relatedClass=""TestRelatedClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" owningBOHasForeignKey=""true"" reverseRelationship=""TestClass"">
						        <relatedProperty property=""OtherFieldID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
					        <relationship name=""TestClass"" type=""single"" relatedClass=""TestClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" reverseRelationship=""TestRelatedClass"" owningBOHasForeignKey=""false"" >
						        <relatedProperty property=""TestClassID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
					</classes>
			";
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            //--------------------Execute Test-------------------------
            try
            {
                loader.LoadClassDefs(classDefsString);
                Assert.Fail("expected Err");
            }
                //---------------Test Result -----------------------
            catch (InvalidXmlDefinitionException ex)
            {
                StringAssert.Contains("do not have the same properties defined as the relationship keys", ex.Message);
            }
        }

        [Test]
        public void Test_Invalid_Relationship_PropDefDoesNotExistOnRelatedClassDef()
        {
            //----------------------Test Setup ----------------------
            const string classDefsString = @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
					        <relationship name=""TestRelatedClass"" type=""single"" relatedClass=""TestRelatedClass"" relatedAssembly=""Habanero.Test.BO.Loaders"">
						        <relatedProperty property=""TestClassID"" relatedProperty=""PropDefDoesNonExist"" />
					        </relationship>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
						</class>
					</classes>
			";
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            //--------------------Execute Test-------------------------
            try
            {
                loader.LoadClassDefs(classDefsString);
                Assert.Fail("expected Err");
            }
            //---------------Test Result -----------------------
            catch (InvalidXmlDefinitionException ex)
            {
                StringAssert.Contains("In a 'relatedProperty' element " 
                    + "for the 'TestRelatedClass' relationship of the 'TestClass' " 
                    + "class, the property 'PropDefDoesNonExist'", ex.Message);
            }
        }

        [Test]
        public void Test_Invalid_Relationship_SingleSingleRelationships_BothSetAsOwning()
        {
            //----------------------Test Setup ----------------------
            const string classDefsString = @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
							<property  name=""ForeignKeyProp"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
					        <relationship name=""TestRelatedClass"" type=""single"" relatedClass=""TestRelatedClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" owningBOHasForeignKey=""true"" reverseRelationship=""TestClass"">
						        <relatedProperty property=""ForeignKeyProp"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
					        <relationship name=""TestClass"" type=""single"" relatedClass=""TestClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" owningBOHasForeignKey=""true"" reverseRelationship=""TestRelatedClass"">
						        <relatedProperty property=""TestClassID"" relatedProperty=""ForeignKeyProp"" />
					        </relationship>
						</class>
					</classes>
			";
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            //--------------------Execute Test-------------------------
            try
            {
                loader.LoadClassDefs(classDefsString);
                Assert.Fail("expected Err");
            }
            //---------------Test Result -----------------------
            catch (InvalidXmlDefinitionException ex)
            {
                StringAssert.Contains("The relationship 'TestRelatedClass' could not be loaded because the reverse relationship 'TestClass' ", ex.Message);
                StringAssert.Contains("are both set up as owningBOHasForeignKey = true", ex.Message);
            }
        }
        [Test]
        public void Test_Valid_Relationship_SingleSingleRelationships_OnlyOneHasOwningBOHasForeignKey()
        {
            //----------------------Test Setup ----------------------
            const string classDefsString = @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
					        <relationship name=""TestRelatedClass"" type=""single"" relatedClass=""TestRelatedClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" owningBOHasForeignKey=""true"" reverseRelationship=""TestClass"">
						        <relatedProperty property=""TestClassID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
					        <relationship name=""TestClass"" type=""single"" relatedClass=""TestClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" reverseRelationship=""TestRelatedClass"" owningBOHasForeignKey=""false"" >
						        <relatedProperty property=""TestClassID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
					</classes>
			";
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            //--------------------Execute Test-------------------------
            ClassDefCol classDefList = loader.LoadClassDefs(classDefsString);
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDefList.Count);
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestClass"), "Class 'TestClass' should have been loaded.");
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestRelatedClass"), "Class 'TestRelatedClass' should have been loaded.");

        }
        [Test]
        public void Test_Valid_Relationship_SingleSingleRelationships_CanDetermine_OwningBOHasForeignKey()
        {
            //----------------------Test Setup ----------------------
            const string classDefsString = @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
					        <relationship name=""TestRelatedClass"" type=""single"" relatedClass=""TestRelatedClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" reverseRelationship=""TestClass"">
						        <relatedProperty property=""TestClassID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
					        <relationship name=""TestClass"" type=""single"" relatedClass=""TestClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" reverseRelationship=""TestRelatedClass"" >
						        <relatedProperty property=""TestClassID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
					</classes>
			";
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            //--------------------Execute Test-------------------------
            ClassDefCol classDefList = loader.LoadClassDefs(classDefsString);
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDefList.Count);
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestClass"), "Class 'TestClass' should have been loaded.");
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestRelatedClass"), "Class 'TestRelatedClass' should have been loaded.");

        }
        [Test]
        public void Test_Valid_Relationship_SingleSingleRelationships_CanDetermine_OwningBOHasForeignKey_SecondClass()
        {
            //----------------------Test Setup ----------------------
            const string classDefsString = @"
					<classes>

						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
					        <relationship name=""TestClass"" type=""single"" relatedClass=""TestClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" reverseRelationship=""TestRelatedClass"" >
						        <relatedProperty property=""TestClassID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
					        <relationship name=""TestRelatedClass"" type=""single"" relatedClass=""TestRelatedClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" reverseRelationship=""TestClass"">
						        <relatedProperty property=""TestClassID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
					</classes>
			";
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            //--------------------Execute Test-------------------------
            ClassDefCol classDefList = loader.LoadClassDefs(classDefsString);
            //---------------Test Result -----------------------
            Assert.AreEqual(2, classDefList.Count);
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestClass"), "Class 'TestClass' should have been loaded.");
            Assert.IsTrue(classDefList.Contains("Habanero.Test.BO.Loaders", "TestRelatedClass"), "Class 'TestRelatedClass' should have been loaded.");

        }

        [Test]
        public void Test_Invalid_Relationship_SingleSingleRelationships_ReverseRelationshipNotDefined()
        {
            //----------------------Test Setup ----------------------
            const string classDefsString = @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
					        <relationship name=""TestRelatedClass"" type=""single"" relatedClass=""TestRelatedClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" owningBOHasForeignKey=""true"" reverseRelationship=""ReverseRelNonExist"">
						        <relatedProperty property=""TestClassID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
						<class name=""TestRelatedClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestRelatedClassID"" />
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestRelatedClassID""/>
                            </primaryKey>
					        <relationship name=""TestClass"" type=""single"" relatedClass=""TestClass"" relatedAssembly=""Habanero.Test.BO.Loaders"" owningBOHasForeignKey=""true"" reverseRelationship=""TestRelatedClass"">
						        <relatedProperty property=""TestClassID"" relatedProperty=""TestClassID"" />
					        </relationship>
						</class>
					</classes>
			";
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            //--------------------Execute Test-------------------------
            try
            {
                loader.LoadClassDefs(classDefsString);
                Assert.Fail("expected Err");
            }
            //---------------Test Result -----------------------
            catch (InvalidXmlDefinitionException ex)
            {
                StringAssert.Contains("The relationship 'TestRelatedClass' could not be loaded for because " 
                        + "the reverse relationship 'ReverseRelNonExist'", ex.Message);
            }
        }
        [Test]
        public void Test_Invalid_Relationship_RelatedClassDefDoesNotExist()
        {
            //----------------------Test Setup ----------------------
            const string classDefsString = @"
					<classes>
						<class name=""TestClass"" assembly=""Habanero.Test.BO.Loaders"" >
							<property  name=""TestClassID"" />
                            <primaryKey>
                                <prop name=""TestClassID""/>
                            </primaryKey>
					        <relationship name=""TestRelatedClass"" type=""single"" relatedClass=""RelatedClassDoesNotExist"" relatedAssembly=""Habanero.Test.BO.Loaders"">
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
					</classes>
			";
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            //--------------------Execute Test-------------------------
            try
            {
                loader.LoadClassDefs(classDefsString);
                Assert.Fail("expected Err");
            }
            //---------------Test Result -----------------------
            catch (InvalidXmlDefinitionException ex)
            {
                StringAssert.Contains("The relationship 'TestRelatedClass' could not be loaded for because when trying to retrieve its related class the folllowing", ex.Message);
            }
        }
        [Test, ExpectedException(typeof(XmlException))]
        public void TestNoRootNodeException()
        {
            XmlClassDefsLoader loader = new XmlClassDefsLoader();
            loader.LoadClassDefs(@"<invalidRootNode>");
        }
    }


}