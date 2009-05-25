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

using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.UI.Grid;
using Habanero.Util;

namespace Habanero.UI.Grid
{
    /// <summary>
    /// Manages a filter clause that filters which data to
    /// display in a DataView, according to some criteria set on an integer column
    /// </summary>
    public class DataViewIntegerFilterClause : DataViewFilterClause
    {
        /// <summary>
        /// Constructor to create a new filter clause
        /// </summary>
        /// <param name="filterColumn">The column of data on which to do the
        /// filtering</param>
        /// <param name="clauseOperator">The clause operator</param>
        /// <param name="filterValue">The filter value to compare to</param>
        internal DataViewIntegerFilterClause(string filterColumn, FilterClauseOperator clauseOperator, int filterValue)
            : base(filterColumn, clauseOperator, filterValue)
        {
            if (_clauseOperator == FilterClauseOperator.OpLike)
            {
                throw new HabaneroArgumentException("clauseOperator",
                                                    "Operator Like is not supported for non string operands");
            }
        }

        /// <summary>
        /// Returns the filter value as a string
        /// </summary>
        /// <returns>Returns a string</returns>
        protected override string CreateValueClause()
        {
            return _filterValue.ToString();
        }
    }
}