using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System;

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

    public async Task LoginAsync(string email, string password)
    {
        var accountDto = new AccountDTO { Email = email, Password = password };
        try
        {
            var result = await _repo.SignInAsync(accountDto, email, password);
            MyAccount = result;
            Debug.Log("로그인 및 MyAccount 할당 완료");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"로그인 실패: {ex.Message}");
            throw new Exception($"로그인 실패: {ex.Message}");
        }
    }

    public async Task CreateAccountAsync(string email, string password, string nickname)
    {
        try
        {
            var result = await _repo.CreateAccountAsync(email, nickname, password);
            MyAccount = result;
            Debug.Log("계정 생성 및 MyAccount 할당 완료");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"계정 생성 실패: {ex.Message}");
            throw new Exception($"계정 생성 실패: {ex.Message}");
        }
    }

    public async Task CreateAccountAsync(string email, string password)
    {
        try
        {
            var result = await _repo.CreateAccountAsync(email, password);
            MyAccount = result?.ToDTO();
            Debug.Log("계정 생성 및 MyAccount 할당 완료");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"계정 생성 실패: {ex.Message}");
        }
    }

    public void SignOut()
    {
        _repo.SignOut(MyAccount);
        Debug.Log("로그아웃 및 MyAccount 초기화 완료");
    }

    public async Task<AccountDTO> GetAccountDTOByEmailAsync(string email)
    {
        try
        {
            var dto = await _repo.GetAccountDTOByEmail(email);
            if (dto == null)
            {
                Debug.LogWarning("해당 이메일의 계정 정보를 찾을 수 없습니다.");
            }
            return dto;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"계정 정보 조회 실패: {ex.Message}");
            return null;
        }
    }

}
