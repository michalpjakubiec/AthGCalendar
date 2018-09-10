using AthGCalendar.Logic;
using AthGCalendar.Models;
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
            //return Redirect("~/");
            return RedirectToAction("AddEvent");
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddEvent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEvent(GoogleEventInfo info)
        {
            if (string.IsNullOrWhiteSpace(GoogleOauthTokenService.OauthToken))
            {
                var redirectUri = GoogleCalendarSyncer.GetOauthTokenUri(this);
                return Redirect(redirectUri);
            }
            else
            {
                var success = GoogleCalendarSyncer.AddToGoogleCalendar(this, info);
                if (!success)
                {
                    return Json("Token was revoked. Try again.");
                }
            }
            return RedirectToAction("AddEvent");
        }
    }
}