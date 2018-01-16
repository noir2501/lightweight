using System;
using System.ComponentModel;
using System.Net.Mail;

namespace Lightweight.Business.Providers.Mail
{
    public interface IMailServiceProvider : IDisposable
    {
        event SendCompletedEventHandler SendCompleted;
        void OnSendCompleted(object sender, AsyncCompletedEventArgs e);

        void Send(MailMessage message);
        void Send(string from, string recipients, string subject, string body);
        void SendAsync(MailMessage message, object token);
        void SendAsync(string from, string recipients, string subjects, string body, object token);
        void SendAsyncCancel();
    }
}