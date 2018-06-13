using AthGCalendar.Logic;
using AthGCalendar.Services;
using System.Web.Mvc;

namespace AthGCalendar.Controllers
{
    public class GoogleCalendarController : Controller
    {
        [HttpPost]
        public ActionResult SyncToGoogleCalendar()
        {
            if (string.IsNullOrWhiteSpace(GoogleOauthTokenService.OauthToken))
            {
                var redirectUri = GoogleCalendarSyncer.GetOauthTokenUri(this);
                return Redirect(redirectUri);
            }
            else
            {
                var success = GoogleCalendarSyncer.SyncToGoogleCalendar(this);
                if (!success)
                {
                    return Json("Token was revoked. Try again.");
                }
            }
            return Redirect("~/");
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}