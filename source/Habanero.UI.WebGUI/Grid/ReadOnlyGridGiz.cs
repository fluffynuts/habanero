using Habanero.UI.Base;

namespace Habanero.UI.WebGUI
{
    public class ReadOnlyGridGiz : GridBaseGiz, IReadOnlyGrid
    {
        public ReadOnlyGridGiz()
        {

            this.ReadOnly = true;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.SelectionMode = Gizmox.WebGUI.Forms.DataGridViewSelectionMode.FullRowSelect;
        }

    }
}