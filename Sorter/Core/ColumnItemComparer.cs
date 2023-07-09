using System;
using System.Collections.Generic;

namespace Sorter.Core
{
    public class ColumnItemComparer : IComparer<ColumnItem>
    {
        public static ColumnItemComparer Instance = new();
        public int Compare(ColumnItem x, ColumnItem y)
        {
            var stringPartComparison = string.Compare(x.StringPart, y.StringPart, StringComparison.Ordinal);
            if (stringPartComparison != 0) return stringPartComparison;
            return x.NumberPart.CompareTo(y.NumberPart);
        }
    }
}