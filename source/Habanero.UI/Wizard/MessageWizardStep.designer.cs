namespace Habanero.UI.Wizard
{
    partial class MessageWizardStep
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._uxMessageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _uxMessageLabel
            // 
            this._uxMessageLabel.Location = new System.Drawing.Point(3, 10);
            this._uxMessageLabel.Name = "_uxMessageLabel";
            this._uxMessageLabel.Size = new System.Drawing.Size(303, 123);
            this._uxMessageLabel.TabIndex = 0;
            this._uxMessageLabel.Text = "Please replace this message";
            // 
            // MessageWizardStep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._uxMessageLabel);
            this.Name = "MessageWizardStep";
            this.Size = new System.Drawing.Size(322, 300);
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.Label _uxMessageLabel;
    }
}