using System.Text.RegularExpressions;

public class AccountNicknameSpecification : ISpecification<string>
{
    private static readonly string[] ForbiddenWords = { "�ٺ�", "��û��" };
    public string ErrorMessage { get; private set; }

    // �г���: �ѱ� �Ǵ� ����� ����, 2~7��
    //private static readonly Regex NicknameRegex = new Regex(@"^[��-�Ra-zA-Z]{2,7}$", RegexOptions.Compiled);


    public bool IsSatisfiedBy(string nickname)
    {
        return !string.IsNullOrEmpty(nickname)
            && nickname.Length >= 2 && nickname.Length <= 7
            && Regex.IsMatch(nickname, @"^[a-zA-Z��-�R]+$")
            && !ContainsForbiddenWord(nickname);
    }

    public string GetErrorMessage(string nickname)
    {
        if (string.IsNullOrEmpty(nickname))
            return "Nickname cannot be null or empty";
        if (nickname.Length < 2 || nickname.Length > 7)
            return "Nickname must be between 2 and 7 characters long";
        if (!Regex.IsMatch(nickname, @"^[a-zA-Z��-�R]+$"))
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
    //        ErrorMessage = "�г����� ������� �� �����ϴ�.";
    //        return false;
    //    }

    //    if (!NicknameRegex.IsMatch(value))
    //    {
    //        ErrorMessage = "�г����� 2�� �̻� 7�� ������ �ѱ� �Ǵ� �����̾�� �մϴ�.";
    //        return false;
    //    }

    //    foreach (var forbidden in ForbiddenNicknames)
    //    {
    //        if (value.Contains(forbidden))
    //        {
    //            ErrorMessage = $"�г��ӿ� �������� �ܾ ���ԵǾ� �ֽ��ϴ�: {forbidden}";
    //            return false;
    //        }
    //    }


    //    return true;
    //}
}