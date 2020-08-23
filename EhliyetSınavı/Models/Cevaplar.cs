namespace EhliyetSınavı.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cevaplar")]
    public partial class Cevaplar
    {
        public int Id { get; set; }


        [StringLength(50)]
        [Display(Name = "Kullanıcı Adı")]
        public string userName { get; set; }

        [StringLength(50)]
        [Display(Name = "Doğru Cevap")]
        public string dogruSayisi { get; set; }

        [StringLength(50)]
        [Display(Name = "Yanlış Cevap")]
        public string yanlisSayisi { get; set; }

        [Display(Name = "Sınav Tarihi")]
        public DateTime? sinavTarihi { get; set; }
        
        [Display(Name = "Sınav Numarası")]
        public int? sinavNo { get; set; }

        [Display(Name = "Sınav Türü")]
        public int? sinavTuru { get; set; }
    }
}
