using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Tactuum.Core.Attributes;

namespace Tactuum.Core.Extensions
{
    public static class EnumExtensions
    {
        public static T Add<T>(this Enum enumType, T flag)
        {
            try
            {
                return (T)(object)(((int)(object)enumType | (int)(object)flag));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not add value from enumerated Enum '{0}'.",
                        typeof(T).Name), ex);
            }
        }

        public static T Remove<T>(this Enum enumType, T flag)
        {
            try
            {
                return (T)(object)(((int)(object)enumType & ~(int)(object)flag));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated Enum '{0}'.",
                        typeof(T).Name), ex);
            }
        }

        /// <summary>
        /// Get enum display name from the DisplayAttribute for the supplied enum value.
        /// Note: does not work for multiple flag values.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum enumType)
        {
            Type type = enumType.GetType();
            FieldInfo fieldInfo = type.GetField(enumType.ToString());
            if (!fieldInfo.IsDefined(typeof(DisplayAttribute), false))
            {
                return enumType.ToString();
            }
            object[] displayAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
            return ((DisplayAttribute)displayAttributes[0]).Name;
        }

        public static string GetDisplayName<T>(this T enumType)
        {
            Type type = enumType.GetType();
            FieldInfo fieldInfo = type.GetField(enumType.ToString());

            if (!fieldInfo.IsDefined(typeof(DisplayAttribute), false))
            {
                return enumType.ToString();
            }
            object[] displayAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
            return ((DisplayAttribute)displayAttributes[0]).Name;
        }

        public static int GetOrderId<T>(this T enumType)
        {
            Type type = enumType.GetType();
            FieldInfo fieldInfo = type.GetField(enumType.ToString());

            if (!fieldInfo.IsDefined(typeof(OrderByAttribute), false))
            {
                return -1;
            }
            object[] displayAttributes = fieldInfo.GetCustomAttributes(typeof(OrderByAttribute), false);
            return ((OrderByAttribute)displayAttributes[0]).OrderId;
        }

        /// <summary>
        /// Get enum display name from the DisplayAttribute for the supplied enum value.
        /// Note: does not work for multiple flag values.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static string GetDisplayDescription(this Enum enumType)
        {
            Type type = enumType.GetType();
            FieldInfo fieldInfo = type.GetField(enumType.ToString());
            if (!fieldInfo.IsDefined(typeof(DisplayAttribute), false))
            {
                return enumType.ToString();
            }
            object[] displayAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
            return ((DisplayAttribute)displayAttributes[0]).Description;
        }

        public static IList<string> GetDisplayNames<T>()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException();

            return
                Enum.GetNames(typeof(T))
                    .Select(value => Enum.Parse(typeof(T), value))
                    .Select(enumValue => enumValue.GetDisplayName())
                    .ToList();
        }



        public static IList<Tuple<int, string>> GetValuesAndDisplayNames<T>()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException();

            var values =
                Enum.GetNames(typeof(T))
                    .Select(value => Enum.Parse(typeof(T), value))
                    .ToList();

            IList<Tuple<int, string, int>> valuesAndNames = new List<Tuple<int, string, int>>();
            foreach (var value in values)
            {
                valuesAndNames.Add(new Tuple<int, string, int>((int)value, value.GetDisplayName(), value.GetOrderId()));
            }
            return valuesAndNames.OrderBy(i => i.Item3).ThenBy(i => i.Item1).Select(i => new Tuple<int, string>(i.Item1, i.Item2)).ToList();
        }



        public static T GetEnumValueFromDisplayName<T>(string displayName)
        {
            var type = typeof(T);

            try
            {
                return (T)(Enum.GetNames(type)
                                .Select(name => new { EnumType = Enum.Parse(type, name), DisplayName = Enum.Parse(type, name).GetDisplayName() })
                                .Where(types => types.DisplayName.ToLowerInvariant().Equals(displayName.ToLowerInvariant()))
                                .FirstOrDefault()
                                .EnumType);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not find Enum value equals to this display name '{0}'.",
                        displayName), ex);
            }
        }
    }
}
