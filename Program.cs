namespace EmailTool
{
    /// <summary>
    /// Class that is the entry point of the program.
    /// </summary>
    class Program
    {
        public static void Main(String[] args)
        {
            // Dispose the EmailSender object after use.
            using(var newEmail = new EmailSender()){
                var helper = new EmailUtil(newEmail);
                helper.SetupEmail();
                helper.SendEmail();
            }
        }
    }
}