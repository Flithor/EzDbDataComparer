using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EasyDatabaseCompare.Model;

namespace EasyDatabaseCompare.StyleSelectors
{
    public class FieldsTextBlockTemplateSelector : DataTemplateSelector
    {
        internal ResourceDictionary Resources = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/EasyDatabaseCompare;Component/ControlResources.xaml",
                UriKind.Absolute)
        };
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var connFieldInfo = (ConnectionFieldInfo)item;
            var markCount = connFieldInfo.FieldName.Count(c => c == '\u200B');
            switch (markCount)
            {
                case 1:
                    return (DataTemplate)Resources["CheckBoxField"];
                case 2:
                    return (DataTemplate)Resources["ComboBoxField"];
                case 3:
                    return (DataTemplate)Resources["PasswordField"];
                default:
                    return (DataTemplate)Resources["TextBoxField"];
            }
        }
    }
}
