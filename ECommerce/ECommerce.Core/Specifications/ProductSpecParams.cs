using System.Diagnostics.Eventing.Reader;
using System.Runtime.Serialization;

namespace ECommerce.Core.Specifications
{
    public class ProductSpecParams
    {
        public SortOptions? Sort { get; set; }
        public int? FilterByProduct {  get; set; } 
        public int? FilterByBrand {  get; set; }
        public int? FilterByType {  get; set; }

        private int size { get; set; } = 10;
        public int PageSize { 
            get => size; 
            set => size = value > 20 ? 20 : value;
        }
        public int PageIndex { get; set; } = 1;

        private string? search { get; set; }
        public string? SearchByName
        {
            get => search;
            set => search = value?.ToLower();
        }

        public decimal? priceLower { get; set; }
        public decimal? priceUpper { get; set; }

        public bool? isFav { get; set; }
        public bool? isLike { get; set; }
        public bool? LowQuantity { get; set; }
    }
}
