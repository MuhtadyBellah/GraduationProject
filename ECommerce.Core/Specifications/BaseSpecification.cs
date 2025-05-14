using ECommerce.Core.Models;
using System.Linq.Expressions;

namespace ECommerce.Core.Specifications
{
    public class BaseSpecification<T> : ISpecific<T> where T : BaseEntity
    {
        public BaseSpecification(){ }
        public BaseSpecification(Expression<Func<T, bool>> creiterial)
            => Creiteria = creiterial;

        public Expression<Func<T, bool>> Creiteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = [];
        public Expression<Func<T, object>> Order { get; set; }
        public Expression<Func<T, object>> OrderDesc { get; set; }
        public int Take {  get; set; }
        public int Skip {  get; set; }
        public bool IsPagination { get; set; }



        public void OrderBy(Expression<Func<T, object>> OrderBy) => Order = OrderBy;
        public void OrderByDesc(Expression<Func<T, object>> OrderBy) => OrderDesc = OrderBy;

        public void Pagination(int skip, int take)
        {
            Skip = skip; 
            Take = take; 
            IsPagination = true;
        }
    
    }
}