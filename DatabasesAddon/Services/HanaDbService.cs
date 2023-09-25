using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabasesAddon.Services
{
    public class HanaDbService
    {
        private readonly string _connection;
        private  string _queryFilepath;

        public HanaDbService(string connection, string queryFilepath = null)
        {
            _connection = connection;
            _queryFilepath = queryFilepath;
        }

        public DataTable ExecuteQuery(DataTable table, string query = null) 
        {
            using (var hanaConnection = new HanaConnection(_connection))
            {
                hanaConnection.Open();

                if (_queryFilepath != null)
                {
                    using (StreamReader reader = new StreamReader(_queryFilepath))
                    {
                        query = reader.ReadToEnd();
                    }
                }

                using (var cmd = new HanaCommand(query, hanaConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] array = new object[table.Columns.Count];

                            for (int j = 0; j < table.Columns.Count; j++)
                            {
                                var columnValue = reader.GetValue(j);
                                array[j] = columnValue;
                            }
                            table.Rows.Add(array);
                        }
                    }
                }
            }
            return table;
        }
    }
}
