using System;
using System.Collections.Generic;

namespace API.DTO
{
    public class OrderDetailDTO
    {
        public Guid? Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

    }

    public class OrderDetailWithDataDTO : OrderDetailDTO
    {
        public OrderDTO Order { get; set; }

        public ProductDTO Product { get; set; }
    }



}
