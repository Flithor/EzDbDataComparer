using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.Entities
{
    public class DbConnectionStringInfo
    {
        public static DbConnectionStringInfo Empty { get; } = new DbConnectionStringInfo(string.Empty);
        public DbConnectionStringInfo(params string[] fields)
        {
            if (fields.Length < 1)
                throw new ArgumentException("Invalid parameters!");
            if (fields.Length == 1)
                ConnectionString = fields[0];
            else
                ConnecgtionStringFieldValues = fields;
        }
        public string ConnectionString { get; set; }
        public string CreateConnectionString(Func<string[], string> func)
        {
            return ConnectionString ?? (ConnectionString = func?.Invoke(ConnecgtionStringFieldValues));
        }
        public string[] ConnecgtionStringFieldValues { get; }
    }
}
