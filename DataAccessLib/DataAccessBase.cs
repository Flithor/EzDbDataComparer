using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static DataAccessLib.DataAccessBase;

namespace DataAccessLib
{
    public abstract class DataAccessBase
    {
        internal DataAccessBase(string connectionString)
        {
            ConnectionString = SetTimeOut(connectionString);
            CheckConnection();
        }
        internal DataAccessBase(params string[] fields)
        {
            ConnectionString = SetTimeOut(BuildConnectionString(fields));
            CheckConnection();
        }

        internal string ConnectionString { get; }

        internal abstract string BuildConnectionString(params string[] fields);
        internal abstract string SetTimeOut(string connStr);
        internal abstract bool CheckConnection();

        public abstract IEnumerable<string> QueryAllTableName();

        public abstract DataSet QueryTables(IEnumerable<string> TableNames, bool withSchma = true, Action processCallBack = null);
    }

    public class DataAccessFactory
    {
        public static DataAccessBase Create(string dbType, string connstr)
        {
            Type t = GetDBType(dbType);
            return (DataAccessBase)Activator.CreateInstance(t, connstr);
        }
        public static DataAccessBase Create(string dbType, params string[] fields)
        {
            Type t = GetDBType(dbType);
            return (DataAccessBase)Activator.CreateInstance(t, fields);
        }

        private static Type GetDBType(string dbType)
        {
            var t = GetDbTypes().Single(dbt => TrimTypeName(dbt.Name).Equals(dbType, StringComparison.OrdinalIgnoreCase));
            if(t == null)
                throw new ArgumentException($"{nameof(dbType)}: {dbType}");
            if(!t.IsSubclassOf(typeof(DataAccessBase)))
                throw new Exception("bug");
            return t;
        }

        public static string[] DbTypes = GetDbTypesString().ToArray();

        public static string[] GetDBTypeConnectionFields(string dbType)
        {
            Type t = GetDBType(dbType);
            var constructors = t.GetConstructors();
            var targetConstructor = constructors.Where(c => c.GetParameters().Length > 1).First();
            return targetConstructor.GetParameters().Select(para => para.Name).ToArray();
        }

        private static IEnumerable<Type> GetDbTypes()
        {
            var assembly = Assembly.GetAssembly(typeof(DataAccessFactory));
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(DataAccessBase)));
        }

        private static string TrimTypeName(string name)
        {
            var pattern = "(.*)DataAccess";
            var m = Regex.Match(name, pattern);
            return m.Groups[1].Value;
        }

        private static IEnumerable<string> GetDbTypesString()
        {
            return GetDbTypes().Select(t => TrimTypeName(t.Name));
        }
    }
}
