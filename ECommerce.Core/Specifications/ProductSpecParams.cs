using System.Runtime.Serialization;

namespace ECommerce.Core.Specifications
{
    public enum SortOptions
    {
        [EnumMember(Value = "Name")]
        Name,
        [EnumMember(Value = "NameDesc")]
        NameDesc,
        [EnumMember(Value = "Price")]
        Price,
        [EnumMember(Value = "PriceDesc")]
        PriceDesc,
        [EnumMember(Value = "Brand")]
        Brand,
        [EnumMember(Value = "Type")]
        Type
    }

    public class ProductSpecParams
    {
        public SortOptions? Sort { get; set; }
        public int? FilterByProduct {  get; set; } 
        public int? FilterByBrand {  get; set; }
        public int? FilterByType {  get; set; }

        private int size = 5;
        public int PageSize { 
            get => size; 
            set => size = value > 10 ? 10 : value;
        }
        public int PageIndex { get; set; } = 1;

        private string? search { get; set; }
        public string? SearchByName
        {
            get => search;
            set => search = value?.ToLower();
        }
    }

}
