using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestProject.EvaluateTool;
using TestProject.Models;

namespace TestProject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        private EvaluateDBEntities db = new EvaluateDBEntities();

        [HttpPost]
        public ActionResult Index(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string host = uri.Scheme + "://" + uri.Host;

                List<string> sitemapURLs = SitemapHelper.GetSitemapURLs(host);

                if (sitemapURLs.Count > 0)
                {
                    Evaluate evaluate = new Evaluate { Host = host, Date = DateTime.Now };
                    db.Evaluates.Add(evaluate);
                    db.SaveChanges();

                    Task.Run(() => new EvaluateSite(sitemapURLs, evaluate.Id));

                    ViewBag.EvaluateId = evaluate.Id;
                    ViewBag.Host = host;
                    ViewBag.URLsCount = sitemapURLs.Count;
                }
                else
                {
                    ViewBag.Error = "Sitemap can not be found or Sitemap does not contain URLs!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();
        }

        [HttpPost]
        public JsonResult GetURLs(int evaluateId)
        {
            List<URL> urls = new List<URL>();

            lock (EvaluateSite.EvaluateURLs)
            {
                foreach (var data in EvaluateSite.EvaluateURLs.Where(x => x.EvaluateId == evaluateId && !x.IsRead))
                {
                    urls.Add(data);
                    data.IsRead = true;
                }
            }

            return Json(urls, JsonRequestBehavior.AllowGet);
        }

        public ActionResult History()
        {
            IEnumerable<Evaluate> evaluates = from eval in db.Evaluates select eval;
            return View(evaluates);
        }

        public ActionResult Details(int id)
        {
            IEnumerable<URL> urls = from url in db.URLs where url.EvaluateId == id select url;
            return View(urls);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}