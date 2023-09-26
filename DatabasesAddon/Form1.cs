using ADGV;
using DatabasesAddon.Services;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DatabasesAddon
{
    public partial class Form1 : Form
    {
        public ReportController Controller { get; private set; }

        public AdvancedDataGridView WindGrid { get { return AdvancedDataGridView1; } }

        public ComboBox ComboBox { get { return comboBox1; } }

        private string _updatedQuery;

        public Form1()
        {
            InitializeComponent();
            Controller = new ReportController(new HanaDbService(ConfigurationManager.AppSettings["ConnectionString"]), this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Controller.FillComboBox();
        }

        private void advancedDataGridView1_SortStringChanged(object sender, EventArgs e)
        {
            var advancedGrid = (AdvancedDataGridView)sender;
            var sortString = advancedGrid.SortString;

            string pattern = @"\[[^\]]+\] (ASC|DESC)";
            Match match = Regex.Match(sortString, pattern);

            if (match.Success)
            {
                var resultTable = radioButton1.Checked ? Controller.FillGridWithCompareAmountsQuery() : Controller.FillGridwithCompareCashFlowsQuery();

                Controller.SortGrid(resultTable, sortString);
            }
        }

        private void advancedDataGridView1_FilterStringChanged(object sender, EventArgs e)
        {
            var advancedGrid = (AdvancedDataGridView)sender;
            var filterString = advancedGrid.FilterString;

            string pattern = @"\(\[([^\]]+)\] IN \(([^)]+)\)\)";
            Match match = Regex.Match(filterString, pattern);

            if (match.Success)
            {
                string columnName = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                var splittedFilters = value.Split(',');

                var resultTable = radioButton1.Checked ? Controller.FillGridWithCompareAmountsQuery() : Controller.FillGridwithCompareCashFlowsQuery();

                Controller.FilterGrid(resultTable, columnName, splittedFilters);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable resultTable = radioButton1.Checked ? Controller.FillGridWithCompareAmountsQuery() : Controller.FillGridwithCompareCashFlowsQuery();

            if (resultTable != null)
            {
              Controller.ShowAllRows(resultTable);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            var query = ConfigurationManager.AppSettings["CompareAmountsQuery"];

            Controller = new ReportController(RSM.Core.SDK.DI.DIApplication.Company, this, new HanaDbService(connectionString, query));

            if (_updatedQuery != null) Controller.FillGridWithCompareAmountsQuery(_updatedQuery);
            else Controller.FillGridWithCompareAmountsQuery();
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            Controller = new ReportController(RSM.Core.SDK.DI.DIApplication.Company, this, new HanaDbService(ConfigurationManager.AppSettings["ConnectionString"], ConfigurationManager.AppSettings["CompareCashFlowsQuery"]));

            if (_updatedQuery != null) Controller.FillGridwithCompareCashFlowsQuery(_updatedQuery);
            else Controller.FillGridwithCompareCashFlowsQuery();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = comboBox1.GetItemText(comboBox1.SelectedItem); 
            string changedQuery = radioButton1.Checked ?  ChangeQuery(selectedValue, ConfigurationManager.AppSettings["CompareAmountsQuery"]) :
               ChangeQuery(selectedValue, ConfigurationManager.AppSettings["CompareCashFlowsQuery"]);

            if (radioButton1.Checked)
            {
                Controller.FillGridWithCompareAmountsQuery(changedQuery);
            }
            else if (radioButton2.Checked) 
            {
                Controller.FillGridwithCompareCashFlowsQuery(changedQuery);
            }
            
        }

        #region Helper Methods

        private string ChangeQuery(string newString, string queryFilepath, string searchString = "CONSBASESRGRE")
        {
            using (StreamReader reader = new StreamReader(queryFilepath))
            {
                _updatedQuery = reader.ReadToEnd();
            }

            _updatedQuery = _updatedQuery.Replace(searchString, newString);
         
            return _updatedQuery;
        }

        #endregion
    }
}
