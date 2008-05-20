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

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Habanero.Base.Exceptions;
using Habanero.UI.Forms;
using Habanero.Util.File;
using Noogen.WinForms;

namespace Habanero.UI
{
    /// <summary>
    /// Produces a range of user interface objects as requested
    /// </summary>
    public class ControlFactory
    {
        /// <summary>
        /// A constructor to initialise an instance of the factory
        /// </summary>
        public ControlFactory()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Creates a label
        /// </summary>
        /// <param name="text">The text to appear in the label</param>
        /// <param name="isBold">Whether the text appears in bold lettering</param>
        /// <returns>Returns the new Label object</returns>
        public static Label CreateLabel(string text, bool isBold)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.FlatStyle = FlatStyle.System;
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

        /// <summary>
        /// Creates a label with bold text to serve as a heading
        /// </summary>
        /// <param name="text">The text to appear in the label</param>
        /// <returns>Returns the new Label object</returns>
        public static Label CreateHeadingLabel(string text)
        {
            Label lbl = CreateLabel(text, true);
            lbl.Font = new Font(lbl.Font.FontFamily, 9, FontStyle.Bold);
            lbl.Width = lbl.PreferredWidth + 20;
            return lbl;
        }

        /// <summary>
        /// Creates a new button
        /// </summary>
        /// <param name="text">The text to appear on the button</param>
        /// <returns>Returns the new Button object</returns>
        public static Button CreateButton(string text)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Name = text;
            btn.FlatStyle = FlatStyle.System;
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
        public static Button CreateButton(string text, EventHandler clickHandler)
        {
            Button btn = CreateButton(text);
            btn.Click += clickHandler;
            return btn;
        }

        /// <summary>
        /// Creates a new empty TextBox
        /// </summary>
        /// <returns>Returns the new TextBox object</returns>
        public static TextBox CreateTextBox()
        {
            TextBox tb = new TextBox();
            return tb;
        }

        /// <summary>
        /// Creates a new PasswordTextBox that masks the letters as the user
        /// types them
        /// </summary>
        /// <returns>Returns the new PasswordTextBox object</returns>
        public static TextBox CreatePasswordTextBox()
        {
            TextBox tb = new PasswordTextBox();
            return tb;
        }

        /// <summary>
        /// Creates a new empty ComboBox
        /// </summary>
        /// <returns>Returns the new ComboBox object</returns>
        public static ComboBox CreateComboBox()
        {
            NComboBox cbx = new NComboBox();
            cbx.DisableEntryNotInList = true;
            cbx.AutoComplete = true;
            cbx.CharacterCasing = CharacterCasing.Normal;
            cbx.ShowDropDownDuringInput = true;
            cbx.ClearSelection = true;
            return cbx;
        }

        /// <summary>
        /// Creates a new file choosing tool
        /// </summary>
        /// <returns>Returns the new FileChooser object</returns>
        public static FileChooser CreateFileChooser()
        {
            return new FileChooser();
        }

        /// <summary>
        /// Creates a new empty TreeView
        /// </summary>
        /// <param name="name">The name of the view</param>
        /// <returns>Returns a new TreeView object</returns>
        public static TreeView CreateTreeView(string name)
        {
            TreeView tv = new TreeView();
            tv.Name = name;
            return tv;
        }

        /// <summary>
        /// Creates a new panel
        /// </summary>
        /// <param name="name">The name of the panel</param>
        /// <returns>Returns a new Panel object</returns>
        public static Panel CreatePanel(string name)
        {
            Panel pnl = new Panel();
            pnl.Name = name;
            return pnl;
        }

        /// <summary>
        /// Creates a control for the given type and assembly name.
        /// </summary>
        /// <param name="typeName">The name of the control type</param>
        /// <param name="assemblyName">The assembly name of the control type</param>
        /// <returns>Returns either the control of the specified type or
        ///          the default type, which is usually TextBox.
        /// </returns>
        public static Control CreateControl(string typeName, string assemblyName)
        {
            Type controlType = null;

            if (String.IsNullOrEmpty(typeName) || String.IsNullOrEmpty(assemblyName))
            {
                controlType = typeof(TextBox);
            }
            else
            {
                TypeLoader.LoadClassType(ref controlType, assemblyName, typeName,
                                         "field", "field definition");
            }

            return CreateControl(controlType);
        }

        /// <summary>
        /// Creates a new control of the type specified.
        /// </summary>
        /// <param name="controlType">The control type, which must be a
        /// sub-type of Control</param>
        /// <returns>Returns a new object of the type requested</returns>
        public static Control CreateControl(Type controlType)
        {
            Control ctl;
            if (controlType.IsSubclassOf(typeof (Control)))
            {
                if (controlType == typeof (ComboBox))
                {
                    ctl = CreateComboBox();
                }
                else
                {
                    ctl = (Control) Activator.CreateInstance(controlType);
                }
                PropertyInfo infoFlatStyle =
                    ctl.GetType().GetProperty("FlatStyle", BindingFlags.Public | BindingFlags.Instance);
                if (infoFlatStyle != null)
                {
                    infoFlatStyle.SetValue(ctl, FlatStyle.System, new object[] {});
                }
            }
            else
            {
                throw new UnknownTypeNameException("The control type name " + controlType.Name +
                                                   " does not inherit from Control.");
            }
            if (ctl.GetType() == typeof (DateTimePicker))
            {
                DateTimePicker editor = (DateTimePicker) ctl;
            }
            return ctl;
        }

        /// <summary>
        /// Creates a new DateTimePicker
        /// </summary>
        /// <returns>Returns a new DateTimePicker object</returns>
        public static Control CreateDateTimePicker()
        {
            DateTimePicker editor = new DateTimePicker();
            return editor;
        }

        /// <summary>
        /// Creates a new DateTimePicker with a specified date
        /// </summary>
        /// <param name="defaultDate">The initial date value</param>
        /// <returns>Returns a new DateTimePicker object</returns>
        public static Control CreateDateTimePicker(DateTime defaultDate)
        {
            DateTimePicker editor = (DateTimePicker) CreateDateTimePicker();
            editor.Value = defaultDate;
            return editor;
        }

        /// <summary>
        /// Creates a new DateTimePicker
        /// </summary>
        /// <returns>Returns a new DateTimePicker object</returns>
        public static DateTimePicker CreateStandardDateTimePicker()
        {
            DateTimePicker picker = new DateTimePicker();
            return picker;
        }

        /// <summary>
        /// Creates a new DateTimePicker that is formatted to handle months
        /// and years
        /// </summary>
        /// <returns>Returns a new DateTimePicker object</returns>
        public static Control CreateMonthPicker()
        {
            DateTimePicker editor = (DateTimePicker) CreateDateTimePicker();
            editor.Format = DateTimePickerFormat.Custom;
            editor.CustomFormat = "MMM yyyy";
            return editor;
        }

        /// <summary>
        /// Creates a new empty data grid
        /// </summary>
        /// <param name="name">The name of the grid</param>
        /// <returns>Returns a new DataGrid object</returns>
        public static DataGrid CreateDataGrid(string name)
        {
            DataGrid grid = new DataGrid();
            grid.Name = name;
            return grid;
        }

        /// <summary>
        /// Creates a new numeric up-down control that is formatted with
        /// two decimal places for monetary use
        /// </summary>
        /// <returns>Returns a new NumericUpDown object</returns>
        public static NumericUpDown CreateNumericUpDownMoney()
        {
            NumericUpDown ctl = new NumericUpDown();
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
        public static NumericUpDown CreateNumericUpDownInteger()
        {
            NumericUpDown ctl = new NumericUpDown();
            ctl.DecimalPlaces = 0;
            ctl.Maximum = Int32.MaxValue;
            ctl.Minimum = Int32.MinValue;
            return ctl;
        }

        /// <summary>
        /// Creates a new CheckBox
        /// </summary>
        /// <returns>Returns a new CheckBox object</returns>
        public static CheckBox CreateCheckBox()
        {
            CheckBox cbx = new CheckBox();
            cbx.FlatStyle = FlatStyle.System;
            return cbx;
        }

        /// <summary>
        /// Creates a new CheckBox with a specified initial checked state
        /// </summary>
        /// <param name="defaultValue">Whether the initial box is ticked</param>
        /// <returns>Returns a new CheckBox object</returns>
        public static CheckBox CreateCheckBox(bool defaultValue)
        {
            CheckBox cbx = CreateCheckBox();
            cbx.Checked = defaultValue;
            cbx.FlatStyle = FlatStyle.System;
            return cbx;
        }

        /// <summary>
        /// Creates a new progress bar
        /// </summary>
        /// <returns>Returns a new ProgressBar object</returns>
        public static ProgressBar CreateProgressBar()
        {
            ProgressBar bar = new ProgressBar();
            return bar;
        }

        /// <summary>
        /// Creates a new splitter which enables the user to resize 
        /// docked controls
        /// </summary>
        /// <returns>Returns a new Splitter object</returns>
        public static Splitter CreateSplitter()
        {
            Splitter splitter = new Splitter();
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
        public static TabPage CreateTabPage(string title)
        {
            TabPage page = new TabPage(title);
            return page;
        }

        /// <summary>
        /// Creates a new radio button
        /// </summary>
        /// <param name="text">The text to appear next to the radio button</param>
        /// <returns>Returns a new RadioButton object</returns>
        public static RadioButton CreateRadioButton(string text)
        {
            RadioButton rButton = new RadioButton();
            rButton.Text = text;
            rButton.AutoCheck = true;
            rButton.FlatStyle = FlatStyle.System;
            rButton.Width = CreateLabel(text, false).PreferredWidth + 25;
            return rButton;
        }

        /// <summary>
        /// Creates a new GroupBox
        /// </summary>
        /// <returns>Returns the new GroupBox</returns>
        public static GroupBox CreateGroupBox( )
        {
            return new GroupBox();
        }
    }
}