using MimeKit;
using MailKit.Net.Smtp;

namespace Backend.Utils
{
    public class EmailSender
    {
        private MimeMessage Message {get; set;} = new MimeMessage();

        /// <summary>
        /// Gets and Sets the email of the sender.
        /// </summary>
        public string SenderEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets and Sets the name associated with the Sender's email
        /// </summary>
        public string SenderName { get; set;} = string.Empty;

        /// <summary>
        /// Gets and Sets the email's body
        /// </summary>
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Gets and Sets the email's subject
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// List of strings containing Attachment's file paths.
        /// </summary>        
        private List<string> Attachments { get; set; } = [];
        
        /// <summary>
        /// Gets and Sets the host (email provider)
        /// </summary>
        public string Host { get; set; } = "smtp.gmail.com";

        /// <summary>
        /// Gets and Sets the Credential Unique identifier to retrieve the email's password stored in the local computer's Windows Credential Manager System.<para/>
        /// See also: <seealso cref="CredentialManager"/>, <seealso cref="Credential"/>
        /// </summary>
        public string CredentialID { get; } = SysCredentailTargets.EmailApp;

        /// <summary>
        /// Gets and Sets a flag telling if the <see cref="Host"/> requires authentication to send an email.
        /// </summary>
        public bool AuthenticationRequired { get; set; } = true;

        /// <summary>
        /// Gets and Sets the Port to use.
        /// </summary>
        public int Port { get; set; } = 587;

        /// <summary>
        /// Prepare the body of the email, attachments included.
        /// </summary>
        private void BuildBody() 
        {
            BodyBuilder bodyBuilder = new()
            {
                TextBody = Body
            };

            foreach(string attachmentPath in Attachments)
                bodyBuilder.Attachments.Add(attachmentPath);

            Message.Body = bodyBuilder.ToMessageBody();
        }

        /// <summary>
        /// Add a new file's path to the Attachment list.
        /// </summary>
        /// <param name="path">the Attachment's file path</param>
        public void AddAttachment(string path) => Attachments.Add(path);        

        /// <summary>
        /// Add a receiver.
        /// </summary>
        /// <param name="receiverAddress">the email address of the receiver</param>
        /// <param name="receiverName">the name associated to the email address of the receiver</param>
        public void AddReceiver(string receiverAddress, string receiverName) => Message.To.Add(new MailboxAddress(receiverName, receiverAddress));

        /// <summary>
        /// Send the email.
        /// </summary>
        /// <exception cref="Exception">Sender and Receiver not set</exception>
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
                        client.Authenticate(SenderEmail, CredentialManager.Get(CredentialID).Password);

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
