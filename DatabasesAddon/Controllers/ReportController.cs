using System;
using DatabasesAddon.Helpers;
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



        public ReportController(SAPbobsCOM.Company company, Form1 activeForm, HanaDbService hanaDbService)
        {
            Company = company;
            HanaDbService = hanaDbService;
            ComboBox = activeForm.ComboBox;
            AdvancedGrid = activeForm.WindGrid;
        }

        public void FillGridWithCompareAmountsQuery()
        {
            try
            {
               var resultTable = HanaDbService.ExecuteQuery(GetAmountQueryColumnsList());
                AdvancedGrid.DataSource = resultTable;
            }
            catch (Exception ex)
            {
                RSM.Core.SDK.UI.UIApplication.ShowError(ex.Message);
            }
        }

        public void FillGridwithCompareCashFlowsQuery()
        {
            try
            {
                var resultTable = HanaDbService.ExecuteQuery(GetCashFlowQueryColumnList());
                AdvancedGrid.DataSource = resultTable;
            }
            catch (Exception ex)
            {
                RSM.Core.SDK.UI.UIApplication.ShowError(ex.Message);
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

        public void ShowAllRows()
        {
            //DataView dv = ResultTable.DefaultView;
            //dv.RowFilter = string.Empty;
        }

        public void FillComboBox()
        {
            string query = "Select DatabaseName from ODBC";
            var resultTable = new DataTable();
            var dbNames = HanaDbService.ExecuteQuery(resultTable, query);

            foreach (var row in dbNames.Rows)
            {
                ComboBox.Items.Add(row);
            }
        }

        #region Creating Table Functions

        public static DataTable GetAmountQueryColumnsList()
        {
            var resultTable = new DataTable(); 

            resultTable.Columns.Add("U_SenderBranchCompany", typeof(string));
            resultTable.Columns.Add("U_OriginlJdNum", typeof(int));
            resultTable.Columns.Add("Debit", typeof(double));
            resultTable.Columns.Add("Credit", typeof(double));
            resultTable.Columns.Add("CntRows", typeof(int));
            resultTable.Columns.Add("SRGRE", typeof(string));
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

        public static DataTable GetCashFlowQueryColumnList()
        {
            var resultTable = new DataTable();
            return resultTable;
        }
        #endregion


        #region Private Methods

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
