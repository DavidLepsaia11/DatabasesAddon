using System;
using Sap.Data.Hana;
using System.Data;
//using SAPbouiCOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Windows.Forms;
using ADGV;
using DatabasesAddon.Enums;
using System.Configuration;
using DatabasesAddon.Services;

namespace DatabasesAddon
{
    public class ReportController
    {
        private readonly SAPbobsCOM.Company Company;
        public HanaDbService HanaDbService { get; private set; }




        //Form components 
        public AdvancedDataGridView AdvancedGrid { get; set; }
        public ComboBox ComboBox { get; private set; }


        public ReportController(HanaDbService hanaDbService, Form1 activeForm)
        {
            HanaDbService = hanaDbService;
            ComboBox = activeForm.ComboBox;
        }

        public ReportController(SAPbobsCOM.Company company, Form1 activeForm, HanaDbService hanaDbService)
        {
            Company = company;
            HanaDbService = hanaDbService;
            AdvancedGrid = activeForm.WindGrid;
        }

        public DataTable FillGridWithCompareAmountsQuery(string updatedQuery = "")
        {
            try
            {
               var filledTable = HanaDbService.ExecuteQuery(GetAmountQueryColumnsTable(), updatedQuery);
                AdvancedGrid.DataSource = filledTable;

                return filledTable;
            }
            catch (Exception ex)
            {
                RSM.Core.SDK.UI.UIApplication.ShowError(ex.Message);

                return default;
            }
        }

        public DataTable FillGridwithCompareCashFlowsQuery(string updatedQuery = "")
        {
            try
            {
                var filledTable = HanaDbService.ExecuteQuery(GetCashFlowQueryColumnsTable(), updatedQuery);
                AdvancedGrid.DataSource = filledTable;
                return filledTable;
            }
            catch (Exception ex)
            {
                RSM.Core.SDK.UI.UIApplication.ShowError(ex.Message);
                return default;
            }
        }

        public void FilterGrid(DataTable resultTable, string columnName, params string[] values)
        {
            DataView dv = resultTable.DefaultView;
            object returnedValue;
            DataTypeEnum dataType;
            dv.RowFilter = string.Empty;
            bool isFirstLine = true;
            bool datehasHourAndMinute;

            foreach (var value in values)
            {
                (returnedValue, dataType) = ReturnDefinedType(value);
                if (dataType == DataTypeEnum.isDouble || dataType == DataTypeEnum.isInt)
                {
                    if (isFirstLine)
                    {
                        dv.RowFilter += $"{columnName} = {returnedValue}";
                        isFirstLine = false;
                    }
                    else dv.RowFilter += $" OR {columnName} = {returnedValue}";
                }
                else if (dataType == DataTypeEnum.isDateTime)
                {
                    var dateTime = (DateTime)returnedValue;
                    datehasHourAndMinute = dateTime.Hour != 0 && dateTime.Minute != 0;

                    if (isFirstLine)
                    {
                        dv.RowFilter += datehasHourAndMinute ? $"{columnName} = '{dateTime.Year}-{dateTime.Month}-{dateTime.Day} {dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}'" : $"{columnName} = '{dateTime.Year}-{dateTime.Month}-{dateTime.Day}'";
                        isFirstLine = false;
                    }
                    else dv.RowFilter += datehasHourAndMinute ? $" OR {columnName} = '{dateTime.Year}-{dateTime.Month}-{dateTime.Day} {dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}'" : $" OR {columnName} = '{dateTime.Year}-{dateTime.Month}-{dateTime.Day}'";
                }
                else
                {
                    if (isFirstLine)
                    {
                        dv.RowFilter += $"{columnName} LIKE '%{returnedValue}%'";
                        isFirstLine = false;
                    }
                    else dv.RowFilter += $" OR {columnName} LIKE '%{returnedValue}%'";
                }
            }

        }

        public void SortGrid(DataTable resultTable, string sortString)
        {
            DataView dv = resultTable.DefaultView;
            sortString = sortString.Replace("[", "").Replace("]", "");
            dv.Sort = sortString;

            AdvancedGrid.DataSource = dv;
        }

        public void ShowAllRows(DataTable resultTable)
        {
            DataView dv = resultTable.DefaultView;
            dv.RowFilter = string.Empty;
        }

        public void FillComboBox()
        {
            string query = $"select \"U_DBName\" from \"@RSM_IC_DBLIST1\" where \"U_DBType\" = 2";

            var resultTable = new DataTable();
            resultTable.Columns.Add("U_DBName", typeof(string));

            var dbNames = HanaDbService.ExecuteQuery(resultTable, query);

            var list = new List<object>();

            for (int i = 0; i < dbNames.Rows.Count; i++)
            {
               ComboBox.Items.Add(dbNames.Rows[i]["U_DBName"]);
            }
        }

        #region Creating Table Functions

        private  DataTable GetAmountQueryColumnsTable()
        {
            var resultTable = new DataTable(); 

            resultTable.Columns.Add("U_SenderBranchCompany", typeof(string));
            resultTable.Columns.Add("U_OriginlJdNum", typeof(int));
            resultTable.Columns.Add("Debit", typeof(double));
            resultTable.Columns.Add("Credit", typeof(double));
            resultTable.Columns.Add("CntRows", typeof(int));
            resultTable.Columns.Add("DBName", typeof(string));
            resultTable.Columns.Add("TransId", typeof(int));
            resultTable.Columns.Add("Debit2", typeof(string));
            resultTable.Columns.Add("Credit2", typeof(string));
            resultTable.Columns.Add("RefDate", typeof(DateTime));
            resultTable.Columns.Add("CreateDate", typeof(DateTime));
            resultTable.Columns.Add("U_UpdateTS", typeof(DateTime));
            resultTable.Columns.Add("SyncMessage", typeof(string));
            resultTable.Columns.Add("SyncDate", typeof(DateTime));
            resultTable.Columns.Add("UpdateMessage", typeof(string));
            resultTable.Columns.Add("LastUpdate", typeof(DateTime));

            return resultTable;
        }

        private  DataTable GetCashFlowQueryColumnsTable()
        {
            var resultTable = new DataTable();
            resultTable.Columns.Add("U_SenderBranchCompany", typeof(string));
            resultTable.Columns.Add("U_OriginlJdNum", typeof(int));
            resultTable.Columns.Add("Line_ID", typeof(int));
            resultTable.Columns.Add("MinMaxCFWName", typeof(string));
            resultTable.Columns.Add("MaxMaxCFWName", typeof(string));
            resultTable.Columns.Add("Debit", typeof(double));
            resultTable.Columns.Add("Credit", typeof(double));
            resultTable.Columns.Add("CountRows", typeof(int));
            resultTable.Columns.Add("BranchBase", typeof(string));
            resultTable.Columns.Add("TransId", typeof(int));
            resultTable.Columns.Add("Line_ID2", typeof(int));
            resultTable.Columns.Add("MaxCFWName", typeof(string));
            resultTable.Columns.Add("Debit2", typeof(double));
            resultTable.Columns.Add("Credit2", typeof(double));
            resultTable.Columns.Add("RefDate", typeof(DateTime));
            resultTable.Columns.Add("CreateDate", typeof(DateTime));
            resultTable.Columns.Add("U_UpdateTS", typeof(DateTime));
            resultTable.Columns.Add("SyncMessage", typeof(string));
            resultTable.Columns.Add("SyncDate", typeof(DateTime));
            resultTable.Columns.Add("UpdateMessage", typeof(string));
            resultTable.Columns.Add("LastUpdate", typeof(DateTime));

            return resultTable;
        }
        #endregion


        #region Helper Methods

        private (object, DataTypeEnum) ReturnDefinedType(string input)
        {
            int intValue;
            double doubleValue;
            DateTime dateTime;

            input = input.Replace("'", "").Trim();

            if (int.TryParse(input, out intValue))
            {
                return (intValue, DataTypeEnum.isInt);
            }
            else if (double.TryParse(input, out doubleValue))
            {
                return (doubleValue, DataTypeEnum.isDouble);
            }
            else if (DateTime.TryParse(input, out dateTime))
            {
                return (dateTime, DataTypeEnum.isDateTime);
            }
            else
            {
                return (input, DataTypeEnum.isString);
            }
        }

 
        #endregion

    }
}
