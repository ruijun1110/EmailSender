using Microsoft.Extensions.Configuration;

namespace EmailTool
{
    /// <summary>
    /// Class that reads the appsettings.json file and configure the corresponding email components.
    /// </summary>
    public class ReadConfig
    {
        public IConfiguration ConfigReader { get; private set; }

        /// <summary>
        /// Constructor that reads the appsettings.json file and pass the configuration file reader to set up each email component.
        /// </summary>
        public ReadConfig(Sender sender, Recipients recipients, EmailContent emailContent)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("Sender object is null. ReadConfig class cannot be initialized.");
            }
            if (recipients == null)
            {
                throw new ArgumentNullException("Receivers object is null. ReadConfig class cannot be initialized.");
            }
            if (emailContent == null)
            {
                throw new ArgumentNullException("Email object is null. ReadConfig class cannot be initialized.");
            }
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);
            this.ConfigReader = builder.Build();
            sender.SetSender(this.ConfigReader);
            recipients.SetRecipients(this.ConfigReader);
            emailContent.SetEmailContent(this.ConfigReader);
        }
    }
}