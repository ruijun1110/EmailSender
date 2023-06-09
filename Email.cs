using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Text;


namespace EmailTool
{
    /// <summary>
    /// Class that represents the email sender object, which configure and sends the email using SMTP.
    /// </summary>
    public class Email : IDisposable
    {
        // Used for dispose stream, To detect redundant calls.
        private bool _disposedValue;
        private int _maxAttempts = 3;
        public SmtpClient Client { get; private set; }
        public MailMessage Message { get; private set; }


        public Email()
        {
            this.Client = new SmtpClient();
            this.Message = new MailMessage();
        }

        /// <summary>
        /// Method that configures the parameters of the SMTP client.
        /// The Sender object contains the data of the sender's email account and server that read from the configuration file.
        /// </summary>
        public void SetClient(Sender sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("Sender object is null. SetClient function failed.");
            }
            Client.Host = sender.Host;
            Client.Port = sender.Port;
            Client.UseDefaultCredentials = false;
            Client.Credentials = new NetworkCredential(sender.Email, sender.Token);
            Client.EnableSsl = true;
            Client.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        /// <summary>
        /// Method that configures the parameters of the mail message.
        /// The Sender object contains the data of the sender's email account that read from the configuration file.
        /// The Recipients object contains the data of the receivers' email accounts that read from the configuration file.
        /// </summary>
        public void SetMessage(EmailContent emailContent, Sender sender, Recipients recipients)
        {
            if (emailContent == null)
            {
                throw new ArgumentNullException("Email object is null. SetMessage function failed.");
            }
            if (sender == null)
            {
                throw new ArgumentNullException("Sender object is null. SetMessage function failed.");
            }
            if (recipients == null)
            {
                throw new ArgumentNullException("Receivers object is null. SetMessage function failed.");
            }
            Message.From = new MailAddress(sender.Email, sender.Name);
            Message.Subject = emailContent.Subject;
            Message.Body = emailContent.Body;
            Message.IsBodyHtml = emailContent.IsBodyHtml;
            Message.SubjectEncoding = Encoding.UTF8;
            Message.BodyEncoding = Encoding.UTF8;
            foreach (String toEmail in recipients.ToEmails)
            {
                Message.To.Add(toEmail);
            }
            foreach (String ccEmail in recipients.CcEmails)
            {
                Message.CC.Add(ccEmail);
            }
            foreach (String bccEmail in recipients.BccEmails)
            {
                Message.Bcc.Add(bccEmail);
            }
        }

        /// <summary>
        /// Method that adds the attachments with local file path to the mail message.
        /// </summary>
        public void AddAttachment(EmailContent emailContent)
        {
            if (emailContent == null)
            {
                throw new ArgumentNullException("Email object is null. AddAttachment function failed.");
            }
            foreach (EmailFile file in emailContent.Attachments)
            {
                if (file.Path == null)
                {
                    throw new Exception("Attachment path is null. AddAttachment function failed. Please check the appsettings.json file.");
                }
                Attachment attachment = new Attachment(file.Path);
                AddAttachment(attachment);
            }
        }

        /// <summary>
        /// Method that adds the attachments in byte array to the mail message.
        /// </summary> 
        public void AddAttachment(byte[] fileInBytes, string fileName)
        {
            // Convert byte array to bytes stream
            var fileInByteStream = new MemoryStream(fileInBytes);
            AddAttachment(fileInByteStream, fileName);
        }

        /// <summary>
        /// Method that adds the attachments in bytes stream to the mail message.
        /// </summary>
        public void AddAttachment(MemoryStream fileInByteStream, string fileName)
        {
            Attachment attachment = new Attachment(fileInByteStream, fileName);
            AddAttachment(attachment);
        }

        public void AddAttachment(Attachment attachment)
        {
            var attempts = 0;
            while (true)
            {
                try
                {
                    Message.Attachments.Add(attachment);
                    break;
                }
                catch (Exception e)
                {
                    attempts++;
                    if (attempts >= _maxAttempts)
                    {
                        throw new Exception($"Failed to add attachment {attachment.Name} after multiple attempts due to {e.GetType().Name}", e);
                    }
                }
            }
        }

        /// <summary>
        /// Method that sends the mail message and time the process.
        /// </summary>
        public void Send()
        {
            var stopwatch = new Stopwatch();
            var attempts = 0;
            while (true)
            {
                try
                {
                    stopwatch.Start();
                    this.Client.Send(this.Message);
                    break;
                }
                catch (SmtpException se)
                {
                    attempts++;
                    if (attempts >= _maxAttempts)
                    {
                        throw new Exception($"Failed to send email after multiple attempts due to {se.GetType().Name}. Exception details: {se.Message}", se);
                    }
                }
            }
            stopwatch.Stop();
            TimeSpan timeElapsed = stopwatch.Elapsed;
            Console.WriteLine($"Email sent! Time taken to send email: {timeElapsed.TotalSeconds} seconds");
        }

        public void Dispose() => Dispose(true);

        /// <summary>
        /// Method that dispose the stream.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Client.Dispose();
                    Message.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}