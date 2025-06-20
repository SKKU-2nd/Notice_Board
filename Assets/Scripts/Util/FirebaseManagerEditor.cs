using Firebase.Extensions;
using System;
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
            // 싱글톤 인스턴스 사용
            var manager = AccountManager.Instance;
            if (manager != null)
            {
                // 리플렉션으로 private 필드에 값 할당
                var emailField = typeof(AccountManager).GetField("_email", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var passwordField = typeof(AccountManager).GetField("_password", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                emailField.SetValue(manager, loginEmail);
                passwordField.SetValue(manager, loginPassword);

                manager.Login(loginEmail, loginPassword);
                Debug.Log("로그인 시도 완료");
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

            var repo = new AccountRepository();
            repo.CreateAccount(signupEmail, signupNickname, signupPassword,
                account => Debug.Log("계정 생성 성공: " + account.Email),
                error => Debug.LogError(error));
        }

        GUILayout.Space(10);

        GUILayout.Label("분신술", EditorStyles.boldLabel);
        if (GUILayout.Button("무작위 계정 1개 생성"))
        {
            var repo = new AccountRepository();
            string randomId = GenerateRandomString(4);
            string randomEmail = $"user{randomId}@test.com";
            string randomPassword = GenerateRandomPassword(10);
            string randomNickname = GenerateRandomNickname();

            repo.CreateAccount(randomEmail, randomNickname, randomPassword,
                account => Debug.Log($"무작위 계정 생성 성공: {account.Email} / {account.Nickname}"),
                error => Debug.LogError(error));
        }

        GUILayout.Space(10);

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
