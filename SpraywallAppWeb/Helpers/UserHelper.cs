namespace SpraywallAppWeb.Helpers;


public static class UserHelper
{
    // Validate an email address
    // TODO: Confirm email exists (external service?)
    internal static bool IsValidEmail(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith("."))
        {
            return false;
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch
        {
            return false;
        }
    }
}