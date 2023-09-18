using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabasesAddon
{
    public partial class Form1 : Form
    {
        public ReportController Controller { get; private set; }

        public DataGridView WindGrid { get { return dataGridView1; } }

        public Form1()
        {
              InitializeComponent();
              Controller = new ReportController(RSM.Core.SDK.DI.DIApplication.Company, this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Controller.FillReportGrid();
        }
    }
}
