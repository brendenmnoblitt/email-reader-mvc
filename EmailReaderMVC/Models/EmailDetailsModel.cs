namespace EmailReaderMVC.Models
{
    public class EmailDetailsModel
    {
        public string MessageID { get; set; }
        public string? MessageDate { get; set; }
        public string? MessageSender { get; set; }
        public string? SenderName { get; set; }
        public string? SenderEmail { get; set; }
        public string? SubjectLine { get; set; }
        public string? MessageBodyText { get; set; }
        public string? MessageBodyHTML { get; set; }
    }
}
