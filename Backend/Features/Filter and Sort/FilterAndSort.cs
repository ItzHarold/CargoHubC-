using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Backend.Features.FilterAndSort
{
    public static class QueryHelper
    {
        // Generic filter and sort method
        public static IEnumerable<T> FilterAndSort<T>(
            this IEnumerable<T> source,
            Dictionary<string, string?>? filters = null,
            string? sortBy = null,
            bool sortDescending = false)
        {
            // Apply filters
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    if (!string.IsNullOrEmpty(filter.Value))
                    {
                        source = ApplyFilter(source, filter.Key, filter.Value);
                    }
                }
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                source = ApplySort(source, sortBy, sortDescending);
            }

            return source;
        }

        // Helper method to apply filters
        private static IEnumerable<T> ApplyFilter<T>(IEnumerable<T> source, string propertyName, string filterValue)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var value = Expression.Constant(filterValue, typeof(string));
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            
            // Check if the method exists
            if (containsMethod == null)
            {
                throw new ArgumentException($"Method 'Contains' is not available for the property '{propertyName}'");
            }
            
            var containsExpression = Expression.Call(property, containsMethod, value);
            var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);

            return source.AsQueryable().Where(lambda);
        }

        // Helper method to apply sorting
        private static IEnumerable<T> ApplySort<T>(IEnumerable<T> source, string sortBy, bool sortDescending)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            // Check if the property exists on the type
            var property = Expression.Property(parameter, sortBy);
            if (property == null)
            {
                throw new ArgumentException($"Property '{sortBy}' does not exist on type '{typeof(T)}'");
            }

            var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(property, typeof(object)), parameter);

            return sortDescending
                ? source.AsQueryable().OrderByDescending(lambda)
                : source.AsQueryable().OrderBy(lambda);
        }
    }
}
