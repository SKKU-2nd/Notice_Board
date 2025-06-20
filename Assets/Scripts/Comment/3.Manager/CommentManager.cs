using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CommentManager : MonoSingleton<CommentManager>
{
    private CommentRepository _commentRepository;

    public event Action<PostDTO> OnDataChanged;

    private void EnsureRepository()
    {
        if (_commentRepository == null)
        {
            if (FirebaseManager.Instance?.DB == null)
            {
                Debug.LogError("Firebase DB 인스턴스가 null입니다.");
                return;
            }

            _commentRepository = new CommentRepository(FirebaseManager.Instance.DB);
        }
    }

    // 댓글 작성
    public async Task CreateComment(string postId, string authorId, string content)
    {
        EnsureRepository();

        string commentId = _commentRepository.GenerateNewCommentId(postId);
        var comment = new Comment(commentId, authorId, content, DateTime.UtcNow);
        await _commentRepository.AddComment(postId, comment.ToDto());

        var postDto = await PostManager.Instance.GetPost(postId);
        OnDataChanged?.Invoke(postDto);

        await PostManager.Instance.IncrementCommentCount(postId);
    }

    // 댓글 리스트 조회
    public async Task<List<CommentDTO>> GetComments(string postId)
    {
        EnsureRepository();
        return await _commentRepository.GetComments(postId);
    }

    // 댓글 단건 조회
    public async Task<CommentDTO> GetComment(string postId, string commentId)
    {
        EnsureRepository();
        return await _commentRepository.GetComment(postId, commentId);
    }

    // 댓글 삭제
    public async Task DeleteComment(string postId, string commentId, string requesterId)
    {
        EnsureRepository();

        var commentDto = await _commentRepository.GetComment(postId, commentId);
        var comment = new Comment(commentDto);

        var result = comment.CanDelete(requesterId);

        if (result.IsSuccess)
        {
            await _commentRepository.DeleteComment(postId, commentId);
        }
        else
        {
            Debug.LogError(result.Message);
        }
    }
    
    public async Task DeleteAllComments(string postId, string requesterId)
    {
        EnsureRepository();

        var comments = await _commentRepository.GetComments(postId);

        foreach (var commentDto in comments)
        {
            var comment = new Comment(commentDto);
            
            await _commentRepository.DeleteComment(postId, comment.CommentID);
        }

        // PostManager 쪽 CommentCount를 0으로 초기화하려면 아래 줄도 추가할 수 있음
        await PostManager.Instance.ResetCommentCount(postId);  // 이 함수가 있다면
    }
}