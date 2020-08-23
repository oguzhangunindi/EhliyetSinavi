namespace EhliyetSınavı.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SoruBankası
    {
        public int Id { get; set; }

        public string Soru { get; set; }

        public string A { get; set; }

        public string B { get; set; }

        public string C { get; set; }

        public string D { get; set; }

        [StringLength(50)]
        public string DCevap { get; set; }

        public int? sinavTuru { get; set; }
    }
}
