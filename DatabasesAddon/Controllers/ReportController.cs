using System;
using DatabasesAddon.Helpers;
using Sap.Data.Hana;
using SAPbouiCOM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabasesAddon
{
    public class ReportController
    {
        private readonly IForm Form;
        private readonly SAPbobsCOM.Company Company;

        public SAPbouiCOM.Grid ReportGrid { get { return (SAPbouiCOM.Grid)Form.Items.Item("Item_2").Specific; } }

        public ReportController(SAPbobsCOM.Company company, SAPbouiCOM.IForm form)
        {
            Company = company;
            Form = form;
        }

        public void FillReportGrid()
        {
            //string filePath = @"C:\Users\dlepsaia\source\repos\DatabasesAddon\DatabasesAddon\AddonFile\QueryFile.txt";
            string query = "Select top 10  \"TransId\" from \"OJDT\"";
            //string server = "10.132.10.104:30015";
            //string user = "SYSTEM";
            //string password = "GusSERta1";
            string connectionString = "Server=10.132.10.104:30015;UserName=SYSTEM;Password=GusSERta1;CS=TSINANDALIREAL";

            try
            {

                using (var hanaConnection = new HanaConnection(connectionString))
                {
                    hanaConnection.Open();
                    //using (StreamReader reader = new StreamReader(filePath))
                    //{
                    //    query = reader.ReadToEnd();
                    //}
                    using (var cmd = new HanaCommand(query, hanaConnection))
                    {
                        using (var reader = cmd.ExecuteReader()) 
                        {
                            // Print column names
                            var sbCol = new System.Text.StringBuilder();
                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                if (i > 0)
                                {
                                    sbCol.Append(", ");
                                }
                                sbCol.Append(reader.GetName(i));
                            }
                             var rame = sbCol.ToString();

                            // Print rows
                            while (reader.Read())
                            {
                                var sbRow = new System.Text.StringBuilder();
                                for (var i = 0; i < reader.FieldCount; i++)
                                {
                                    if (i > 0)
                                    {
                                        sbRow.Append(", ");
                                    }
                                    sbRow.Append(reader[i]);
                                }
                                Console.WriteLine(sbRow.ToString());
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                RSM.Core.SDK.UI.UIApplication.ShowError(ex.Message);
            }

        }
    }
}
