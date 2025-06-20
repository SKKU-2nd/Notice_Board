using UnityEngine;
public class Account 
{
    public readonly string AccountID; //고유값
    public readonly string Email; 
    public readonly string Nickname;
    public readonly string Password;
    public readonly string ProfilePath;

    public Account(string email, string nickname, string password, string profilepath = null)
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
            Debug.LogError(emailSpec.GetErrorMessage(email));
            return;
        }

        // 닉네임 검증
        if (!nicknameSpec.IsSatisfiedBy(nickname))
        {
            Debug.LogError(nicknameSpec.GetErrorMessage(nickname));
            return;
        }

        // 비밀번호 검증
        if (!passwordSpec.IsSatisfiedBy(password))
        {
            Debug.LogError(passwordSpec.GetErrorMessage(password));
            return;
        }

        Email = email;
        Nickname = nickname;
        Password = password;
        ProfilePath = profilepath;
    }

    public AccountDTO ToDTO()
    {
        return new AccountDTO(Email, Nickname, Password, ProfilePath);
    }
}
