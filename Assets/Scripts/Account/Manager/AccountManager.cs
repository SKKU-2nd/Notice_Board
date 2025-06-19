using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

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

    private AccountRepository _repo = new AccountRepository();


    protected override void Awake()
    {
        base.Awake();
    }


    public void Login()
    {
        _repo.SignIn(_email, _password,
            account => {
                MyAccount = account;
                Debug.Log("로그인 및 MyAccount 할당 완료");
            },
            error => {
                Debug.LogError(error);
            });
    }

    public void CreateAccount(string email = null, string password = null)
    {
        _repo.CreateAccount(email, password,
            account => {
                Debug.Log("계정 생성 및 저장 완료");
            },
            error => {
                Debug.LogError(error);
            });
    }

    public void SignOut()
    {
        _repo.SignOut();
        Debug.Log("로그아웃 및 MyAccount 초기화 완료");
    }
}
