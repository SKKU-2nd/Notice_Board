using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using UnityEngine;

public class AccountRepository 
{
    private Account _myAccount;
    public Account MyAccount
    {
        get { return _myAccount; }
        private set { _myAccount = value; }
    }

    private FirebaseAuth Auth => FirebaseManager.Instance.Auth;
    private FirebaseFirestore DB => FirebaseManager.Instance.DB;

    // 로그인
    public void Login(string email, string password, Action<Account> onSuccess = null, Action<string> onError = null)
    {
        if (Auth == null)
        {
            onError?.Invoke("Firebase 인증이 초기화되지 않았습니다.");
            Debug.LogError("Firebase 인증이 초기화되지 않았습니다.");
            return;
        }

        Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                onError?.Invoke("이메일/비밀번호 로그인 작업이 취소되었습니다.");
                Debug.LogError("이메일/비밀번호 로그인 작업이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                onError?.Invoke($"이메일/비밀번호 로그인 중 오류 발생: {task.Exception}");
                Debug.LogError($"이메일/비밀번호 로그인 중 오류 발생: {task.Exception}");
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.LogFormat("로그인 성공: {0} ({1})", newUser.DisplayName, newUser.UserId);

            // Firestore에서 유저 데이터 가져오기 (이메일을 문서 ID로 사용)
            DB.Collection("UserDB").Document(email).GetSnapshotAsync().ContinueWithOnMainThread(snapshotTask =>
            {
                if (snapshotTask.IsCanceled)
                {
                    onError?.Invoke("유저 정보 조회 작업이 취소되었습니다.");
                    Debug.LogError("유저 정보 조회 작업이 취소되었습니다.");
                    return;
                }
                if (snapshotTask.IsFaulted)
                {
                    onError?.Invoke($"유저 정보 조회 중 오류 발생: {snapshotTask.Exception}");
                    Debug.LogError($"유저 정보 조회 중 오류 발생: {snapshotTask.Exception}");
                    return;
                }

                var snapshot = snapshotTask.Result;
                if (snapshot.Exists)
                {
                    var data = snapshot.ToDictionary();
                    if (data != null)
                    {
                        // 각 필드를 안전하게 꺼내서 DTO에 할당
                        string email = data.ContainsKey("Email") ? data["Email"] as string : null;
                        string nickname = data.ContainsKey("Nickname") ? data["Nickname"] as string : null;
                        string password = data.ContainsKey("Password") ? data["Password"] as string : null;

                        var dto = new AccountDTO(email, nickname, password);
                        var account = dto.ToDomain();
                        _myAccount = account;
                        Debug.Log($"MyAccount 할당 완료: {account.Email}, {account.Nickname}");
                        onSuccess?.Invoke(account);
                    }
                    else
                    {
                        onError?.Invoke("UserDB에서 데이터를 불러오지 못했습니다.");
                        Debug.LogWarning("UserDB에서 데이터를 불러오지 못했습니다.");
                    }
                }
                else
                {
                    onError?.Invoke("해당 유저의 데이터가 UserDB에 존재하지 않습니다.");
                    Debug.LogWarning("해당 유저의 데이터가 UserDB에 존재하지 않습니다.");
                }
            });
        });
    }

    // 계정 생성
    public void CreateAccount(string email, string password, Action<Account> onSuccess = null, Action<string> onError = null)
    {
        if (Auth == null)
        {
            onError?.Invoke("Firebase 인증이 초기화되지 않았습니다.");
            Debug.LogError("Firebase 인증이 초기화되지 않았습니다.");
            return;
        }
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            onError?.Invoke("이메일 또는 비밀번호가 비어 있습니다.");
            Debug.LogError("이메일 또는 비밀번호가 비어 있습니다.");
            return;
        }

        //여기가 변해야함
        string nickname = email.Split('@')[0];

        Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                onError?.Invoke("계정 생성 작업이 취소되었습니다.");
                Debug.LogError("계정 생성 작업이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                onError?.Invoke($"계정 생성 중 오류 발생: {task.Exception}");
                Debug.LogError($"계정 생성 중 오류 발생: {task.Exception}");
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.LogFormat("계정 생성 성공: {0} ({1})", newUser.Email, newUser.UserId);

            try
            {
                var account = new Account(email, nickname, password);
                SaveAccountToUserDB(account, () => onSuccess?.Invoke(account), onError);
            }
            catch (Exception ex)
            {
                onError?.Invoke($"Account 객체 생성 또는 저장 중 오류: {ex}");
                Debug.LogError($"Account 객체 생성 또는 저장 중 오류: {ex}");
            }
        });
    }

    // Firestore에 계정 정보 저장
    public void SaveAccountToUserDB(Account account, Action onSuccess = null, Action<string> onError = null)
    {
        if (DB == null)
        {
            onError?.Invoke("Firestore가 초기화되지 않았습니다.");
            Debug.LogError("Firestore가 초기화되지 않았습니다.");
            return;
        }

        var dto = account.ToDTO();

        DB.Collection("UserDB").Document(dto.Email).SetAsync(dto)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                onError?.Invoke("계정 정보 저장 작업이 취소되었습니다.");
                Debug.LogError("계정 정보 저장 작업이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                onError?.Invoke($"계정 정보 저장 중 오류 발생: {task.Exception}");
                Debug.LogError($"계정 정보 저장 중 오류 발생: {task.Exception}");
                return;
            }
            Debug.Log("계정 정보가 UserDB(Firestore)에 저장되었습니다.");
            onSuccess?.Invoke();
        });
    }
}
