using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tactuum.Core.Util
{
    public static class TypeComparer
    {
        public static IList<TypeComparisonResult> Compare(object oldValue, object newValue)
        {
            var properties = oldValue.GetType().GetProperties();

            List<TypeComparisonResult> results = new List<TypeComparisonResult>();

            foreach (var p in properties)
            {
                try
                {
                    results.Add(GetTypeComparisonResultFromProperty(oldValue, newValue, p));
                }
                catch (NullReferenceException) { }
            }

            return results;
        }

        private static TypeComparisonResult GetTypeComparisonResultFromProperty(object oldValue, object newValue, PropertyInfo p)
        {
            //For the purposes of string value comparison - pretend null (no value) and empty string are the same

            string oldVal;
            string newVal;
            try
            {
                oldVal = p.GetValue(oldValue).ToString();
            }
            catch
            {
                oldVal = string.Empty;
            }
            try
            {
                newVal = p.GetValue(newValue).ToString();
            }
            catch
            {
                newVal = string.Empty;
            }

            TypeComparisonResult result = new TypeComparisonResult()
            {
                Name = p.Name,
                NewValueText = newVal,
                OldValueText = oldVal
            };
            return result;
        }
    }
}
