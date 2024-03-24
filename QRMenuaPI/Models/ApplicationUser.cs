using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using QRMenuaPI.Models;

namespace QRMenuaPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(100)")]
        public override string UserName { get; set; } = "";
        [StringLength(100, MinimumLength = 2)]
        [Column(TypeName = "nvarchar(100)")]
        public string? Name { get; set; }
        public DateTime RegisterDate { get; set; }
        [EmailAddress]
        [StringLength(100, MinimumLength = 2)]
        [Column(TypeName = "varchar(100)")]
        public override string Email { get; set; } = "";
        [Required]
        [Phone]
        [StringLength(30)]
        [Column(TypeName = "varchar(30)")]
        public string Phone { get; set; } = "";
        public byte StateId { get; set; }
        [ForeignKey("StateId")]
        public State? State { get; set; }
        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }


    }
}