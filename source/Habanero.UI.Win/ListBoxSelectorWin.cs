using System;
using Habanero.Base;
using Habanero.BO;
using Habanero.UI.Base;

namespace Habanero.UI.Win
{
    /// <summary>
    /// Provides an implementation of <see cref="IBOListBoxSelector"/> that is specialised for showing a collection of 
    /// Business Objects (<see cref="IBusinessObjectCollection"/>) in a <see cref="IListBox"/> and allowing the user to select one.
    /// </summary>
    public class ListBoxSelectorWin : ListBoxWin, IBOListBoxSelector
    {
        private readonly ListBoxCollectionManager _manager;

        ///<summary>
        /// Constructor for <see cref="ListBoxSelectorWin"/>
        ///</summary>
        ///<param name="controlFactory"></param>
        public ListBoxSelectorWin(IControlFactory controlFactory)
        {
            if (controlFactory == null) throw new ArgumentNullException("controlFactory");
            _manager = new ListBoxCollectionManager(this, controlFactory);
            _manager.BusinessObjectSelected += delegate { FireBusinessObjectSelected(); };
        }

        ///<summary>
        /// Constructor for <see cref="ListBoxSelectorWin"/>
        ///</summary>
        public ListBoxSelectorWin():this(GlobalUIRegistry.ControlFactory)
        {
        }
                
        private void FireBusinessObjectSelected()
        {
            if (this.BusinessObjectSelected != null)
            {
                this.BusinessObjectSelected(this, new BOEventArgs(this.SelectedBusinessObject));
            }
        }
        /// <summary>
        /// Gets and Sets the business object collection displayed in the grid.  This
        /// collection must be pre-loaded using the collection's Load() command or from the
        /// <see cref="IBusinessObjectLoader"/>.
        /// The default UI definition will be used, that is a 'ui' element 
        /// without a 'name' attribute.
        /// </summary>
        public IBusinessObjectCollection BusinessObjectCollection
        {
            get { return _manager.Collection; }
            set { _manager.SetCollection(value, true); }
        }

        /// <summary>
        /// Gets and sets the currently selected business object in the grid
        /// </summary>
        public IBusinessObject SelectedBusinessObject
        {
            get { return _manager.SelectedBusinessObject; }
            set { _manager.SelectedBusinessObject = value; }
        }

        /// <summary>
        /// Event Occurs when a business object is selected
        /// </summary>
        public event EventHandler<BOEventArgs> BusinessObjectSelected;

        /// <summary>
        /// Clears the business object collection and the rows in the data table
        /// </summary>
        public void Clear()
        {
            _manager.Clear();
        }

        ///<summary>
        /// Returns the Underlying <see cref="IListBox"/> that is used by this selector
        ///</summary>
        public IListBox ListBox
        {
            get { return this; }
        }
        /// <summary>Gets the number of rows displayed in the <see cref="IBOColSelectorControl"></see>.</summary>
        /// <returns>The number of rows in the <see cref="IBOColSelectorControl"></see>.</returns>
        public int NoOfItems
        {
            get { return _manager.NoOfItems; }
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
        /// Gets and sets whether this selector autoselects the first item or not when a new collection is set.
        /// </summary>
        public bool AutoSelectFirstItem
        {
            get { return _manager.AutoSelectFirstItem; }
            set { _manager.AutoSelectFirstItem = value; }
        }
    }
}