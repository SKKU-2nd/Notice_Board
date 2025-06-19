using System;
using UnityEditor;
using UnityEngine;

public class FirebaseManagerEditor : EditorWindow
{
    private string email = "";
    private string password = "";

    [MenuItem("Tools/Firebase 로그인 테스트")]
    public static void ShowWindow()
    {
        GetWindow<FirebaseManagerEditor>("Firebase 로그인 테스트");
    }

    private void OnGUI()
    {
        GUILayout.Label("Firebase 로그인 테스트", EditorStyles.boldLabel);

        email = EditorGUILayout.TextField("이메일", email);
        password = EditorGUILayout.TextField("비밀번호", password);

        GUILayout.Space(10);

        if (GUILayout.Button("로그인 시도"))
        {
            // 싱글톤 인스턴스 사용
            var manager = FirebaseManager.Instance;
            if (manager != null)
            {
                // 리플렉션으로 private 필드에 값 할당
                var emailField = typeof(FirebaseManager).GetField("_email", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var passwordField = typeof(FirebaseManager).GetField("_password", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                emailField.SetValue(manager, email);
                passwordField.SetValue(manager, password);

                manager.Login();
                Debug.Log("로그인 시도 완료");
            }
            else
            {
                EditorUtility.DisplayDialog("오류", "FirebaseManager 싱글톤 인스턴스를 찾을 수 없습니다.", "확인");
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("계정 생성"))
        {
            var manager = FirebaseManager.Instance;
            if (manager != null)
            {
                manager.CreateAccount(email, password);
                Debug.Log("계정 생성 시도 완료");
            }
            else
            {
                EditorUtility.DisplayDialog("오류", "FirebaseManager 싱글톤 인스턴스를 찾을 수 없습니다.", "확인");
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("무작위 계정 1개 생성"))
        {
            var manager = FirebaseManager.Instance;
            if (manager != null)
            {
                string randomEmail = $"user_{Guid.NewGuid()}@test.com";
                string randomPassword = GenerateRandomPassword(10);

                manager.CreateAccount(randomEmail, randomPassword);
                Debug.Log($"무작위 계정 생성 시도: {randomEmail} / {randomPassword}");
            }
            else
            {
                EditorUtility.DisplayDialog("오류", "FirebaseManager 싱글톤 인스턴스를 찾을 수 없습니다.", "확인");
            }
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
}
