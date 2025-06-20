using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PostListSlot : MonoBehaviour
{
    [SerializeField]
    private Image _profileImage;
    [SerializeField]
    private TextMeshProUGUI _nickNameText;
    [SerializeField]
    private TextMeshProUGUI _timeText;
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

    public async void Refresh(PostDTO postDto)
    {
        _postDTO = postDto;

        var author = await AccountManager.Instance.GetAccountDTOByEmailAsync(_postDTO.AuthorID);

        _nickNameText.text = author.Nickname;
        
        _timeText.text = _postDTO.CreatedAt.ToString("yyyy-MM-dd");

        _likeButton.PostId = _postDTO.PostId;
        
        _contentText.text = _postDTO.Content;
        string information = $"좋아요 {postDto.LikeCount} 댓글 {postDto.CommentCount}";
        _informationText.text = information;
        
        _uiText.TextChanged();
    }
}