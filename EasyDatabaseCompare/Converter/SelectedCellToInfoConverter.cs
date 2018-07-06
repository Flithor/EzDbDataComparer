using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasyDatabaseCompare.Converter
{
    public class SelectedCellToInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cell = (DataGridCellInfo)value;
            var col = cell.Column;
            var cName = col?.Header.ToString();
            if (col == null || cName == "Table Name") return string.Empty;
            var row = cell.Item as System.Data.DataRowView;
            var tName = row.Row[0].ToString();
            //(cell.Column.GetCellContent(cell.Item) as TextBlock).Text;
            return $"{tName},{cName}";
        }
    }
}
