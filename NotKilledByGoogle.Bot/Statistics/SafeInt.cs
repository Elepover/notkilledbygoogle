using System.Threading;

namespace NotKilledByGoogle.Bot.Statistics
{
    public class SafeInt
    {
        public SafeInt() {}
        public SafeInt(int init) => _i = init;
        
        public int I => _i;
        private int _i;

        public void Zero()
        {
            lock (this)
            {
                _i = 0;
            }
        }
        public void Inc() => Interlocked.Increment(ref _i);
        public void Inc(int val) => Interlocked.Add(ref _i, val);
        public void Dec() => Interlocked.Decrement(ref _i);
        public void Dec(int val) => Interlocked.Add(ref _i, -val);

        public static SafeInt operator -(SafeInt val) => new(-val._i);
        public static bool operator ==(SafeInt val1, SafeInt val2) => val1._i == val2._i;
        public static bool operator !=(SafeInt val1, SafeInt val2) => val1._i != val2._i;
        public static bool operator >(SafeInt val1, SafeInt val2) => val1._i > val2._i;
        public static bool operator <(SafeInt val1, SafeInt val2) => val1._i < val2._i;
        public static bool operator >=(SafeInt val1, SafeInt val2) => val1._i >= val2._i;
        public static bool operator <=(SafeInt val1, SafeInt val2) => val1._i <= val2._i;

        public override bool Equals(object? obj)
        {
            if (obj is not null and SafeInt safeInt)
            {
                return safeInt._i == _i;
            }

            return false;
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => _i;
        public override string ToString() => _i.ToString();
    }
}
