using System;

public class PostDto
{
    public readonly string PostId;
    public readonly string AuthorID;
    public readonly string Content;
    public readonly DateTime CreatedAt;
    public readonly int LikeCount;
    public readonly int CommentCount;

    public PostDto(string postId, string authorID, string content, DateTime createdAt, int likeCount, int commentCount)
    {
        PostId = postId;
        AuthorID = authorID;
        Content = content;
        CreatedAt = createdAt;
        LikeCount = likeCount;
        CommentCount = commentCount;
    }
    
    public PostDto(Post post)
        : this(post.PostId, post.AuthorID, post.Content, post.CreatedAt, post.LikeCount, post.CommentCount)
    {
    }
}