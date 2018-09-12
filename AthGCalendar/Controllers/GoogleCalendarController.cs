using AthGCalendar.Logic;
using AthGCalendar.Models;
using AthGCalendar.Services;
using Google.Apis.Calendar.v3.Data;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace AthGCalendar.Controllers
{
    public class GoogleCalendarController : Controller
    {
        public ActionResult _SyncToGoogleCalendarError()
        {
            return View();
        }

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
                    var redirectUri = GoogleCalendarSyncer.GetOauthTokenUri(this);
                    return Redirect(redirectUri);

                    //return Json("Token was revoked. Try again.");
                    //return RedirectToAction("_SyncToGoogleCalendarError");
                }
            }
            //return Redirect("~/");
            return RedirectToAction("GetEvents");
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
                    return RedirectToAction("_SyncToGoogleCalendarError");
                    //return Json("Token was revoked. Try again.");
                }
            }
            return RedirectToAction("GetEvents");
        }

        public ActionResult GetEvents()
        {
            var events = GoogleCalendarSyncer.GetEvents(this);
            //return Json(events, JsonRequestBehavior.AllowGet);
            return View(events);
        }

        public JsonResult GetNextEvent()
        {
            var events = GoogleCalendarSyncer.GetEvents(this);
            var nextEvent = events.Where(x => x.Start.DateTime.HasValue)
                .Select(x => new { start = x.Start.DateTimeRaw, title = x.Summary })
                .OrderBy(x => x.start).FirstOrDefault();
            return Json(nextEvent, JsonRequestBehavior.AllowGet);
        }
    }
}