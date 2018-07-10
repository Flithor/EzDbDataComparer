using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace EasyDatabaseCompare.UserControls
{
    class ListBoxExtensions
    {
        public static IList GetBindableSelectedItems(DependencyObject obj)
        {
            return (IList)obj.GetValue(BindableSelectedItemsProperty);
        }

        public static void SetBindableSelectedItems(DependencyObject obj, IList value)
        {
            obj.SetValue(BindableSelectedItemsProperty, value);
        }

        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.RegisterAttached("BindableSelectedItems", typeof(IList), typeof(ListBoxExtensions), new PropertyMetadata(null, OnBindableSelectedItemsChanged));

        private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox Target)
            {
                if (e.NewValue is IList nv)
                {
                    Target.SelectedItems.Clear(); ;
                    foreach (var v in nv)
                        Target.SelectedItems.Add(v);
                }
            }
        }
    }
}