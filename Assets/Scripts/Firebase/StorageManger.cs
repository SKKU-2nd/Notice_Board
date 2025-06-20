using UnityEngine;
using System.IO;
using System.Windows.Forms;
using UnityEngine.UI;
using Firebase.Storage;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class StorageManger : MonoSingleton<StorageManger>
{
    private FirebaseStorage _storage;
        
    private OpenFileDialog OpenDialog;
    private Stream openStream = null;
    private StorageReference storageRef;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        OpenDialog = new OpenFileDialog();
        OpenDialog.Filter = "jpg files (*.jpg) |*.jpg|png files (*.png) |*.jpg|All files  (*.*)|*.*";
        OpenDialog.FilterIndex = 3;
        OpenDialog.Title = "Image Dialog";

        storageRef = _storage.GetReferenceFromUrl("gs://noticeboard-60361.firebasestorage.app");
        EnsureStorage();
    }

    public async void LoadImageToUI(string imagePath, Image targetImage)
    {
        Uri downloadUri = await GetDownloadUrl(imagePath);
        if (downloadUri == null)
        {
            Debug.LogError("Download URL is null");
            return;
        }

        StartCoroutine(LoadImageCoroutine(downloadUri, sprite => {
            if (targetImage != null)
                targetImage.sprite = sprite;
        }));
    }

    private async Task<Uri> GetDownloadUrl(string imagePath)
    {
        try
        {
            EnsureStorage();
            return await _storage.GetReference(imagePath).GetDownloadUrlAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"GetDownloadUrl 실패: {e.Message}");
            return null;
        }
    }

    private IEnumerator LoadImageCoroutine(Uri uri, Action<Sprite> onSuccess)
    {
        using var request = UnityWebRequestTexture.GetTexture(uri);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"이미지 다운로드 실패: {request.error}");
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        onSuccess?.Invoke(sprite);
    }
    
    private void EnsureStorage()
    {
        if (_storage == null)
        {
            if (FirebaseManager.Instance?.Storage == null)
                throw new Exception("Firebase Storage 인스턴스가 null이므로 Storage를 사용할 수 없습니다.");

            _storage = FirebaseManager.Instance.Storage;
        }
    }

    // 업로드 파일 선택
    public string FileOpen()
    {
        if (OpenDialog.ShowDialog() == DialogResult.OK)
        {
            if ((openStream = OpenDialog.OpenFile()) != null)
            {
                openStream.Close();
                return OpenDialog.FileName;
            }
        }
        return null;
    }

    // 나중에 어디이미지가 변할지 코드에 적용해야함  
    public void SetImage()
    {
        string fileName = FileOpen();
        if (!string.IsNullOrEmpty(fileName))
        {
            Debug.Log(fileName);
            // 확장자 체크 (이미지 파일만 처리)  
            string ext = Path.GetExtension(fileName).ToLower();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            {
                byte[] fileData = File.ReadAllBytes(fileName);
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(fileData))
                {
                    // Texture2D를 Sprite로 변환 후 Image에 적용  
                    Sprite sprite = Sprite.Create(
                        tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f)
                    );
                    // 적용할 Image에 Sprite적용
                    // GetComponent<Image>().sprite = sprite;
                }
            }
        }
    }

    private void UploadImage(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            StorageReference imageRef = storageRef.Child("images/" + fileName);

            // 확장자에 따라 ContentType 지정
            string ext = Path.GetExtension(filePath).ToLower();
            string contentType = "application/octet-stream"; // 기본값

            if (ext == ".jpg" || ext == ".jpeg")
                contentType = "image/jpeg";
            else if (ext == ".png")
                contentType = "image/png";

            var metadata = new MetadataChange
            {
                ContentType = contentType
            };

            imageRef.PutBytesAsync(fileData, metadata).ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Image upload failed: " + task.Exception);
                }
                else
                {
                    Debug.Log("Image uploaded successfully: " + fileName);
                }
            });
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
        }
    }


}