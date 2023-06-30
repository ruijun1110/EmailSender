namespace EmailTool
{
    /// <summary>
    /// Class that is the entry point of the program.
    /// </summary>
    class Program
    {
        public static void Main(String[] args)
        {
            var sender = new Sender();
            var recipients = new Recipients();
            var emailContent = new EmailContent();
            var configuration = new ReadConfig(sender, recipients, emailContent);
            if (emailContent.DraftOnly)
            {
                // Dispose the Draft object after use.
                using(var draft = new Draft())
                {
                    // Build connection with the email client, construct and save the email draft.
                    draft.SetConnection(sender);
                    draft.AddAttachment(emailContent);
                    draft.SetDraft(emailContent, sender, recipients);
                    draft.SaveAndDisconnect();
                }
            }
            else
            {
                // Dispose the Email object after use.
                using (var email = new Email())
                {
                    email.SetClient(sender);
                    email.SetMessage(emailContent, sender, recipients);
                    // Use when there is attachment generated from local file.
                    email.AddAttachment(emailContent);
                    // Use when there is attachment generated from byte array.
                    // this.Email.AddAttachment(byte[], "filename.extension");
                    // Use when there is attachment generated from stream.
                    // this.Email.AddAttachment(stream, "filename.extension");
                    email.Send();
                }
            }
        }
    }
}