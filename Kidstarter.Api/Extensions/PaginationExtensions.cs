using Kidstarter.Api.Models;
using Kidstarter.Core.Models;

namespace Kidstarter.Api.Extensions
{
    public static class PaginationExtensions
    {
        public static PagedRequest ToPagedRequest(this Pager pager)
        {
            return new((int)pager.PageSize, (int)pager.Page);
        }

        public static PagedRequest ToPagedRequest(this PagerConstrained pager)
        {
            return new(pager.PageSize, pager.Page);
        }
    }
}