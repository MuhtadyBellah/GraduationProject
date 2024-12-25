using ECommerce.Core.Models;

namespace ECommerce.Core.Specifications
{
    public class ProductFilterationCount : BaseSpecification<Product>
    {
        public ProductFilterationCount(ProductSpecParams param) :
            base(p =>
                (!param.FilterByProduct.HasValue || p.Id == param.FilterByProduct)
                &&
                (string.IsNullOrEmpty(param.SearchByName) || p.Name.ToLower().Contains(param.SearchByName))
                &&
                (!param.FilterByBrand.HasValue || p.ProductBrandId == param.FilterByBrand)
                && (!param.FilterByType.HasValue || p.ProductTypeId == param.FilterByType)
                ) 
        { }
    }
}
