using System;
using System.Collections.Generic;
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
    [SerializeField]
    private List<Button> _postShowButtons;
    [SerializeField]
    private UI_Text _uiText;
    
    public List<Button> PostShowButtons => _postShowButtons;
    
    private PostDTO _postDTO;
    public PostDTO PostDTO => _postDTO;

    public event Action<PostDTO> PostShow;

    public async void Refresh(PostDTO postDto)
    {
        _postDTO = postDto;

        var author = await AccountManager.Instance.GetAccountDTOByEmailAsync(_postDTO.AuthorID);

        _nickNameText.text = author.Nickname;
        
        _timeText.text = _postDTO.CreatedAt.ToString("yyyy-MM-dd");

        _likeButton.Refresh(postDto.IsLiked(AccountManager.Instance.MyAccount.Email));
        
        _contentText.text = _postDTO.Content;
        string information = $"좋아요 {postDto.LikeCount} 댓글 {postDto.CommentCount}";
        _informationText.text = information;

        await StorageManger.Instance.LoadImageToUI(author.ProfilePath, _profileImage);
        
        _uiText.TextChanged();
    }
}