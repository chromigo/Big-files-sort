using System;

namespace Sorter.Core
{
    public struct ColumnItem : IEquatable<ColumnItem>
    {
        public static readonly ColumnItem Sentinel = new() {NumberPart = int.MaxValue, StringPart = new('z', 1024)}; 
        public string StringPart;
        public int NumberPart;
        
        public bool IsSentinel()
        {
            return Equals(Sentinel);
        }

        public bool Equals(ColumnItem other)
        {
            return ColumnItemComparer.Instance.Compare(this, other) == 0;;
        }

        public override bool Equals(object obj)
        {
            return obj is ColumnItem other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StringPart, NumberPart);
        }
    }
}