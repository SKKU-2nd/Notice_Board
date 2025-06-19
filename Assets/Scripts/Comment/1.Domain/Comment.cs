using System;

public class Comment
{
    // 고유 값
    public readonly string CommentID;
    public string AuthorID { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // 생성자
    public Comment(string commentId, string authorID, string content, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(commentId))
            throw new Exception("댓글 ID가 유효하지 않습니다.");

        if (string.IsNullOrWhiteSpace(authorID))
            throw new Exception("작성자 이메일이 유효하지 않습니다.");

        if (string.IsNullOrWhiteSpace(content))
            throw new Exception("댓글 내용은 비워둘 수 없습니다.");
        
        if (createdAt < DateTime.UtcNow.AddYears(-100))
            throw new Exception("너무 오래된 날짜는 유효하지 않습니다.");
        
        if (createdAt > DateTime.UtcNow)
            throw new Exception("미래의 날짜는 유효하지 않습니다.");

        CommentID = commentId;
        AuthorID = authorID;
        Content = content;
        CreatedAt = createdAt;
    }
    
    public Comment(CommentDTO dto)
        : this(dto.CommentID, dto.AuthorID, dto.Content, dto.CreatedAt)
    {
    }
    
    public Result CanDelete(string requesterId)
    {
        if (AuthorID == requesterId)
        {
            return new Result(true);
        }
        else
        {
            return new Result(false, "작성자가 아니면 삭제할 수 없습니다.");
        }
    }

    public CommentDTO ToDto() => new CommentDTO(this);
}