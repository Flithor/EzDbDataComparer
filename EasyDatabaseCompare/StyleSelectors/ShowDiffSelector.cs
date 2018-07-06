using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EasyDatabaseCompare.StyleSelectors
{
    class ShowDiffSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            return base.SelectStyle(item, container);
        }
    }
    class ShowDiffTemplateSelector : DataTemplateSelector
    {
        public IEnumerable<ControlTemplate> Templates;
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return base.SelectTemplate(item, container);
        }
    }
}
