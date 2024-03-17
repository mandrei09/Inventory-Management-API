using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Optima.Fais.EfRepository
{
    public static class ExpressionHelper
    {
        public static Expression<Func<TSource, object>> GenericEvaluateOrderBy<TSource>(string propertyName)
        {
            var type = typeof(TSource);
            //var parameter = Expression.Parameter(type, "p");
            //var propertyReference = Expression.Convert(Expression.PropertyOrField(parameter, propertyName), typeof(object));
            //return Expression.Lambda<Func<TSource, object>>(propertyReference, new[] { parameter });

            var parameter = Expression.Parameter(type, "p");
            Expression expression = parameter;
            foreach (var property in propertyName.Split('.'))
            {
                expression = Expression.PropertyOrField(expression, property);
            }

            var propertyReference = Expression.Convert(expression, typeof(object));
            return Expression.Lambda<Func<TSource, object>>(propertyReference, new[] { parameter });
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                              Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> GetAllPartsFilter<T>(string filter, Func<string, Expression<Func<T, bool>>> filterPredicate)
        {
            Expression<Func<T, bool>> predicate = null;
            string[] filterParts = filter.Split(null);

            if (filterParts.Length > 0) predicate = filterPredicate(filterParts[0]);

            for (int i = 1; i < filterParts.Length; i++)
            {
                predicate = ExpressionHelper.And<T>(predicate, filterPredicate(filterParts[i]));
            }

            return predicate;
        }

        //string filter, int? wordCount, int? letterCount, string searchType, string conditionType
        public static Expression<Func<T, bool>> GetAllPartsFilterExtended<T>(Func<string, Expression<Func<T, bool>>> filterPredicate, string filter,
            int wordCount, int letterCount, string conditionType = "AND")
        {
            Expression<Func<T, bool>> predicate = null;
            int lastIndex = 0;
            string subFilter = string.Empty;

            if (filter == null) return predicate;

            while (filter.Contains("  ")) filter = filter.Replace("  ", " ");

            string[] filterParts = filter.Split(null);
            lastIndex = ((wordCount == 0) || (wordCount >= filterParts.Length)) ? filterParts.Length : wordCount;

            for (int i = 0; i < lastIndex; i++)
            {
                subFilter = ((letterCount == 0) || (letterCount >= filterParts[i].Length)) ? filterParts[i] : filterParts[i].Substring(0, letterCount);

                if (predicate == null)
                    predicate = filterPredicate(subFilter);
                else
                    predicate = conditionType.ToUpper().CompareTo("OR") == 0
                        ? ExpressionHelper.Or<T>(predicate, filterPredicate(subFilter))
                        : ExpressionHelper.And<T>(predicate, filterPredicate(subFilter));
            }

            return predicate;
        }

        public static Expression<Func<T, bool>> GetInListPredicate<T, V>(Func<V, Expression<Func<T, bool>>> filterPredicate, List<V> list)
        {
            Expression<Func<T, bool>> predicate = null;

            if ((list != null) && (list.Count > 0))
            {
                predicate = filterPredicate(list[0]);

                for (int index = 1; index < list.Count; index++)
                {
                    V id = list[index];
                    predicate = ExpressionHelper.Or<T>(predicate, filterPredicate(id));
                }

            }

            return predicate;
        }

        //public static Expression<Func<T, bool>> GetAllPartsFilter<T>(string filter, Func<string, Expression<Func<T, bool>>> filterPredicate)
        //{
        //    Expression<Func<T, bool>> predicate = null;
        //    string[] filterParts = filter.Split(null);

        //    if (filterParts.Length > 0) predicate = filterPredicate(filterParts[0]);

        //    for (int i = 1; i < filterParts.Length; i++)
        //    {
        //        predicate = ExpressionHelper.And<T>(predicate, filterPredicate(filterParts[i]));
        //    }

        //    return predicate;
        //}
    }
}
