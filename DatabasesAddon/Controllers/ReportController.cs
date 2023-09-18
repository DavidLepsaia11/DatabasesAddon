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


namespace DatabasesAddon
{
    public class ReportController
    {
     //   private readonly IForm Form;
        private readonly SAPbobsCOM.Company Company;

      //  public SAPbouiCOM.Grid ReportGrid { get { return (SAPbouiCOM.Grid)Form.Items.Item("Item_2").Specific; } }
        public DataGridView WindGrid { get; set; }

        //public ReportController(SAPbobsCOM.Company company, SAPbouiCOM.IForm form)
        //{
        //    Company = company;
        //    Form = form;
        //}

        public ReportController(SAPbobsCOM.Company company, Form1 ActiveForm)
        {
            Company = company;
            WindGrid = ActiveForm.WindGrid;
        }

        public void FillReportGrid()
        {
            string filePath = @"C:\Users\dlepsaia\source\repos\DatabasesAddon\DatabasesAddon\AddonFile\QueryFile.txt";
            string query = "Select top 10  \"TransId\" , \"SysTotal\" from \"OJDT\"";
            //string server = "10.132.10.104:30015";
            //string user = "SYSTEM";
            //string password = "GusSERta1";
            string connectionString = "Server=10.132.10.104:30015;UserName=SYSTEM;Password=GusSERta1;CS=CONS_MANAGEMENT";

            try
            {
                var resultDataTable = new System.Data.DataTable();

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
                            var columns = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string colname = reader.GetName(i);
                                //   ReportGrid.DataTable.Columns.Add(colname, BoFieldsType.ft_AlphaNumeric);
                                if (resultDataTable.Columns.Contains(colname))
                                {
                                    colname += "2";
                                    resultDataTable.Columns.Add(colname);
                                }
                                else
                                {
                                    resultDataTable.Columns.Add(colname);
                                }
                                columns.Add(colname);
                            }

                            //// Print rows
                            //int indx = 1;

                            //foreach (string column in listColumns)
                            //{
                            //    while (reader.Read())
                            //    {

                            //    }
                            //}
                            // int indx;
                            while (reader.Read())
                            {
                                // indx = 0;
                                //   ReportGrid.DataTable.Rows.Add();
                                object[] array = new object[columns.Count];

                                for (int j = 0; j < columns.Count; j++)
                                {
                                    var columnValue = reader.GetValue(j);
                                    array[j] = columnValue;
                                }
                                resultDataTable.Rows.Add(array);
                                //  //  ReportGrid.DataTable.SetValue(columns[j], indx, columnValue);
                            }
                        }
                    }
                }
                WindGrid.DataSource = resultDataTable;
            }
            catch (Exception ex)
            {
                RSM.Core.SDK.UI.UIApplication.ShowError(ex.Message);
            }

        }
    }
}
