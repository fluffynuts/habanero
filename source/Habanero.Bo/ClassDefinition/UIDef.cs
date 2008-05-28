//---------------------------------------------------------------------------------
// Copyright (C) 2007 Chillisoft Solutions
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

namespace Habanero.BO.ClassDefinition
{
    /// <summary>
    /// Defines how the properties of a class are displayed in a user
    /// interface, as specified in the class definitions xml file.
    /// This consists of definitions for a grid display and an editing form.
    /// </summary>
    public class UIDef
    {
        private string _name;
        private UIForm _uiForm;
        private UIGrid _uiGrid;

        /// <summary>
        /// Constructor to initialise a new definition with the name, form
        /// and grid definitions provided
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="uiForm">The form definition</param>
        /// <param name="uiGrid">The grid definition</param>
        public UIDef(string name, UIForm uiForm, UIGrid uiGrid)
        {
            _name = name;
            _uiForm = uiForm;
            _uiGrid = uiGrid;
        }

        /// <summary>
        /// Returns the form definition
        /// </summary>
        public UIForm UIForm
        {
            get { return _uiForm; }
            protected set { _uiForm = value; }
        }

        /// <summary>
        /// Returns the name
        /// </summary>
        public string Name
        {
            get { return _name; }
            protected set { _name = value; }
        }

        /// <summary>
        /// Returns the grid definition
        /// </summary>
        public UIGrid UIGrid
        {
            get { return _uiGrid; }
            protected set { _uiGrid = value; }
        }

        /// <summary>
        /// Returns the form property definitions
        /// </summary>
        /// <returns>Returns a UIForm object</returns>
        public UIForm GetUIFormProperties()
        {
            return this.UIForm;
        }

        /// <summary>
        /// Returns the grid property definitions
        /// </summary>
        /// <returns>Returns a UIGridDef object</returns>
        public UIGrid GetUIGridProperties()
        {
            return this.UIGrid;
        }

        ///<summary>
        ///Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        ///</summary>
        ///
        ///<returns>
        ///true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        ///</returns>
        ///
        ///<param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            UIDef otherUIDef = obj as UIDef;
            if (otherUIDef == null) return false;
            if (this.Name != otherUIDef.Name) return false;
            if (this.UIForm != otherUIDef.UIForm) return false;
            if (this.UIGrid != otherUIDef.UIGrid) return false;
            return true;
        }

        ///<summary>
        /// overloads the operator == 
        ///</summary>
        ///<param name="a"></param>
        ///<param name="b"></param>
        ///<returns></returns>
        public static bool operator ==(UIDef a, UIDef b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        ///<summary>
        /// overloads the operator != 
        ///</summary>
        ///<param name="a"></param>
        ///<param name="b"></param>
        ///<returns></returns>
        public static bool operator !=(UIDef a, UIDef b)
        {
            return !(a == b);
        }

        ///<summary>
        /// Clones the collection of ui columns this performs a copy of all uicolumns but does not copy the uiFormFields.
        ///</summary>
        ///<returns>a new collection that is a shallow copy of this collection</returns>
        public UIDef Clone()
        {
            UIDef newUIForm = new UIDef(this.Name, this.UIForm, this.UIGrid);
            return newUIForm;
        }
    }
}