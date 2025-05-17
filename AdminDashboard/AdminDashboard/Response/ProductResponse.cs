using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard
{
    public class ProductPaged
    {
        public int size { get; set; }
        public int index {  get; set; }
        public int count {  get; set; }
        public List<ProductResponse> data { get; set; }
    }

    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public string UrlGlb { get; set; }
        public decimal Price { get; set; }
        public int ProductBrandId { get; set; }
        public string ProductBrand { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductType { get; set; }
        public int Quantity { get; set; }
        public bool IsFav { get; set; }
        public bool IsLike { get; set; }
    }
}