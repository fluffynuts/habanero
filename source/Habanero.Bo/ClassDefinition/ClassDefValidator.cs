﻿using System;
using System.Collections.Generic;
using Habanero.Base;
using Habanero.Base.Exceptions;

namespace Habanero.BO.ClassDefinition
{
    ///<summary>
    /// This class validates all Class Definitions once they have been loaded.
    /// It can be used to validate Class Definitions regardless of how they are 
    /// loaded (i.e. whether they are loaded via a DB, XML or Reflection (AutoMapping)
    ///</summary>
    public class ClassDefValidator
    {
        private readonly IDefClassFactory _defClassFactory;

        ///<summary>
        /// Constructs a Class Def Validator with a <see cref="IDefClassFactory"/>
        ///</summary>
        ///<param name="defClassFactory"></param>
        public ClassDefValidator(IDefClassFactory defClassFactory) {
            _defClassFactory = defClassFactory;
        }

        ///<summary>
        /// Validates the ClassDefinitions contained in classDefCol
        ///</summary>
        ///<param name="classDefCol"></param>
        public void ValidateClassDefs(ClassDefCol classDefCol)
        {
            UpdateOwningBOHasForeignKey(classDefCol);
            CheckRelationships(classDefCol);
            UpdateKeyDefinitionsWithBoProp(classDefCol);
            UpdatePrimaryKeys(classDefCol);
            //TODO Brett 02 Feb 2010 check valid business object lookup definition i.e. is property valid and is sort direction valid

        }

        private static void UpdateOwningBOHasForeignKey(ClassDefCol classDefCol)
        {
            foreach (IClassDef classDef in classDefCol)
            {
                foreach (IRelationshipDef relationshipDef in classDef.RelationshipDefCol)
                {
                    if (relationshipDef is MultipleRelationshipDef)
                    {
                        relationshipDef.OwningBOHasForeignKey = true;
                    }
                    else if (relationshipDef is SingleRelationshipDef && relationshipDef.OwningBOHasForeignKey)
                    {
                        relationshipDef.OwningBOHasForeignKey = !OwningClassHasPrimaryKey(relationshipDef, classDef, classDefCol);
                        ((SingleRelationshipDef)relationshipDef).OwningBOHasPrimaryKey = OwningClassHasPrimaryKey(relationshipDef, classDef, classDefCol);
                    }
                }
            }
        }


        private static void UpdatePrimaryKeys(IEnumerable<IClassDef> col)
        {
            foreach (IClassDef classDef in col)
            {
                IPrimaryKeyDef primaryKeyDef = classDef.PrimaryKeyDef;
                if (primaryKeyDef == null) continue;
                if (!primaryKeyDef.IsGuidObjectID) continue;
                IPropDef keyPropDef = primaryKeyDef[0];
                if (primaryKeyDef.IsGuidObjectID && (keyPropDef.PropertyType != typeof(Guid)))
                {
                    throw new InvalidXmlDefinitionException("In the class called '" + classDef.ClassNameFull +
                        "', the primary key is set as IsObjectID but the property '" + keyPropDef.PropertyName +
                    "' defined as part of the ObjectID primary key is not a Guid.");
                }
                keyPropDef.Compulsory = true;
                keyPropDef.ReadWriteRule = PropReadWriteRule.WriteNew;
            }
        }

        private void UpdateKeyDefinitionsWithBoProp(ClassDefCol col)
        {
            Dictionary<IClassDef, IPropDefCol> loadedFullPropertyLists = new Dictionary<IClassDef, IPropDefCol>();
            foreach (IClassDef classDef in col)
            {
                UpdateKeyDefinitionsWithBoProp(loadedFullPropertyLists, classDef, col);
            }
        }


        private void UpdateKeyDefinitionsWithBoProp
            (IDictionary<IClassDef, IPropDefCol> loadedFullPropertyLists, IClassDef classDef, ClassDefCol col)
        {
            //This method fixes all the references for a particulare class definitions key definition
            // the issue is that the key definition at the beginiing has a reference to a PropDef that is not
            // valid i.e. does not reference the Prop Def for a particular property.
            // This method attempts to find the actual prop def from the class def and associated it with the keydef.
            if (classDef == null) return;
            IPropDefCol allPropsForAClass = GetAllClassDefProps(loadedFullPropertyLists, classDef, col);
            foreach (IKeyDef keyDef in classDef.KeysCol)
            {
                List<string> propNames = new List<string>();
                foreach (IPropDef propDef in keyDef)
                {
                    propNames.Add(propDef.PropertyName);
                }
                keyDef.Clear();
                //Check Key Properties
                foreach (string propName in propNames)
                {
                    if (!allPropsForAClass.Contains(propName))
                    {
                        throw new InvalidXmlDefinitionException
                            (String.Format
                                 ("In a 'prop' element for the '{0}' key of "
                                  + "the '{1}' class, the propery '{2}' given in the "
                                  + "'name' attribute does not exist for the class or for any of it's superclasses. "
                                  + "Either add the property definition or check the spelling and "
                                  + "capitalisation of the specified property.", keyDef.KeyName,
                                  classDef.ClassName, propName));
                    }
                    IPropDef keyPropDef = allPropsForAClass[propName];
                    keyDef.Add(keyPropDef);
                }
            }
        }

        internal IPropDefCol GetAllClassDefProps
            (IDictionary<IClassDef, IPropDefCol> loadedFullPropertyLists, IClassDef classDef, ClassDefCol col)
        {
            IPropDefCol allProps;
            if (loadedFullPropertyLists.ContainsKey(classDef))
            {
                allProps = loadedFullPropertyLists[classDef];
            }
            else
            {
                allProps = _defClassFactory.CreatePropDefCol();
                IClassDef currentClassDef = classDef;
                while (currentClassDef != null)
                {
                    foreach (IPropDef propDef in currentClassDef.PropDefcol)
                    {
                        if (allProps.Contains(propDef.PropertyName)) continue;
                        allProps.Add(propDef);
                    }
                    currentClassDef = GetSuperClassClassDef(currentClassDef, col);
                }
                loadedFullPropertyLists.Add(classDef, allProps);
            }
            return allProps;
        }



        private static IClassDef GetSuperClassClassDef(IClassDef currentClassDef, ClassDefCol col)
        {
            ISuperClassDef superClassDef = currentClassDef.SuperClassDef;
            return superClassDef == null ? null : col[superClassDef.AssemblyName, superClassDef.ClassName];
        }


        private void CheckRelationships(ClassDefCol classDefs)
        {
            Dictionary<IClassDef, IPropDefCol> loadedFullPropertyLists = new Dictionary<IClassDef, IPropDefCol>();
            foreach (IClassDef classDef in classDefs)
            {
                CheckRelationshipsForAClassDef(loadedFullPropertyLists, classDef, classDefs);
            }
        }



        private void CheckRelationshipsForAClassDef
            (IDictionary<IClassDef, IPropDefCol> loadedFullPropertyLists, IClassDef classDef, ClassDefCol classDefs)
        {
            if (classDef == null) return;

            foreach (IRelationshipDef relationshipDef in classDef.RelationshipDefCol)
            {
                var relatedObjectClassDef = GetRelatedObjectClassDef(classDefs, relationshipDef);
                ValidateReverseRelationship(classDef, relationshipDef, relatedObjectClassDef);
                ValidateRelKeyDef(classDef, classDefs, relationshipDef, relatedObjectClassDef, loadedFullPropertyLists);
            }
        }

        private static IClassDef GetRelatedObjectClassDef(ClassDefCol classDefs, IRelationshipDef relationshipDef)
        {
            IClassDef relatedObjectClassDef;
            try
            {
                relatedObjectClassDef =
                    classDefs[relationshipDef.RelatedObjectAssemblyName, relationshipDef.RelatedObjectClassNameWithTypeParameter];
            }
            catch (HabaneroDeveloperException)
            {
                try
                {
                    relatedObjectClassDef =
                        ClassDef.ClassDefs[relationshipDef.RelatedObjectAssemblyName, relationshipDef.RelatedObjectClassNameWithTypeParameter];
                }
                catch (HabaneroDeveloperException ex)
                {
                    throw new InvalidXmlDefinitionException
                        (string.Format
                             ("The relationship '{0}' could not be loaded because when trying to retrieve its related class the folllowing error was thrown '{1}'",
                              relationshipDef.RelationshipName, ex.Message), ex);
                }
            }
            return relatedObjectClassDef;
        }

        private void ValidateRelKeyDef
            (IClassDef classDef, ClassDefCol classDefs, IRelationshipDef relationshipDef, IClassDef relatedObjectClassDef,
             IDictionary<IClassDef, IPropDefCol> loadedFullPropertyLists)
        {
            IPropDefCol allPropsForClassDef = GetAllClassDefProps(loadedFullPropertyLists, classDef, classDefs);
            IPropDefCol allPropsForRelatedClassDef = GetAllClassDefProps
                (loadedFullPropertyLists, relatedObjectClassDef, classDefs);
            // Check Relationship Properties
            foreach (IRelPropDef relPropDef in relationshipDef.RelKeyDef)
            {
                string ownerPropertyName = relPropDef.OwnerPropertyName;
                if (!allPropsForClassDef.Contains(ownerPropertyName))
                {
                    throw new InvalidXmlDefinitionException
                        (String.Format
                             ("In a 'relatedProperty' element for the '{0}' relationship of "
                              + "the '{1}' class, the property '{2}' given in the "
                              + "'property' attribute does not exist for the class or for any of it's superclasses. "
                              + "Either add the property definition or check the spelling and "
                              +
                              "capitalisation of the specified property. Check in the ClassDefs.xml file or fix in Firestarter",
                              relationshipDef.RelationshipName, classDef.ClassName, ownerPropertyName));
                }
                string relatedClassPropName = relPropDef.RelatedClassPropName;
                if (!allPropsForRelatedClassDef.Contains(relatedClassPropName))
                {
                    throw new InvalidXmlDefinitionException
                        (String.Format
                             ("In a 'relatedProperty' element for the '{0}' relationship of "
                              + "the '{1}' class, the property '{2}' given in the "
                              +
                              "'relatedProperty' attribute does not exist for the Related class '{3}' or for any of it's superclasses. "
                              + "Either add the property definition or check the spelling and "
                              +
                              "capitalisation of the specified property. Check in the ClassDefs.xml file or fix in Firestarter",
                              relationshipDef.RelationshipName, classDef.ClassName, relatedClassPropName,
                              relatedObjectClassDef.ClassNameFull));
                }
            }
        }

        private static void ValidateReverseRelationship
            (IClassDef classDef, IRelationshipDef relationshipDef, IClassDef relatedClassDef)
        {

            if (!HasReverseRelationship(relationshipDef)) return;

            string reverseRelationshipName = relationshipDef.ReverseRelationshipName;
            if (!relatedClassDef.RelationshipDefCol.Contains(reverseRelationshipName))
            {
                throw new InvalidXmlDefinitionException
                    (string.Format
                         ("The relationship '{0}' could not be loaded for because the reverse relationship '{1}' defined for class '{2}' is not defined as a relationship for class '{2}'. Please check your ClassDefs.xml or fix in Firestarter.",
                          relationshipDef.RelationshipName, reverseRelationshipName, relatedClassDef.ClassNameFull));
            }

            var reverseRelationshipDef = relatedClassDef.RelationshipDefCol[reverseRelationshipName];
            CheckReverseRelationshipRelKeyDefProps(relationshipDef, relatedClassDef, reverseRelationshipName, reverseRelationshipDef, classDef);
            //            if (!reverseRelationshipDef.OwningBOHasForeignKey) return;
            //
            //            if (OwningClassHasPrimaryKey(reverseRelationshipDef, relatedClassDef))
            //            {
            //                reverseRelationshipDef.OwningBOHasForeignKey = false;
            //                return;
            //            }
            if (relationshipDef.OwningBOHasForeignKey && reverseRelationshipDef.OwningBOHasForeignKey)
            {
                string errorMessage = string.Format
                    ("The relationship '{0}' could not be loaded because the reverse relationship '{1}' defined for the related class '{2}' and the relationship '{3}' defined for the class '{4}' are both set up as owningBOHasForeignKey = true. Please check your ClassDefs.xml or fix in Firestarter.",
                     relationshipDef.RelationshipName, reverseRelationshipName, relatedClassDef.ClassNameFull,
                     relationshipDef.RelationshipName, classDef.ClassNameFull);
                throw new InvalidXmlDefinitionException(errorMessage);
            }
        }
        /// <summary>
        /// Checks to see if the relationship and reverse relationship are defined for the same relationship.
        /// </summary>
        /// <param name="relationshipDef"></param>
        /// <param name="relatedClassDef"></param>
        /// <param name="reverseRelationshipName"></param>
        /// <param name="reverseRelationshipDef"></param>
        /// <param name="classDef"></param>
        private static void CheckReverseRelationshipRelKeyDefProps(IRelationshipDef relationshipDef, IClassDef relatedClassDef, string reverseRelationshipName, IRelationshipDef reverseRelationshipDef, IClassDef classDef)
        {
            if (!ReverseRelationshipHasSameProps(relationshipDef, reverseRelationshipDef))
            {
                string message = string.Format
                    ("The relationship '{0}' could not be loaded because the reverse relationship '{1}' "
                     + "defined for the related class '{2}' and the relationship '{3}' defined for the class '{4}' do not have the same properties defined as the relationship keys"
                     , relationshipDef.RelationshipName, reverseRelationshipName, relatedClassDef.ClassNameFull,
                     relationshipDef.RelationshipName, classDef.ClassNameFull);
                throw new InvalidXmlDefinitionException(message);
            }
        }

        private static bool ReverseRelationshipHasSameProps(IRelationshipDef relationshipDef, IRelationshipDef reverseRelationshipDef)
        {
            if (relationshipDef.RelKeyDef.Count != reverseRelationshipDef.RelKeyDef.Count) return false;
            foreach (IRelPropDef relPropDef in relationshipDef.RelKeyDef)
            {
                bool foundMatch = false;
                foreach (IRelPropDef reverseRelPropDef in reverseRelationshipDef.RelKeyDef)
                {
                    if (relPropDef.OwnerPropertyName == reverseRelPropDef.RelatedClassPropName
                        && relPropDef.RelatedClassPropName == reverseRelPropDef.OwnerPropertyName)
                    {
                        foundMatch = true;
                        break;
                    }
                }
                if (!foundMatch) return false;
            }
            return true;
        }

        private static bool OwningClassHasPrimaryKey(IRelationshipDef relationshipDef, IClassDef classDef, ClassDefCol classDefCol)
        {
            //For each Property in the Relationship Key check if it is defined as the primary key for the
            //class if it is then check the other properties else this is not a primaryKey
            IPrimaryKeyDef primaryKeyDef = ClassDefHelper.GetPrimaryKeyDef(classDef, classDefCol);
            foreach (IRelPropDef relPropDef in relationshipDef.RelKeyDef)
            {
                bool isInKeyDef = false;
                foreach (IPropDef propDef in primaryKeyDef)
                {
                    if (propDef.PropertyName != relPropDef.OwnerPropertyName)
                    {
                        isInKeyDef = false;
                        break;
                    }
                    isInKeyDef = true;
                }
                if (!isInKeyDef) return false;
            }
            return true;
        }

        private static bool HasReverseRelationship(IRelationshipDef relationshipDef)
        {
            return !string.IsNullOrEmpty(relationshipDef.ReverseRelationshipName);
        }
    }
}
