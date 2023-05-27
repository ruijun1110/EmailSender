using Microsoft.Extensions.Configuration;

namespace EmailTool
{
    /// <summary>
    /// Class that represents the email sender object.
    /// </summary>
    public class Sender
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Token { get; private set; }
        public string Host { get; private set; }

        // Port can use 587 or 25, do not use 465.
        public int Port { get; private set; }

        /// <summary>
        /// Method that configures the sender of the email. 
        /// </summary>
        public void SetSender(IConfiguration configReader)
        {
            this.Name = configReader.GetValue<string>("Sender:Name");
            this.Email = configReader.GetValue<string>("Sender:Email");
            this.Token = configReader.GetValue<string>("Sender:Token");
            this.Host = configReader.GetValue<string>("Sender:Host");
            this.Port = configReader.GetValue<int>("Sender:Port");
            if ((this.Name == null) || (this.Email == null) || (this.Token == null) || (this.Host == null) || (this.Port == 0))
            {
                throw new Exception("Sender configuration is not complete, some fields are missing. Please check the appsettings.json file.");
            }
        }
    }
}