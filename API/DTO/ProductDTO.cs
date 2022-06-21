using System;
using System.Collections.Generic;

namespace API.DTO
{
    public class ProductDTO
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Gender { get; set; }

        public int Price { get; set; }
    }

    
}
