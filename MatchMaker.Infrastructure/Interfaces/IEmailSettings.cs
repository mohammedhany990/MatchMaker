namespace MatchMaker.Infrastructure.Interfaces
{
    public interface IEmailSettings
    {
        void SendEmail(Core.Entities.Email email);
    }
}
