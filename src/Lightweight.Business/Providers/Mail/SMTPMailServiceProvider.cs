using System;

namespace Lightweight.Business.Providers.Mail
{
    public class SMTPMailServiceProvider : MailServiceProviderBase
    {
        System.Net.Mail.SmtpClient client;

        const string HostKey = "host";
        const string PortKey = "port";
        const string UserNameKey = "username";
        const string PasswordKey = "password";
        const string EnableSSLKey = "ssl";

        public override event System.Net.Mail.SendCompletedEventHandler SendCompleted;

        public override void OnSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (SendCompleted != null)
                SendCompleted(sender, e);
        }

        public override void Send(System.Net.Mail.MailMessage message)
        {
            client.Send(message);
        }

        public override void Send(string from, string recipients, string subject, string body)
        {
            client.Send(from, recipients, subject, body);
        }

        public override void SendAsync(System.Net.Mail.MailMessage message, object token)
        {
            client.SendAsync(message, token);
        }

        public override void SendAsync(string from, string recipients, string subject, string body, object token)
        {
            client.SendAsync(from, recipients, subject, body, token);
        }

        public override void SendAsyncCancel()
        {
            client.SendAsyncCancel();
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);


            string host = config.Get(HostKey);
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("host", "No host defined for the SMTP server in the configuration file.");

            string port = config.Get(PortKey);
            int portValue;

            if (!string.IsNullOrEmpty(port) && int.TryParse(port, out portValue))
                client = new System.Net.Mail.SmtpClient(host, portValue);
            else
                client = new System.Net.Mail.SmtpClient(host);

            string username = config.Get(UserNameKey);
            string password = config.Get(PasswordKey);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                client.Credentials = new System.Net.NetworkCredential(username, password);
            }
            else
                client.UseDefaultCredentials = true;

            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

            bool ssl = false;
            string enableSSL = config.Get(EnableSSLKey);
            if (!string.IsNullOrEmpty(enableSSL))
                bool.TryParse(enableSSL, out ssl);

            client.EnableSsl = ssl;

            client.SendCompleted += OnSendCompleted;
        }

        public override void Dispose()
        {
            client.Dispose();
        }
    }
}