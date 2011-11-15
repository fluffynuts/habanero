#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
using System;
using System.Collections;
using System.Reflection;
using Habanero.Base.Exceptions;
using NUnit.Framework;

namespace Habanero.Test.Base
{
    /// <summary>
    /// This is an extensible exceptions tester.
    /// If your exception is standard, with four overloaded constructors:
    /// (), (message), (message, inner), (ser,stream), then you only
    /// need to add the new exception to the appropriate list in the setup.
    /// If the assembly is new, add the assembly to the namespaces array
    /// and copy the structure of the other namespaces in the setup method.
    /// 
    /// If your exception has extra constructors or special formatting of
    /// the message, then you'll need to add some extra test methods for
    /// your exception.
    /// </summary>
    [TestFixture]
    public class TestExceptions
    {
        private ArrayList exceptions = new ArrayList();
        private string[,] namespaces = {
                                           //Namespace, Assembly
                                           {"Habanero.Base.Exceptions", "Habanero.Base"},
                                           {"Habanero.BO", "Habanero.BO"},
                                           {"Habanero.DB", "Habanero.DB"}
 
                                       };


        [TestFixtureSetUp]
        public void InitialiseExceptionList()
        {
            string[] baseExceptions =
                {
                    "HabaneroApplicationException",
                    "InvalidObjectIdException",
                    "InvalidXmlDefinitionException",
                    "UserException",
                    "CannotSaveException",
                    "UnknownTypeNameException",
                    "InvalidPropertyException",
                    "InvalidPropertyNameException",
                    "InvalidRelationshipAccessException",
                    "ReflectionException",
                    "InvalidKeyException",
                };

            string[] boExceptions =
                {
                    
                   
                    "InvalidPropertyValueException",
                    
                    "RelationshipNotFoundException",
                    "BusinessObjectNotFoundException",

                    "BusObjectInAnInvalidStateException",
                    "BusObjectConcurrencyControlException", //
                    "BusObjOptimisticConcurrencyControlException", //
                    "BusObjDeleteConcurrencyControlException", //
                    "BusObjBeginEditConcurrencyControlException", //
                    "EditingException", //
                    "BusObjDuplicateConcurrencyControlException" //
                };

            string[] dbExceptions =
                {
                    "DatabaseConnectionException", //
                    "DatabaseReadException", //
                    "DatabaseWriteException" //
                };

            exceptions.Add(baseExceptions);
            exceptions.Add(boExceptions);
            exceptions.Add(dbExceptions);

            if (exceptions.Count > namespaces.GetLength(0))
            {
                throw new HabaneroApplicationException("Not all exception categories " +
                                                       "have a corresponding namespace declaration.");
            }
        }

        [Test]
        public void Test_Construct_HabaneroApplicationException_WithDeveloperMessage_ShouldHaveDevMessage()
        {
            //---------------Set up test pack-------------------
            const string expectedDevMessage = "DeveloperMessage";
            const string expectedUserMessage = "IserMessage";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var habaneroApplicationException = new HabaneroApplicationException(expectedUserMessage, expectedDevMessage);
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedUserMessage, habaneroApplicationException.Message);
            Assert.AreEqual(expectedDevMessage, habaneroApplicationException.DeveloperMessage);
        }

        [Test]
        public void Test_Construct_HabaneroApplicationException_WithDeveloperMessage_WithInnerException_ShouldHaveDevMessage()
        {
            //---------------Set up test pack-------------------
            const string expectedDevMessage = "DeveloperMessage";
            const string expectedUserMessage = "IserMessage";
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var habaneroApplicationException = new HabaneroApplicationException(expectedUserMessage, expectedDevMessage, new Exception());
            //---------------Test Result -----------------------
            Assert.AreEqual(expectedUserMessage, habaneroApplicationException.Message);
            Assert.AreEqual(expectedDevMessage, habaneroApplicationException.DeveloperMessage);
        }


        [Test]
        public void TestParameterless()
        {
            int counter = 0;
            foreach (string[] nspace in exceptions)
            {
                foreach (string s in nspace)
                {
                    Exception ex = (Exception) Activator.CreateInstance(GetType(s, counter), new object[] {});
                    string expected =
                        String.Format("Exception of type '{0}.{1}' was thrown.", namespaces[counter,0], s);
                    Assert.AreEqual(expected, ex.Message);
                }
                counter++;
            }
        }

        [Test]
        public void TestMessage()
        {
            int counter = 0;
            foreach (string[] nspace in exceptions)
            {
                foreach (string s in nspace)
                {
                    Exception ex = (Exception)Activator.CreateInstance(GetType(s, counter), new object[] { "message" });
                    Assert.AreEqual("message", ex.Message);
                }
                counter++;
            }
        }

        [Test]
        public void TestMessageAndInnerException()
        {
            Exception inner = new Exception();
            int counter = 0;
            foreach (string[] nspace in exceptions)
            {
                foreach (string s in nspace)
                {
                    Exception ex = (Exception)Activator.CreateInstance(GetType(s, counter), new object[] { "message", inner });
                    Assert.AreEqual("message", ex.Message);
                    Assert.AreEqual(inner, ex.InnerException);
                }
                counter++;
            }
        }

        [Test]
        public void TestHabaneroArgumentException()
        {
            UserException inner = new UserException();

            HabaneroArgumentException hae = new HabaneroArgumentException("param");
            Assert.AreEqual("The argument 'param' is not valid. ", hae.Message);
            hae = new HabaneroArgumentException("param", "message");
            Assert.AreEqual("The argument 'param' is not valid. message", hae.Message);
            hae = new HabaneroArgumentException("param", "message", inner);
            Assert.AreEqual("The argument 'param' is not valid. message", hae.Message);
            Assert.AreEqual(inner, hae.InnerException);
            hae = new HabaneroArgumentException("param", inner);
            Assert.AreEqual("The argument 'param' is not valid. ", hae.Message);
            Assert.AreEqual(inner, hae.InnerException);
            hae = new HabaneroArgumentException();
            Assert.AreEqual("Exception of type 'Habanero.Base.Exceptions.HabaneroArgumentException' was thrown.", hae.Message);
        }




        private Type GetType(string className, int assemblyNo)
        {
            string assName = Assembly.Load(namespaces[assemblyNo, 1]).ToString();
            string fullclassName = namespaces[assemblyNo, 0] + "." + className;
            Type exType = Type.GetType(fullclassName + ", " + assName);
            if (exType == null)
            {
                throw new TypeLoadException("Could not find: " + fullclassName + ", " + assName);
            }
            return exType;
        }
    }
}