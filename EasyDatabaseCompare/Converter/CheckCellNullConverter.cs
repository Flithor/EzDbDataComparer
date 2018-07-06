using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasyDatabaseCompare.Converter
{
    public class CheckCellNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //return null;

            var cell = value as DataGridCell;
            if (!(cell.DataContext is DataRowView row)) return null;
            var data = row.Row;
            var col = cell.Column.Header.ToString();

            var cellValue = row[col];
            var result = cellValue == DBNull.Value || cellValue == null;

            //if (cell.Content is TextBlock tb && result)
            //{
            //    //var binding = BindingOperations.GetBinding(tb, TextBox.TextProperty);
            //    BindingOperations.ClearBinding(tb, TextBlock.TextProperty);
            //}

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
