using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Goowitter_v07.Controllers
{
    public class SearchController : Controller
    {

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                DataManagementService service = new DataManagementService();
                ViewBag.UserData = service.LoadSearch(User.Identity.Name).ToList();
            }
            return View();
        }

        public async Task<ActionResult> Search(string Query, DateTime DateFrom, DateTime DateTo)
        {
            DataManagement.Search model = null;
            SearchService search = new Services.SearchService();
            if (User.Identity.IsAuthenticated)
            {
                //some stuff
                model = await search.Search(Query, DateFrom, DateTo, User.Identity.Name);
            }
            else 
            {
                model = await search.Search(Query, DateFrom, DateTo, "");
            }
            //ThreadPool.QueueUserWorkItem(_ => search.Search(model.Query, new DateTime(2014, 05, 02), new DateTime(2014, 05, 03)));
            //Thread thread = new Thread(_ => search.Search(Query, new DateTime(2014, 05, 02), new DateTime(2014, 05, 03)));
            //thread.Start();
           /* DataManagement.Search model = new DataManagement.Search();
            model.Query = Query;
            model.From = DateFrom;
            model.To = DateTo;
            model.SDate = DateTime.Now;
            Task.Run(async () => await search.Search(model));*/
            if (User.Identity.IsAuthenticated)
            {
                DataManagementService service = new DataManagementService();
                ViewBag.UserData = service.LoadSearch(User.Identity.Name).ToList();
            }
            return View("Index", model);
        }

        public ActionResult RefreshMap(DataManagement.Search model)
        {
            /*DataManagementService service = new DataManagementService();
            var result = service.LoadSearch(model.Id);
            if(result.Count() != 0)
            {
                return View("Search", result.First());
            }*/
            return View("Search", model);
        }
    }
}
