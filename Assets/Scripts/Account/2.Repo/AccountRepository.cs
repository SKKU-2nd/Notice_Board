using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class AccountRepository
{
    private FirebaseAuth Auth => FirebaseManager.Instance.Auth;
    private FirebaseFirestore DB => FirebaseManager.Instance.DB;

    // 로그인
    public async Task<AccountDTO> SignInAsync(AccountDTO myAccountDto, string email, string password)
    {
        if (Auth == null)
            throw new InvalidOperationException("Firebase 인증이 초기화되지 않았습니다.");

        var authResult = await Auth.SignInWithEmailAndPasswordAsync(email, password);
        FirebaseUser newUser = authResult.User;
        Debug.LogFormat("로그인 성공: {0} ({1})", newUser.DisplayName, newUser.UserId);

        var snapshot = await DB.Collection("UserDB").Document(email).GetSnapshotAsync();
        if (!snapshot.Exists)
            throw new Exception("해당 유저의 데이터가 UserDB에 존재하지 않습니다.");

        var data = snapshot.ToDictionary();
        if (data == null)
            throw new Exception("UserDB에서 데이터를 불러오지 못했습니다.");

        string foundEmail = data.ContainsKey("Email") ? data["Email"] as string : null;
        string nickname = data.ContainsKey("Nickname") ? data["Nickname"] as string : null;
        string foundPassword = data.ContainsKey("Password") ? data["Password"] as string : null;

        var dto = new AccountDTO(foundEmail, nickname, foundPassword);
        myAccountDto.Email = dto.Email;
        myAccountDto.Nickname = dto.Nickname;
        myAccountDto.Password = dto.Password;
        Debug.Log($"MyAccount 할당 완료: {dto.Email}, {dto.Nickname}");
        return dto;
    }

    public void SignOut(AccountDTO myAccountDto)
    {
        if (Auth == null)
            throw new InvalidOperationException("Firebase 인증이 초기화되지 않았습니다.");

        Auth.SignOut();
        myAccountDto.Email = null;
        myAccountDto.Nickname = null;
        myAccountDto.Password = null;
        Debug.Log("로그아웃 성공");
    }

    // 계정 생성(닉 없음)
    public async Task<Account> CreateAccountAsync(string email, string password)
    {
        if (Auth == null)
            throw new InvalidOperationException("Firebase 인증이 초기화되지 않았습니다.");
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            throw new ArgumentException("이메일 또는 비밀번호가 비어 있습니다.");

        string nickname = email.Split('@')[0];
        var authResult = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
        FirebaseUser newUser = authResult.User;
        Debug.LogFormat("계정 생성 성공: {0} ({1})", newUser.Email, newUser.UserId);

        var account = new Account(email, nickname, password);
        await SaveAccountToUserDBAsync(account.ToDTO());
        return account;
    }

    // 계정 생성(닉 받음)
    public async Task<AccountDTO> CreateAccountAsync(string email, string nickname, string password)
    {
        if (Auth == null)
            throw new InvalidOperationException("Firebase 인증이 초기화되지 않았습니다.");
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname))
            throw new ArgumentException("이메일, 닉네임 또는 비밀번호가 비어 있습니다.");

        var authResult = await Auth.CreateUserWithEmailAndPasswordAsync(email, password);
        FirebaseUser newUser = authResult.User;
        Debug.LogFormat("계정 생성 성공: {0} ({1})", newUser.Email, newUser.UserId);

        var account = new Account(email, nickname, password);
        var dto = account.ToDTO();
        await SaveAccountToUserDBAsync(dto);
        return dto;
    }

    // Firestore에 계정 정보 저장
    public async Task SaveAccountToUserDBAsync(AccountDTO account)
    {
        if (DB == null)
            throw new InvalidOperationException("Firestore가 초기화되지 않았습니다.");

        await DB.Collection("UserDB").Document(account.Email).SetAsync(account);
        Debug.Log("계정 정보가 UserDB(Firestore)에 저장되었습니다.");
    }

    // 닉네임 변경
    public async Task ChangeNicknameAsync(AccountDTO myAccountDto, string newNickname)
    {
        if (myAccountDto == null || string.IsNullOrEmpty(myAccountDto.Email))
            throw new InvalidOperationException("로그인된 계정이 없습니다.");

        var nicknameSpec = new AccountNicknameSpecification();
        if (!nicknameSpec.IsSatisfiedBy(newNickname))
            throw new ArgumentException(nicknameSpec.GetErrorMessage(newNickname));

        var docRef = DB.Collection("UserDB").Document(myAccountDto.Email);
        await docRef.UpdateAsync("Nickname", newNickname);
        myAccountDto.Nickname = newNickname;
        Debug.Log("닉네임이 성공적으로 변경되었습니다.");
    }

    // Email로 유저 DTO 가져오기
    public async Task<AccountDTO> GetAccountDTOByEmail(string email)
    {
        if (DB == null)
            throw new InvalidOperationException("Firestore가 초기화되지 않았습니다.");

        var snapshot = await DB.Collection("UserDB").Document(email).GetSnapshotAsync();
        if (snapshot.Exists)
        {
            var data = snapshot.ToDictionary();
            string foundEmail = data.ContainsKey("Email") ? data["Email"] as string : null;
            string nickname = data.ContainsKey("Nickname") ? data["Nickname"] as string : null;
            string password = data.ContainsKey("Password") ? data["Password"] as string : null;

            return new AccountDTO(foundEmail, nickname, password);
        }
        else
        {
            Debug.LogWarning("해당 유저의 데이터가 UserDB에 존재하지 않습니다.");
            return null;
        }
    }
}
