public class Account 
{
    public readonly string Email;
    public readonly string Nickname;
    public readonly string Password;

    private static readonly string[] ForbiddenWords = { "바보", "멍청이" };


    public Account(string email, string nickname, string password)
    {
        //규칙을 객체로 캡슐화?하여 분리하기
        //DDD 명세
        // 명세 객체 생성
        var emailSpec = new AccountEmailSpecification();
        var nicknameSpec = new AccountNicknameSpecification();
        var passwordSpec = new AccountPasswordSpecification();

        // 이메일 검증
        if (!emailSpec.IsSatisfiedBy(email))
        {
            throw new System.ArgumentException(emailSpec.GetErrorMessage(email), nameof(email));
        }

        // 닉네임 검증
        if (!nicknameSpec.IsSatisfiedBy(nickname))
            throw new System.ArgumentException(nicknameSpec.GetErrorMessage(nickname), nameof(nickname));

        // 비밀번호 검증
        if (!passwordSpec.IsSatisfiedBy(password))
            throw new System.ArgumentException(passwordSpec.GetErrorMessage(password), nameof(password));

        Email = email;
        Nickname = nickname;
        Password = password;
    }

    public AccountDTO ToDTO()
    {
        return new AccountDTO(Email, Nickname, Password);
    }
}
