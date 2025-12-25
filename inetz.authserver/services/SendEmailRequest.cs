namespace inetz.authserver.services
{
    public class SendEmailRequest
    {
        public string Subject { get; set; }
        public string Recipient { get; set; }

        public string Body { get; set; }
        public SendEmailRequest ( string _recipient, string _subject, string _body )
        {
            Subject = _subject;
            Recipient = _recipient;
            Body = _body;
        }
    }
}
