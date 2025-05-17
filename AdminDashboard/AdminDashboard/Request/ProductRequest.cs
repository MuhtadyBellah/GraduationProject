using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard.Request
{
    public class ProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int quantity { get; set; }
        public int productTypeId { get; set; }
        public int productBrandId { get; set; }
        public IFormFile PictureFile { get; set; }
        public IFormFile PictureFileGlB {  get; set; }
    }
}
