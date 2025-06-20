using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_LikeButton : MonoBehaviour
{
    [SerializeField]
    private Sprite _likeOnSprite;
    [SerializeField]
    private Sprite _likeOffSprite;
    [SerializeField]
    private Image _heartImage;

    private Button _button;

    private string _postId;

    public string PostId { get => _postId; set => _postId = value; }

    private void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(ToggleLike);
    }

    private async void ToggleLike()
    {
        bool result = await PostManager.Instance.ToggleLike(PostId, AccountManager.Instance.MyAccount.AccountID);
        // 하트 온오프
        _heartImage.sprite = result ? _likeOnSprite : _likeOffSprite;
    }
}