namespace EmailTool
{
    /// <summary>
    /// Class that represents the email utility object, which setup and send the email.
    /// </summary>
    public class EmailUtil
    {
        private EmailSender Email { get; set; }

        public EmailUtil(EmailSender email)
        {
            if (email == null)
            {
                throw new Exception("Email object is null. EmailUtil class cannot be initialized.");
            }
            this.Email = email;
        }

        /// <summary>
        /// Method that helps to configure the different component objects of the email using the configuration file.
        /// And then use the component objects to setup the EmailSender Object.
        /// <Example>
        /// In Program.cs:
        /// <code>
        /// var email = new EmailSender();
        /// var emailUtil = new EmailUtil(email);
        /// emailUtil.SetupEmail();
        /// emailUtil.SendEmail();
        /// </code>
        /// </Example>
        /// </summary>
        public void SetupEmail()
        {
            var sender = new Sender();
            var recipients = new Recipients();
            var emailContent = new EmailContent();
            var configuration = new ReadConfig(sender, recipients, emailContent);
            this.Email.SetClient(sender);
            this.Email.SetMessage(emailContent, sender, recipients);
            // Use when there is attachment generated from local file.
            this.Email.AddAttachment(emailContent);
            // Use when there is attachment generated from byte array.
            // this.Email.AddAttachment(byte[], "filename.extension");
            // Use when there is attachment generated from stream.
            // this.Email.AddAttachment(stream, "filename.extension");
        }

        public void SendEmail()
        {
            this.Email.TimeAndSend();
        }
    }
}