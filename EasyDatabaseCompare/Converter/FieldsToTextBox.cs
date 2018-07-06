using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace EasyDatabaseCompare.Converter
{
    class FieldsToTextBox : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fields = value as string[];
            var textBoxs = new TextBox[fields.Length];
            //var uniformGrid = new System.Windows.Controls.Primitives.UniformGrid { Columns = fields.Length };
            for (int i = 0; i < fields.Length; i++)
            {
                var binding = new Binding($"Fields[{i}]");
                var t = new TextBox { Tag = fields[i] };
                BindingOperations.SetBinding(t, TextBox.TextProperty, binding);
                textBoxs[i] = t;
                //uniformGrid.Children.Add(t);
            }
            return textBoxs;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
