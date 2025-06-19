using Firebase.Firestore;
using System;
using System.Collections.Generic;

[FirestoreData]
public class PostSaveData
{
    [FirestoreProperty] public string PostId { get; private set; }
    [FirestoreProperty] public string AuthorID { get; private set; }
    [FirestoreProperty] public string Content { get; private set; }
    [FirestoreProperty] public DateTime CreatedAt { get; private set; }
    [FirestoreProperty] public List<string> LikeUserIDList { get; private set; }
    [FirestoreProperty] public int CommentCount { get; private set; }

    public PostSaveData() { }

    public PostSaveData(PostDTO dto)
    {
        PostId = dto.PostId;
        AuthorID = dto.AuthorID;
        Content = dto.Content;
        CreatedAt = dto.CreatedAt;
        CommentCount = dto.CommentCount;
        LikeUserIDList = dto.LikeUserIDList;
    }

    public PostDTO ToDto()
    {
        return new PostDTO(PostId, AuthorID, Content, CreatedAt, LikeUserIDList, CommentCount);
    }
}