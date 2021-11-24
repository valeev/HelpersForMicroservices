using System.Linq.Expressions;

namespace Infrastructure.Helpers
{
    /// <summary>
    /// Sorting helper
    /// </summary>
    public static class SortHelper
    {
        /// <summary>
        /// Sort records
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="records"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static IQueryable<T> Sort<T>(IQueryable<T> records, string sortBy)
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return records;
            }
            var sortingArray = sortBy.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            if (sortingArray.Length < 1 || sortingArray.Length > 2)
            {
                return records;
            }
            if (sortingArray.Length == 1 || (sortingArray.Length == 2 && sortingArray[1].ToLower() == "asc"))
            {
                records = records.OrderBy(ToLambda<T>(sortingArray[0].ToLower()));
            }
            else if (sortingArray.Length == 2 && sortingArray[1].ToLower() == "desc")
            {
                records = records.OrderByDescending(ToLambda<T>(sortingArray[0].ToLower()));
            }
            return records;
        }

        #region Helpers

        /// <summary>
        /// Lambda sorting
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }

        #endregion
    }
}
