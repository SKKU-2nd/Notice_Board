public class AccountDTO
{
    public string Email { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }

    public AccountDTO() { }

    public AccountDTO(string email, string nickname, string password)
    {
        Email = email;
        Nickname = nickname;
        Password = password;
    }

    // Account 도메인 객체를 DTO로 변환하는 생성자
    public AccountDTO(Account account)
    {
        Email = account.Email;
        Nickname = account.Nickname;
        Password = account.Password;
    }

    // DTO를 Account 도메인 객체로 변환하는 메서드
    public Account ToDomain()
    {
        return new Account(Email, Nickname, Password);
    }
}
