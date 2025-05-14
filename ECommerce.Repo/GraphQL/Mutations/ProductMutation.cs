using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Repo.GraphQL.Types;
using GraphQL;
using GraphQL.Types;

namespace ECommerce.Repo.GraphQL.Mutations
{
    public class ProductMutation : ObjectGraphType
    {
        public ProductMutation(IUnitWork _repos) 
        {
            Field<ProductTypeQL>("createProduct")
                .Description("Add Product")
                .Argument<NonNullGraphType<ProductInputType>>("product", "Product input")
                .ResolveAsync(async context =>
                {
                    var product = context.GetArgument<Product>("product");
                    if(product == null)  return null;
                    await _repos.Repo<Product>().AddAsync(product);
                    return product;
                });

            Field<ProductTypeQL>("updateProduct")
                .Description("Update Product")
                .Argument<NonNullGraphType<IdGraphType>>("id", "Product Id")
                .Argument<NonNullGraphType<ProductInputType>>("product", "Product input")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    var product = context.GetArgument<Product>("product");
                    if (product == null) return null;
                    _repos.Repo<Product>().Update(product);
                    await _repos.CompleteAsync();
                    return product;
                });

            Field<ProductTypeQL>("deleteProduct")
                .Description("Delete Product")
                .Argument<NonNullGraphType<IdGraphType>>("id", "Product Id")
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    var product = await _repos.Repo<Product>().GetByIdAsync(id);
                    if (product == null) return false;
                    _repos.Repo<Product>().Delete(product);
                    await _repos.CompleteAsync();
                    return true;
                });
        }
    }
}
