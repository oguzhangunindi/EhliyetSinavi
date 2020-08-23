using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EhliyetSınavı.Models;
namespace EhliyetSınavı.Controllers
{
    public class CevaplarController : Controller
    {
        private EhliyetDB db = new EhliyetDB();
        public ActionResult Cevaplar()
        {
            string userName = HttpContext.Request.Cookies["ActiveUser"]["userName"];
            var kullaniciCevap = db.Cevaplar.Where(x => x.userName == userName);
            return View(kullaniciCevap);
        }
        [ActionName("CevaplarDelete"), HttpPost]
        public ActionResult CevaplarDeletePost(int id)
        {
            Cevaplar silinecekCevap = db.Cevaplar.Find(id);
            if (ModelState.IsValid)
            {
                db.Cevaplar.Remove(silinecekCevap);
                db.SaveChanges();
                ModelState.Clear();
            }
            return RedirectToAction("Cevaplar");
        }
        [ActionName("CevaplarDelete"), HttpGet]
        public ActionResult CevaplarDeleteGet(int id)
        {
            Cevaplar silinecekCevap = db.Cevaplar.Find(id);
            return View(silinecekCevap);
        }
        public ActionResult CevaplarDetails(int id)
        {
            var sınavDetayGor = db.Cevaplar.Where(x => x.Id == id).Single();
            if (sınavDetayGor.sinavTuru < 3 && sınavDetayGor.sinavTuru > 0)
            {
                int sıralıSınavNo = (int)sınavDetayGor.sinavNo;
                TempData["sıralıSınavNo"] = sıralıSınavNo;
                TempData["sıralıSınavAd"] = sınavDetayGor.userName;
                return View(db.Sınavlar.Where(x => x.sinavNo == sıralıSınavNo).ToList());
            }
            else if (sınavDetayGor.sinavTuru == 3)
            {
                int kSinavNo = (int)sınavDetayGor.sinavNo;
                TempData["kSınavNo"] = kSinavNo;
                TempData["kSınavAd"] = sınavDetayGor.userName;
                return View(db.Sınavlar.Where(x => x.sinavNo == kSinavNo).ToList());


            }
            return RedirectToAction("Giris", "Kullanıcılar");
        }
    }
}