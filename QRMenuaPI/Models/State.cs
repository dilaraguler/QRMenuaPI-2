using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QRMenuaPI.Models
{
    public class State

    //Primary key eklediğimiz zaman Identıty otomatik olarak eklenir. Manuel olarak devre dışı bırakırız.
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] //Identıty olmaması için belirtme
        public byte Id { get; set; }
        [Required] // Zorunlu en az bir karakter yazılmak zorunda
        [StringLength(10)] //Maximum 10 karakter
        [Column(TypeName = "nvarchar(10)")] //String olarak girdiğimiz bilginin type türünü belirtmek için
        public string Name { get; set; } = "";

       
    }
}
