using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Ninject.Modules;

namespace DataAccessLib
{
    #region OldDataAccessCode
    public abstract class OldDataAccessBase
    {
        internal OldDataAccessBase(string connectionString)
        {
            ConnectionString = SetTimeOut(connectionString);
            CheckConnection();
        }
        internal OldDataAccessBase(params string[] fields)
        {
            ConnectionString = SetTimeOut(BuildConnectionString(fields));
            CheckConnection();
        }

        internal string ConnectionString { get; }

        internal abstract string BuildConnectionString(params string[] fields);
        internal abstract string SetTimeOut(string connStr);
        internal abstract bool CheckConnection();

        public abstract string[] QueryAllTableName();

        public abstract DataSet QueryTables(string[] tableNames, Action processCallBack = null);
    }

    public class OldDataAccessFactory
    {
        public static OldDataAccessBase Create(string dbType, string connstr)
        {
            Type t = GetDbType(dbType);
            return (OldDataAccessBase)Activator.CreateInstance(t, connstr);
        }
        public static OldDataAccessBase Create(string dbType, params string[] fields)
        {
            var t = GetDbType(dbType);
            return (OldDataAccessBase)Activator.CreateInstance(t, fields);
        }

        private static Type GetDbType(string dbType)
        {
            var t = GetDbTypes().Single(dbt => dbt.Name.Equals(dbType, StringComparison.OrdinalIgnoreCase));
            if (t == null)
                throw new ArgumentException($"{nameof(dbType)}: {dbType}");
            if (!t.IsSubclassOf(typeof(OldDataAccessBase)))
                throw new Exception("bug");
            return t;
        }

        public static string[] DbTypes = GetDbTypesString().ToArray();

        public static string[] GetDbTypeConnectionFields(string dbType)
        {
            var t = GetDbType(dbType);
            return t.GetConstructors().First(c => c.GetParameters().Length > 1)?.GetParameters().Select(para => para.Name.Replace('_', ' ')).ToArray();
        }

        private static IEnumerable<Type> GetDbTypes()
        {
            var assembly = Assembly.GetAssembly(typeof(OldDataAccessFactory));
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(OldDataAccessBase)));
        }

        private static IEnumerable<string> GetDbTypesString()
        {
            return GetDbTypes().Select(t => t.Name);
        }
    }
    #endregion
}
