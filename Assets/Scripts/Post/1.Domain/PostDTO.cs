using System;
using System.Collections.Generic;

public class PostDTO
{
    public readonly string PostId;
    public readonly string AuthorID;
    public readonly string Content;
    public readonly DateTime CreatedAt;
    public readonly List<string> LikeUserIDList;
    public readonly int CommentCount;

    public int LikeCount => LikeUserIDList.Count;

    public PostDTO(string postId, string authorID, string content, DateTime createdAt, List<string> likeUserIDList, int commentCount)
    {
        PostId = postId;
        AuthorID = authorID;
        Content = content;
        CreatedAt = createdAt;
        LikeUserIDList = likeUserIDList;
        CommentCount = commentCount;
    }
    
    public PostDTO(Post post)
        : this(post.PostId, post.AuthorID, post.Content, post.CreatedAt, post.LikeUserIDList, post.CommentCount)
    {
    }
}