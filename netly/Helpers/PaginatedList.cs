using System;
using System.Collections.Generic;
using System.Linq;

namespace netly.Helpers
{

    /// <summary>
    /// Helper class for displaying a page of data from an IEnumerable or IQueryable source
    /// </summary>
    /// <typeparam name="T">The class type to enumerate</typeparam>
    public class PaginatedList<T> : List<T>
    {

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        /// <summary>
        /// IEnumerable constructor
        /// </summary>
        /// <param name="source">The IEnumerable source to page through</param>
        /// <param name="pageIndex">The index of the page to display (i.e. page 2 of 10)</param>
        /// <param name="pageSize">The number of elements per page</param>
        public PaginatedList(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
        }

        /// <summary>
        /// IQueryable constructor
        /// </summary>
        /// <param name="source">The IQueryable source to page through</param>
        /// <param name="pageIndex">The index of the page to display (i.e. page 2 of 10)</param>
        /// <param name="pageSize">The number of elements per page</param>
        public PaginatedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            this.AddRange(source.Skip(PageIndex * PageSize).Take(PageSize));
        }

        /// <summary>
        /// Flag to indicate if there are previous pages
        /// </summary>
        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 0);
            }
        }

        /// <summary>
        /// Flag to indicate if there are subsequent pages
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return (PageIndex + 1 < TotalPages);
            }
        }
    }
}
