using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace testNetCancellationToken.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        [HttpGet]
        public ActionResult Index()
        {
            Debug.WriteLine($"start - {DateTime.Now:O}");

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [AsyncTimeout(20000)]
        [Route("test")]
        [HttpGet]
        public async Task<ActionResult> Test(CancellationToken cancellationToken)
        {
            Debug.WriteLine($"start - {DateTime.Now:O}, Request.IsAjaxRequest - {Request.IsAjaxRequest()}");
            
            var now = DateTime.Now;

            while (DateTime.Now < now.AddSeconds(30))
            {
                Debug.WriteLine($"get data - {DateTime.Now:O}");

                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.WriteLine($"cancel - {DateTime.Now:O}");

                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                await BotherGoogle(cancellationToken).ConfigureAwait(false);

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            Debug.WriteLine($"return - {DateTime.Now:O}");

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private async Task BotherGoogle(CancellationToken token)
        {
            using (var httpClient = new HttpClient())
            {
                await httpClient.GetAsync("http://www.google.be", token)
                                .ConfigureAwait(false);
            }
        }
    }
}