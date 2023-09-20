
namespace DatabasesAddon
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AdvancedDataGridView1 = new ADGV.AdvancedDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.AdvancedDataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // AdvancedDataGridView1
            // 
            this.AdvancedDataGridView1.AllowUserToAddRows = false;
            this.AdvancedDataGridView1.AllowUserToDeleteRows = false;
            this.AdvancedDataGridView1.AutoGenerateContextFilters = true;
            this.AdvancedDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AdvancedDataGridView1.DateWithTime = false;
            this.AdvancedDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AdvancedDataGridView1.Location = new System.Drawing.Point(0, 0);
            this.AdvancedDataGridView1.Name = "AdvancedDataGridView1";
            this.AdvancedDataGridView1.ReadOnly = true;
            this.AdvancedDataGridView1.Size = new System.Drawing.Size(1264, 561);
            this.AdvancedDataGridView1.TabIndex = 0;
            this.AdvancedDataGridView1.TimeFilter = false;
            this.AdvancedDataGridView1.SortStringChanged += new System.EventHandler(this.advancedDataGridView1_SortStringChanged);
            this.AdvancedDataGridView1.FilterStringChanged += new System.EventHandler(this.advancedDataGridView1_FilterStringChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(1264, 561);
            this.Controls.Add(this.AdvancedDataGridView1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AdvancedDataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ADGV.AdvancedDataGridView AdvancedDataGridView1;
    }
}