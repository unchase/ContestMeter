using System.Linq;
using System.Web.Mvc;
using ContestMeter.Web.Site.Database;

namespace ContestMeter.Web.Site.Controllers
{
    public class HomeController : Controller
    {
        private readonly ContestMeterDbContext _db = new ContestMeterDbContext();

        public ActionResult Index()
        {
            var model = _db.NewsItems.OrderByDescending(ni => ni.CreatedDate).ToList();
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "О тестирующей системе ContestMeter.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Контакты.";

            return View();
        }
    }
}