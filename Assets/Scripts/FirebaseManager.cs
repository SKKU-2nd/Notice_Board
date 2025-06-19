using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseManager : MonoSingleton<FirebaseManager>
{
    [SerializeField]
    private string _email;
    [SerializeField]
    private string _password;

    private FirebaseAuth _auth;
    private FirebaseFirestore _db;

    protected override void Awake()
    {
        base.Awake();
        // Firebase 의존성 확인 및 초기화  
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase dependencies are available.");
                _auth = FirebaseAuth.DefaultInstance;
                _db = FirebaseFirestore.DefaultInstance;
            }
            else
            {
                Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }

    public void Login()
    {
        // Firebase 인증이 초기화되지 않은 경우 에러 출력
        if (_auth == null)
        {
            Debug.LogError("Firebase 인증이 초기화되지 않았습니다.");
            return;
        }

        // 이메일과 비밀번호로 로그인 시도
        _auth.SignInWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(task =>
        {
            // 로그인 작업이 취소된 경우
            if (task.IsCanceled)
            {
                Debug.LogError("이메일/비밀번호 로그인 작업이 취소되었습니다.");
                return;
            }
            // 로그인 작업 중 오류가 발생한 경우
            if (task.IsFaulted)
            {
                Debug.LogError($"이메일/비밀번호 로그인 중 오류 발생: {task.Exception}");
                return;
            }

            // 로그인 성공 시 사용자 정보 출력
            FirebaseUser newUser = task.Result.User;
            Debug.LogFormat("로그인 성공: {0} ({1})", newUser.DisplayName, newUser.UserId);
        });
    }


    //계정 생성
    public void CreateAccount(string email = null, string password = null)
    {
        if (_auth == null)
        {
            Debug.LogError("Firebase 인증이 초기화되지 않았습니다.");
            return;
        }
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("이메일 또는 비밀번호가 비어 있습니다.");
            return;
        }

        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("계정 생성 작업이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"계정 생성 중 오류 발생: {task.Exception}");
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.LogFormat("계정 생성 성공: {0} ({1})", newUser.Email, newUser.UserId);
        });
    }

    public void SignOut()
    {
        if (_auth == null)
        {
            Debug.LogError("Firebase 인증이 초기화되지 않았습니다.");
            return;
        }
        _auth.SignOut();
        Debug.Log("사용자가 로그아웃되었습니다.");
    }

    //로그인 성공 시 유저 정보 저장 (UserId, 닉네임)

}
