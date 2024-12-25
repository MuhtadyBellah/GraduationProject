using ECommerce.Core.Models;
using ECommerce.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repo
{
    public static class SpecificEvalutor<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> input, ISpecific<T> spec)
        {
            var query = input;
            if(spec.Creiteria is not null) query = query.Where(spec.Creiteria);

            if(spec.Order is not null) query = query.OrderBy(spec.Order);
            if(spec.OrderDesc is not null) query = query.OrderByDescending(spec.OrderDesc);
            if (spec.IsPagination) query = query.Skip(spec.Skip).Take(spec.Take);

            query = spec.Includes.Aggregate(query, (curr, inc) => curr.Include(inc));
            
            return query;
        }
    }
}
