using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Post : MonoBehaviour
{
    // 상세보기 UI
    [SerializeField]
    private TextMeshProUGUI _contentText;
    [SerializeField]
    private TextMeshProUGUI _userNicknameText;
    [SerializeField]
    private TextMeshProUGUI _infoText;
    [SerializeField]
    private Button _backButton;
    [SerializeField]
    private GameObject _backUI;

    private void Awake()
    {
        _backButton.onClick.AddListener(GoBack);
    }

    // 스프라이트 추가
    public void Active(PostDTO postDto)
    {
        gameObject.SetActive(true);
        _contentText.text = postDto.Content;
        // 유저 정보 가져와서 넣기
        // var user = AccountManager.Instance.GetAccountDTO(AuthorID);
        _userNicknameText.text = postDto.AuthorID;
        string information = $"{postDto.CreatedAt}";
        _infoText.text = information;
    }

    private void GoBack()
    {
        gameObject.SetActive(false);
        _backUI.gameObject.SetActive(true);
    }
}
