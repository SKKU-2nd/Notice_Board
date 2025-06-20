using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WritePost : MonoBehaviour
{
    [SerializeField]
    private Button _backButton;

    [SerializeField]
    private Button _postButton;

    [SerializeField]
    private TMP_InputField _contentText;

    [SerializeField]
    private GameObject _postAfterShowPanel;

    private void Awake()
    {
        _backButton.onClick.AddListener(() =>
        {
            _postAfterShowPanel.SetActive(true);
            gameObject.SetActive(false);
        });
        _postButton.onClick.AddListener(Post);
    }

    private async void Post()
    {
        // 글 게시
        await PostManager.Instance.CreatePost(AccountManager.Instance.MyAccount.Email, _contentText.text);
        _postAfterShowPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}