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

namespace DatabasesAddon
{
    public class ReportController
    {
     //   private readonly IForm Form;
        private readonly SAPbobsCOM.Company Company;

      //  public SAPbouiCOM.Grid ReportGrid { get { return (SAPbouiCOM.Grid)Form.Items.Item("Item_2").Specific; } }
        public AdvancedDataGridView WindGrid { get; set; }

        public  BindingSource BindingSource { get; private set; }

        public DataTable ResultTable { get; private set; }
        //public ReportController(SAPbobsCOM.Company company, SAPbouiCOM.IForm form)
        //{
        //    Company = company;
        //    Form = form;
        //}

        public ReportController(SAPbobsCOM.Company company, Form1 ActiveForm)
        {
            Company = company;
            WindGrid = ActiveForm.WindGrid;
            ResultTable = new System.Data.DataTable();
        }

        public void FillReportGrid()
        {
            string filePath = @"C:\Users\dlepsaia\source\repos\DatabasesAddon\DatabasesAddon\AddonFile\QueryFile.txt";
            string query = "";
            string connectionString = "Server=10.132.10.104:30015;UserName=SYSTEM;Password=GusSERta1;CS=CONS_MANAGEMENT";

            try
            {
                using (var hanaConnection = new HanaConnection(connectionString))
                {
                    hanaConnection.Open();
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        query = reader.ReadToEnd();
                    }
                    using (var cmd = new HanaCommand(query, hanaConnection))
                    {
                        var dict = new Dictionary<string, string>();

                        using (var reader = cmd.ExecuteReader())
                        {
                            var columns = GetColumnsList();
                            while (reader.Read())
                            {
                                object[] array = new object[columns.Count];

                                for (int j = 0; j < columns.Count; j++)
                                {
                                    var columnValue = reader.GetValue(j);
                                    array[j] = columnValue;
                                }
                                ResultTable.Rows.Add(array);
                            }
                        }
                    }
                }
                WindGrid.DataSource = ResultTable;
            }
            catch (Exception ex)
            {
                RSM.Core.SDK.UI.UIApplication.ShowError(ex.Message);
            }

        }

        private List<string> GetColumnsList()
        {
            var columnList = new List<string>();

            ResultTable.Columns.Add("U_SenderBranchCompany", typeof(string));
            ResultTable.Columns.Add("U_OriginlJdNum", typeof(int));
            ResultTable.Columns.Add("Debit", typeof(double));
            ResultTable.Columns.Add("Credit", typeof(double));
            ResultTable.Columns.Add("CntRows", typeof(int));
            ResultTable.Columns.Add("SRGRE", typeof(string));
            ResultTable.Columns.Add("TransId", typeof(int));
            ResultTable.Columns.Add("Debit2", typeof(string));
            ResultTable.Columns.Add("Credit2", typeof(string));
            ResultTable.Columns.Add("RefDate", typeof(DateTime));
            ResultTable.Columns.Add("CreateDate", typeof(DateTime));
            ResultTable.Columns.Add("U_UpdateTS", typeof(DateTime));
            ResultTable.Columns.Add("SyncMessage", typeof(string));
            ResultTable.Columns.Add("SyncDate", typeof(DateTime));
            ResultTable.Columns.Add("UpdateMessage", typeof(string));
            ResultTable.Columns.Add("LastUpdate", typeof(DateTime));


            for (int i = 0; i < ResultTable.Columns.Count; i++)
            {
                columnList.Add(ResultTable.Columns[i].ColumnName);
            }

            return columnList;
        }

        public void FilterGrid(string columnName, params string[] values) 
        {
            DataView dv = ResultTable.DefaultView;
            object returnedValue;
            DataTypeEnum dataType;
            dv.RowFilter = string.Empty;
            bool isFirstLine = true;
            bool datehasHourAndMinute;

            foreach (var value in values)
            {
              ( returnedValue, dataType) = ReturnDefinedType(value);
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
                    var dateTime = (DateTime) returnedValue;
                    datehasHourAndMinute = dateTime.Hour != 0 && dateTime.Minute != 0;

                    if (isFirstLine) 
                    {
                        dv.RowFilter += datehasHourAndMinute ? $"{columnName} = '{dateTime.Year}-{dateTime.Month}-{dateTime.Day} {dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}'"  : $"{columnName} = '{dateTime.Year}-{dateTime.Month}-{dateTime.Day}'";
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

        public void SortGrid(string sortString) 
        {
            DataView dv = ResultTable.DefaultView;
            sortString = sortString.Replace("[", "").Replace("]", "");
            dv.Sort = sortString;

            WindGrid.DataSource = dv;
        }

        private (object, DataTypeEnum) ReturnDefinedType(string input) 
        {
            int intValue;
            double doubleValue;
            DateTime dateTime;

            input = input.Replace("'", "");

            if (int.TryParse(input, out intValue))
            {
                return (intValue, DataTypeEnum.isInt);
            }
            else if (double.TryParse(input, out doubleValue))
            {
                return (doubleValue, DataTypeEnum.isDouble);
            }
            else if(DateTime.TryParse(input, out dateTime))
            {
                return (dateTime, DataTypeEnum.isDateTime);
            }
            else
            {
                return (input, DataTypeEnum.isString);
            }
        }
    }
}
