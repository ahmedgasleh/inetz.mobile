namespace inetz.authserver.services
{
    public interface IMailService
    {
        Task SendEmailAsync ( SendEmailRequest sendEmailRequest );
    }
}
