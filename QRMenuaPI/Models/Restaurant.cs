using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace QRMenuaPI.Models
{
	public class Restaurant
	{
        public int Id { get; set; }

        [StringLength(200, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; } = "";

        [StringLength(5, MinimumLength = 5)]
        [Column(TypeName = "char(5)")]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; } = "";

        [Column(TypeName = "nvarchar(200)")]
        [StringLength(200, MinimumLength = 5)]
        public string Adress { get; set; } = "";


        [Phone]
        [StringLength(30)]
        [Column(TypeName = "varchar(30)")]
        public string Phone { get; set; } = "";


        [EmailAddress]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string EMail { get; set; } = "";

        public DateTime RegisterDate { get; set; }


        [StringLength(11, MinimumLength = 10)]
        [Column(TypeName = "varchar(11)")]
        public string TaxNumber { get; set; } = "";


        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? WebAddress { get; set; }

        public byte StateId { get; set; }

        [ForeignKey("StateId")] //State clasından bilgi çekebilmek için kullanırız
        public State? State { get; set; }

        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        [JsonIgnore]
       public Company? Company { get; set; }

        public virtual List<Category>? Categories { get; set; }
    }


}

