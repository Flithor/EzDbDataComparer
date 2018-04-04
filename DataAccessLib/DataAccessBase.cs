using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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

        public abstract string[] QueryAllTableName();

        public abstract DataSet QueryTables(string[] tableNames, bool withSchma = true, Action processCallBack = null);
    }

    public class DataAccessFactory
    {
        public static DataAccessBase Create(string dbType, string connstr)
        {
            Type t = GetDbType(dbType);
            return (DataAccessBase)Activator.CreateInstance(t, connstr);
        }
        public static DataAccessBase Create(string dbType, params string[] fields)
        {
            var t = GetDbType(dbType);
            return (DataAccessBase)Activator.CreateInstance(t, fields);
        }

        private static Type GetDbType(string dbType)
        {
            var t = GetDbTypes().Single(dbt => TrimTypeName(dbt.Name).Equals(dbType, StringComparison.OrdinalIgnoreCase));
            if (t == null)
                throw new ArgumentException($"{nameof(dbType)}: {dbType}");
            if (!t.IsSubclassOf(typeof(DataAccessBase)))
                throw new Exception("bug");
            return t;
        }

        public static string[] DbTypes = GetDbTypesString().ToArray();

        public static string[] GetDbTypeConnectionFields(string dbType)
        {
            var t = GetDbType(dbType);
            return t.GetConstructors().First(c => c.GetParameters().Length > 1)?.GetParameters().Select(para => para.Name.Replace('_',' ')).ToArray();
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
