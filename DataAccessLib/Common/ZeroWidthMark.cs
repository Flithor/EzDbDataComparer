using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.Common
{
    public static class ZeroWidthMark
    {
        public static string StringMark => string.Empty;
        public static string BooleanMark => "\u200B";
        public static string SelectMark => "\u200B\u200B";
    }
}
