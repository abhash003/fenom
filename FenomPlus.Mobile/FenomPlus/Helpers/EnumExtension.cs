// Credits: https://gist.github.com/cocowalla
using System;
using System.ComponentModel;
using System.Reflection;
using ConcurrDict = System.Collections.Concurrent.ConcurrentDictionary<string, string>;
namespace FenomPlus.Helpers
{
    public static class EnumExtensions
    {
        // Note that we never need to expire these cache items, so we just use ConcurrentDictionary rather than MemoryCache
        private static readonly ConcurrDict DescriptionCache = new ConcurrDict();

        public static string Description(this Enum value)
        {
            var key = $"{value.GetType().FullName}.{value}";

            var description = DescriptionCache.GetOrAdd(key, x =>
            {
                var name = (DescriptionAttribute[])value
                    .GetType()
                    .GetTypeInfo()
                    .GetField(value.ToString())
                    .GetCustomAttributes(typeof(DescriptionAttribute), false);

                return name.Length > 0 ? name[0].Description : value.ToString();
            });
            return description;
        }
    }
}
