using System.Collections.Generic;
using System.Linq;

namespace NotKilledByGoogle.Bot.Statistics
{
    public abstract class StatsBase
    {
        /// <summary>
        /// Registered statistics data for easy enumeration.
        /// </summary>
        public List<SafeIntTag> RegisteredSafeInts { get; } = new();
        
#pragma warning disable CS8618
        protected StatsBase()
#pragma warning restore CS8618
        {
            GetType()
                .GetProperties()
                .Where(x => x.PropertyType == typeof(SafeInt))
                .ForEach(x =>
                {
                    var id = x.Name;
                    var displayName = id;
                    var newSafeInt = new SafeInt();
                    x.SetValue(this, newSafeInt);

                    var foundAttr =
                        x.CustomAttributes.FirstOrDefault(attrib => attrib.AttributeType == typeof(StatNameAttribute));
                    if (foundAttr is not null)
                        displayName = foundAttr.ConstructorArguments[0].Value?.ToString() ?? displayName;
                    RegisteredSafeInts.Add(new SafeIntTag(id, displayName, newSafeInt));
                });
        }

        public void Clear()
            => RegisteredSafeInts.ForEach(x => x.SafeInt.Zero());

        public SafeIntTag this[string id]
            => RegisteredSafeInts.First(x => x.Id == id);
    }
}