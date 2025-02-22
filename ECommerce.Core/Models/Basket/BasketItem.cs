﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Basket
{
    public class BasketItem
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string PictureUrl { get; set; }
        public string UrlGlb { get; set; }
        public decimal Price { get; set; }
        public string ProductBrand { get; set; }
        public string ProductType { get; set; }
        public int Quantity { get; set; }
    }
}
