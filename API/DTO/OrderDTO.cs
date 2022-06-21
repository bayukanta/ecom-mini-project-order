using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace API.DTO
{
    public class OrderDTO
    {
        public Guid? Id { get; set; }

        public Guid UserId { get; set; }

        public string Status { get; set; }
    }

   
}
