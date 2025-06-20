using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Storage;
using UnityEngine;

public class FirebaseManager : MonoSingleton<FirebaseManager>
{
    private FirebaseAuth _auth;
    private FirebaseFirestore _db;
    private FirebaseStorage _storage;

    public FirebaseAuth Auth => _auth;
    public FirebaseFirestore DB => _db;
    public FirebaseStorage Storage => _storage;

    protected override void Awake()
    {
        base.Awake();
        // Firebase 의존성 확인 및 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Firebase dependencies are available.");
                _auth = FirebaseAuth.DefaultInstance;
                _db = FirebaseFirestore.DefaultInstance;
                _storage= FirebaseStorage.DefaultInstance;
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }
}
