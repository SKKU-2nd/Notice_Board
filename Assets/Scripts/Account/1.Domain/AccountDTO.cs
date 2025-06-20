using Firebase.Firestore;

[FirestoreData]
public class AccountDTO
{
    [FirestoreProperty]
    public string Email { get; set; }
    [FirestoreProperty]
    public string Nickname { get; set; }
    [FirestoreProperty]
    public string Password { get; set; }
    [FirestoreProperty]
    public string ProfilePath { get; set; }

    public AccountDTO() { }

    public AccountDTO(string email, string nickname, string password, string profilePath)
    {
        Email = email;
        Nickname = nickname;
        Password = password;
        ProfilePath = profilePath;
    }

    // Account 도메인 객체를 DTO로 변환하는 생성자
    public AccountDTO(Account account)
    {
        Email = account.Email;
        Nickname = account.Nickname;
        Password = account.Password;
        ProfilePath = account.ProfilePath;
    }

    // DTO를 Account 도메인 객체로 변환하는 메서드
    public Account ToDomain()
    {
        return new Account(Email, Nickname, Password, ProfilePath);
    }
}
