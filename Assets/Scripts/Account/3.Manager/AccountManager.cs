using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using TMPro;

public class AccountManager : MonoSingleton<AccountManager>
{
    private Account _myAccount;
    public AccountDTO MyAccount
    {
        get { return _myAccount?.ToDTO(); }
        private set
        {
            if (value != null)
                _myAccount = value.ToDomain();
            else
                _myAccount = null;
        }
    }
    private AccountRepository _repo = new AccountRepository();

    protected override void Awake()
    {
        base.Awake();
    }

    public void Login(string email, string password)
    {
        var accountDto = new AccountDTO { Email = email, Password = password };
        _repo.SignIn(accountDto, email, password,
            account =>
            {
                MyAccount = account; // ToDTO() 호출 필요 없음
                Debug.Log("계정 생성 및 MyAccount 할당 완료");
            },
            error =>
            {
                Debug.LogError(error);
            });
    }

    public void CreateAccount(string email, string password, string nickname)
    {
        _repo.CreateAccount(email, nickname, password,
            account =>
            {
                MyAccount = account; // ToDTO() 호출 필요 없음
                Debug.Log("계정 생성 및 MyAccount 할당 완료");
            },
            error =>
            {
                Debug.LogError(error);
            });
    }

    public void SignOut()
    {
        _repo.SignOut(MyAccount);
        Debug.Log("로그아웃 및 MyAccount 초기화 완료");
    }
}
