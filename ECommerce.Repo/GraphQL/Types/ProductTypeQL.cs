using ECommerce.Core.Models;
using GraphQL.Types;

namespace ECommerce.Repo.GraphQL.Types
{
    public class ProductTypeQL : ObjectGraphType<Product>
    {
        public ProductTypeQL() 
        {
            Field(x => x.Id);
            Field(x => x.Name);
            Field(x => x.Description, nullable: true);
            Field(x => x.PictureUrl);
            Field(x => x.UrlGlb);
            Field(x => x.Price);
            Field(x => x.ProductBrandId);
            Field(x => x.ProductTypeId);
            Field(x => x.Quantity);
        }       
    }
}
