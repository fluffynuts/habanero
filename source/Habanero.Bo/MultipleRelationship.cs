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

using System;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO.ClassDefinition;

namespace Habanero.BO
{
    /// <summary>
    /// Manages a relationship where the relationship owner relates to several
    /// other objects
    /// </summary>
    public class MultipleRelationship : Relationship
    {
        //private BusinessObjectCollection<BusinessObject> _boCol;

        /// <summary>
        /// Constructor to initialise a new relationship
        /// </summary>
        /// <param name="owningBo">The business object that owns the
        /// relationship</param>
        /// <param name="lRelDef">The relationship definition</param>
        /// <param name="lBOPropCol">The set of properties used to
        /// initialise the RelKey object</param>
        public MultipleRelationship(IBusinessObject owningBo, RelationshipDef lRelDef, BOPropCol lBOPropCol)
            : base(owningBo, lRelDef, lBOPropCol)
        {
        }

        protected override IBusinessObjectCollection GetRelatedBusinessObjectColInternal<TBusinessObject>()
        {
            //TODO: Need a strategy for what should be happening here when the collection is previously loaded.
            //I would suggest option 1
            //1) The collection is reloaded from the database as is currently being done.
            //2) The collection is is returned
            if(_boCol != null)
            {
                BORegistry.DataAccessor.BusinessObjectLoader.Refresh((BusinessObjectCollection<TBusinessObject>)_boCol);
                return _boCol;
            }

            Type relatedBusinessObjectType = _relDef.RelatedObjectClassType;
            Type genericType = typeof (TBusinessObject);

            CheckTypeCanBeCreated(relatedBusinessObjectType);

            CheckTypeIsASubClassOfGenericType<TBusinessObject>(relatedBusinessObjectType, genericType);

            IBusinessObjectCollection boCol = BORegistry.DataAccessor.BusinessObjectLoader.GetRelatedBusinessObjectCollection<TBusinessObject>(this);

            if (_relDef.KeepReferenceToRelatedObject)
            {
                _boCol = boCol;
            }
            return boCol;
        }

        private static void CheckTypeIsASubClassOfGenericType<TBusinessObject>(Type type, Type collectionItemType)
        {
            if (!(type == collectionItemType || type.IsSubclassOf(collectionItemType)))
            {
                throw new HabaneroArgumentException(String.Format(
                                                        "An error occurred while attempting to load a related " +
                                                        "business object collection of type '{0}' into a " +
                                                        "collection of the specified generic type('{1}').",
                                                        type, typeof(TBusinessObject)));
            }
        }

        private static void CheckTypeCanBeCreated(Type type)
        {
            //Check that the type can be created and raise appropriate error 
            try
            {
                Activator.CreateInstance(type, true);
            }
            catch (Exception ex)
            {
                throw new UnknownTypeNameException(String.Format(
                                                       "An error occurred while attempting to load a related " +
                                                       "business object collection, with the type given as '{0}'. " +
                                                       "Check that the given type exists and has been correctly " +
                                                       "defined in the relationship and class definitions for the classes " +
                                                       "involved.", type), ex);
            }
        }
    }
}