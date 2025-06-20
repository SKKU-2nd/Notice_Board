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
        await PostManager.Instance.ToggleLike(_postId, AccountManager.Instance.MyAccount.Email);
    }

    public async void Refresh(bool active)
    {
        _heartImage.sprite = active ? _likeOnSprite : _likeOffSprite;
        _heartImage.alphaHitTestMinimumThreshold = 0f;
    }
}