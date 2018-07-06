using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ComparisonLib;

namespace DataModelsLib.Model
{
    public class DataCacheModel : INotifyPropertyChanged
    {
        private string[] _surpportDbType;
        private string[] _dbConnectionFields;
        private QueryResults _sourceData;
        private QueryResults _targetData;
        private IEnumerable<DataDiff> _dataDifference;
        internal IDataQuery Querier { get; set; }

        internal QueryResults SourceData
        {
            get => _sourceData;
            set
            {
                _sourceData = value;
                OnPropertyChanged(nameof(SourceData));
            }
        }

        internal QueryResults TargetData
        {
            get => _targetData;
            set
            {
                _targetData = value;
                OnPropertyChanged(nameof(TargetData));
            }
        }

        public IEnumerable<DataDiff> DataDifference
        {
            get => _dataDifference;
            set
            {
                _dataDifference = value;
                OnPropertyChanged(nameof(DataDifference));
            }
        }

        public string[] SurpportDbType
        {
            get => _surpportDbType;
            internal set
            {
                _surpportDbType = value;
                OnPropertyChanged(nameof(SurpportDbType));
            }
        }

        public string[] DbConnectionFields
        {
            get => _dbConnectionFields;
            internal set
            {
                _dbConnectionFields = value;
                OnPropertyChanged(nameof(DbConnectionFields));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
