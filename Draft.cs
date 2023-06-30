using MailKit.Net.Imap;
using MimeKit;
using MailKit.Security;
using MailKit;

namespace EmailTool
{
    class Draft : IDisposable
    {
        private bool _disposedValue;
        public ImapClient Client { get; private set; }
        public MimeMessage DraftEmail { get; private set; }
        public IMailFolder DraftFolder { get; private set; }
        public Multipart DraftBody { get; private set; }


        public Draft()
        {
            this.Client = new ImapClient();
            this.DraftEmail = new MimeMessage();
            this.DraftBody = new Multipart("mixed");
        }

        /// <summary>
        /// Method that establish connection with the email client using IMAP.
        /// </summary>
        public void SetConnection(Sender sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("Sender object is null. SetClient function failed.");
            }
            this.Client.Connect(sender.Host, sender.Port, SecureSocketOptions.SslOnConnect);
            this.Client.Authenticate(sender.Email, sender.Token);
        }

        /// <summary>
        /// Method that set up the parameters of the email draft body.
        /// </summary>
        public void SetDraft(EmailContent emailContent, Sender sender, Recipients recipients)
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
            this.DraftEmail.From.Add(new MailboxAddress(sender.Name, sender.Email));
            this.DraftEmail.Subject = emailContent.Subject;
            var textBody = new TextPart("plain") { Text = emailContent.Body };
            this.DraftBody.Add(textBody);
            foreach (string toEmail in recipients.ToEmails)
            {
                this.DraftEmail.To.Add(new MailboxAddress("", toEmail));
            }
            foreach (string ccEmail in recipients.CcEmails)
            {
                this.DraftEmail.Cc.Add(new MailboxAddress("", ccEmail));
            }
            foreach (string bccEmail in recipients.BccEmails)
            {
                this.DraftEmail.Bcc.Add(new MailboxAddress("", bccEmail));
            }
            DraftEmail.Body = this.DraftBody;
        }

        /// <summary>
        /// Method that add local file attachment to the email draft.
        /// </summary>
        public void AddAttachment(EmailContent emailContent)
        {
            if (emailContent == null)
            {
                throw new ArgumentNullException("Email object is null. AddAttachment function failed.");
            }
            foreach (EmailFile file in emailContent.Attachments)
            {
                var attachment = new MimePart("application", "octet-stream")
                {
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = file.Name
                };
                try
                {
                    attachment.Content = new MimeContent(System.IO.File.OpenRead(file.Path));
                }
                catch (System.IO.FileNotFoundException e)
                {
                    throw new System.IO.FileNotFoundException("Attachment path is invalid. AddAttachment function failed. Please check the appsettings.json file.", e);
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    throw new System.IO.DirectoryNotFoundException("Attachment directory is invalid. AddAttachment function failed. Please check the appsettings.json file.", e);
                }
                catch (System.IO.IOException e)
                {
                    throw new System.IO.IOException("Failed to open file. AddAttachment function failed. Please check the appsettings.json file.", e);
                }
                this.DraftBody.Add(attachment);
            }
        }

        /// <summary>
        /// Method that add byte array attachment to the email draft.
        /// </summary>
        public void AddAttachment(byte[] fileInBytes, string fileName)
        {
            // Convert byte array to bytes stream
            var fileInByteStream = new MemoryStream(fileInBytes);
            AddAttachment(fileInByteStream, fileName);
        }

        /// <summary>
        /// Method that add byte stream attachment to the email draft.
        /// </summary>
        public void AddAttachment(MemoryStream fileInByteStream, string fileName)
        {
            var attachment = new MimePart("application", "octet-stream")
            {
                Content = new MimeContent(fileInByteStream),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                FileName = fileName
            };
            this.DraftBody.Add(attachment);
        }

        /// <summary>
        /// Method that save the email draft to the Drafts folder and disconnect from the email client.
        /// </summary>
        public void SaveAndDisconnect()
        {
            this.DraftFolder = this.Client.GetFolder("Drafts");
            this.DraftFolder.Open(FolderAccess.ReadWrite);
            this.DraftFolder.Append(this.DraftEmail);
            this.Client.Disconnect(true);
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
                    DraftEmail.Dispose();
                    DraftBody.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}