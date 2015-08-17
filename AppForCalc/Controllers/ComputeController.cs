using System.Data;
using System.Web.Mvc;

namespace AppForCalc.Controllers
{
    public class ComputeController : Controller
    {
        //
        // GET: /Compute/

        public ActionResult Index(string x)
        {
            DataTable dt = new DataTable();
            var v = dt.Compute(x.Replace(" ", "+"), "");
            ViewBag.Message = v;
            return View();
        }

    }
}
