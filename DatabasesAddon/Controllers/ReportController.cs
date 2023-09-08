using DatabasesAddon.Helpers;
using SAPbouiCOM;
using System;
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
            string filePath = @"C:\Users\dlepsaia\source\repos\DatabasesAddon\DatabasesAddon\AddonFile\QueryFile.txt";
            string query = "";
            // "select  top 10 * from [@RSM_IC_DBLIST1] "
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    query = reader.ReadToEnd();
                }

                ReportGrid.Item.Enabled = false;
                ReportGrid.DataTable.ExecuteQuery(HanaTranslator.QueryHanaTransalte(query));
                ReportGrid.AutoResizeColumns();


            }
            catch (Exception ex)
            {
                RSM.Core.SDK.UI.UIApplication.ShowError(ex.Message);
            }

        }
    }
}
