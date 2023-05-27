using Microsoft.Extensions.Configuration;

namespace EmailTool
{
    /// <summary>
    /// Class that represents the email recipients object.
    /// </summary>
    public class Recipients
    {
        public List<String> ToEmails { get; private set; }
        public List<String> CcEmails { get; private set; }
        public List<String> BccEmails { get; private set; }

        /// <summary>
        /// Method that configures the lists of recipients of the email.
        /// </summary>
        public void SetRecipients(IConfiguration configReader)
        {
            var toEmails = configReader.GetSection("Recipients").GetSection("To").Get<List<string>>();
            var ccEmails = configReader.GetSection("Recipients").GetSection("Cc").Get<List<string>>();
            var bccEmails = configReader.GetSection("Recipients").GetSection("Bcc").Get<List<string>>();
            this.ToEmails = toEmails;
            this.CcEmails = ccEmails;
            this.BccEmails = bccEmails;
            if ((this.ToEmails == null) || (this.CcEmails == null) || (this.BccEmails == null))
            {
                throw new Exception("Receiver configuration is not complete, some fields are missing. Please check the appsettings.json file.");
            }
        }
    }
}
