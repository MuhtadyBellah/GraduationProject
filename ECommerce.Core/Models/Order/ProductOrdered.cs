using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Order
{
    public class ProductOrdered
    {
        public ProductOrdered() { }

        public ProductOrdered(int productId, string productName, string pictureUrl, string urlGlb)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = pictureUrl;
            UrlGlb = urlGlb;
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public string UrlGlb { get; set; }
    }
}
