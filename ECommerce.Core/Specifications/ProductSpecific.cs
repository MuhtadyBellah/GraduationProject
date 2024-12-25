using ECommerce.Core.Models;

namespace ECommerce.Core.Specifications
{
    public class ProductSpecific : BaseSpecification<Product>
    {
        public ProductSpecific(ProductSpecParams param) : 
            base(p=>
                (!param.FilterByProduct.HasValue || p.Id == param.FilterByProduct)
                &&
                (string.IsNullOrEmpty(param.SearchByName) || p.Name.ToLower().Contains(param.SearchByName))
                &&
                (!param.FilterByBrand.HasValue || p.ProductBrandId == param.FilterByBrand)
                && (!param.FilterByType.HasValue || p.ProductTypeId == param.FilterByType)
                )
        {
            Includes.Add(p => p.ProductType);
            Includes.Add(p => p.ProductBrand);
            switch (param.Sort)
            {
                case SortOptions.Name:
                    OrderBy(p => p.Name);
                    break;
                case SortOptions.NameDesc:
                    OrderByDesc(p => p.Name);
                    break;
                case SortOptions.Price:
                    OrderBy(p => p.Price);
                    break;
                case SortOptions.PriceDesc:
                    OrderByDesc(p => p.Price);
                    break;
                case SortOptions.Brand:
                    OrderBy(p => p.ProductBrand.Name);
                    break;
                case SortOptions.Type:
                    OrderBy(p => p.ProductType.Name);
                    break;
                default:
                    OrderBy(p => p.Id); // Default case if no sort option is provided
                    break;
            }

            // Products = 100
            // PageSize = 10
            // PageIndex = 5
            //skip(40) take(10)

            Pagination((param.PageIndex-1) * param.PageSize, param.PageSize);
        }

        public ProductSpecific(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.ProductType);
            Includes.Add(p => p.ProductBrand);
        }
        public ProductSpecific(List<int> ids) : base(p => ids.Contains(p.Id))
        {
            Includes.Add(p => p.ProductType);
            Includes.Add(p => p.ProductBrand);
        }
    }
}
