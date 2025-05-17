using GraphQL.Types;

namespace ECommerce.Repo.GraphQL.Types
{
    public class ProductInputType : InputObjectGraphType
    {
        public ProductInputType()
        {
            Name = "productInput";
            Field<NonNullGraphType<StringGraphType>>("Name");
            Field<NonNullGraphType<StringGraphType>>("Description");
            Field<NonNullGraphType<FloatGraphType>>("Price");
            Field<NonNullGraphType<IntGraphType>>("ProductTypeId");
            Field<NonNullGraphType<IntGraphType>>("ProductBrandId");
            Field<NonNullGraphType<StringGraphType>>("PictureUrl");
            Field<NonNullGraphType<StringGraphType>>("UrlGlb");
            Field<NonNullGraphType<DecimalGraphType>>("Quantity");
        }
    }
}