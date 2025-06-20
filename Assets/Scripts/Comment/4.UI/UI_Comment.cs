using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Comment : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _commentText;
    
    [SerializeField]
    private TextMeshProUGUI _nickNameText;

    [SerializeField]
    private TextMeshProUGUI _infoText;
    
    [SerializeField]
    private Image _profileSprite;

    public void Refresh(CommentDTO comment)
    {
        gameObject.SetActive(true);
        _commentText.text = comment.Content;
        // 유저 정보 가져와서 하기
    }
}