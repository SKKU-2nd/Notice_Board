using System.Text.RegularExpressions;

public class AccountNicknameSpecification : ISpecification<string>
{
    private static readonly string[] ForbiddenWords = { "바보", "멍청이" };
    public string ErrorMessage { get; private set; }

    // 닉네임: 한글 또는 영어로 구성, 2~7자
    //private static readonly Regex NicknameRegex = new Regex(@"^[가-힣a-zA-Z]{2,7}$", RegexOptions.Compiled);


    public bool IsSatisfiedBy(string nickname)
    {
        return !string.IsNullOrEmpty(nickname)
            && nickname.Length >= 2 && nickname.Length <= 7
            && Regex.IsMatch(nickname, @"^[a-zA-Z가-힣]+$")
            && !ContainsForbiddenWord(nickname);
    }

    public string GetErrorMessage(string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
            return "Nickname cannot be null or empty";
        if (nickname.Length < 2 || nickname.Length > 7)
            return "Nickname must be between 2 and 7 characters long";
        if (!Regex.IsMatch(nickname, @"^[a-zA-Z가-힣]+$"))
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
    //        ErrorMessage = "닉네임은 비어있을 수 없습니다.";
    //        return false;
    //    }

    //    if (!NicknameRegex.IsMatch(value))
    //    {
    //        ErrorMessage = "닉네임은 2자 이상 7자 이하의 한글 또는 영문이어야 합니다.";
    //        return false;
    //    }

    //    foreach (var forbidden in ForbiddenNicknames)
    //    {
    //        if (value.Contains(forbidden))
    //        {
    //            ErrorMessage = $"닉네임에 부적절한 단어가 포함되어 있습니다: {forbidden}";
    //            return false;
    //        }
    //    }


    //    return true;
    //}
}