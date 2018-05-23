using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.Entities
{
    public class DbConnectionStringInfo 
    {
        public DbConnectionStringInfo(params string[] fields)
        {
            if (fields.Length < 1)
                throw new ArgumentException("Invalid parameters!");
            if (fields.Length == 1)
                ConnectionString = fields[0];
            else
                ConnecgtionStringFields = fields;
        }
        public string ConnectionString { get; set; }
        public string CreateConnectionString(Func<IEnumerable<string>, string> func)
        {
            return ConnectionString ?? (ConnectionString = func?.Invoke(ConnecgtionStringFields));
        }
        public IEnumerable<string> ConnecgtionStringFields { get; }
    }
}
