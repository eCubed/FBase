using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FBase.Foundations
{
    public static class ResultSetHelper
    {
        public static ResultSet<T> GetResults<T, TKey>(IQueryable<T> filteredResults,
                                          int page = 1, int pageSize = 10) where T : class
        {
            ResultSet<T> RS = new ResultSet<T>();
            RS.TotalRecordsCount = filteredResults.Count();

            RS.CurrentPage = (page <= 0) ? 1 : page;
            RS.PageSize = (pageSize <= 0) ? 10 : pageSize;
            RS.NumPages = (RS.PageSize > 0) ? ((RS.TotalRecordsCount - 1) / RS.PageSize) + 1 : 1;

            RS.HasNextResults = RS.CurrentPage < RS.NumPages;
            RS.HasPreviousResults = RS.CurrentPage > 1;

            RS.Records = filteredResults.Skip((RS.CurrentPage - 1) * RS.PageSize).Take(RS.PageSize).ToList();

            return RS;
        }

        public static ResultSet<U> Convert<T, U>(ResultSet<T> resultSet, Func<T, U> projector)
            where T : class
            where U : class
        {
            ResultSet<U> projectedRS = new ResultSet<U>();
            projectedRS.CurrentPage = resultSet.CurrentPage;
            projectedRS.HasNextResults = resultSet.HasNextResults;
            projectedRS.HasPreviousResults = resultSet.HasPreviousResults;
            projectedRS.NumPages = resultSet.NumPages;
            projectedRS.PageSize = resultSet.PageSize;
            projectedRS.TotalRecordsCount = resultSet.TotalRecordsCount;

            List<U> newList = new List<U>();

            foreach(var item in resultSet.Records)
            {
                newList.Add(projector.Invoke(item));
            }

            projectedRS.Records = newList;

            return projectedRS;
        }
    }
}
