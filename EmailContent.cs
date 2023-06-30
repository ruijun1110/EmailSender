using Microsoft.Extensions.Configuration;

namespace EmailTool
{
    /// <summary>
    /// Class that represents the email content object.
    /// </summary>
    public class EmailContent
    {
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public bool IsBodyHtml { get; private set; }
        public bool DraftOnly { get; private set; }
        public List<EmailFile> Attachments { get; private set; }

        /// <summary>
        /// Method that configures the contents of the email. 
        /// </summary>
        public void SetEmailContent(IConfiguration configReader)
        {
            this.Subject = configReader.GetValue<string>("Email:Subject");
            this.Body = configReader.GetValue<string>("Email:Body");
            this.IsBodyHtml = configReader.GetValue<bool>("Email:IsBodyHtml");
            this.DraftOnly = configReader.GetValue<bool>("Email:DraftOnly");
            this.Attachments = configReader.GetSection("Email").GetSection("Attachments").Get<List<EmailFile>>();
            if ((this.Subject == null) || (this.Body == null) || (this.Attachments == null))
            {
                throw new Exception("Email configuration is not complete, some fields are missing. Please check the appsettings.json file.");
            }
        }
    }
}
