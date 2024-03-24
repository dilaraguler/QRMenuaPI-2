using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace QRMenuaPI.Models
{
	public class Category
	{
        [Key]
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; } = "";

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string? Description { get; set; }

        public byte StateId { get; set; }


        [ForeignKey("StateId")]
        public State? State { get; set; }

        public int RestaurantId { get; set; }

        [ForeignKey("RestaurantId")]
        [JsonIgnore]
        public Restaurant? Restaurants { get; set; }

        public virtual List<Food>? Foods { get; set; }   
    }
}

