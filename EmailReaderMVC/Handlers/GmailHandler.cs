using System.Text.RegularExpressions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using EmailReaderMVC.Utils;
using EmailReaderMVC.Models;
using System.Text;
using System.Reflection.Metadata;
using Microsoft.Extensions.WebEncoders.Testing;

namespace EmailReaderMVC.Handlers
{
    public class GmailHandler
    {

        private const string _gmailUser = "me";
        public GmailService service;

        public GmailHandler(UserCredential credentials)
        {
            this.service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials
            });
        }

        public (List<string>, string) GetMessageIds(string targetPageId = null)
        {
            var messageListQuery = service.Users.Messages.List(_gmailUser);
            messageListQuery.MaxResults = 20;
            messageListQuery.LabelIds = "INBOX";
            if (targetPageId != null)
            {
                messageListQuery.PageToken = targetPageId;
            }

            var messageList = messageListQuery.Execute();


            List<string> messageIds = new List<string>();
            string nextPageId = messageList.NextPageToken;

            foreach (var email in messageList.Messages)
            {
                messageIds.Add(email.Id);
            }

            return (messageIds, nextPageId);
        }

        public Message GetMessage(string messageId)
        {
            Console.WriteLine(messageId);
            var messageQuery = service.Users.Messages.Get(_gmailUser, messageId);
            messageQuery.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Full;
            Message message = messageQuery.Execute();
            return message;
        }

        public EmailDetailsModel ParseEmailDetails(Message message)
        {
            string messageId = message.Id;
            string messageDate = null;
            string messageSender = null;
            string senderName = null;
            string senderEmail = null;
            string subjectLine = null;
            string messageText = null;
            string messageHTML = null;
            // Parse header
            foreach (var header in message.Payload.Headers)
            {
                switch (header.Name)
                {
                    case "Subject":
                        subjectLine = header.Value;
                        break;
                    case "Date":
                        messageDate = header.Value;
                        break;
                    case "From":
                        messageSender = header.Value;
                        (senderName, senderEmail) = ParseSender(messageSender);
                        break;
                    default:
                        break;
                }
            }
            // Parse body
            (messageHTML, messageText) = ParseEmailBody(message);
            var EmailDetails = new EmailDetailsModel
            {
                MessageID = messageId,
                MessageDate = messageDate,
                MessageSender = messageSender,
                SenderName = senderName,
                SenderEmail = senderEmail,
                SubjectLine = subjectLine,
                MessageBodyHTML = messageHTML,
                MessageBodyText = messageText,
            };

            return EmailDetails;
        }
        
        public (string, string) ParseEmailBody(Message message)
        {
            string base64UrlEmailBody = null;
            string messageBody = null;

            string messageText = null;
            string messageHTML = null;
            try
            {
                if (message.Payload.Parts != null)
                {
                    foreach (var part in message.Payload.Parts)
                    {
                        base64UrlEmailBody = part.Body.Data;
                        if (base64UrlEmailBody != null)
                        {
                            string base64EmailBody = Parser.Base64UrlToBase64(base64UrlEmailBody);
                            messageBody = Parser.DecodeBase64(base64EmailBody);
                            switch (part.MimeType)
                            {
                                case "text/html":
                                    messageHTML = messageBody;
                                    break;
                                case "text/plain":
                                    messageText = messageBody;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    base64UrlEmailBody = message.Payload.Body.Data;
                    string base64EmailBody = Parser.Base64UrlToBase64(base64UrlEmailBody);
                    messageText = Parser.DecodeBase64(base64EmailBody);
                }

            }
            catch (Exception ex)
            {
                // FIXME {@brenden.noblitt}: come back to later for better handling
                Console.WriteLine(ex.Message);
            }
            
            return (messageHTML, messageText);
        }

        public (string, string) ParseSender(string sender)
        {
            // FIXME {@brenden.noblitt}: need exception handling
            string[] parts = sender.Split('<');
            string senderName = parts[0].Trim();
            string senderEmail;
            try
            {
                senderEmail = parts[1].TrimEnd('>');
            }
            catch
            {
                senderEmail = "";
            }
            
            return (senderName, senderEmail);
        }
        
        public int GetTotalMessageCount()
        {
            var userData = service.Users.GetProfile(_gmailUser).Execute();
            if (userData.MessagesTotal != null)
                return (int)userData.MessagesTotal;
            return 0;
        }
    }
}
