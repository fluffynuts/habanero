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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Gizmox.WebGUI.Forms;
using Habanero.Base;
using Habanero.BO;
using Habanero.UI.Base;

namespace Habanero.UI.VWG
{
    /// <summary>
    /// Provides a DataGridView that is adapted to show business objects
    /// </summary>
    public abstract class GridBaseVWG : DataGridViewVWG, IGridBase
    {
        /// <summary>
        /// Occurs when a business object is selected
        /// </summary>
        public event EventHandler<BOEventArgs> BusinessObjectSelected;

        /// <summary>
        /// Occurs when a row is double-clicked by the user
        /// </summary>
        public event RowDoubleClickedHandler RowDoubleClicked;

        /// <summary>
        /// Occurs when the collection in the grid is changed
        /// </summary>
        public event EventHandler CollectionChanged;

        /// <summary>
        /// Occurs when a business object is being edited
        /// </summary>
        public event EventHandler<BOEventArgs> BusinessObjectEdited;

        private readonly GridBaseManager _manager;

        protected GridBaseVWG()
        {
            _manager = new GridBaseManager(this);
            this.SelectionChanged += delegate { FireBusinessObjectSelected(); };
            _manager.CollectionChanged += delegate { FireCollectionChanged(); };
        }

        /// <summary>
        /// Returns the grid base manager for this grid, which centralises common
        /// logic for the different implementations
        /// </summary>
        public GridBaseManager GridBaseManager
        {
            get { return _manager; }
        }

        /// <summary>
        /// Creates a dataset provider that is applicable to this grid. For example, a readonly grid would
        /// return a <see cref="ReadOnlyDataSetProvider"/>, while an editable grid would return an editable one.
        /// </summary>
        /// <param name="col">The collection to create the datasetprovider for</param>
        /// <returns>Returns the data set provider</returns>
        public abstract IDataSetProvider CreateDataSetProvider(IBusinessObjectCollection col);

        private void FireBusinessObjectSelected()
        {
            if (this.BusinessObjectSelected != null)
            {
                this.BusinessObjectSelected(this, new BOEventArgs(this.SelectedBusinessObject));
            }
        }

        /// <summary>
        /// Reloads the grid based on the grid returned by GetBusinessObjectCollection
        /// </summary>
        public void RefreshGrid()
        {
            _manager.RefreshGrid();
        }

        /// <summary>
        /// Clears the business object collection and the rows in the data table
        /// </summary>
        public void Clear()
        {
            _manager.Clear();
        }

        /// <summary>
        /// Sets the business object collection displayed in the grid.  This
        /// collection must be pre-loaded using the collection's Load() command.
        /// The default UI definition will be used, that is a 'ui' element 
        /// without a 'name' attribute.
        /// </summary>
        /// <param name="col">The collection of business objects to display.  This
        /// collection must be pre-loaded.</param>
        public void SetBusinessObjectCollection(IBusinessObjectCollection col)
        {
            _manager.SetBusinessObjectCollection(col);
        }

        /// <summary>
        /// Returns the business object collection being displayed in the grid
        /// </summary>
        /// <returns>Returns a business collection</returns>
        public IBusinessObjectCollection GetBusinessObjectCollection()
        {
            return _manager.GetBusinessObjectCollection();
        }

        private void FireCollectionChanged()
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets and sets the currently selected business object in the grid
        /// </summary>
        public IBusinessObject SelectedBusinessObject
        {
            get { return _manager.SelectedBusinessObject; }
            set
            {
                _manager.SelectedBusinessObject = value;
                this.FireBusinessObjectSelected();
            }
        }

        /// <summary>
        /// Gets a List of currently selected business objects
        /// </summary>
        public IList<BusinessObject> SelectedBusinessObjects
        {
            get { return _manager.SelectedBusinessObjects; }
        }

        /// <summary>
        /// Returns the business object at the specified row number
        /// </summary>
        /// <param name="row">The row number in question</param>
        /// <returns>Returns the busines object at that row, or null
        /// if none is found</returns>
        public IBusinessObject GetBusinessObjectAtRow(int row)
        {
            return _manager.GetBusinessObjectAtRow(row);
        }

        /// <summary>
        /// Applies a filter clause to the data table and updates the filter.
        /// The filter allows you to determine which objects to display using
        /// some criteria.  This is typically generated by an <see cref="IFilterControl"/>.
        /// </summary>
        /// <param name="filterClause">The filter clause</param>
        public void ApplyFilter(IFilterClause filterClause)
        {
            _manager.ApplyFilter(filterClause);
        }

        /// <summary>
        /// Gets and sets the delegated grid loader for the grid.
        /// <br/>
        /// This allows the user to implememt a custom
        /// loading strategy. This can be used to load a collection of business objects into a grid with images or buttons
        /// that implement custom code. (Grids loaded with a custom delegate generally cannot be set up to filter 
        /// (grid filters a dataview based on filter criteria),
        /// but can be set up to search (a business object collection loaded with criteria).
        /// For a grid to be filterable the grid must load with a dataview.
        /// <br/>
        /// If no grid loader is specified then the default grid loader is employed. This consists of parsing the collection into 
        /// a dataview and setting this as the datasource.
        /// </summary>
        public GridLoaderDelegate GridLoader
        {
            get { return _manager.GridLoader; }
            set { _manager.GridLoader = value; }
        }

        /// <summary>
        /// Gets the grid's DataSet provider, which loads the collection's
        /// data into a DataSet suitable for the grid
        /// </summary>
        public IDataSetProvider DataSetProvider
        {
            get { return _manager.DataSetProvider; }
        }

        /// <summary>
        /// Fires an event indicating that the selected business object
        /// is being edited
        /// </summary>
        /// <param name="bo">The business object being edited</param>
        /// TODO: this is badly named (why do we indicate the BO, but say "Selected") - this should be a
        /// verb, as in FireBusinessObjectEdited
        public void SelectedBusinessObjectEdited(BusinessObject bo)
        {
            FireSelectedBusinessObjectEdited(bo);
        }

        private void FireSelectedBusinessObjectEdited(IBusinessObject bo)
        {
            if (this.BusinessObjectEdited != null)
            {
                this.BusinessObjectEdited(this, new BOEventArgs(bo));
            }
        }
    }
}