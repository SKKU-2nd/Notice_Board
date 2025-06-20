using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class FirebaseManagerEditor : EditorWindow
{
    // 로그인용 입력값
    private string loginEmail = "";
    private string loginPassword = "";

    // 계정 생성용 입력값
    private string signupNickname = "";
    private string signupEmail = "";
    private string signupPassword = "";
    private string signupPasswordConfirm = "";

    [MenuItem("Tools/Firebase 로그인 테스트")]
    public static void ShowWindow()
    {
        GetWindow<FirebaseManagerEditor>("Firebase 로그인 테스트");
    }

    private void OnGUI()
    {
        GUILayout.Label("Firebase 로그인 테스트", EditorStyles.boldLabel);

        loginEmail = EditorGUILayout.TextField("이메일", loginEmail);
        loginPassword = EditorGUILayout.TextField("비밀번호", loginPassword);

        GUILayout.Space(10);

        if (GUILayout.Button("로그인 시도"))
        {
            var manager = AccountManager.Instance;
            if (manager != null)
            {
                // async 람다로 비동기 처리
                _ = LoginAsync(manager, loginEmail, loginPassword);
            }
            else
            {
                EditorUtility.DisplayDialog("오류", "FirebaseManager 싱글톤 인스턴스를 찾을 수 없습니다.", "확인");
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("로그아웃 시도"))
        {
            var manager = AccountManager.Instance;
            if (manager != null)
            {
                manager.SignOut();
                Debug.Log("로그아웃 완료");
            }
            else
            {
                EditorUtility.DisplayDialog("오류", "FirebaseManager 싱글톤 인스턴스를 찾을 수 없습니다.", "확인");
            }
        }

        GUILayout.Space(10);

        GUILayout.Label("사용자 커스텀 계정 생성", EditorStyles.boldLabel);
        signupNickname = EditorGUILayout.TextField("닉네임", signupNickname);
        signupEmail = EditorGUILayout.TextField("이메일", signupEmail);
        signupPassword = EditorGUILayout.TextField("비밀번호", signupPassword);
        signupPasswordConfirm = EditorGUILayout.TextField("비밀번호 확인", signupPasswordConfirm);
        if (GUILayout.Button("계정 생성"))
        {
            if (signupPassword != signupPasswordConfirm)
            {
                EditorUtility.DisplayDialog("오류", "비밀번호와 비밀번호 확인이 일치하지 않습니다.", "확인");
                return;
            }

            _ = CreateAccountAsync(signupEmail, signupPassword, signupNickname);
        }

        GUILayout.Space(10);

        GUILayout.Label("분신술", EditorStyles.boldLabel);
        if (GUILayout.Button("무작위 계정 1개 생성"))
        {
            string randomId = GenerateRandomString(4);
            string randomEmail = $"user{randomId}@test.com";
            string randomPassword = GenerateRandomPassword(10);
            string randomNickname = GenerateRandomNickname();

            _ = CreateAccountAsync(randomEmail, randomPassword, randomNickname);
        }

        GUILayout.Space(10);
    }

    // 비동기 로그인
    private async Task LoginAsync(AccountManager manager, string email, string password)
    {
        try
        {
            await manager.LoginAsync(email, password);
            Debug.Log("로그인 시도 완료");
        }
        catch (Exception ex)
        {
            EditorUtility.DisplayDialog("로그인 오류", ex.Message, "확인");
            Debug.LogError(ex);
        }
    }

    // 비동기 계정 생성
    private async Task CreateAccountAsync(string email, string password, string nickname)
    {
        try
        {
            await AccountManager.Instance.CreateAccountAsync(email, password, nickname);
            Debug.Log($"계정 생성 성공: {email} / {nickname}");
        }
        catch (Exception ex)
        {
            EditorUtility.DisplayDialog("계정 생성 오류", ex.Message, "확인");
            Debug.LogError(ex);
        }
    }

    // 임의의 10자리 비밀번호 생성 함수
    private string GenerateRandomPassword(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new System.Random();
        char[] buffer = new char[length];
        for (int i = 0; i < length; i++)
        {
            buffer[i] = chars[random.Next(chars.Length)];
        }
        return new string(buffer);
    }

    private string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var random = new System.Random();
        char[] buffer = new char[length];
        for (int i = 0; i < length; i++)
        {
            buffer[i] = chars[random.Next(chars.Length)];
        }
        return new string(buffer);
    }

    // 무작위 닉네임 생성 함수 (예시: Nick_XXXX)
    private string GenerateRandomNickname()
    {
        return $"Nick{GenerateRandomString(3)}";
    }
}
