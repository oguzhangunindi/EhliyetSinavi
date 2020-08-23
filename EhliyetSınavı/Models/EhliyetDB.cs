namespace EhliyetSınavı.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EhliyetDB : DbContext
    {
        public EhliyetDB()
            : base("name=EhliyetDB")
        {
        }

        public virtual DbSet<Cevaplar> Cevaplar { get; set; }
        public virtual DbSet<Kullanıcılar> Kullanıcılar { get; set; }
        public virtual DbSet<Sınavlar> Sınavlar { get; set; }
        public virtual DbSet<SoruBankası> SoruBankası { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
