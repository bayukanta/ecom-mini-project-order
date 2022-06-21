using System;
using System.Collections.Generic;

namespace API.DTO
{
    public class ProductSearchDTO
    {
        
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Gender { get; set; }

        public int PriceMin { get; set; }

        public int PriceMax { get; set; }
    }

    
}
