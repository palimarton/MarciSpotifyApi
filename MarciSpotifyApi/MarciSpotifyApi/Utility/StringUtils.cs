namespace MarciSpotifyApi.Utility;

public class StringUtils
{
    private static readonly Random random = new();

    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string ComputeSHA256String(string source)
    {
        return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(source)))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
