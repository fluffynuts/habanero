using System;
using Habanero.Base;
using Habanero.BO.ClassDefinition;
using Habanero.UI.Base;
using DialogResult = Habanero.UI.Base.DialogResult;
using MessageBoxButtons = Habanero.UI.Base.MessageBoxButtons;
using MessageBoxIcon = Habanero.UI.Base.MessageBoxIcon;

namespace Habanero.UI.VWG
{
    /// <summary>
    /// Represents a control to edit a collection of business objects.  A grid
    /// lists the objects as specified by SetBusinessObjectCollection and a control
    /// below the grid allows the selected business object to be edited.  Default
    /// buttons are provided: Save, New, Delete and Cancel.
    /// <br/>
    /// The editing control is
    /// specified here as a IBusinessObjectControl, allowing the developer to pass
    /// in a custom control, but the default instantiation uses a IBusinessObjectPanel,
    /// which is more suited to displaying errors.  If the developer provides a custom
    /// control, they are responsible for updating the business object status
    /// and displaying useful feedback to the user (by
    /// catching appropriate events on the business object or the controls).
    /// <br/>
    /// Some customisation is provided through the GridWithPanelControlStrategy,
    /// including how controls should be enabled for the appropriate environment.
    /// </summary>
    /// TODO: This uses ReadOnlyGridControl due to some flaw in ReadOnlyGrid. Look at switching back
    /// to the grid in the future.  What happens when you double-click?
    public class GridWithPanelControlVWG<TBusinessObject> : UserControlVWG, IGridWithPanelControl<TBusinessObject>
        where TBusinessObject : class, IBusinessObject, new()
    {
        private GridWithPanelControlManager<TBusinessObject> _gridWithPanelControlManager;
        
        public GridWithPanelControlVWG(IControlFactory controlFactory, string uiDefName)
        {
            IBusinessObjectControl businessObjectControl = new BusinessObjectPanelVWG<TBusinessObject>(controlFactory, uiDefName);
            _gridWithPanelControlManager = new GridWithPanelControlManager<TBusinessObject>(this, controlFactory, businessObjectControl, uiDefName);
            //_gridWithPanelControlManager.SetupControl();
            //SetupControl(controlFactory, businessObjectControl, uiDefName);
            _gridWithPanelControlManager.GridWithPanelControlStrategy = new GridWithPanelControlStrategyVWG<TBusinessObject>(this);
        }
        public GridWithPanelControlVWG(IControlFactory controlFactory, IBusinessObjectControl businessObjectControl)
            : this(controlFactory, businessObjectControl, "default")
        {
        }

        public GridWithPanelControlVWG(IControlFactory controlFactory, IBusinessObjectControl businessObjectControl, string uiDefName)
        {
            _gridWithPanelControlManager = new GridWithPanelControlManager<TBusinessObject>(this, controlFactory, businessObjectControl, uiDefName);
            //_gridWithPanelControlManager.SetupControl();
            _gridWithPanelControlManager.GridWithPanelControlStrategy = new GridWithPanelControlStrategyVWG<TBusinessObject>(this);
        }

        /// <summary>
        /// Called when the user attempts to move away from a dirty business object
        /// and needs to indicate Yes/No/Cancel to the option of saving.  This delegate
        /// facility is provided primarily to facilitate testing.
        /// </summary>
        public ConfirmSave ConfirmSaveDelegate
        {
            get { return _gridWithPanelControlManager.ConfirmSaveDelegate; }
            set { _gridWithPanelControlManager.ConfirmSaveDelegate = value; }
        }

        /// <summary>
        /// Sets the business object collection to populate the grid.  If the grid
        /// needs to be cleared, set an empty collection rather than setting to null.
        /// Until you set a collection, the controls are disabled, since any given
        /// collection needs to be provided by a suitable context.
        /// </summary>
        public void SetBusinessObjectCollection(IBusinessObjectCollection col)
        {
            _gridWithPanelControlManager.SetBusinessObjectCollection(col);
        }

        /// <summary>
        /// Gets the grid control
        /// </summary>
        public IReadOnlyGridControl ReadOnlyGridControl
        {
            get { return _gridWithPanelControlManager.ReadOnlyGridControl; }
        }

        /// <summary>
        /// Gets the control used to edit the selected business object
        /// </summary>
        public IBusinessObjectControl BusinessObjectControl
        {
            get { return _gridWithPanelControlManager.BusinessObjectControl; }
        }

        /// <summary>
        /// Gets the control holding the buttons
        /// </summary>
        public IButtonGroupControl Buttons
        {
            get { return _gridWithPanelControlManager.Buttons; }
        }

        public TBusinessObject CurrentBusinessObject
        {
            get { return _gridWithPanelControlManager.CurrentBusinessObject; }
        }

        /// <summary>
        /// Gets the strategy used to provide custom behaviour in the control
        /// </summary>
        public IGridWithPanelControlStrategy<TBusinessObject> GridWithPanelControlStrategy
        {
            get { return _gridWithPanelControlManager.GridWithPanelControlStrategy; }
            set { _gridWithPanelControlManager.GridWithPanelControlStrategy = value; }
        }

        /// <summary>
        /// Gets the business object currently selected in the grid
        /// </summary>
        TBusinessObject IGridWithPanelControl<TBusinessObject>.CurrentBusinessObject
        {
            get { return CurrentBusinessObject; }
        }
    }

    /// <summary>
    /// Represents a panel containing a PanelInfo used to edit a single business object.
    /// </summary>
    public class BusinessObjectPanelVWG<T> : UserControlVWG, IBusinessObjectPanel where T : class, IBusinessObject
    {
        private IPanelInfo _panelInfo;

        public BusinessObjectPanelVWG(IControlFactory controlFactory, string uiDefName)
        {
            PanelBuilder panelBuilder = new PanelBuilder(controlFactory);
            _panelInfo = panelBuilder.BuildPanelForForm(ClassDef.Get<T>().UIDefCol[uiDefName].UIForm);
            BorderLayoutManager layoutManager = controlFactory.CreateBorderLayoutManager(this);
            layoutManager.AddControl(_panelInfo.Panel, BorderLayoutManager.Position.Centre);
        }

        /// <summary>
        /// Gets or sets the business object being represented
        /// </summary>
        public IBusinessObject BusinessObject
        {
            get { return _panelInfo.BusinessObject; }
            set { _panelInfo.BusinessObject = value; }
        }

        /// <summary>
        /// Gets and sets the PanelInfo object created by the control
        /// </summary>
        public IPanelInfo PanelInfo
        {
            get { return _panelInfo; }
            set { _panelInfo = value; }
        }
    }

    /// <summary>
    /// Provides a strategy to add custom behaviour to a GridWithPanelControl
    /// </summary>
    public class GridWithPanelControlStrategyVWG<TBusinessObject> : IGridWithPanelControlStrategy<TBusinessObject>
    {
        private IGridWithPanelControl<TBusinessObject> _gridWithPanelControl;

        public GridWithPanelControlStrategyVWG(IGridWithPanelControl<TBusinessObject> gridWithPanelControl)
        {
            _gridWithPanelControl = gridWithPanelControl;
        }

        /// <summary>
        /// Provides custom control state.  Since this is called after the default
        /// implementation, it overrides it.
        /// </summary>
        /// <param name="lastSelectedBusinessObject">The previous selected business
        /// object in the grid - used to revert when a user tries to change a grid
        /// row while an object is dirty or invalid</param>
        public void UpdateControlEnabledState(IBusinessObject lastSelectedBusinessObject)
        {
            IButton cancelButton = _gridWithPanelControl.Buttons["Cancel"];
            IButton deleteButton = _gridWithPanelControl.Buttons["Delete"];
            IButton saveButton = _gridWithPanelControl.Buttons["Save"];
            IButton newButton = _gridWithPanelControl.Buttons["New"];

            if (_gridWithPanelControl.ReadOnlyGridControl.Grid.Rows.Count == 0)
            {
                cancelButton.Enabled = false;
                deleteButton.Enabled = false;
                saveButton.Enabled = false;
                newButton.Enabled = true;
            }
            else
            {
                cancelButton.Enabled = true;
                deleteButton.Enabled = true;
                saveButton.Enabled = true;
                newButton.Enabled = true;
            }
        }

        /// <summary>
        /// Whether to show the save confirmation dialog when moving away from
        /// a dirty object
        /// </summary>
        public bool ShowConfirmSaveDialog
        {
            get { return false; }
        }

        /// <summary>
        /// Indicates whether PanelInfo.ApplyChangesToBusinessObject needs to be
        /// called to copy control values to the business object.  This will be
        /// the case if the application uses minimal events and does not update
        /// the BO every time a control value changes.
        /// </summary>
        public bool CallApplyChangesToEditBusinessObject
        {
            get { return true; }
        }

        /// <summary>
        /// Indicates whether the grid should be refreshed.  For instance, a VWG
        /// implementation needs regular refreshes due to the lack of synchronisation,
        /// but this behaviour has some adverse affects in the WinForms implementation
        /// </summary>
        public bool RefreshGrid
        {
            get { return true; }
        }
    }
}
