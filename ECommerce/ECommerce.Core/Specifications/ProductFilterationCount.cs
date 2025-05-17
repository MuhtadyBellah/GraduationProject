using ECommerce.Core.Models;

namespace ECommerce.Core.Specifications
{
    public class ProductFilterationCount : BaseSpecification<Product>
    {
        public ProductFilterationCount(ProductSpecParams param) :
            base(p =>
                (!param.FilterByProduct.HasValue || p.Id == param.FilterByProduct)
                &&
                (string.IsNullOrEmpty(param.SearchByName) || p.Name.ToLower().StartsWith(param.SearchByName))
                &&
                (!param.FilterByBrand.HasValue || p.ProductBrandId == param.FilterByBrand)
                && 
                (!param.FilterByType.HasValue || p.ProductTypeId == param.FilterByType)
                &&
                (!param.isFav.HasValue || p.Favorites.Any(f => f.isFavorite == param.isFav))
                &&
                (!param.isLike.HasValue || p.Favorites.Any(f => f.isLike == param.isLike))
                &&
                (!param.priceLower.HasValue || p.Price <= param.priceLower)
                &&
                (!param.priceUpper.HasValue || p.Price >= param.priceUpper)
                &&
                (!param.LowQuantity.HasValue || ((param.LowQuantity == true) ? p.Quantity < 3 : p.Quantity >= 3))
            ) 
        {}
    }
}
