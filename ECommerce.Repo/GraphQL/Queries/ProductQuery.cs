using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Repo.Data;
using ECommerce.Repo.GraphQL.Types;
using GraphQL;
using GraphQL.Types;

namespace ECommerce.Repo.GraphQL.Queries
{
    public class ProductQuery : ObjectGraphType
    {
        public ProductQuery(IUnitWork _repos)
        {
            Field<ListGraphType<ProductTypeQL>>("products")
                .Description("List of All products")
                .ResolveAsync(async context => await _repos.Repo<Product>().GetAllAsync());
        
            Field<ProductTypeQL>("product")
                .Description("Get product by Id")
                .Argument<NonNullGraphType<IntGraphType>>("id", "Product Id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id", int.MinValue);
                    return await _repos.Repo<Product>().GetByIdAsync(id);
                });
        }
    }
}
