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
using Habanero.BO;
using Habanero.BO.ClassDefinition;
using Habanero.UI.Base;
using log4net;

namespace Habanero.UI.Win
{
    /// <summary>
    /// Provides a form used to edit business objects.  This form will usually
    /// be constructed using a UI Form definition provided in the class definitions.
    /// The appropriate UI definition is typically set in the constructor.
    /// </summary>
    public class DefaultBOEditorFormWin : FormWin, IDefaultBOEditorForm
    {
        private static readonly ILog log = LogManager.GetLogger("Habanero.UI.Win.DefaultBOEditorFormWin");
        private readonly BusinessObject _bo;
        private readonly IControlFactory _controlFactory;
        private readonly string _uiDefName;
        private readonly IPanelInfo _panelInfo;
        private readonly IPanel _boPanel;
        private readonly IButtonGroupControl _buttons;
        private readonly PostObjectEditDelegate _action;


        /// <summary>
        /// Constructs the <see cref="DefaultBOEditorFormWin"/> class  with 
        /// the specified <see cref="BusinessObject"/>, uiDefName, <see cref="IControlFactory"/> and post edit action. 
        /// </summary>
        /// <param name="bo">The business object to represent</param>
        /// <param name="uiDefName">The name of the ui def to use.</param>
        /// <param name="controlFactory">The <see cref="IControlFactory"/> to use for creating the Editor form controls</param>
        /// <param name="action">Action to be performed when the editing is completed or cancelled. Typically used if you want to update
        /// a grid or a list in an asynchronous environment (E.g. to select the recently edited item in the grid).</param>
        public DefaultBOEditorFormWin(BusinessObject bo, string uiDefName, IControlFactory controlFactory, PostObjectEditDelegate action)
            : this(bo, uiDefName, controlFactory)
        {
            _action = action;
        }
                /// <summary>
        /// Constructs the <see cref="DefaultBOEditorFormWin"/> class  with 
        /// the specified businessObject, uiDefName and post edit action. 
        /// </summary>
        /// <param name="bo">The business object to represent</param>
        /// <param name="uiDefName">The name of the ui def to use.</param>
        /// <param name="controlFactory">The <see cref="IControlFactory"/> to use for creating the Editor form controls</param>
        /// <param name="creator">The Creator used to Create the Group Control.</param>
        public DefaultBOEditorFormWin(BusinessObject bo, string uiDefName, IControlFactory controlFactory, GroupControlCreator creator)
        {
            _bo = bo;
            _controlFactory = controlFactory;
            GroupControlCreator = creator;
            _uiDefName = uiDefName;

            BOMapper mapper = new BOMapper(bo);

            UIForm def;
            if (_uiDefName.Length > 0)
            {
                UIDef uiMapper = mapper.GetUIDef(_uiDefName);
                if (uiMapper == null)
                {
                    throw new NullReferenceException("An error occurred while " +
                                                     "attempting to load an object editing form.  A possible " +
                                                     "cause is that the class definitions do not have a " +
                                                     "'form' section for the class, under the 'ui' " +
                                                     "with the name '" + _uiDefName + "'.");
                }
                def = uiMapper.UIForm;
            }
            else
            {
                UIDef uiMapper = mapper.GetUIDef();
                if (uiMapper == null)
                {
                    throw new NullReferenceException("An error occurred while " +
                                                     "attempting to load an object editing form.  A possible " +
                                                     "cause is that the class definitions do not have a " +
                                                     "'form' section for the class.");
                }
                def = uiMapper.UIForm;
            }
            if (def == null)
            {
                throw new NullReferenceException("An error occurred while " +
                                                 "attempting to load an object editing form.  A possible " +
                                                 "cause is that the class definitions do not have a " +
                                                 "'form' section for the class.");
            }

            PanelBuilder panelBuilder = new PanelBuilder(_controlFactory);
            //_panelInfo = panelBuilder.BuildPanelForForm(_bo.ClassDef.UIDefCol["default"].UIForm);
            _panelInfo = panelBuilder.BuildPanelForForm(_bo.ClassDef.UIDefCol[uiDefName].UIForm, this.GroupControlCreator);

            _panelInfo.BusinessObject = _bo;
            _boPanel = _panelInfo.Panel;
            _buttons = _controlFactory.CreateButtonGroupControl();
            _buttons.AddButton("Cancel", CancelButtonHandler);
            IButton okbutton = _buttons.AddButton("OK", OKButtonHandler);
            okbutton.NotifyDefault(true);
            this.AcceptButton = (ButtonWin)okbutton;
            this.Load += delegate { FocusOnFirstControl(); };

            this.Text = def.Title;
            SetupFormSize(def);
            MinimizeBox = false;
            MaximizeBox = false;
            //this.ControlBox = false;

            CreateLayout();
            OnResize(new EventArgs());

        }
        /// <summary>
        /// Constructs the <see cref="DefaultBOEditorFormWin"/> class  with 
        /// the specified <see cref="BusinessObject"/>, uiDefName and <see cref="IControlFactory"/>. 
        /// </summary>
        /// <param name="bo">The business object to represent</param>
        /// <param name="uiDefName">The name of the ui def to use.</param>
        /// <param name="controlFactory">The <see cref="IControlFactory"/> to use for creating the Editor form controls</param>
        public DefaultBOEditorFormWin(BusinessObject bo, string uiDefName, IControlFactory controlFactory)
            : this(bo, uiDefName, controlFactory, controlFactory.CreateTabControl)
        {
        }

        protected IPanel BoPanel
        {
            get { return _boPanel; }
        }

        protected void CreateLayout()
        {
            BorderLayoutManager borderLayoutManager = new BorderLayoutManagerWin(this, _controlFactory);
            borderLayoutManager.AddControl(BoPanel, BorderLayoutManager.Position.Centre);
            borderLayoutManager.AddControl(Buttons, BorderLayoutManager.Position.South);
        }

        private void SetupFormSize(UIForm def)
        {
            int width = def.Width;
            int minWidth = _boPanel.Width +
                           Margin.Left + Margin.Right;
            if (width < minWidth)
            {
                width = minWidth;
            }
            int height = def.Height;
            int minHeight = _boPanel.Height + _buttons.Height +
                            Margin.Top + Margin.Bottom;
            if (height < minHeight)
            {
                height = minHeight;
            }
            Height = height;
            Width = width;
        }

        private void FocusOnFirstControl()
        {
            //IControlHabanero controlToFocus = _panelInfo.FirstControlToFocus;
            //MethodInfo focusMethod = controlToFocus.GetType().
            //    GetMethod("Focus", BindingFlags.Instance | BindingFlags.Public);
            //if (focusMethod != null)
            //{
            //    focusMethod.Invoke(controlToFocus, new object[] { });
            //}
        }

        private void OKButtonHandler(object sender, EventArgs e)
        {
            try
            {
                _panelInfo.ApplyChangesToBusinessObject();
                TransactionCommitter committer = CreateSaveTransaction();
                committer.CommitTransaction();
                DialogResult = DialogResult.OK;
                Close();
                if (_action != null)
                {
                    _action(this._bo, false);
                }
                _panelInfo.BusinessObject = null;
            }
            catch (Exception ex)
            {
                log.Error(ExceptionUtilities.GetExceptionString(ex, 0, true));
                GlobalRegistry.UIExceptionNotifier.Notify(ex, "There was a problem saving for the following reason(s):",
                    "Saving Problem");
            }
        }

        protected virtual TransactionCommitter CreateSaveTransaction()
        {
            TransactionCommitter committer = (TransactionCommitter) BORegistry.DataAccessor.CreateTransactionCommitter();
            committer.AddBusinessObject(_bo);
            return committer;
        }

        private void CancelButtonHandler(object sender, EventArgs e)
        {
            _panelInfo.BusinessObject = null;
            _bo.CancelEdits();
            DialogResult = Base.DialogResult.Cancel;
            Close();
            if (_action != null)
            {
                _action(this._bo, true);
            }
        }

        /// <summary>
        /// Gets the button control for the buttons in the form
        /// </summary>
        public IButtonGroupControl Buttons
        {
            get { return _buttons; }
        }

        /// <summary>
        /// Gets the object containing all information related to the form, including
        /// its controls, mappers and business object
        /// </summary>
        public IPanelInfo PanelInfo
        {
            get { return _panelInfo; }
        }

        ///<summary>
        /// The Creator (<see cref="IDefaultBOEditorForm.GroupControlCreator"/> used to create the <see cref="IGroupControl"/>
        ///</summary>
        public GroupControlCreator GroupControlCreator { get; protected set; }

        /// <summary>
        /// Pops the form up in a modal dialog.  If the BO is successfully edited and saved, returns true,
        /// else returns false.
        /// </summary>
        /// <returns>True if the edit was a success, false if not</returns>
        bool IDefaultBOEditorForm.ShowDialog()
        {
            {
                return this.ShowDialog() == (System.Windows.Forms.DialogResult)Base.DialogResult.OK;
            }
        }
    }
}