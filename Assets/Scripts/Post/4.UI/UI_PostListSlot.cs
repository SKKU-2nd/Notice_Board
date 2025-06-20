using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PostListSlot : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _contentText;
    [SerializeField]
    private TextMeshProUGUI _informationText;
    [SerializeField]
    private UI_LikeButton _likeButton;
    
    private PostDTO _postDTO;
    public PostDTO PostDTO => _postDTO;
    
    private Button _postShowButton;
    public Button PostShowButton => _postShowButton;
    
    private UI_Text _uiText;

    private void Awake()
    {
        _postShowButton = GetComponent<Button>();
        _uiText = GetComponentInChildren<UI_Text>();
    }

    public void Refresh(PostDTO postDto)
    {
        _postDTO = postDto;

        _likeButton.PostId = _postDTO.PostId;
        
        _contentText.text = _postDTO.Content;
        // 유저 정보 가져와서 넣기
        // var user = AccountManager.Instance.
        string information = $"{_postDTO.CreatedAt} {_postDTO.AuthorID} {_postDTO.CommentCount}";
        _informationText.text = information;
        
        _uiText.TextChanged();
    }
}