using MailKit.Net.Smtp;

using MimeKit;

namespace WebApplication3.Foundation.Helper
{
    public static class EmailHelper
    {
        private static readonly string smtpServer;
        private static readonly int smtpPort;
        private static readonly string smtpUser;
        private static readonly string smtpPass;

        static EmailHelper()
        {
            smtpServer = ConfigHelper.GetString("SmtpServer");
            smtpPort = ConfigHelper.GetInt("SmtpPort");
            smtpUser = ConfigHelper.GetString("SmtpUser");
            smtpPass = ConfigHelper.GetString("SmtpPass");
        }

        public static void SendEmail(string toEmail, string subject, string body, bool isBodyHtml = false)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(smtpUser, smtpUser));
                message.To.Add(new MailboxAddress(toEmail, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = isBodyHtml ? body : null, TextBody = isBodyHtml ? null : body };
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(smtpServer, smtpPort, true); // 使用隐式SSL
                    client.Authenticate(smtpUser, smtpPass);

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (SmtpCommandException ex)
            {
                // 处理SMTP异常
                Console.WriteLine($"发送邮件时发生SMTP错误: {ex.Message}", ex);
                throw new Exception($"发送邮件时发生SMTP错误: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // 处理其他异常
                Console.WriteLine($"发送邮件时发生未知错误: {ex.Message}", ex);
                throw new Exception($"发送邮件时发生未知错误: {ex.Message}", ex);
            }
        }
    }
}
