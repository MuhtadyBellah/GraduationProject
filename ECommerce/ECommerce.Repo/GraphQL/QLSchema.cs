using ECommerce.Repo.GraphQL.Mutations;
using ECommerce.Repo.GraphQL.Queries;
using GraphQL.Types;

namespace ECommerce.Repo.GraphQL
{
    public class QLSchema : Schema
    {
        public QLSchema(ProductQuery productQuery, ProductMutation productMutation)
        {
            this.Query = productQuery;
            this.Mutation  = productMutation;
        }
    }
}
