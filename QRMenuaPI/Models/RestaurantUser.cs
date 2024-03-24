using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRMenuaPI.Models
{
	public class RestaurantUser
	{
        public string UserId { get; set; } = "";
        [ForeignKey("UserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        public int RestaurantId { get; set; }
        [ForeignKey("RestaurantId")]
        public Restaurant? Restaurant { get; set; }
    }
}

