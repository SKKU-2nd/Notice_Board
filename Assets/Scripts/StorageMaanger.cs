using UnityEngine;
using System.IO;
using System.Windows.Forms;
using UnityEngine.UI;
using Firebase.Storage;

public class StorageMaanger : MonoBehaviour
{
    private OpenFileDialog OpenDialog;
    private Stream openStream = null;
    private FirebaseStorage storage; 
    private StorageReference storageRef;

    private void Awake()
    {
        OpenDialog = new OpenFileDialog();
        OpenDialog.Filter = "jpg files (*.jpg) |*.jpg|png files (*.png) |*.jpg|All files  (*.*)|*.*";
        OpenDialog.FilterIndex = 3;
        OpenDialog.Title = "Image Dialog";

        storage = FirebaseStorage.DefaultInstance; 
        storageRef = storage.GetReferenceFromUrl("gs://noticeboard-60361.firebasestorage.app");
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

    // 나중에 어디이미지가 변할지 코드에 적용해야함  
    public void OnClickImage()
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
                    GetComponent<Image>().sprite = sprite;
                }
            }
        }
    }


    private void OnGUI()
    {
        if (GUI.Button(new Rect(100, 100, 300, 200), "ImageSet"))
        {
            string fileName = FileOpen();
            if (!string.IsNullOrEmpty(fileName))
            {
                UploadImage(fileName);
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
