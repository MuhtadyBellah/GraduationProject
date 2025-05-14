using ECommerce.Core.Models;
using System.Linq.Expressions;

namespace ECommerce.Core.Specifications
{
    public interface ISpecific<T> where T : BaseEntity
    {
        //where(p=>p.)
        public Expression<Func<T, bool>> Creiteria { get; set; }

        // include(p=>p.).include(p=>p.)
        public List<Expression<Func<T, object>>> Includes { get; set; }

        // orderBy(p=>p.)
        public Expression<Func<T, object>> Order { get; set; }

        // orderByDesc(p=>p.)
        public Expression<Func<T, object>> OrderDesc { get; set; }

        // Take(2)
        public int Take { get; set; }

        // Skip(2)
        public int Skip { get; set; }
        public bool IsPagination { get; set; }
    }
}
