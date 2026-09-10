namespace SJBRAssociatesWeb.Helpers;

public static class PhoneUtils
{
    /// <summary>
    /// Strips everything but digits from a display phone number
    /// (e.g. "+91 9701414852" -> "919701414852").
    /// Use the result for wa.me links (no "+") and prefix with "+" for tel: links.
    /// </summary>
    public static string ToDigits(string phone)
        => new(phone.Where(char.IsDigit).ToArray());
}
