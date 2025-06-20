using UnityEngine;
using System.IO;
using System.Windows.Forms;
using UnityEngine.UI;
using Firebase.Storage;
using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class StorageManger : MonoSingleton<StorageManger>
{
    private FirebaseStorage _storage;

    private OpenFileDialog OpenDialog;
    private Stream openStream = null;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        OpenDialog = new OpenFileDialog();
        OpenDialog.Filter = "JPEG files (*.jpg;*.jpeg)|*.jpg;*.jpeg|PNG files (*.png)|*.png|All files (*.*)|*.*";
        OpenDialog.FilterIndex = 3;
        OpenDialog.Title = "Image Dialog";

        EnsureStorage();
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

    public async Task LoadImageToUI(string imagePath, Image targetImage)
    {
        Uri downloadUri = await GetDownloadUrl(imagePath);
        if (downloadUri == null)
        {
            Debug.LogError("Download URL is null");
            return;
        }

        Sprite sprite = await DownloadSprite(downloadUri);
        if (sprite != null && targetImage != null)
        {
            targetImage.sprite = sprite;
        }
    }

    public async Task<Uri> GetDownloadUrl(string imagePath)
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

    public async Task<Sprite> DownloadSprite(Uri uri)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri);
        var op = request.SendWebRequest();

        while (!op.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"이미지 다운로드 실패: {request.error}");
            return null;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }

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

    public void SetImage(Image targetImage)
    {
        string fileName = FileOpen();
        if (!string.IsNullOrEmpty(fileName))
        {
            Debug.Log(fileName);
            string ext = Path.GetExtension(fileName).ToLower();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            {
                byte[] fileData = File.ReadAllBytes(fileName);
                Texture2D tex = new Texture2D(2, 2);
                if (tex.LoadImage(fileData))
                {
                    Sprite sprite = Sprite.Create(
                        tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f)
                    );
                    targetImage.sprite = sprite;
                }
            }
        }
    }

    public async Task UploadImage(string filePath)
    {
        EnsureStorage();

        if (!File.Exists(filePath))
        {
            Debug.LogError("File does not exist: " + filePath);
            return;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        string fileName = Path.GetFileName(filePath);
        StorageReference imageRef = _storage.GetReference($"images/{fileName}");

        string ext = Path.GetExtension(filePath).ToLower();
        string contentType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };

        var metadata = new MetadataChange { ContentType = contentType };

        try
        {
            await imageRef.PutBytesAsync(fileData, metadata);
            Debug.Log($"Image uploaded successfully: {fileName}");

            Uri downloadUrl = await imageRef.GetDownloadUrlAsync();
            Debug.Log($"Download URL: {downloadUrl.AbsoluteUri}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Image upload failed: {ex.Message}");
        }
    }
}
