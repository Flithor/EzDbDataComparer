using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ComparisonLib;

namespace EasyDatabaseCompare.Model
{
    public class DataCacheModel
    {
        internal DataSet SourceData { get; set; }
        internal DataSet TargetData { get; set; }
        public List<DataDiff> DataCompareResult { get; set; }
    }
    public class ConnectionFieldInfo
    {
        public ConnectionFieldInfo(string fieldName, string fieldValue)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
        }
        public string FieldName { get; }
        public string FieldValue { get; set; }
    }
}
