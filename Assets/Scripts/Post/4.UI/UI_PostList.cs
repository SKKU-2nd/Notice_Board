using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PostList : MonoBehaviour
{
    [SerializeField]
    private UI_Post _postUI;

    [SerializeField]
    private GameObject _postPreviewSlotPrefab;

    [Header("콘텐츠")]
    [SerializeField]
    private Transform _content;

    private List<UI_PostListSlot> _postSlotList;

    private void Start()
    {
        _postSlotList = new List<UI_PostListSlot>();
        PostManager.Instance.OnDataChanged += Refresh;
    }

    private async void OnEnable()
    {
        Refresh();
    }

    private async void Refresh()
    {
        var postList = await PostManager.Instance.GetPosts();

        for (int i = 0; i < postList.Count; i++)
        {
            if (_postSlotList.Count <= i)
            {
                var slot = Instantiate(_postPreviewSlotPrefab, _content).GetComponent<UI_PostListSlot>();
                foreach (var showButtons in slot.PostShowButtons)
                {
                    showButtons.onClick.AddListener(() => ShowPost(slot.PostDTO));
                }

                _postSlotList.Add(slot);
            }

            _postSlotList[i].Refresh(postList[i]);
        }

        for (int i = postList.Count; i < _postSlotList.Count; i++)
        {
            _postSlotList[i].gameObject.SetActive(false);
        }
    }


    private void ShowPost(PostDTO post)
    {
        _postUI.Refresh(post);
        gameObject.SetActive(false);
    }
}