using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public List<OrderDetail> OrderDetails { get; set; }

        //should be "Pending", "Approved", or "Rejected"
        public string Status { get; set; }

        public Order()
        {
            CreatedDate = DateTime.UtcNow;
            Status = "Pending";
        }
    }
}
