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
    private UI_Comment _commentPrefab;
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
    
    private List<UI_Comment> _commentList = new List<UI_Comment>();

    private void Awake()
    {
        foreach (Button button in _backButton)
        {
            button.onClick.AddListener(GoBack);
        }
    }

    // 스프라이트 추가
    public async void Active(PostDTO postDto)
    {
        gameObject.SetActive(true);
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
                var slot = Instantiate(_commentPrefab, _contentContainer).GetComponent<UI_Comment>();
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
