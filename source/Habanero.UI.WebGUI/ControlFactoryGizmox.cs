using System;
using System.Drawing;
using System.Reflection;
using Gizmox.WebGUI.Forms;
using Habanero.Base;
using Habanero.Base.Exceptions;
using Habanero.BO;
using Habanero.UI.Base;
using Habanero.UI.Base.FilterControl;

namespace Habanero.UI.WebGUI
{
    public class ControlFactoryGizmox : IControlFactory
    {
        public const int TEXTBOX_HEIGHT = 20;
        public IFilterControl CreateFilterControl()
        {
            return new FilterControlGiz(this);
        }

        public ITextBox CreateTextBox()
        {
            TextBoxGiz tb = new TextBoxGiz();
            tb.Height = TEXTBOX_HEIGHT;
            return tb;
        }

        public IComboBox CreateComboBox()
        {
            ComboBoxGiz comboBox = new ComboBoxGiz();
            comboBox.Height = TEXTBOX_HEIGHT; 
            return comboBox;
        }

        public IListBox CreateListBox()
        {
            return new ListBoxGiz();
        }

        public IMultiSelector<T> CreateMultiSelector<T>()
        {
            return new MultiSelectorGiz<T>();
        }

        public IButton CreateButton()
        {
            return new ButtonGiz();
        }

        /// <summary>
        /// Creates a new button
        /// </summary>
        /// <param name="text">The text to appear on the button</param>
        /// <returns>Returns the new Button object</returns>
        public IButton CreateButton(string text)
        {
            IButton btn = CreateButton();
            btn.Text = text;
            btn.Name = text;
            ((Button) btn).FlatStyle = FlatStyle.Standard;
            btn.Width = CreateLabel(text, false).PreferredWidth + 20;
            return btn;
        }

        /// <summary>
        /// Creates a new button with an attached event handler to carry out
        /// further instructions if the button is pressed
        /// </summary>
        /// <param name="text">The text to appear on the button</param>
        /// <param name="clickHandler">The method that handles the event</param>
        /// <returns>Returns the new Button object</returns>
        public IButton CreateButton(string text, EventHandler clickHandler)
        {
            IButton btn = CreateButton(text);
            btn.Click += clickHandler;
            return btn;
        }


        public ILabel CreateLabel()
        {
            return CreateLabel("");
        }

        public ILabel CreateLabel(string labelText)
        {
            LabelGiz label = new LabelGiz(labelText);
            label.Width = label.PreferredWidth;
            label.Height = 15;
            label.TabStop = false;
            return label;
        }

        public IDateTimePicker CreateDateTimePicker()
        {
            return new DateTimePickerGiz();
        }

        public BorderLayoutManager CreateBorderLayoutManager(IControlChilli control)
        {
            return new BorderLayoutManagerGiz(control, this);
        }

        public IPanel CreatePanel()
        {
            return new PanelGiz();
        }

        public IReadOnlyGrid CreateReadOnlyGrid()
        {
            return new ReadOnlyGridGiz();
        }


        public IReadOnlyGridControl CreateReadOnlyGridControl()
        {
            return new ReadOnlyGridControlGiz();
        }

        public IButtonGroupControl CreateButtonGroupControl()
        {
            return new ButtonGroupControlGiz(this);
        }

        public IReadOnlyGridButtonsControl CreateReadOnlyGridButtonsControl()
        {
            return new ReadOnlyGridButtonsControlGiz(this);
        }

        public IPanel CreatePanel(IControlFactory controlFactory)
        {
            IPanel pnl = new PanelGiz();
            return pnl;
        }

        /// <summary>
        /// Creates a new panel
        /// </summary>
        /// <param name="name">The name of the panel</param>
        /// <returns>Returns a new Panel object</returns>
        /// <param name="controlFactory">The control factory that the panel will use to create any controls</param>
        public IPanel CreatePanel(string name, IControlFactory controlFactory)
        {
            IPanel pnl = CreatePanel(controlFactory);
            pnl.Name = name;
            return pnl;
        }


        public ILabel CreateLabel(string labelText, bool isBold)
        {
            ILabel lbl = CreateLabel(labelText);
            lbl.AutoSize = true;
            lbl.Text = labelText;
            ((Label) lbl).FlatStyle = FlatStyle.Standard;
            if (isBold)
            {
                lbl.Font = new Font(lbl.Font, FontStyle.Bold);
            }
            lbl.Width = lbl.PreferredWidth;
            if (isBold)
            {
                lbl.Width += 10;
            }
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.TabStop = false;
            return lbl;
        }

        public ITextBox CreatePasswordTextBox()
        {
            ITextBox tb = CreateTextBox();
            tb.PasswordChar = '*';
            return tb;
        }

        public IToolTip CreateToolTip()
        {
            return new ToolTipGiz();
        }

        public IControlChilli CreateControl()
        {
            ControlGiz cntrl = new ControlGiz();
            cntrl.Height = 10;
            return cntrl;
        }

        /// <summary>
        /// Creates a new empty TreeView
        /// </summary>
        /// <param name="name">The name of the view</param>
        /// <returns>Returns a new TreeView object</returns>
        public ITreeView CreateTreeView(string name)
        {
            ITreeView tv = new TreeViewGiz();
            tv.Name = name;
            return tv;
        }

        /// <summary>
        /// Creates a new control of the type specified.
        /// </summary>
        /// <param name="controlType">The control type, which must be a
        /// sub-type of Control</param>
        /// <returns>Returns a new object of the type requested</returns>
        public IControlChilli CreateControl(Type controlType)
        {
            IControlChilli ctl;
            if (controlType.IsSubclassOf(typeof (Control)))
            {
                if (controlType == typeof (ComboBox)) return CreateComboBox();
                if (controlType == typeof (CheckBox)) return CreateCheckBox();
                if (controlType == typeof (TextBox)) return CreateTextBox();
                if (controlType == typeof (ListBox)) return CreateListBox();
                if (controlType == typeof (DateTimePicker)) return CreateDateTimePicker();

                ctl = (IControlChilli) Activator.CreateInstance(controlType);
                PropertyInfo infoFlatStyle =
                    ctl.GetType().GetProperty("FlatStyle", BindingFlags.Public | BindingFlags.Instance);
                if (infoFlatStyle != null)
                {
                    infoFlatStyle.SetValue(ctl, FlatStyle.Standard, new object[] {});
                }
            }
            else
            {
                throw new UnknownTypeNameException(
                    string.Format(
                    "The control type name {0} does not inherit from {1}.", controlType.FullName, typeof(Control)));
            }
            return ctl;
        }


        /// <summary>
        /// Creates a new DateTimePicker with a specified date
        /// </summary>
        /// <param name="defaultDate">The initial date value</param>
        /// <returns>Returns a new DateTimePicker object</returns>
        public IDateTimePicker CreateDateTimePicker(DateTime defaultDate)
        {
            IDateTimePicker editor = CreateDateTimePicker();
            editor.Value = defaultDate;
            return editor;
        }

        /// <summary>
        /// Creates a new DateTimePicker that is formatted to handle months
        /// and years
        /// </summary>
        /// <returns>Returns a new DateTimePicker object</returns>
        public IDateTimePicker CreateMonthPicker()
        {
            DateTimePickerGiz editor = (DateTimePickerGiz) CreateDateTimePicker();
            editor.Format = DateTimePickerFormat.Custom;
            editor.CustomFormat = "MMM yyyy";
            return editor;
        }

        /// <summary>
        /// Creates a new numeric up-down control that is formatted with
        /// two decimal places for monetary use
        /// </summary>
        /// <returns>Returns a new NumericUpDown object</returns>
        public INumericUpDown CreateNumericUpDownMoney()
        {
            NumericUpDownGiz ctl = new NumericUpDownGiz();
            ctl.DecimalPlaces = 2;
            ctl.Maximum = Int32.MaxValue;
            ctl.Minimum = Int32.MinValue;
            return ctl;
        }

        /// <summary>
        /// Creates a new numeric up-down control that is formatted with
        /// zero decimal places for integer use
        /// </summary>
        /// <returns>Returns a new NumericUpDown object</returns>
        public INumericUpDown CreateNumericUpDownInteger()
        {
            NumericUpDownGiz ctl = new NumericUpDownGiz();
            ctl.DecimalPlaces = 0;
            ctl.Maximum = Int32.MaxValue;
            ctl.Minimum = Int32.MinValue;
            return ctl;
        }


        public ICheckBox CreateCheckBox()
        {
            return CreateCheckBox(false);
        }

        /// <summary>
        /// Creates a new CheckBox with a specified initial checked state
        /// </summary>
        /// <param name="defaultValue">Whether the initial box is ticked</param>
        /// <returns>Returns a new CheckBox object</returns>
        public ICheckBox CreateCheckBox(bool defaultValue)
        {
            CheckBoxGiz cbx = new CheckBoxGiz();
            cbx.Checked = defaultValue;
            cbx.FlatStyle = FlatStyle.Standard;
            cbx.Height = CreateTextBox().Height;
                // set the combobox to the default height of a text box on this machine.
            cbx.Width = cbx.Height;
            cbx.BackColor = SystemColors.Control;

            return cbx;
        }

        /// <summary>
        /// Creates a new progress bar
        /// </summary>
        /// <returns>Returns a new ProgressBar object</returns>
        public IProgressBar CreateProgressBar()
        {
            ProgressBarGiz bar = new ProgressBarGiz();
            return bar;
        }

        /// <summary>
        /// Creates a new splitter which enables the user to resize 
        /// docked controls
        /// </summary>
        /// <returns>Returns a new Splitter object</returns>
        public ISplitter CreateSplitter()
        {
            SplitterGiz splitter = new SplitterGiz();
            Color newBackColor =
                Color.FromArgb(Math.Min(splitter.BackColor.R - 30, 255), Math.Min(splitter.BackColor.G - 30, 255),
                               Math.Min(splitter.BackColor.B - 30, 255));
            splitter.BackColor = newBackColor;
            return splitter;
        }

        /// <summary>
        /// Creates a new tab page
        /// </summary>
        /// <param name="title">The page title to appear in the tab</param>
        /// <returns>Returns a new TabPage object</returns>
        public ITabPage CreateTabPage(string title)
        {
            TabPageGiz page = new TabPageGiz();
            page.Text = title;
            return page;
        }

        /// <summary>
        /// Creates a new radio button
        /// </summary>
        /// <param name="text">The text to appear next to the radio button</param>
        /// <returns>Returns a new RadioButton object</returns>
        public IRadioButton CreateRadioButton(string text)
        {
            RadioButtonGiz rButton = new RadioButtonGiz();
            rButton.Text = text;
            //TODO_REmoved when porting rButton.AutoCheck = true;
            //TODO_REmoved when portingrButton.FlatStyle = FlatStyle.Standard;
            rButton.Width = CreateLabel(text, false).PreferredWidth + 25;
            return rButton;
        }

        /// <summary>
        /// Creates a new GroupBox
        /// </summary>
        /// <returns>Returns the new GroupBox</returns>
        public IGroupBox CreateGroupBox()
        {
            return new GroupBoxGiz();
        }

        /// <summary>
        /// Creates a form in which a business object can be edited
        /// </summary>
        /// <param name="bo">The business object to edit</param>
        /// <param name="uiDefName">The name of the set of ui definitions
        /// used to design the edit form. Setting this to an empty string
        /// will use a ui definition with no name attribute specified.</param>
        /// <returns>Returns a DefaultBOEditorFormGiz object</returns>
        public IDefaultBOEditorForm CreateBOEditorForm(BusinessObject bo, string uiDefName)
        {
            return new DefaultBOEditorFormGiz(bo, uiDefName, this);
        }
        public IDefaultBOEditorForm CreateBOEditorForm(BusinessObject bo, string uiDefName, PostObjectPersistingDelegate action)
        {
            return new DefaultBOEditorFormGiz(bo, uiDefName, this, action);
        }

        public IDefaultBOEditorForm CreateBOEditorForm(BusinessObject bo)
        {
            return new DefaultBOEditorFormGiz(bo, "default", this);
        }

        public ITabControl CreateTabControl()
        {
            return new TabControlGiz();
        }

        /// <summary>
        /// Creates a multi line textbox, setting the scrollbars to vertical
        /// </summary>
        /// <param name="numLines"></param>
        public ITextBox CreateTextBoxMultiLine(int numLines)
        {
            TextBoxGiz tb = (TextBoxGiz) CreateTextBox();
            tb.Multiline = true;
            tb.AcceptsReturn = true;
            tb.Height = tb.Height*numLines;
            tb.ScrollBars = ScrollBars.Vertical;
            return tb;
        }

        public IDataGridViewColumn CreateDataGridViewColumn()
        {
            DataGridViewColumnGiz col = new DataGridViewColumnGiz();
            return col;
        }

        public IWizardControl CreateWizardControl(IWizardController wizardController)
        {
            return new WizardControlGiz(wizardController, this);
        }


    }
}