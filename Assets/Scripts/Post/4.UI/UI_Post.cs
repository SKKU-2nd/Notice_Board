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
    private TextMeshProUGUI _infoText;
    [SerializeField]
    private List<Button> _backButton;
    [SerializeField]
    private GameObject _backUI;

    private UI_WriteComment _writeComment;
    
    private List<UI_CommentSlot> _commentList = new List<UI_CommentSlot>();
    
    private string _currentPostID;

    private void Awake()
    {
        foreach (Button button in _backButton)
        {
            button.onClick.AddListener(GoBack);
        }

        _writeComment = GetComponent<UI_WriteComment>();

        CommentManager.Instance.OnDataChanged += Refresh;
    }

    // 스프라이트 추가
    public async void Refresh(PostDTO postDto)
    {
        gameObject.SetActive(true);
        _writeComment.PostID = postDto.PostId;
        _contentText.text = postDto.Content;
        // 유저 정보 가져와서 넣기
        // var user = AccountManager.Instance.GetAccountDTO(AuthorID);
        _userNicknameText.text = postDto.AuthorID;
        string information = $"{postDto.CreatedAt}";
        _infoText.text = information;

        var commentList = await CommentManager.Instance.GetComments(postDto.PostId);
        
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
