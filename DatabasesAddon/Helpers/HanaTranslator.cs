using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translator;

namespace DatabasesAddon.Helpers
{
    public static class HanaTranslator
    {
        public static string QueryHanaTransalte(string query)
        {
            var XCompany = RSM.Core.SDK.DI.DIApplication.Company;
            bool IsHana = (XCompany.DbServerType.ToString() == "dst_HANADB" ? true : false);
            if (IsHana)
            {
                int numOfStatements;
                int numOfErrors;
                TranslatorTool TranslateTool = new TranslatorTool();
                query = TranslateTool.TranslateQuery(query, out numOfStatements, out numOfErrors);
                return query;
            }
            else
            {
                return query;
            }
        }
    }
}
