using System.Text.RegularExpressions;

public class AccountNicknameSpecification : ISpecification<string>
{
    private static readonly string[] ForbiddenWords = { "¹Ùº¸", "¸ÛÃ»ÀÌ" };
    public string ErrorMessage { get; private set; }

    // ´Ğ³×ÀÓ: ÇÑ±Û ¶Ç´Â ¿µ¾î·Î ±¸¼º, 2~7ÀÚ
    //private static readonly Regex NicknameRegex = new Regex(@"^[°¡-ÆRa-zA-Z]{2,7}$", RegexOptions.Compiled);


    public bool IsSatisfiedBy(string nickname)
    {
        return !string.IsNullOrEmpty(nickname)
            && nickname.Length >= 2 && nickname.Length <= 7
            && Regex.IsMatch(nickname, @"^[a-zA-Z°¡-ÆR]+$")
            && !ContainsForbiddenWord(nickname);
    }

    public string GetErrorMessage(string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
            return "Nickname cannot be null or empty";
        if (nickname.Length < 2 || nickname.Length > 7)
            return "Nickname must be between 2 and 7 characters long";
        if (!Regex.IsMatch(nickname, @"^[a-zA-Z°¡-ÆR]+$"))
            return "Nickname must contain only Korean or English letters";
        if (ContainsForbiddenWord(nickname))
            return "Nickname contains inappropriate words";
        return string.Empty;
    }

    private bool ContainsForbiddenWord(string nickname)
    {
        string lower = nickname.ToLower();
        foreach (var word in ForbiddenWords)
        {
            if (lower.Contains(word))
                return true;
        }
        return false;
    }

    //public bool IsSatisfiedBy(string value)
    //{
    //    if (string.IsNullOrEmpty(value))
    //    {
    //        ErrorMessage = "´Ğ³×ÀÓÀº ºñ¾îÀÖÀ» ¼ö ¾ø½À´Ï´Ù.";
    //        return false;
    //    }

    //    if (!NicknameRegex.IsMatch(value))
    //    {
    //        ErrorMessage = "´Ğ³×ÀÓÀº 2ÀÚ ÀÌ»ó 7ÀÚ ÀÌÇÏÀÇ ÇÑ±Û ¶Ç´Â ¿µ¹®ÀÌ¾î¾ß ÇÕ´Ï´Ù.";
    //        return false;
    //    }

    //    foreach (var forbidden in ForbiddenNicknames)
    //    {
    //        if (value.Contains(forbidden))
    //        {
    //            ErrorMessage = $"´Ğ³×ÀÓ¿¡ ºÎÀûÀıÇÑ ´Ü¾î°¡ Æ÷ÇÔµÇ¾î ÀÖ½À´Ï´Ù: {forbidden}";
    //            return false;
    //        }
    //    }


    //    return true;
    //}
}