using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PostManager : MonoSingleton<PostManager>
{
    private PostRepository _postRepository;

    // 게시글 목록 변경 시 UI에 알림
    public event Action OnDataChanged;

    public async Task CreatePost(string authorId, string content)
    {
        EnsureRepository();

        var docRef = _postRepository.CreateDocumentReference();
        string postId = docRef.Id;

        var post = new Post(postId, authorId, content, DateTime.UtcNow, new List<string>(), 0);
        var postDto = post.ToDto();

        await _postRepository.AddPost(docRef, postDto);
    }

    public async Task<List<PostDTO>> GetPosts()
    {
        EnsureRepository();
        
        return await _postRepository.GetPosts();
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

    public async Task ToggleLike(string postId, string userId)
    {
        EnsureRepository();

        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);

        post.SetLike(userId);
        await _postRepository.UpdatePost(post.ToDto());
        
        OnDataChanged?.Invoke();
    }

    public async Task IncrementCommentCount(string postId)
    {
        EnsureRepository();

        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);

        post.SetCommentCount(postDto.CommentCount + 1);
        await _postRepository.UpdatePost(post.ToDto());
    }

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