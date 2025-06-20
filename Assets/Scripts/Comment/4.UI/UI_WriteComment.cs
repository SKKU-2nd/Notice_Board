using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WriteComment : MonoBehaviour
{
    [SerializeField]
    private Button _commentButton;

    [SerializeField]
    private TMP_InputField _contentText;

    private string _postID;
    public string PostID { get { return _postID; } set { _postID = value; } }

    private void Awake()
    {
        _commentButton.onClick.AddListener(Post);
    }

    private async void Post()
    {
        // 글 게시
        await CommentManager.Instance.CreateComment(PostID, AccountManager.Instance.MyAccount.Email, _contentText.text);
    }
}