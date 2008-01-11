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

using System;
using System.Collections;

namespace Habanero.BO.ClassDefinition
{
    /// <summary>
    /// Manages property definitions for a column in a user interface grid,
    /// as specified in the class definitions xml file
    /// </summary>
    public class UIGridColumn
    {
        private string _heading;
        private string _propertyName;
        private Type _gridControlType;
        private bool _editable;
        private int _width;
        private PropAlignment _alignment;
        private readonly Hashtable _parameters;

        /// <summary>
        /// An enumeration to specify a horizontal alignment in a grid
        /// </summary>
        public enum PropAlignment
        {
            left,
            right,
            centre
        }

        /// <summary>
        /// Constructor to initialise a new definition
        /// </summary>
        /// <param name="heading">The heading</param>
        /// <param name="propertyName">The property name</param>
        /// <param name="gridControlType">The grid control type</param>
        /// <param name="editable">Whether the grid is read-only (cannot be
        /// edited directly)</param>
        /// <param name="width">The width</param>
        /// <param name="alignment">The horizontal alignment</param>
        public UIGridColumn(string heading, string propertyName, Type gridControlType, bool editable, int width,
                            PropAlignment alignment, Hashtable parameters)
        {
            _heading = heading;
            _propertyName = propertyName;
            _gridControlType = gridControlType;
            _editable = editable;
            _width = width;
            _alignment = alignment;
            _parameters = parameters;
        }

        /// <summary>
        /// Returns the heading
        /// </summary>
        public string Heading
        {
            get { return _heading; }
            protected set { _heading = value; }
        }

        /// <summary>
        /// Returns the property name
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
            protected set { _propertyName = value; }
        }

        /// <summary>
        /// Returns the grid control type
        /// </summary>
        public Type GridControlType
        {
            get { return _gridControlType; }
            protected set { _gridControlType = value; }
        }

        /// <summary>
        /// Indicates whether the column is editable
        /// </summary>
        public bool Editable
        {
            get { return _editable; }
            protected set { _editable = value; }
        }

        /// <summary>
        /// Returns the width
        /// </summary>
        public int Width
        {
            get { return _width; }
            protected set { _width = value; }
        }

        /// <summary>
        /// Returns the horizontal alignment
        /// </summary>
        public PropAlignment Alignment
        {
            get { return _alignment; }
            protected set { _alignment = value; }
        }

        /// <summary>
        /// Returns the Hashtable containing the property parameters
        /// </summary>
        public Hashtable Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Returns the parameter value for the name provided
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <returns>Returns the parameter value or null if not found</returns>
        public object GetParameterValue(string parameterName)
        {
            if (_parameters.ContainsKey(parameterName))
            {
                return _parameters[parameterName];
            }
            else
            {
                return null;
            }
        }
    }
}