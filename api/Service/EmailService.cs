namespace npm.api.API.Service
{
    using NLog;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using static Web.Configs.Config;

    public class EmailService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static SMTPConfig config { get; set; } = Web.Configs.Config.Instance.Email;

        private readonly string templateRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "Email");

        public Task Send(string address, string subject, string type, object context)
        {
            var template = Scriban.Template.Parse(File.ReadAllText(Path.Combine(templateRoot, type + ".html")));
            var body = template.Render(context);
            return Send(address, subject, body);
        }

        public Task Send(string address, string subject, string body)
        {
            return Task.Run(() =>
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new System.Net.Mail.MailAddress(config.SenderAddress);
                mail.To.Add(address);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(config.Host, config.Port);
                smtp.Credentials = new System.Net.NetworkCredential(config.UserName, config.Password);
                try
                {
                    smtp.Send(mail);
                    mail.Dispose();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    logger.Error(ex);
                }
            });
        }
    }
}