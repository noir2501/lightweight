using System.Configuration.Provider;
using System.Net.Mail;

namespace Lightweight.Business.Providers.Mail
{
    public abstract class MailServiceProviderBase : ProviderBase, IMailServiceProvider
    {
        public abstract event SendCompletedEventHandler SendCompleted;
        public abstract void OnSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
        public abstract void Send(MailMessage message);
        public abstract void Send(string from, string recipients, string subject, string body);
        public abstract void SendAsync(MailMessage message, object token);
        public abstract void SendAsync(string from, string recipients, string subjects, string body, object token);
        public abstract void SendAsyncCancel();

        public abstract void Dispose();
    }
}