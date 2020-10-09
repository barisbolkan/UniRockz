using System;
using System.Buffers;
using System.Text.Json;

namespace UniRockz.Domain.Utils
{
    public static class JsonExtensions
    {
        public static T? GetProperty<T>(this JsonElement element, string propertyName) where T: struct
        {
            if (element.TryGetProperty(propertyName, out var elem))
            {
                return Convert.ChangeType(elem.ToString(), typeof(T)) as T?;
            }

            return null;
        }

        public static T GetValue<T>(this JsonProperty property) where T: struct
        {
            return (T)Convert.ChangeType(property.Value.GetString(), typeof(T));
        }
    }
}
