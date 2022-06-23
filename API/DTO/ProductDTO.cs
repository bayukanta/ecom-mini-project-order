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

        public string Img { get; set; }

        public string Keterangan { get; set; }

        public int HargaAwal { get; set; }

        public int HargaJual { get; set; }


    }

    
}
