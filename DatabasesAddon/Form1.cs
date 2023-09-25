﻿using ADGV;
using DatabasesAddon.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabasesAddon
{
    public partial class Form1 : Form
    {
        public ReportController Controller { get; private set; }

        public AdvancedDataGridView WindGrid { get { return AdvancedDataGridView1; } }

        public ComboBox ComboBox { get { return comboBox1; } }

        public Form1()
        {
              InitializeComponent();
          //    Controller = new ReportController(RSM.Core.SDK.DI.DIApplication.Company, this, new HanaDbService(ConfigurationManager.AppSettings["ConnectionString"]));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // Controller.FillGridWithCompareAmountsQuery();
        }

        private void advancedDataGridView1_SortStringChanged(object sender, EventArgs e)
        {
            var advancedGrid = (AdvancedDataGridView)sender;
            var sortString = advancedGrid.SortString;

            string pattern = @"\[[^\]]+\] (ASC|DESC)";
            Match match = Regex.Match(sortString, pattern);

            if (match.Success)
            {
                var resultTable = radioButton1.Checked ? ReportController.GetAmountQueryColumnsList() : ReportController.GetCashFlowQueryColumnList();
                Controller.SortGrid(resultTable,sortString);
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

                var resultTable = radioButton1.Checked ? ReportController.GetAmountQueryColumnsList() : ReportController.GetCashFlowQueryColumnList();
                Controller.FilterGrid(resultTable, columnName, splittedFilters);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Controller.ShowAllRows();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            var query = ConfigurationManager.AppSettings["CompareAmountsQuery"];

            Controller = new ReportController(RSM.Core.SDK.DI.DIApplication.Company, this, new HanaDbService(connectionString , query));
            Controller.FillGridWithCompareAmountsQuery();
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            Controller = new ReportController(RSM.Core.SDK.DI.DIApplication.Company, this, new HanaDbService(ConfigurationManager.AppSettings["ConnectionString"], ConfigurationManager.AppSettings["CompareCashFlowsQuery"]));
            Controller.FillGridwithCompareCashFlowsQuery();
        }
    }
}
