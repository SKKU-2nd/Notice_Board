using UnityEngine;
using System;
using System.Threading.Tasks;

public class AccountManager : MonoSingleton<AccountManager>
{
    private string _email;
    private string _password;

    private Account _myAccount;
    public Account MyAccount
    {
        get { return _myAccount; }
        private set { _myAccount = value; }
    }

    private AccountDTO _myAccountDto = new AccountDTO();

    public AccountDTO MyAccountDto
    {
        get { return _myAccountDto; }
        private set { _myAccountDto = value; }
    }

    private AccountRepository _repo = new AccountRepository();


    protected override void Awake()
    {
        base.Awake();
    }

    //테스트용
    public void Login()
    {
        _repo.SignIn(_myAccountDto, _email, _password,
            account =>
            {
                MyAccountDto = account;
                Debug.Log("로그인 및 MyAccount 할당 완료");
            },
            error =>
            {
                Debug.LogError(error);
            });
    }

    public void Login(string email, string password)
    {
        _repo.SignIn(_myAccountDto, email, password,
            dto =>
            {
                MyAccountDto = dto;
                Debug.Log("로그인 및 MyAccountDto 할당 완료");
            },
            error =>
            {
                Debug.LogError(error);
            });
    }

    public void CreateAccount(string email, string password, string nickname)
    {
        _repo.CreateAccount(email, nickname, password,
            dto =>
            {
                MyAccountDto = dto;
                Debug.Log("계정 생성 및 MyAccountDto 할당 완료");
            },
            error =>
            {
                Debug.LogError(error);
            });
    }



    public void SignOut()
    {
        _repo.SignOut(_myAccountDto);
        Debug.Log("로그아웃 및 MyAccountDto 초기화 완료");
    }

    public void ChangeNickname(string newNickname)
    {
        _repo.ChangeNickname(_myAccountDto, newNickname,
            () => Debug.Log("닉네임 변경 성공"),
            error => Debug.LogError($"닉네임 변경 실패: {error}")
        );
    }

    //Email로 유저 DTO 가져오기
    public async Task<AccountDTO> GetAccountDTOByEmail(string email)
    {
        return await _repo.GetAccountDTOByEmail(email);
    }

}
