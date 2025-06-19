using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CommentRepository
{
    private readonly FirebaseFirestore _firestore;
    private const string POST_COLLECTION = "CommentDB";
    private const string COMMENT_SUBCOLLECTION = "comments";

    public CommentRepository(FirebaseFirestore firestore)
    {
        _firestore = firestore;
    }

    // 댓글 추가
    public async Task AddComment(string postId, CommentDTO commentDto)
    {
        var docRef = _firestore
            .Collection(POST_COLLECTION)
            .Document(postId)
            .Collection(COMMENT_SUBCOLLECTION)
            .Document(commentDto.CommentID);

        await docRef.SetAsync(commentDto);
    }

    // 댓글 조회
    public async Task<List<CommentDTO>> GetComments(string postId)
    {
        var querySnapshot = await _firestore
            .Collection(POST_COLLECTION)
            .Document(postId)
            .Collection(COMMENT_SUBCOLLECTION)
            .OrderByDescending("CreatedAt")
            .GetSnapshotAsync();

        var result = new List<CommentDTO>();

        foreach (var doc in querySnapshot.Documents)
        {
            result.Add(doc.ConvertTo<CommentDTO>());
        }

        return result;
    }

    // 댓글 단건 조회
    public async Task<CommentDTO> GetComment(string postId, string commentId)
    {
        var docRef = _firestore
            .Collection(POST_COLLECTION)
            .Document(postId)
            .Collection(COMMENT_SUBCOLLECTION)
            .Document(commentId);

        var snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
            throw new Exception("댓글이 존재하지 않습니다.");

        return snapshot.ConvertTo<CommentDTO>();
    }

    // 댓글 삭제
    public async Task DeleteComment(string postId, string commentId)
    {
        var docRef = _firestore
            .Collection(POST_COLLECTION)
            .Document(postId)
            .Collection(COMMENT_SUBCOLLECTION)
            .Document(commentId);

        await docRef.DeleteAsync();
    }

    // 새 도큐먼트 ID 생성용
    public string GenerateNewCommentId(string postId)
    {
        return _firestore
            .Collection(POST_COLLECTION)
            .Document(postId)
            .Collection(COMMENT_SUBCOLLECTION)
            .Document()
            .Id;
    }
}
