using ComparisonLib;
using EasyDatabaseCompare.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyDatabaseCompare.ViewModel
{
    public partial class WindowViewModel
    {
        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SourceData":
                    CanQueryTarget = DataCache.SourceData != null;
                    break;
                case "TargetData":
                    CanStartComparer = DataCache.TargetData != null;
                    break;
                case "SelecctedDbType":
                    SelectedDbTypeChanged();
                    break;
                case "ConnectionChecked":
                    CanSelectTable = ConnectionChecked;
                    CanQuerySource = ConnectionChecked;
                    break;
                case "CustomConnectionStringMode":
                    if (CustomConnectionStringMode)
                        Fields = new List<ConnectionFieldInfo>
                        {
                            new ConnectionFieldInfo("ConnectionString", string.Empty)
                        };
                    else
                        Fields = DbConnectionFields
                            .Select(f => new ConnectionFieldInfo(f, string.Empty)).ToList();
                    break;
                case "TableNames":
                    if (TableNames != null && TableNames.Length > 0)
                        ConnectionChecked = true;
                    break;
                case "HideEmptyTables":
                case "HideUnchangedTables":
                    if (DataCache.DataCompareResult == null || DataCache.DataCompareResult.Count == 0) return;
                    FilteredComparerResultOverview = CreateFilteredComparerResultOverview(DataCache.DataCompareResult);
                    break;
                case "OverviewSelectedCellInfo":
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
                case "BlackListMode":
                case "ShowSameColumn":
                case "ShowInsertColumn":
                case "ShowDeleteColumn":
                case "ShowChangedColumn":
                case "FilteredComparerResultOverview":
                case "ComparerResultOverview":
                case "SelectedTables":
                case "DbConnectionFields":
                case "DataCompareResult":
                case "CanQuerySource":
                case "CanQueryTarget":
                case "Querier":
                case "CanStartComparer":
                case "CanSelectTable":
                case "Fields":
                    break;
            }
        }

        private void SelectedDbTypeChanged()
        {
            DbConnectionFields = DataAccessorInfo.SupportedDbConnectionStringFields[SelecctedDbType];
            DbConnectionFieldsDefaultValue = DataAccessorInfo.SupportedDbConnectionStringFieldsDefaultValue[SelecctedDbType] ?? new string[DbConnectionFields.Length];

            if (!CustomConnectionStringMode)
                Fields = DbConnectionFields.Zip(DbConnectionFieldsDefaultValue, (fieldName, fieldValue) => (fieldName, fieldValue))
                    .Select(f => new ConnectionFieldInfo(f.fieldName, f.fieldValue)).ToList();
        }
    }
}
