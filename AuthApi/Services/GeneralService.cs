namespace AuthApi.Services
{
    public class GeneralService
    {
        public static bool IsValidEmail(string str)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(str);
                return addr.Address == str;
            }
            catch
            {
                return false;
            }
        }
    }
}
