using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DataAccessLib
{
    public static class DataAccessorInfo
    {
        internal static Type[] SupportedDbType;
        public static string[] SupportedDbTypeNames;
        public static Dictionary<string, string[]> SupportedDbConnectionStringFields;
        public static Dictionary<string, string[]> SupportedDbConnectionStringFieldsDefaultValue;
        static DataAccessorInfo()
        {
            SupportedDbType = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DataAccessorBase))).ToArray();
            SupportedDbTypeNames = SupportedDbType.Select(t => t.Name).ToArray();
            SupportedDbConnectionStringFields = SupportedDbType.ToDictionary(type => type.Name,
                type => (Activator.CreateInstance(type, Entities.DbConnectionStringInfo.Empty) as DataAccessorBase)?.ConnectionStringFieldNames);
            SupportedDbConnectionStringFieldsDefaultValue= SupportedDbType.ToDictionary(type => type.Name,
                type => (Activator.CreateInstance(type, Entities.DbConnectionStringInfo.Empty) as DataAccessorBase)?.ConnectionStringFieldDefaultValue);
        }
    }
}
