using ComparisonLib;
using EasyDatabaseCompare.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime;

namespace EasyDatabaseCompare.ViewModel
{
    public partial class WindowViewModel
    {
        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SourceData):
                    CanQueryTarget = DataCache.SourceData != null;
                    break;
                case nameof(TargetData):
                    CanStartComparer = DataCache.TargetData != null;
                    break;
                case nameof(SelectedDbType):
                    SelectedDbTypeChanged();
                    break;
                case nameof(ConnectionChecked):
                    CanSelectTable = ConnectionChecked;
                    CanQuerySource = ConnectionChecked;
                    break;
                case nameof(CustomConnectionStringMode):
                    if (CustomConnectionStringMode)
                        Fields = new List<ConnectionFieldInfo>
                        {
                            new ConnectionFieldInfo("ConnectionString", string.Empty)
                        };
                    else
                        SelectedDbTypeChanged();
                    break;
                case nameof(TableNames):
                    if (TableNames != null && TableNames.Length > 0)
                        ConnectionChecked = true;
                    break;
                case nameof(HideEmptyTables):
                case nameof(HideUnchangedTables):
                    if (DataCache.DataCompareResult == null || DataCache.DataCompareResult.Count == 0) return;
                    FilteredComparerResultOverview = CreateFilteredComparerResultOverview(DataCache.DataCompareResult);
                    break;
                case nameof(OverviewSelectedCellInfo):
                    if (string.IsNullOrEmpty(OverviewSelectedCellInfo)) break;
                    var split = OverviewSelectedCellInfo.Split(',');
                    var tn = split[0];
                    var ct = split[1];
                    var selectDiff = DataCache.DataCompareResult.Where(diff => diff.SourceTable.TableName == tn);
                    switch (ct)
                    {
                        case "Same":
                            SelectedDetail = selectDiff.First().DisplayTables.SameData;
                            break;
                        case "Changed":
                            SelectedDetail = selectDiff.First().DisplayTables.ChangedData;
                            DiffFields = selectDiff.First().DisplayTables.DiffFieldsOfRow;
                            break;
                        case "Inserted":
                            SelectedDetail = selectDiff.First().DisplayTables.InsertedData;
                            break;
                        case "Deleted":
                            SelectedDetail = selectDiff.First().DisplayTables.DeletedData;
                            break;
                        case "Data In Source":
                            SelectedDetail = selectDiff.First().SourceTable;
                            break;
                        case "Data In Target":
                            SelectedDetail = selectDiff.First().TargetTable;
                            break;
                    }
                    //GC.Collect();
                    break;
                case nameof(BlackListMode):
                case nameof(ShowSameColumn):
                case nameof(ShowInsertColumn):
                case nameof(ShowDeleteColumn):
                case nameof(ShowChangedColumn):
                case nameof(FilteredComparerResultOverview):
                case nameof(ComparerResultOverview):
                case nameof(SelectedTables):
                case nameof(DbConnectionFields):
                case nameof(DataCompareResult):
                case nameof(CanQuerySource):
                case nameof(CanQueryTarget):
                case nameof(Querier):
                case nameof(CanStartComparer):
                case nameof(CanSelectTable):
                case nameof(Fields):
                    break;
            }
        }

        private void SelectedDbTypeChanged()
        {
            DbConnectionFields = DataAccessorInfo.SupportedDbConnectionStringFields[SelectedDbType];
            DbConnectionFieldsDefaultValue = DataAccessorInfo.SupportedDbConnectionStringFieldsDefaultValue[SelectedDbType] ?? new string[DbConnectionFields.Length];

            if (!CustomConnectionStringMode)
                Fields = DbConnectionFields.Zip(DbConnectionFieldsDefaultValue, Tuple.Create)
                    .Select(f => new ConnectionFieldInfo(f.Item1, f.Item2)).ToList();
        }
    }
}
