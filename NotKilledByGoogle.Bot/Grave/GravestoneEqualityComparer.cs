using System.Collections.Generic;

namespace NotKilledByGoogle.Bot.Grave
{
    public class GravestoneEqualityComparer : IEqualityComparer<Gravestone>
    {
        public bool Equals(Gravestone? x, Gravestone? y)
        {
            return x?.Name == y?.Name;
        }

        public int GetHashCode(Gravestone obj)
        {
            var result = 0;
            foreach (var ch in obj.Name.ToCharArray())
            {
                result += ch;
            }
            return result;
        }
    }
}