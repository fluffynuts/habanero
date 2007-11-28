//---------------------------------------------------------------------------------
// Copyright (C) 2007 Chillisoft Solutions
// 
// This file is part of Habanero Standard.
// 
//     Habanero Standard is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Habanero Standard is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Lesser General Public License for more details.
// 
//     You should have received a copy of the GNU Lesser General Public License
//     along with Habanero Standard.  If not, see <http://www.gnu.org/licenses/>.
//---------------------------------------------------------------------------------

using Habanero.BO.ClassDefinition;
using Habanero.BO.Loaders;
using NUnit.Framework;

namespace Habanero.Test.BO.Loaders
{
    /// <summary>
    /// Summary description for TestXmlUIColllectionsLoader.
    /// </summary>
    [TestFixture]
    public class TestXmlUILoader
    {
        private XmlUILoader itsLoader;

        [SetUp]
        public void Setup()
        {
            itsLoader = new XmlUILoader();
        }

        [Test]
        public void TestLoadWithJustForm()
        {
            UIDef def =
                itsLoader.LoadUIDef(
                    @"
				<ui name=""defTestName1"">
					<form>
						<tab name=""testtab"">
							<columnLayout>
								<field label=""testlabel1"" property=""testpropname1"" type=""Button"" mapperType=""testmappertypename1"" />
								<field label=""testlabel2"" property=""testpropname2"" type=""Button"" mapperType=""testmappertypename2"" />
							</columnLayout>
						</tab>
					</form>
				</ui> 
							");
            Assert.IsNotNull(def.UIForm);
            Assert.AreEqual(1, def.UIForm.Count);
        }

        [Test]
        public void TestLoadWithJustGrid()
        {
            UIDef def =
                itsLoader.LoadUIDef(
                    @"
				<ui name=""defTestName1"">
					<grid>
						<column heading=""testheading1"" property=""testpropname1""  />
						<column heading=""testheading2"" property=""testpropname2""  />
						<column heading=""testheading3"" property=""testpropname3""  />
					</grid>
				</ui> 
							");
            Assert.IsNotNull(def.UIGrid);
            Assert.AreEqual(3, def.UIGrid.Count);
        }

        [Test]
        public void TestLoadWithBothGridAndForm()
        {
            UIDef def =
                itsLoader.LoadUIDef(
                    @"
				<ui name=""defTestName1"">
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
							");
            Assert.IsNotNull(def.UIForm);
            Assert.AreEqual(1, def.UIForm.Count);
            Assert.IsNotNull(def.UIGrid);
            Assert.AreEqual(3, def.UIGrid.Count);
        }

        [Test]
        public void TestName()
        {
            UIDef def =
                itsLoader.LoadUIDef(
                    @"
				<ui name=""defTestName1"">
					<grid>
						<column heading=""testheading1"" property=""testpropname1""  />
						<column heading=""testheading2"" property=""testpropname2""  />
						<column heading=""testheading3"" property=""testpropname3""  />
					</grid>
				</ui> ");
            Assert.AreEqual("defTestName1", def.Name);
        }
    }
}