//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Media;

//namespace EasyDatabaseCompare.Converter
//{
//    public class TextBlockHighlightSpanConverter : IMultiValueConverter
//    {
//        private static SolidColorBrush HighlightColor =
//            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5533AAFF"));

//        private static Style NullStyle = new Style(typeof(Run))
//        {
//            Setters =
//            {
//                new Setter(Run.ForegroundProperty,
//                    new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FF555555"))),
//                new Setter(Run.FontStyleProperty, FontStyles.Italic),
//            }
//        };

//        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (values[0] == DependencyProperty.UnsetValue)
//                return new[] { new Run("NULL") { Style = NullStyle } };
//            var val = values[0]?.ToString();
//            if (val == "System.Byte[]") return new Inline[0];
//            var hl = values[1]?.ToString();
//            if (string.IsNullOrEmpty(hl)) return new[] { new Run(val) };
//            var splitAll = SplitAll(val, hl, true);
//            var re = splitAll.Select(m => m.isMatch ? new Run(m.subString) { Background = HighlightColor } : new Run(m.subString)).ToArray();


//            //var normalTexts = val.Split(new[] { hl }, StringSplitOptions.None);
//            //var re = Intersperse(normalTexts.Select(t => new Run(t)), new Run(hl) { Background = HighlightColor })
//            //    .Where(r => !string.IsNullOrEmpty(r.Text)).ToArray();
//            return re;
//        }

//        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
        
//        private IEnumerable<(string subString, bool isMatch)> SplitAll(string orgString, string matchString,
//            bool ignoreCase = false)
//        {
//            if (string.IsNullOrEmpty(matchString)) yield break;
//            var tempStr = orgString;
//            int lastIndex;
//            while ((lastIndex = tempStr.IndexOf(matchString,
//                       ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) >= 0)
//            {
//                var subStr = tempStr.Substring(0, lastIndex);
//                if (!string.IsNullOrEmpty(subStr))
//                    yield return (subStr, false);
//                yield return (tempStr.Substring(lastIndex, matchString.Length), true);
//                tempStr = tempStr.Substring(lastIndex + matchString.Length);
//            }
//            if (!string.IsNullOrEmpty(tempStr))
//                yield return (tempStr, false);
//        }

//    }
//}
