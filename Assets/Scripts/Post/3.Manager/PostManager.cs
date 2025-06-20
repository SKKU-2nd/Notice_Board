using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PostManager : MonoSingleton<PostManager>
{
    private List<PostDTO> _posts;
    private PostRepository _postRepository;

    // 게시글 목록 변경 시 UI에 알림
    public event Action OnDataChanged;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _posts = new List<PostDTO>();

        EnsureRepository();
    }

    public async Task CreatePost(string authorId, string content)
    {
        EnsureRepository();

        var docRef = _postRepository.CreateDocumentReference();
        string postId = docRef.Id;

        var post = new Post(postId, authorId, content, DateTime.UtcNow, new List<string>(), 0);
        var postDto = post.ToDto();

        await _postRepository.AddPost(docRef, postDto);
    }

    public async Task GetPosts()
    {
        EnsureRepository();

        _posts = await _postRepository.GetPosts();
        OnDataChanged?.Invoke();
    }

    public async Task<PostDTO> GetPost(string postId)
    {
        EnsureRepository();
        return await _postRepository.GetPost(postId);
    }

    public async Task EditPost(string postId, string editorId, string newContent)
    {
        EnsureRepository();

        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);

        post.Edit(editorId, newContent);
        await _postRepository.UpdatePost(post.ToDto());
    }

    public async Task DeletePost(string postId, string requesterId)
    {
        EnsureRepository();

        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);

        var result = post.CanDelete(requesterId);

        if (result.IsSuccess)
        {
            await _postRepository.DeletePost(postId);
        }
        else
        {
            Debug.LogError(result.Message);
        }
    }

    public async Task<bool> IsLiked(string postId, string userId)
    {
        EnsureRepository();

        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);
        return post.IsLiked(userId);
    }

    public async Task<bool> ToggleLike(string postId, string userId)
    {
        EnsureRepository();

        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);

        bool isLiked = post.SetLike(userId);
        await _postRepository.UpdatePost(post.ToDto());

        return isLiked;
    }

    public async Task IncrementCommentCount(string postId)
    {
        EnsureRepository();

        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);

        post.SetCommentCount(postDto.CommentCount + 1);
        await _postRepository.UpdatePost(post.ToDto());
    }

    public IReadOnlyList<PostDTO> GetCachedPosts() => _posts;

    private void EnsureRepository()
    {
        if (_postRepository == null)
        {
            if (FirebaseManager.Instance?.DB == null)
                throw new Exception("Firebase DB 인스턴스가 null이므로 PostRepository를 생성할 수 없습니다.");

            _postRepository = new PostRepository(FirebaseManager.Instance.DB);
        }
    }
}