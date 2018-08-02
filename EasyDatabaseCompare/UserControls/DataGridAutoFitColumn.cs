using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EasyDatabaseCompare.Converter;

namespace EasyDatabaseCompare.UserControls
{
    public class DataGridAutoFitColumn : DataGridTextColumn
    {
        public DataGrid ParentDataGrid { get; set; }
        public string BindingField => (Binding as Binding).Path.Path;


        private static SolidColorBrush HighlightColor =
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5533AAFF"));

        private static Style NullStyle = new Style(typeof(Run))
        {
            Setters =
            {
                new Setter(Run.ForegroundProperty,
                    new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF555555"))),
                new Setter(Run.FontStyleProperty, FontStyles.Italic),
            }
        };

        //private TextBlockHighlightSpanConverter _highlightSpanConverter = new TextBlockHighlightSpanConverter();

        public string HighlightString
        {
            get { return (string)GetValue(HighlightStringProperty); }
            set { SetValue(HighlightStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightStringProperty =
            DependencyProperty.Register("HighlightString", typeof(string), typeof(DataGridAutoFitColumn), new PropertyMetadata(string.Empty));

        public Type ColumnContextType
        {
            get { return (Type)GetValue(ColumnContextTypeProperty); }
            set { SetValue(ColumnContextTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnContextType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnContextTypeProperty =
            DependencyProperty.Register("ColumnContextType", typeof(Type), typeof(DataGridAutoFitColumn),
                new PropertyMetadata(null));

        private BindingBase _binding;


        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var vm = ParentDataGrid.DataContext as ViewModel.IWindowViewModel;
            if (vm?.SelectedDetail.TableName == "Changed")
            {
                var rowView = dataItem as DataRowView;
                if (vm.DiffFields[rowView.Row].Contains(BindingField))
                {
                    //var org = rowView.Row[BindingField, DataRowVersion.Original].ToString();
                    //var @new = rowView.Row[BindingField, DataRowVersion.Current].ToString();
                    var grid = new Grid
                    {
                        RowDefinitions =
                        {
                            new RowDefinition {Height = GridLength.Auto},
                            new RowDefinition {Height = GridLength.Auto}
                        }
                    };
                    var orgEle = AutoFitGenerateElement(cell, dataItem, true);
                    var orgBorder = new Border { Background = OriginalValueBorderBackgroundBrush };
                    //if (orgEle is TextBlock orgTb)
                    //{
                    //    //BindingOperations.ClearBinding(orgTb, TextBlock.TextProperty);
                    //    //orgTb.Text = org;
                    //}
                    orgBorder.Child = orgEle;

                    var newEle = AutoFitGenerateElement(cell, dataItem);
                    var newBorder = new Border { Background = NewValueBorderBackgroundBrush };
                    //if (newEle is TextBlock newTb)
                    //{
                    //    //BindingOperations.ClearBinding(newTb, TextBlock.TextProperty);
                    //    //newTb.Text = @new;
                    //}
                    newBorder.Child = newEle;

                    Grid.SetRow(orgBorder, 0);
                    Grid.SetRow(newBorder, 1);
                    grid.Children.Add(orgBorder);
                    grid.Children.Add(newBorder);
                    return grid;
                }
            }

            //default
            return AutoFitGenerateElement(cell, dataItem);

            //var result = new AutoFreshElement(cell, RecreateElement);
            //result.GenerateCheckBox += GenerateCheckBox;
            //result.GenerateTextBlock += GenerateTextBlock;

            //cell.DataContextChanged += Cell_DataContextChanged;
            //return RecreateElement(cell, dataItem);
        }

        //private void Cell_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var cell = sender as DataGridCell;
        //    cell.Content = RecreateElement(cell, cell.DataContext);

        //}

        private static readonly SolidColorBrush OriginalValueBorderBackgroundBrush =
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5500FF00"));

        private static readonly SolidColorBrush NewValueBorderBackgroundBrush =
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#55FF0000"));
        //private FrameworkElement RecreateElement(DataGridCell cell, object dataItem)
        //{
        //    var vm = ParentDataGrid.DataContext as ViewModel.IWindowViewModel;
        //    if (vm?.SelectedDetail.TableName == "Changed")
        //    {
        //        var rowView = dataItem as DataRowView;
        //        if (vm.DiffFields[rowView.Row].Contains(BindingField))
        //        {
        //            //var org = rowView.Row[BindingField, DataRowVersion.Original].ToString();
        //            //var @new = rowView.Row[BindingField, DataRowVersion.Current].ToString();
        //            var grid = new Grid
        //            {
        //                RowDefinitions =
        //                {
        //                    new RowDefinition {Height = GridLength.Auto},
        //                    new RowDefinition {Height = GridLength.Auto}
        //                }
        //            };
        //            var orgEle = AutoFitGenerateElement(cell, dataItem, true);
        //            var orgBorder = new Border { Background = OriginalValueBorderBackgroundBrush };
        //            orgBorder.Child = orgEle;

        //            var newEle = AutoFitGenerateElement(cell, dataItem);
        //            var newBorder = new Border { Background = NewValueBorderBackgroundBrush };
        //            newBorder.Child = newEle;

        //            Grid.SetRow(orgBorder, 0);
        //            Grid.SetRow(newBorder, 1);
        //            grid.Children.Add(orgBorder);
        //            grid.Children.Add(newBorder);
        //            return grid;
        //        }
        //    }

        //    //default
        //    return AutoFitGenerateElement(cell, dataItem);
        //}

        private TextBlock GenerateTextBlock(DataGridCell cell, object dataItem, bool isGetOrg = false)
        {
            var re = base.GenerateElement(cell, dataItem) as TextBlock;
            re.TextWrapping = TextWrapping.NoWrap;
            re.TextTrimming = TextTrimming.CharacterEllipsis;

            //var oldBinding = BindingOperations.GetBinding(re, TextBlock.TextProperty);
            //var inlineBinding = new MultiBinding
            //{
            //    Bindings =
            //    {
            //        oldBinding,
            //        new Binding(nameof(HighlightString)) {Source = this}
            //    },
            //    Converter = _highlightSpanConverter,
            //};

            //BindingOperations.SetBinding(re, TextBlockExtensions.BindableInlinesProperty, inlineBinding);

            if (isGetOrg)
            {
                //BindingOperations.ClearBinding(re, TextBlock.TextProperty);
                BindingOperations.SetBinding(re, TextBlock.TextProperty,
                    new Binding($"Row[{BindingField},256]") { Mode = BindingMode.OneWay });
            }
            var row = (dataItem as DataRowView).Row;
            var val = row[BindingField, isGetOrg ? DataRowVersion.Original : DataRowVersion.Current];
            if (val == DBNull.Value)
            {
                BindingOperations.ClearBinding(re, TextBlock.TextProperty);
                re.Inlines.Add(new Run("NULL") { Style = NullStyle });
                re.ToolTip = "This cell is null";
                if (HighlightString.ToLower() == "`null") re.Background = HighlightColor;
            }
            else if (string.IsNullOrEmpty(val.ToString()))
            {
                BindingOperations.ClearBinding(re, TextBlock.TextProperty);
                re.Inlines.Add(new Run("Empty String") { Style = NullStyle });
                re.ToolTip = "This cell is an empty string";
                if (HighlightString == "` ") re.Background = HighlightColor;
            }
            else if (val.GetType().IsArray && val is byte[] dataBytes)
            {
                var tryImg = TryLoadImage(dataBytes);
                if (tryImg == null)
                {
                    re.ToolTip = new TextBlock
                    {
                        Text = $"byte[] {{{string.Join(", ", dataBytes)}}}",
                        TextTrimming = TextTrimming.CharacterEllipsis,
                        TextWrapping = TextWrapping.Wrap,
                        MaxWidth = 500,
                        MaxHeight = 500
                    };
                    return re;
                }
                //BindingOperations.ClearBinding(re, TextBlock.TextProperty);
                re.Text = string.Empty;
                re.Background = new ImageBrush { ImageSource = tryImg, Stretch = Stretch.UniformToFill };
                re.ToolTip = new Image { Source = tryImg };
                return re;
            }
            if (string.IsNullOrEmpty(HighlightString))
            {
                if (re.ToolTip is null)
                    BindingOperations.SetBinding(re, FrameworkElement.ToolTipProperty,
                        new Binding("Text") { Source = re });
                return re;
            }
            BindingOperations.ClearBinding(re, TextBlock.TextProperty);
            var splitAll = SplitAll(val.ToString(), HighlightString, true);
            re.Inlines.AddRange(splitAll.Select(m => m.Item2 ? new Run(m.Item1) { Background = HighlightColor } : new Run(m.Item1)));
            return re;
        }

        private FrameworkElement GenerateCheckBox(DataGridCell cell, object dataItem, bool isGetOrg = false)
        {
            var row = (dataItem as DataRowView).Row;
            var val = row[BindingField, isGetOrg ? DataRowVersion.Original : DataRowVersion.Current];
            if (val == DBNull.Value)
                return GenerateTextBlock(cell, dataItem, isGetOrg);

            CheckBox checkBox = new CheckBox { IsThreeState = false };
            ApplyStyle(false, true, checkBox);

            if (isGetOrg)
                BindingOperations.SetBinding(checkBox,
                    System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
                    new Binding($"Row[{BindingField},256]") { Mode = BindingMode.OneWay });
            else
                ApplyBinding(checkBox, System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty);

            if (val is bool bVal) checkBox.IsChecked = bVal;
            return checkBox;
        }

        private static readonly DataGridCheckBoxColumn DefaultCheckBoxColumn = new DataGridCheckBoxColumn();
        private static Style DefaultCheckBoxStyle => DefaultCheckBoxColumn.ElementStyle;
        internal void ApplyStyle(bool isEditing, bool defaultToElementStyle, FrameworkElement element)
        {
            Style style = PickStyle(isEditing, defaultToElementStyle);
            if (style == null)
                return;
            element.Style = DefaultCheckBoxStyle;
        }
        private Style PickStyle(bool isEditing, bool defaultToElementStyle)
        {
            Style style = isEditing ? EditingElementStyle : ElementStyle;
            if (isEditing & defaultToElementStyle && style == null)
                style = ElementStyle;
            return style;
        }
        internal void ApplyBinding(DependencyObject target, DependencyProperty property)
        {
            BindingBase binding = Binding;
            if (binding != null)
                BindingOperations.SetBinding(target, property, binding);
            else
                BindingOperations.ClearBinding(target, property);
        }

        private FrameworkElement AutoFitGenerateElement(DataGridCell cell, object dataItem, bool isGetOrg = false)
        {
            var rowView = dataItem as DataRowView;
            var dType = rowView.Row.Table.Columns[BindingField].DataType;
            switch (Type.GetTypeCode(dType))
            {
                case TypeCode.Boolean: //bool
                    return GenerateCheckBox(cell, dataItem, isGetOrg);
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal: //numric
                    return GenerateTextBlock(cell, dataItem, isGetOrg);
                case TypeCode.DateTime: //datetime
                    return GenerateTextBlock(cell, dataItem, isGetOrg);
                case TypeCode.Char:
                case TypeCode.String: //string
                    return GenerateTextBlock(cell, dataItem, isGetOrg);
                case TypeCode.DBNull:
                case TypeCode.Empty: //null value
                    return GenerateTextBlock(cell, dataItem, isGetOrg);
                case TypeCode.Object: //others
                    return GenerateTextBlock(cell, dataItem, isGetOrg);
                default:
                    break;
            }
            return GenerateTextBlock(cell, dataItem);
        }

        private static BitmapImage TryLoadImage(byte[] imageData)
        {
            try
            {
                if (imageData == null || imageData.Length == 0) return null;
                var image = new BitmapImage();
                using (var mem = new MemoryStream(imageData))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }
                image.Freeze();
                return image;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //Console.WriteLine("this not a image stream");
            }

            return null;
        }

        private IEnumerable<Tuple<string, bool>> SplitAll(string orgString, string matchString,
            bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(matchString)) yield break;
            var tempStr = orgString;
            int lastIndex;
            while ((lastIndex = tempStr.IndexOf(matchString,
                       ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) >= 0)
            {
                var subStr = tempStr.Substring(0, lastIndex);
                if (!string.IsNullOrEmpty(subStr))
                    yield return Tuple.Create(subStr, false);
                yield return Tuple.Create(tempStr.Substring(lastIndex, matchString.Length), true);
                tempStr = tempStr.Substring(lastIndex + matchString.Length);
            }
            if (!string.IsNullOrEmpty(tempStr))
                yield return Tuple.Create(tempStr, false);
        }
    }
}
