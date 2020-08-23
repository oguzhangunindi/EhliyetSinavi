using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EhliyetSınavı.Models
{
    public class Fonksiyonlar
    {
        public int RastgeleSayi()
        {
            Random rnd = new Random();
            EhliyetDB db = new EhliyetDB();
            int SonsuzDongu = 1;
            int sSinavNo = rnd.Next(1000000, 9999999);
            while (SonsuzDongu < 2) //URETTİGİMİZ RANDOM SINAV NUMARASINDAN VERITABANINDA BULUNUP BULUNMADIGINI KONTROL EDIYORUZ
            {
                var SinavSorgu = db.Sınavlar.Where(x => x.sinavNo == sSinavNo).FirstOrDefault();
                if (SinavSorgu == null)
                {
                    break;
                }
                else
                {
                    sSinavNo = rnd.Next(1000000, 9999999);
                }
            }
            return sSinavNo;
        }
    }
}