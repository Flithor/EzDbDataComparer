using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComparisonLib.Common;

namespace ComparisonLib
{
    public static class DataAccessorInfo
    {
        private static InternalDictionary<string, string[]> _supportedDbConnectionStringFields;
        private static InternalDictionary<string, string[]> _supportedDbConnectionStringFieldsDefaultValue;
        //internal static string[] SupportedDbTypeNames => DataAccessLib.DataAccessorInfo.SupportedDbTypeNames;
        public static InternalDictionary<string, string[]> SupportedDbConnectionStringFields =>
            _supportedDbConnectionStringFields ?? (_supportedDbConnectionStringFields =
                DataAccessLib.DataAccessorInfo.SupportedDbConnectionStringFields);

        public static InternalDictionary<string, string[]> SupportedDbConnectionStringFieldsDefaultValue =>
            _supportedDbConnectionStringFieldsDefaultValue ?? (_supportedDbConnectionStringFieldsDefaultValue =
            DataAccessLib.DataAccessorInfo.SupportedDbConnectionStringFieldsDefaultValue);


    }
}
