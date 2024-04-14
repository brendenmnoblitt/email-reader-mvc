using Google.Apis.Gmail.v1;
using Microsoft.AspNetCore.Mvc;
using EmailReaderMVC.Handlers;
using EmailReaderMVC.Models;
using Google.Apis.Auth.OAuth2;

namespace EmailReaderMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AuthHandler authHandler;
        private readonly GmailHandler gmailHandler;

        public HomeController()
        {
            authHandler = new AuthHandler();
            var credentials = authHandler.GetCredentials();
            gmailHandler = new GmailHandler(credentials);
        }

        public ActionResult Index()
        {
            var model = LoadEmails();
            return View(model);
        }

        public List<EmailDetailsModel> LoadEmails()
        {
            List<EmailDetailsModel> emailDetailsList = new List<EmailDetailsModel>();

            var messageIds = gmailHandler.GetMessageIds();
            var messageIdsList = messageIds.Item1;

            foreach (var messageId in messageIdsList)
            {
                var message = gmailHandler.GetMessage(messageId);
                var emailDetails = gmailHandler.ParseEmailDetails(message);
                emailDetailsList.Add(emailDetails);
            }

            return emailDetailsList;

        }
    }
}