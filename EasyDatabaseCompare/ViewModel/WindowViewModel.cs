using System.Collections.Generic;
using ComparisonLib;
using EasyDatabaseCompare.Model;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Input;
using System;
using System.Collections;

namespace EasyDatabaseCompare.ViewModel
{
    public interface IWindowViewModel : INotifyPropertyChanged
    {
        List<DataDiff> DataCompareResult { get; set; }
        string SelectedDbType { get; set; }
        List<ConnectionFieldInfo> Fields { get; }
        double QueryTargetProcess { get; set; }
        bool CanStartComparer { get; }
        bool CustomConnectionStringMode { get; set; }
        bool ConnectionChecked { get; }
        string[] TableNames { get; set; }
        bool CanSelectTable { get; }
        bool BlackListMode { get; set; }
        HashSet<string> SelectedTables { get; set; }
        string[] TargetTables { get; }
        bool CanQuerySource { get; }
        double QuerySourceProcess { get; set; }
        bool CanQueryTarget { get; }
        DataTable ComparerResultOverview { get; set; }
        bool HideEmptyTables { get; set; }
        bool HideUnchangedTables { get; set; }
        bool ShowSameColumn { get; set; }
        bool ShowInsertColumn { get; set; }
        bool ShowDeleteColumn { get; set; }
        bool ShowChangedColumn { get; set; }
        DataTable FilteredComparerResultOverview { get; set; }
        string FilterString { get; set; }
        double ComparerProcess { get; set; }
        string[] SurpportDbType { get; }
        string[] DbConnectionFields { get; }
        string OverviewSelectedCellInfo { get; set; }
        DataTable SelectedDetail { get; set; }
        Dictionary<DataRow, string[]> DiffFields { get; set; }

        ICommand ResetCommand { get; }
        ICommand CheckConnectionCommand { get; }
        ICommand QuerySourceCommand { get; }
        ICommand QueryTargetCommand { get; }
        ICommand QueryTargetWithComparerCommand { get; }
        ICommand StartComparerCommand { get; }
        ICommand MoveTargetToSourceCommand { get; }
        ICommand DisplayTargetDetailCommand { get; }
    }

    public partial class WindowViewModel : IWindowViewModel
    {
        public WindowViewModel(DataCacheModel dataCeche, IDataQuery querier, IDataComparer comparator)
        {
            DataCache = dataCeche;

            Querier = querier;
            Comparator = comparator;

            ResetCommand = new ActionCommand(Reset);

            CheckConnectionCommand = new ActionCommand(CheckConnection);
            QuerySourceCommand = new ActionCommand(QuerySource);
            QueryTargetCommand = new ActionCommand(QueryTarget);
            QueryTargetWithComparerCommand = new ActionCommand(QueryTargetWithComparer);
            StartComparerCommand = new ActionCommand(StartComparer);
            MoveTargetToSourceCommand = new ActionCommand(MoveTargetToSource);

            DisplayTargetDetailCommand = new WithParameterCommand(DisplayTargetDetail);

            SurpportDbType = DataAccessorInfo.SupportedDbConnectionStringFields.Keys.ToArray();
            SelectedDbType = SurpportDbType[0];
            SelectedDbTypeChanged();

            PropertyChanged += PropertyChangedHandler;

            SelectedTables = new HashSet<string>();
        }

        internal IDataQuery Querier { get; }

        private IDataComparer Comparator { get; }

        public DataCacheModel DataCache { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
