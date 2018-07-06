using ComparisonLib;
using EasyDatabaseCompare.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDatabaseCompare.ViewModel
{
    public partial class WindowViewModel
    {
        //private string[] _dbConnectionFields;
        private bool _canQuerySource;
        private bool _canQueryTarget;
        //private IDataQuery _querier;
        private bool _canStartComparer;
        private string _selecctedDbType;
        private bool _canCheckConnection;
        private string[] _selectedTables;
        private bool _canSelectTable;
        private List<ConnectionFieldInfo> _fields;
        private bool _customConnectionStringMode;
        private double _querySourceProcess;
        private double _queryTargetProcess;
        private double _comparerProcess;
        private string[] _tableNames;
        private bool _selectTableBlackListMode;
        private DataTable _comparerResultOverview;
        private DataTable _filteredComparerResultOverview;
        private string _filterString;
        private bool _hideEmptyTables = true;
        private bool _hideUnchangedTables = true;
        private bool _showSameColumn;
        private bool _showInsertColumn = true;
        private bool _showDeleteColumn = true;
        private bool _showChangedColumn = true;
        private string _overviewSelectedCellInfo;
        private DataTable _selectedDetail;

        internal DataSet SourceData
        {
            get => DataCache.SourceData;
            set
            {
                DataCache.SourceData = value;
                OnPropertyChanged(nameof(SourceData));
            }
        }
        internal DataSet TargetData
        {
            get => DataCache.TargetData;
            set
            {
                DataCache.TargetData = value;
                OnPropertyChanged(nameof(TargetData));
            }
        }

        public List<DataDiff> DataCompareResult
        {
            get => DataCache.DataCompareResult;
            set
            {
                DataCache.DataCompareResult = value;
                OnPropertyChanged(nameof(DataCompareResult));
            }
        }


        public string SelecctedDbType
        {
            get => _selecctedDbType;
            set
            {
                _selecctedDbType = value;
                OnPropertyChanged(nameof(SelecctedDbType));
            }
        }

        public List<ConnectionFieldInfo> Fields
        {
            get => _fields;
            internal set
            {
                _fields = value;
                OnPropertyChanged(nameof(Fields));
            }
        }

        public double QueryTargetProcess
        {
            get => _queryTargetProcess;
            set
            {
                _queryTargetProcess = value;
                OnPropertyChanged(nameof(QueryTargetProcess));
            }
        }

        public bool CanStartComparer
        {
            get => _canStartComparer;
            internal set
            {
                _canStartComparer = value;
                OnPropertyChanged(nameof(CanStartComparer));
            }
        }

        public bool CustomConnectionStringMode
        {
            get => _customConnectionStringMode;
            set
            {
                _customConnectionStringMode = value;
                OnPropertyChanged(nameof(CustomConnectionStringMode));
            }
        }

        public bool ConnectionChecked
        {
            get => _canCheckConnection;
            internal set
            {
                _canCheckConnection = value;
                OnPropertyChanged(nameof(ConnectionChecked));
            }
        }

        public string[] TableNames
        {
            get => _tableNames;
            set
            {
                _tableNames = value;
                OnPropertyChanged(nameof(TableNames));
            }
        }

        public bool CanSelectTable
        {
            get => _canSelectTable;
            internal set
            {
                _canSelectTable = value;
                OnPropertyChanged(nameof(CanSelectTable));
            }
        }
        public bool BlackListMode
        {
            get => _selectTableBlackListMode;
            set
            {
                _selectTableBlackListMode = value;
                OnPropertyChanged(nameof(BlackListMode));
            }
        }
        public string[] SelectedTables
        {
            get => _selectedTables;
            set
            {
                _selectedTables = value;
                OnPropertyChanged(nameof(SelectedTables));
            }
        }
        public string[] TargetTables => (BlackListMode ? TableNames.Except(SelectedTables) : SelectedTables)?.ToArray();

        public bool CanQuerySource
        {
            get => _canQuerySource;
            internal set
            {
                _canQuerySource = value;
                OnPropertyChanged(nameof(CanQuerySource));
            }
        }


        public double QuerySourceProcess
        {
            get => _querySourceProcess;
            set
            {
                _querySourceProcess = value;
                OnPropertyChanged(nameof(QuerySourceProcess));
            }
        }

        public bool CanQueryTarget
        {
            get => _canQueryTarget;
            internal set
            {
                _canQueryTarget = value;
                OnPropertyChanged(nameof(CanQueryTarget));
            }
        }

        public DataTable ComparerResultOverview
        {
            get => _comparerResultOverview;
            set
            {
                _comparerResultOverview = value;
                OnPropertyChanged(nameof(ComparerResultOverview));
            }
        }
        #region Filters for overview
        public bool HideEmptyTables
        {
            get => _hideEmptyTables;
            set
            {
                _hideEmptyTables = value;
                OnPropertyChanged(nameof(HideEmptyTables));
            }
        }
        public bool HideUnchangedTables
        {
            get => _hideUnchangedTables;
            set
            {
                _hideUnchangedTables = value;
                OnPropertyChanged(nameof(HideUnchangedTables));
            }
        }
        public bool ShowSameColumn
        {
            get => _showSameColumn;
            set
            {
                _showSameColumn = value;
                OnPropertyChanged(nameof(ShowSameColumn));
            }
        }
        public bool ShowInsertColumn
        {
            get => _showInsertColumn;
            set
            {
                _showInsertColumn = value;
                OnPropertyChanged(nameof(ShowInsertColumn));
            }
        }
        public bool ShowDeleteColumn
        {
            get => _showDeleteColumn;
            set
            {
                _showDeleteColumn = value;
                OnPropertyChanged(nameof(ShowDeleteColumn));
            }
        }
        public bool ShowChangedColumn
        {
            get => _showChangedColumn;
            set
            {
                _showChangedColumn = value;
                OnPropertyChanged(nameof(ShowChangedColumn));
            }
        }
        #endregion
        public DataTable FilteredComparerResultOverview
        {
            get => _filteredComparerResultOverview;
            set
            {
                _filteredComparerResultOverview = value;
                OnPropertyChanged(nameof(FilteredComparerResultOverview));
            }
        }

        public string FilterString
        {
            get => _filterString;
            set
            {
                _filterString = value;
                OnPropertyChanged(nameof(FilterString));
            }
        }

        public double ComparerProcess
        {
            get => _comparerProcess;
            set
            {
                _comparerProcess = value;
                OnPropertyChanged(nameof(ComparerProcess));
            }
        }

        public string[] SurpportDbType { get; }
        public string[] DbConnectionFields { get; set; }
        public string[] DbConnectionFieldsDefaultValue { get; set; }

        public string OverviewSelectedCellInfo
        {
            get => _overviewSelectedCellInfo;
            set
            {
                _overviewSelectedCellInfo = value;
                OnPropertyChanged(nameof(OverviewSelectedCellInfo));
            }
        }

        public DataTable SelectedDetail
        {
            get => _selectedDetail;
            set
            {
                _selectedDetail = value;
                OnPropertyChanged(nameof(SelectedDetail));
            }
        }
        public Dictionary<DataRow, string[]> DiffFields { get; private set; }

        public void OnPropertyChanged(string changedPropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(changedPropertyName));
        }
    }
}
