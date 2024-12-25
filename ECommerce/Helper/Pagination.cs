using ECommerce.DTO;
using System.Collections;

namespace ECommerce.Helper
{
    public class Pagination<T> 
    {
        public int Size { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
        public IEnumerable<T> Data { get; set; }

        public Pagination(int pageIndex, int pageSize, IEnumerable<T> mapProducts, int count)
        {
            Size = pageSize;
            Index = pageIndex;
            Data = mapProducts;
            Count = count;
        }

    }
}
