namespace EhliyetSınavı.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Kullanıcılar
    {
        public int Id { get; set; }

        [StringLength(20)]
        [Display(Name ="Kullanıcı Adı")]
        public string userName { get; set; }

        [StringLength(12)]
        [Display(Name ="Şifre")]
        public string userPassword { get; set; }
    }
}
