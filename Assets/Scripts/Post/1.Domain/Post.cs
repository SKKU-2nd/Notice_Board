using System;
using System.Collections.Generic;

public class Post
{
    // 고유 값
    public readonly string PostId;   // 글의 고유 값 (파이어베이스 도큐먼트 key값)
    public readonly string AuthorID; // 글쓴이 이메일 (유저의 고유 ID)

    // 상태 값
    public string Content { get; private set; }     // 글 내용
    public DateTime CreatedAt { get; private set; } // 작성 일자
    private List<string> _likeUserIDList;           // 좋아요 누른 사람들 (유저의 고유 ID)
    public List<string> LikeUserIDList => _likeUserIDList;
    public int LikeCount => _likeUserIDList.Count;  // 좋아요 수
    public int CommentCount { get; private set; }   // 댓글 수 (게시판에서는 댓글 수만 보이기 때문에 따로 저장)

    // 생성자
    public Post(string postId, string authorID, string content, DateTime createdAt, List<string> likeUserIDList, int commentCount)
    {
        if (string.IsNullOrEmpty(postId))
            throw new Exception("포스트 아이디가 유효하지 않습니다.");

        if (string.IsNullOrEmpty(authorID))
            throw new Exception("작성자 정보가 유효하지 않습니다.");

        if (string.IsNullOrEmpty(content))
            throw new Exception("내용은 비워둘 수 없습니다.");

        if (commentCount < 0)
            throw new Exception("댓글 수는 음수일 수 없습니다.");
        
        if (likeUserIDList == null)
            throw new Exception("좋아요 리스트는 NULL일 수 없습니다.");

        PostId = postId;
        AuthorID = authorID;
        Content = content;
        CreatedAt = createdAt;
        CommentCount = commentCount;
        _likeUserIDList = likeUserIDList;
    }
    
    public Post(PostDTO dto)
        : this(dto.PostId, dto.AuthorID, dto.Content, dto.CreatedAt, dto.LikeUserIDList, dto.CommentCount)
    {
    }

    // 글 수정
    public void Edit(string authorID, string content)
    {
        // 명세
        if (string.IsNullOrEmpty(authorID))
        {
            throw new Exception("유효하지 않은 사용자 ID입니다.");
        }

        if (AuthorID != authorID)
        {
            throw new Exception("작성자가 아닌 사람은 게시글 수정을 할 수 없습니다.");
        }

        Content = content;
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

    // 좋아요 추가 / 삭제
    public bool SetLike(string userID)
    {
        if (string.IsNullOrWhiteSpace(userID))
        {
            throw new Exception("유효하지 않은 사용자 ID입니다.");
        }

        if (!_likeUserIDList.Contains(userID))
        {
            // 추가된 경우
            _likeUserIDList.Add(userID);
            return true;
        }
        else
        {
            // 이미 있었다면 삭제
            _likeUserIDList.Remove(userID);
            return false;
        }
    }

    // 댓글 조작
    public void SetCommentCount(int commentCount)
    {
        if (commentCount < 0)
        {
            throw new Exception("코멘트 수는 0이하일 수 없습니다.");
        }

        CommentCount = commentCount;
    }

    public PostDTO ToDto()
    {
        return new PostDTO(this);
    }
}