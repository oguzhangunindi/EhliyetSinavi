using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EhliyetSınavı.Models;

namespace EhliyetSınavı.Controllers
{
    public class KullanıcılarController : Controller
    {
        EhliyetDB db = new EhliyetDB();
        [HttpPost]
        public ActionResult Giris(string userName, string userPassword)
        {
            var kullanici = db.Kullanıcılar.Where(x => x.userName == userName && x.userPassword == userPassword).SingleOrDefault();
            HttpCookie UserCookie = new HttpCookie("ActiveUser");
            if (kullanici != null)
            {
                UserCookie.Expires.AddHours(1);
                UserCookie["Id"] = kullanici.Id.ToString();
                UserCookie["userName"] = kullanici.userName;
                UserCookie["userPassword"] = kullanici.userPassword;
                HttpContext.Response.Cookies.Add(UserCookie);
                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["hatalıGiris"] = "Kullanıcı Adı veya Şifre Yanlış!";
            }
            return View("Giris");

        }
        [HttpGet]
        public ActionResult Giris()
        {
            if (HttpContext.Request.Cookies["ActiveUser"] != null)
            {
                return RedirectToAction("Index","Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Cikis()
        {
            HttpContext.Response.Cookies["ActiveUser"].Expires = DateTime.Now.AddDays(-1);
            Session.Remove("sınavSecim");
            return RedirectToAction("Giris", "Kullanıcılar");
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Dışardan verilerimize istenmeyen kişilerin ulaşmasını engeller hem viewde hem actionda kullanılmalı.
        public ActionResult Profil([Bind(Include = "Id,userName,userPassword")] Kullanıcılar kullanıcılar)
        {
            if (ModelState.IsValid)
            {
                string kullanıcıId = HttpContext.Request.Cookies["ActiveUser"]["Id"];
                int id = Convert.ToInt32(kullanıcıId);
                var editedUser = db.Kullanıcılar.Find(id);
                editedUser.userName = kullanıcılar.userName;
                editedUser.userPassword = kullanıcılar.userPassword;
                db.Entry(editedUser).State = EntityState.Modified;
                db.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("Index","Home");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Profil()
        {
            string kullanıcıId = HttpContext.Request.Cookies["ActiveUser"]["Id"];
            int id = Convert.ToInt32(kullanıcıId);
            Kullanıcılar kullanıcı = db.Kullanıcılar.Find(id);
            /* string kullanici = HttpContext.Request.Cookies["ActiveUser"]["userName"];
             var kullaniciAdi = db.Kullanıcılar.Where(x => x.userName == kullanici);
           */
            return View(kullanıcı);
        }
        [ActionName("Kayit"), HttpPost]
        public ActionResult KayitPost([Bind(Include = "userName,userPassword")] Kullanıcılar kullanıcılar)
        {
            var kullanici = db.Kullanıcılar.Where(x => x.userName == kullanıcılar.userName).SingleOrDefault();
            if (kullanici == null)
            {
                if (ModelState.IsValid)
                {
                    db.Kullanıcılar.Add(kullanıcılar);
                    db.SaveChanges();
                    ModelState.Clear();
                    TempData["Basarili"] = "Kayıt Olma İşlemi Başarılı!";
                }
                return RedirectToAction("Giris");
            }
            else
            {
                TempData["Hata"] = "Bu Kullanıcı Adı veya Şifre Kullanılmaktadır!";

            }
            return View();
        }

        [ActionName("Kayit"), HttpGet]
        public ActionResult KayitGet()
        {
            if (HttpContext.Request.Cookies["ActiveUser"] != null)
            {
                return RedirectToAction("List","Home");
            }
            return View();
        }
    }
}