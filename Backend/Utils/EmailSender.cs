using MimeKit;
using MailKit.Net.Smtp;

namespace Backend.Utils
{
    public class EmailSender
    {
        public MimeMessage Message {get; set;} = new MimeMessage();
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set;} = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Attachments { get; set; } = string.Empty;
        public string Host { get; set; } = "smtp.gmail.com";
        public string AppPassword { get; set; } = string.Empty;
        public bool AuthenticationRequired { get; set; } = true;
        public int Port { get; set; } = 587;

        public EmailSender() 
        {
        }

        private void BuildBody() 
        {
            BodyBuilder bodyBuilder = new()
            {
                TextBody = Body
            };

            if (!string.IsNullOrEmpty(Attachments))
                bodyBuilder.Attachments.Add(Attachments);

            Message.Body = bodyBuilder.ToMessageBody();
        }

        public void AddReceiver(string receiverName, string receiverAddress) 
        {
            Message.To.Add(new MailboxAddress(receiverName, receiverAddress));
        }

        public void Send() 
        {
            if (Message.To.Count == 0) throw new Exception("No Receiver was added");
            if (string.IsNullOrEmpty(SenderEmail) || string.IsNullOrEmpty(SenderName)) throw new Exception("No Sender information");

            Message.From.Add(new MailboxAddress(SenderName, SenderEmail));
            Message.Subject = Subject;

            BuildBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(Host, 587, MailKit.Security.SecureSocketOptions.StartTls);

                    if (AuthenticationRequired)
                        client.Authenticate(SenderEmail, AppPassword);

                    client.Send(Message);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                }
            }
        }
    }
}
