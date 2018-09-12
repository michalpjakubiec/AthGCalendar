using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Auth.OAuth2.Responses;
using AthGCalendar.Logic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GoogleCalendarReader.Controllers
{
    public class AuthCallbackController : Google.Apis.Auth.OAuth2.Mvc.Controllers.AuthCallbackController
    {
        protected override FlowMetadata FlowData
        {
            get { return flowMetaData; }
        }

        private FlowMetadata flowMetaData { get; }

        public AuthCallbackController()
        {
            var dataStore = new DataStore();
            var clientID = WebConfigurationManager.AppSettings["GoogleClientID"];
            var clientSecret = WebConfigurationManager.AppSettings["GoogleClientSecret"];
            flowMetaData = new GoogleAppFlowMetaData(dataStore, clientID, clientSecret);
        }

        public AuthCallbackController(FlowMetadata flow)
        {
            flowMetaData = flow;
        }

        public override async Task<ActionResult> IndexAsync(AuthorizationCodeResponseUrl authorizationCode, CancellationToken taskCancellationToken)
        {
            if (string.IsNullOrEmpty(authorizationCode.Code))
            {
                var errorResponse = new TokenErrorResponse(authorizationCode);

                return OnTokenError(errorResponse);
            }

            var returnUrl = Request.Url.ToString();
            returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?"));

            await Flow.ExchangeCodeForTokenAsync(UserId, authorizationCode.Code, returnUrl,
                taskCancellationToken);

            var success = GoogleCalendarSyncer.SyncToGoogleCalendar(this);
            if (!success)
            {
                //return RedirectToAction("_SyncToGoogleCalendarError");

                //return Json("Token was revoked. Try again.");
            }

            return Redirect(Url.Content("~/"));
        }

        protected override ActionResult OnTokenError(TokenErrorResponse errorResponse)
        {
            return Redirect(Url.Content("~/"));
        }
    }
}