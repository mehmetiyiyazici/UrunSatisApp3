using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var urunler = UrunleriGetir();
        return View(urunler);
    }

    [HttpPost]
    public IActionResult Index(SatisModel model)
    {
        var urunler = UrunleriGetir();

        var alinacakUrun = new Urun();
        foreach (var urun in urunler)
        {
            if (urun.Ad == model.Ad)
            {
                alinacakUrun = urun;
                break;
            }
        }

        if (alinacakUrun.Stok <= 0)
        {
            ViewData["Msg"] = "<div class=\"alert alert-warning\">Bu ürünün stoğu kalmadı.</div>";
            return View(urunler);
        }
        
        // 10 - 20 = -10
        var paraUstu = model.Para - alinacakUrun.Fiyat;

        if (paraUstu >= 0)
        {
            // stok düşümü
            alinacakUrun.Stok--;
            DegisiklikleriKaydet(urunler);
            
            ViewData["Msg"] = $"<div class=\"alert alert-success\">Teşekkür ederiz. {(paraUstu > 0 ? $"Para üstünüz {paraUstu} TL" : "")}</div>";
        }
        else
        {
            ViewData["Msg"] = $"<div class=\"alert alert-danger\">Paranız yetersiz. {alinacakUrun.Fiyat - model.Para} TL eksik.</div>";
        }
        
        // if (paraUstu > 0)
        // {
        //     ViewData["Msg"] = $"<div class=\"alert alert-success\">Teşekkür ederiz. Para üstünüz {paraUstu} TL</div>";
        // }
        // else if (paraUstu < 0)
        // {
        //     ViewData["Msg"] = $"<div class=\"alert alert-danger\">Paranız yetersiz. {alinacakUrun.Fiyat - model.Para} TL eksik.</div>";
        // }
        // else
        // {
        //     ViewData["Msg"] = "<div class=\"alert alert-success\">Teşekkür ederiz.</div>";
        // }
        
        return View(urunler);
    }

    public void DegisiklikleriKaydet(List<Urun> urunler)
    {
        var satirlarTxt = "";
        foreach (var urun in urunler)
        {
            satirlarTxt += $"{urun.Ad}|{urun.Fiyat}|{urun.Stok}{(urun != urunler.Last() ? "\n" : "")}";
        }
        
        // // ürünlerin sayısı kadar bir string dizisi oluşturuyorum.
        // var satirlar = new string[urunler.Count];
        //
        // for (var i = 0; i < urunler.Count; i++)
        // {
        //     var urun = urunler[i];
        //     // Elma|10|5
        //     satirlar[i] = $"{urun.Ad}|{urun.Fiyat}|{urun.Stok}";
        //     // oluşturduğum her bir satırı for üstündeki kısımda tanımladığım string dizisinin içine yerleştiriyorum
        // }
        //
        // // string dizisini tek bir parça string - metin haline getiriyorum.
        // var satirlarTxt = string.Join('\n', satirlar);
        // // bunu yapma sebebim en sona \n eklememek
        
        // metnimi txt içine yazdırıyorum.
        using StreamWriter writer = new("App_Data/urunler.txt");
        writer.Write(satirlarTxt);
    }

    public List<Urun> UrunleriGetir()
    {
        var urunler = new List<Urun>();

        using StreamReader reader = new("App_Data/urunler.txt");
        var urunlerTxt = reader.ReadToEnd();
        var urunlerSatirlar = urunlerTxt.Split('\n');
        foreach (var satir in urunlerSatirlar)
        {
            var urunSatir = satir.Split('|');
            urunler.Add(new Urun
            {
                Ad = urunSatir[0],
                Fiyat = int.Parse(urunSatir[1]),
                Stok = int.Parse(urunSatir[2])
            });
        }

        return urunler;
    }
}