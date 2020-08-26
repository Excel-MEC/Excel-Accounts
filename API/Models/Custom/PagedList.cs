using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Custom
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        private PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            if(pageSize == 0)
            {
                PageSize = count;
                TotalPages = 1;
            }
            else
            {
                PageSize = pageSize;
                TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            }
            CurrentPage = pageNumber;
            AddRange(items);
        }

        public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = pageSize > 0 
                ? await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync() 
                : await source.ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}