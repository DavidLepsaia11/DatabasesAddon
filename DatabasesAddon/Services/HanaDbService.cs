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
            connection = _connection;
            _queryFilepath = queryFilepath;
        }

        public DataTable ExecuteQuery(int columnsCount, string query = null) 
        {
            var resultTable = new DataTable();

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
                            object[] array = new object[columnsCount];

                            for (int j = 0; j < columnsCount; j++)
                            {
                                var columnValue = reader.GetValue(j);
                                array[j] = columnValue;
                            }
                            resultTable.Rows.Add(array);
                        }
                    }
                }
            }
            return resultTable;
        }
    }
}
