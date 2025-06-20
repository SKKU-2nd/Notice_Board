using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;

public class StorageTestEditor : EditorWindow
{
    private string firebaseImagePath = "images/test.jpg";   // Storage 내 경로
    private string selectedLocalFilePath = "";              // 로컬 파일 경로
    private Sprite previewSprite;                           // 다운로드된 Sprite 미리보기용

    [MenuItem("Tools/Storage 기능 테스트")]
    public static void ShowWindow()
    {
        GetWindow<StorageTestEditor>("Storage 기능 테스트");
    }

    private void OnGUI()
    {
        GUILayout.Label("Storage 기능 테스트", EditorStyles.boldLabel);

        GUILayout.Space(10);
        firebaseImagePath = EditorGUILayout.TextField("Firebase 이미지 경로", firebaseImagePath);

        if (GUILayout.Button("로컬 이미지 선택"))
        {
            selectedLocalFilePath = StorageManger.Instance.FileOpen();
            if (!string.IsNullOrEmpty(selectedLocalFilePath))
                Debug.Log($"선택된 파일: {selectedLocalFilePath}");
            else
                Debug.LogWarning("파일 선택 취소됨");
        }

        // ✅ 에디터에 로컬 이미지 경로 표시
        if (!string.IsNullOrEmpty(selectedLocalFilePath))
        {
            EditorGUILayout.HelpBox($"선택된 파일 경로:\n{selectedLocalFilePath}", MessageType.Info);
        }

        if (GUILayout.Button("업로드"))
        {
            if (!string.IsNullOrEmpty(selectedLocalFilePath))
            {
                RunAsync(() => StorageManger.Instance.UploadImage(selectedLocalFilePath));
            }
            else
            {
                Debug.LogWarning("먼저 로컬 파일을 선택하세요.");
            }
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Download URL 가져오기"))
        {
            RunAsync(async () =>
            {
                var url = await StorageManger.Instance.GetDownloadUrl(firebaseImagePath);
                Debug.Log($"Download URL: {url}");
            });
        }

        if (GUILayout.Button("이미지 다운로드 및 Sprite 생성"))
        {
            RunAsync(async () =>
            {
                var url = await StorageManger.Instance.GetDownloadUrl(firebaseImagePath);
                var sprite = await StorageManger.Instance.DownloadSprite(url);
                if (sprite != null)
                {
                    previewSprite = sprite; // ✅ 미리보기용 저장
                    Debug.Log($"Sprite 생성 완료 - Size: {sprite.texture.width} x {sprite.texture.height}");
                }
                else
                {
                    Debug.LogError("Sprite 생성 실패");
                }
            });
        }

        // ✅ Sprite.texture로 이미지 미리보기
        if (previewSprite != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("다운로드된 이미지 미리보기", EditorStyles.boldLabel);

            Rect previewRect = GUILayoutUtility.GetRect(256, 256, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
            Texture2D tex = previewSprite.texture;

            if (tex != null)
            {
                GUI.DrawTexture(previewRect, tex, ScaleMode.ScaleToFit);
            }
        }
    }

    private async void RunAsync(System.Func<Task> taskFunc)
    {
        try
        {
            await taskFunc();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"에러 발생: {ex.Message}");
        }
    }
}
