using System;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasyDatabaseCompare.Converter
{
    class CompareNearbyCellConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cell = value as DataGridCell;
            var row = cell.DataContext as DataRowView;

            var dataRow = row.Row;
            
            var col = cell.Column.Header.ToString();
            try
            {
                switch (parameter)
                {
                    case "Next":
                        var next = dataRow.Table.Rows[dataRow.Table.Rows.IndexOf(dataRow) + 1];
                        return !Equals(dataRow[col], next[col]);
                    case "Previous":
                        var previous = dataRow.Table.Rows[dataRow.Table.Rows.IndexOf(dataRow) - 1];
                        return !Equals(previous[col], dataRow[col]);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
