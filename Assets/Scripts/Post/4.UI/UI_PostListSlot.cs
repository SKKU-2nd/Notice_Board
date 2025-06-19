using TMPro;
using UnityEngine;

public class UI_PostListSlot : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _contentText;
    [SerializeField]
    private TextMeshProUGUI _informationText;

    public void Refresh(PostDTO postDto)
    {
        _contentText.text = postDto.Content;
        // 유저 정보 가져와서 넣기
        // var user = AccountManager.Instance.
        string information = $"{postDto.CreatedAt} {postDto.AuthorID} {postDto.CommentCount}";
        _informationText.text = information;
    }
}