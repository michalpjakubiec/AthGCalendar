using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using AthGCalendar.Services;
using System;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using System.Web.Mvc;
using AthGCalendar.Models;

namespace AthGCalendar.Logic
{
    public class GoogleCalendarSyncer
    {

        public static string GetOauthTokenUri(Controller controller)
        {
            var authResult = GetAuthResult(controller);
            return authResult.RedirectUri;
        }

        public static bool SyncToGoogleCalendar(Controller controller)
        {
            try
            {
                var authResult = GetAuthResult(controller);

                var service = InitializeService(authResult);

                return true;
            }
            catch (Exception ex)
            {
                GoogleOauthTokenService.OauthToken = null;
                return false;
            }
        }

        public static bool AddToGoogleCalendar(Controller controller, GoogleEventInfo info)
        {
            try
            {
                var authResult = GetAuthResult(controller);

                var service = InitializeService(authResult);

                var calendarId = GetMainCalendarId(service);

                var calendarEvent = GetCalendarEvent(info);

                SyncCalendarEventToCalendar(service, calendarEvent, calendarId);
                return true;
            }
            catch (Exception ex)
            {
                GoogleOauthTokenService.OauthToken = null;
                return false;
            }
        }

        private static AuthorizationCodeWebApp.AuthResult GetAuthResult(Controller controller)
        {
            var dataStore = new DataStore();
            var clientID = WebConfigurationManager.AppSettings["GoogleClientID"];
            var clientSecret = WebConfigurationManager.AppSettings["GoogleClientSecret"];
            var appFlowMetaData = new GoogleAppFlowMetaData(dataStore, clientID, clientSecret);
            var factory = new AuthorizationCodeMvcAppFactory(appFlowMetaData, controller);
            var cancellationToken = new CancellationToken();
            var authCodeMvcApp = factory.Create();
            var authResultTask = authCodeMvcApp.AuthorizeAsync(cancellationToken);
            authResultTask.Wait();
            var result = authResultTask.Result;
            return result;
        }

        private static CalendarService InitializeService(AuthorizationCodeWebApp.AuthResult authResult)
        {
            var result = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = authResult.Credential,
                ApplicationName = "AthGCalendar Test"
            });
            return result;
        }

        public static string GetMainCalendarId(CalendarService service)
        {
            var calendarListRequest = new CalendarListResource.ListRequest(service);
            var calendars = calendarListRequest.Execute();
            var result = calendars.Items.First().Id;
            return result;
        }

        private static Event GetCalendarEvent()
        {
            var result = new Event();
            result.Summary = "Test Event Title";
            result.Description = "Test Event Description";
            result.Sequence = 1;
            var eventDate = new EventDateTime();
            eventDate.DateTime = DateTime.UtcNow;
            result.Start = eventDate;
            result.End = eventDate;
            return result;
        }

        private static Event GetCalendarEvent(GoogleEventInfo info)
        {
            var result = new Event();
            result.Summary = info.Title;
            result.Description = info.Description;
            result.Sequence = 1;
            var eventDateStart = new EventDateTime();
            eventDateStart.DateTime = info.StartDate + info.StartTime ;
            result.Start = eventDateStart;
            var eventDateEnd = new EventDateTime();
            eventDateEnd.DateTime = info.EndDate + info.EndTime;
            result.End = eventDateEnd;
            return result;
        }

        private static void SyncCalendarEventToCalendar(CalendarService service, Event calendarEvent, string calendarId)
        {
            var eventRequest = new EventsResource.InsertRequest(service, calendarEvent, calendarId);
            eventRequest.Execute();
        }
    }
}