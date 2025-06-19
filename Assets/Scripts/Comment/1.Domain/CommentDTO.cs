using Firebase.Firestore;
using System;

[FirestoreData]
public class CommentDTO
{
    [FirestoreProperty] public string CommentID { get; private set; }
    [FirestoreProperty] public string AuthorID { get; private set; }
    [FirestoreProperty] public string Content { get; private set; }
    [FirestoreProperty] public DateTime CreatedAt { get; private set; }

    public CommentDTO() { } // Firestore 역직렬화를 위한 기본 생성자

    public CommentDTO(string commentID, string authorID, string content, DateTime createdAt)
    {
        CommentID = commentID;
        AuthorID = authorID;
        Content = content;
        CreatedAt = createdAt;
    }

    public CommentDTO(Comment comment)
        : this(comment.CommentID, comment.AuthorID, comment.Content, comment.CreatedAt)
    {
    }
}