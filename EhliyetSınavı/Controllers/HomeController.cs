using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using EhliyetSınavı.Models;
namespace EhliyetSınavı.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private EhliyetDB db = new EhliyetDB();
        Fonksiyonlar fns = new Fonksiyonlar();
        [ActionName("Index"), HttpPost]
        public ActionResult IndexPost()
        {
            int radioButtonNo = 1;
            if (Request[radioButtonNo.ToString()] != null)
            {
                if (Convert.ToInt16(Request[radioButtonNo.ToString()]) < 3)
                {
                    Session["sınavSecim"] = Request[radioButtonNo.ToString()];
                    return RedirectToAction("List", "Home");
                }
                else
                    Session["sınavSecim"] = Request[radioButtonNo.ToString()];
                return RedirectToAction("RandomList", "Home");

            }
            if (Session["sınavSecim"] == null) 
            {
                TempData["sınavSec"] = "Sınava Girmeden Önce Sınavı Seçmeniz Gerekmektedir..";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [ActionName("Index"), HttpGet]
        public ActionResult IndexGet()
        {
            if (HttpContext.Request.Cookies["ActiveUser"] != null)
            {
                return View();
            }
            return RedirectToAction("Giris", "Kullanıcılar");

        }
        [ActionName("List"), HttpPost]
        public ActionResult ListPost()
        {
            int sinavNo = Convert.ToInt32(Session["sıralıSinavNo"]);
            string[] kullanıcıCevaplar = new string[10];
            int sınavSecimNo = Convert.ToInt32(Session["sınavSecim"]);
            int dogruCevap = 0;
            int yanlisCevap = 0;
            for (int sayac = 0; sayac <= 9; sayac++)
            {
                kullanıcıCevaplar[sayac] = Request[(sayac + 1).ToString()];
                var sınavSoru = db.Sınavlar.Where(x => x.soruNo == sayac + 1 && x.sinavNo == sinavNo).Single();
                if (kullanıcıCevaplar[sayac] == sınavSoru.DCevap)
                {
                    dogruCevap++;
                    if (ModelState.IsValid)
                    {
                        var userDogruCevapEkle = db.Sınavlar.Where(x => x.Soru == sınavSoru.Soru && x.sinavNo == sinavNo).Single();
                        userDogruCevapEkle.userCevap = kullanıcıCevaplar[sayac];
                        db.Entry(userDogruCevapEkle).State = EntityState.Modified;
                        db.SaveChanges();
                        ModelState.Clear();
                    }
                }
                else
                {
                    yanlisCevap++;
                    if (ModelState.IsValid)
                    {
                        var userYanlisCevapEkle = db.Sınavlar.Where(x => x.Soru == sınavSoru.Soru && x.sinavNo == sinavNo).Single();
                        userYanlisCevapEkle.userCevap = kullanıcıCevaplar[sayac];
                        db.Entry(userYanlisCevapEkle).State = EntityState.Modified;
                        db.SaveChanges();
                        ModelState.Clear();
                    }
                }

            }
            if (HttpContext.Request.Cookies["ActiveUser"] != null)
            {
                string Id = HttpContext.Request.Cookies["ActiveUser"]["Id"];
                int id = Convert.ToInt32(Id);
                Kullanıcılar kullanici = db.Kullanıcılar.Find(id);
                Cevaplar cevap = new Cevaplar();
                if (ModelState.IsValid)
                {
                    cevap.userName = kullanici.userName;
                    cevap.dogruSayisi = dogruCevap.ToString();
                    cevap.yanlisSayisi = yanlisCevap.ToString();
                    cevap.sinavTarihi = DateTime.Now.Date;
                    cevap.sinavNo = Convert.ToInt32( Session["sıralıSinavNo"]);
                    cevap.sinavTuru = sınavSecimNo;
                    db.Cevaplar.Add(cevap);
                    db.SaveChanges();
                    ModelState.Clear();
                }

            }
            return RedirectToAction("Cevaplar", "Cevaplar");

        }
        [ActionName("List"), HttpGet]
        public ActionResult ListGet()
        {
            Sınavlar sınav = new Sınavlar();
            int sınavSecim = Convert.ToInt32(Session["sınavSecim"]);
            if (HttpContext.Request.Cookies["ActiveUser"] != null)
            {
                Session["sıralıSinavNo"] = fns.RastgeleSayi();
                string kullaniciId = HttpContext.Request.Cookies["ActiveUser"]["Id"];
                int id = Convert.ToInt32(kullaniciId);
                Kullanıcılar kullanici = db.Kullanıcılar.Find(id);
                if (sınavSecim != 0)
                {
                    int soruNo = 1 + (sınavSecim - 1) * 10;
                    int soruListeNo = 1;
                    for (int i = 0; i <= 9; i++)
                    {
                        var ekleSoru = db.SoruBankası.ToList().Where(x => x.sinavTuru == sınavSecim && x.Id == soruNo).Single();
                        if (ModelState.IsValid)
                        {
                            sınav.soruNo = soruListeNo;
                            sınav.userName = kullanici.userName;
                            sınav.Soru = ekleSoru.Soru;
                            sınav.A = ekleSoru.A;
                            sınav.B = ekleSoru.B;
                            sınav.C = ekleSoru.C;
                            sınav.D = ekleSoru.D;
                            sınav.DCevap = ekleSoru.DCevap;
                            sınav.sinavNo = (int)Session["sıralıSinavNo"];
                            db.Sınavlar.Add(sınav);
                            db.SaveChanges();
                            ModelState.Clear();
                            soruListeNo++;
                            soruNo++;
                        }
                    }
                    ViewData["SınavaGirecekKisi"] = HttpContext.Request.Cookies["ActiveUser"]["userName"];
                    return View(db.Sınavlar.ToList().Where(x => x.sinavNo == (int)Session["sıralıSinavNo"]));
                }
            }
            else
                TempData["girisYap"] = "Sınava Girmeden Önce Sisteme Kayıt Olunuz..";
            Session.Remove("sınavSecim");
            return RedirectToAction("Giris", "Kullanıcılar");
        }

        [ActionName("RandomList"), HttpPost]
        public ActionResult RandomListPost()
        {
            Sınavlar kSınav = new Sınavlar();
            char[] dCevaplar = new char[10];
            int sinavNo = Convert.ToInt32(Session["ksinavNo"]);
            string[] kullanıcıCevaplar = new string[10];
            int sınavSecimNo = Convert.ToInt32(Session["sınavSecim"]);
            int dogruCevap = 0;
            int yanlisCevap = 0;
            for (int sayac = 0; sayac <= 9; sayac++)
            {
                kullanıcıCevaplar[sayac] = Request[(sayac + 1).ToString()];
                var karısıkSoru = db.Sınavlar.Where(x => x.soruNo == sayac + 1 && x.sinavNo == sinavNo).Single();
                if (kullanıcıCevaplar[sayac] == karısıkSoru.DCevap)
                {
                    dogruCevap++;
                    if (ModelState.IsValid)
                    {
                        var kUserDogruCevapEkle = db.Sınavlar.Where(x => x.Soru == karısıkSoru.Soru && x.sinavNo == sinavNo).Single();
                        kUserDogruCevapEkle.userCevap = kullanıcıCevaplar[sayac];
                        db.Entry(kUserDogruCevapEkle).State = EntityState.Modified;
                        db.SaveChanges();
                        ModelState.Clear();
                    }
                }
                else
                {
                    yanlisCevap++;
                    if (ModelState.IsValid)
                    {
                        var kUserYanlisCevapEkle = db.Sınavlar.Where(x => x.Soru == karısıkSoru.Soru && x.sinavNo == sinavNo).Single();
                        kUserYanlisCevapEkle.userCevap = kullanıcıCevaplar[sayac];
                        db.Entry(kUserYanlisCevapEkle).State = EntityState.Modified;
                        db.SaveChanges();
                        ModelState.Clear();
                    }
                }

            }
            if (HttpContext.Request.Cookies["ActiveUser"] != null)
            {
                string Id = HttpContext.Request.Cookies["ActiveUser"]["Id"];
                int id = Convert.ToInt32(Id);
                Kullanıcılar kullanici = db.Kullanıcılar.Find(id);
                Cevaplar cevap = new Cevaplar();
                if (ModelState.IsValid)
                {
                    cevap.userName = kullanici.userName;
                    cevap.dogruSayisi = dogruCevap.ToString();
                    cevap.yanlisSayisi = yanlisCevap.ToString();
                    cevap.sinavTarihi = DateTime.Now.Date;
                    cevap.sinavNo = Convert.ToInt32(Session["kSinavNo"]);
                    cevap.sinavTuru = 3;
                    db.Cevaplar.Add(cevap);
                    db.SaveChanges();
                    ModelState.Clear();
                }

            }
            return RedirectToAction("Cevaplar", "Cevaplar");

        }
        public int[] sayilar = new int[10];
        [ActionName("RandomList"), HttpGet]
        public ActionResult RandomListGet()
        {
            string Id = HttpContext.Request.Cookies["ActiveUser"]["Id"];
            int id = Convert.ToInt32(Id);
            Kullanıcılar kullanici = db.Kullanıcılar.Find(id);
            Sınavlar kSınav = new Sınavlar();
            Random rnd = new Random();
            int soruIndex = 0;
            int counter = 1;
            int sınavSecim = Convert.ToInt32(Session["sınavSecim"]);
            if (HttpContext.Request.Cookies["ActiveUser"] != null)
            {
                if (sınavSecim == 3)
                {
                    /*  int SonsuzDongu = 1;
                      int kSinavNo = rnd.Next(1000000, 9999999);
                      while (SonsuzDongu < 2) //URETTİGİMİZ RANDOM SINAV NUMARASINDAN VERITABANINDA BULUNUP BULUNMADIGINI KONTROL EDIYORUZ
                      {
                          var kSinavSorgu = db.Sınavlar.Where(x => x.sinavNo == kSinavNo).FirstOrDefault();
                          if (kSinavSorgu == null)
                          {
                              break;
                          }
                          else
                          {
                              kSinavNo = rnd.Next(1000000, 9999999);
                          }
                      }
                    */
                    Session["kSinavNo"] = fns.RastgeleSayi();

                    /* if (db.Sınavlar.Count() != 0)
                     {
                         for (int i = 0; i <= 9; i++)
                         {
                             var silinecekSorular = db.Sınavlar.Where(x => x.sinavNo != ksinavNo).FirstOrDefault();
                             if (ModelState.IsValid)
                             {
                                 db.Sınavlar.Remove(silinecekSorular);
                                 db.SaveChanges();
                                 ModelState.Clear();
                             }
                         }
                     } 
                    */
                    while (soruIndex <= 9)
                    {
                        int sayi = rnd.Next(0, 21);
                        if (sayilar[0] == 0)
                        {
                            sayilar[soruIndex] = sayi;
                            soruIndex++;
                        }
                        if (sayilar.Contains(sayi))
                        {
                            continue;
                        }
                        else
                        {
                            sayilar[soruIndex] = sayi;
                            soruIndex++;
                        }
                    }
                    for (int i = 0; i <= 9; i++)
                    {
                        var rndSoru = db.SoruBankası.ToList().Where(x => x.Id == sayilar[i]).Single();
                        if (ModelState.IsValid)
                        {
                            kSınav.soruNo = counter;
                            kSınav.userName = kullanici.userName;
                            kSınav.Soru = rndSoru.Soru;
                            kSınav.A = rndSoru.A;
                            kSınav.B = rndSoru.B;
                            kSınav.C = rndSoru.C;
                            kSınav.D = rndSoru.D;
                            kSınav.DCevap = rndSoru.DCevap;
                            kSınav.sinavNo = (int)Session["kSinavNo"];
                            db.Sınavlar.Add(kSınav);
                            db.SaveChanges();
                            ModelState.Clear();
                            counter++;
                        }
                    }
                    ViewData["SınavaGirecekKisi"] = HttpContext.Request.Cookies["ActiveUser"]["userName"];
                    return View(db.Sınavlar.ToList().Where(x => x.sinavNo == (int)Session["kSinavNo"]));
                }
                else
                {
                    TempData["sınavSec"] = "Sınava Girmeden Önce Sınavı Seçmeniz Gerekmektedir..";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["girisYap"] = "Sınava Girmeden Önce Sisteme Kayıt Olunuz..";
                Session.Remove("sınavSecim");
            }
            return RedirectToAction("Giris", "Kullanıcılar");
        }
    }
}