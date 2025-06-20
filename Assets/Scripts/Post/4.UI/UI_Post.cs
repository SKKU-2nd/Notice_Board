using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Post : MonoBehaviour
{
    // 상세보기 UI
    [SerializeField]
    private Transform _contentContainer;
    [SerializeField]
    private UI_CommentSlot _commentSlotPrefab;
    [SerializeField]
    private TextMeshProUGUI _contentText;
    [SerializeField]
    private TextMeshProUGUI _userNicknameText;
    [SerializeField]
    private Image _profileSprite; 
    [SerializeField]
    private TextMeshProUGUI _infoText;
    [SerializeField]
    private List<Button> _backButton;
    [SerializeField]
    private GameObject _backUI;
    
    [Header("좋아요")]
    [SerializeField]
    private Sprite _likeOnSprite;
    [SerializeField]
    private Sprite _likeOffSprite;
    [SerializeField]
    private Image _likeImage;
    [SerializeField]
    private UI_LikeButton _likeButton;

    private UI_WriteComment _writeComment;
    
    private List<UI_CommentSlot> _commentList = new List<UI_CommentSlot>();
    
    private string _currentPostID;
    
    private PostDTO _currentPost;

    private void Awake()
    {
        foreach (Button button in _backButton)
        {
            button.onClick.AddListener(GoBack);
        }

        _writeComment = GetComponent<UI_WriteComment>();

        CommentManager.Instance.OnDataChanged += Refresh;
        PostManager.Instance.OnDataChanged += () => Refresh();
    }

    // 스프라이트 추가
    public async void Refresh(PostDTO postDto = null)
    {
        if (postDto == null)
        {
            postDto = await PostManager.Instance.GetPost(_currentPost.PostId);
        }
        gameObject.SetActive(true);
        _currentPost = postDto;
        _writeComment.PostID = _currentPost.PostId;
        _contentText.text = _currentPost.Content;

        var authorDto = await AccountManager.Instance.GetAccountDTOByEmailAsync(_currentPost.AuthorID);
        _userNicknameText.text = authorDto?.Nickname ?? "등록되지않은 사용자";

        var profilePath = authorDto?.ProfilePath;
        await StorageManger.Instance.LoadImageToUI(profilePath, _profileSprite);

        string information = $"{_currentPost.CreatedAt}";
        _infoText.text = information;
        
        // 좋아요
        _likeButton.PostId = _currentPost.PostId;
        var isLiked = _currentPost.IsLiked(authorDto?.Email);
        _likeImage.sprite = isLiked ? _likeOnSprite : _likeOffSprite;
        _likeButton.Refresh(isLiked);

        var commentList = await CommentManager.Instance.GetComments(_currentPost.PostId);
        
        for (int i = 0; i < commentList.Count; i++)
        {
            if (_commentList.Count <= i)
            {
                var slot = Instantiate(_commentSlotPrefab, _contentContainer).GetComponent<UI_CommentSlot>();
                _commentList.Add(slot);
            }
            _commentList[i].Refresh(commentList[i]);
        }
        
        for (int i = commentList.Count; i < _commentList.Count; i++)
        {
            _commentList[i].gameObject.SetActive(false);
        }
    }

    private void GoBack()
    {
        gameObject.SetActive(false);
        _backUI.gameObject.SetActive(true);
    }
}
