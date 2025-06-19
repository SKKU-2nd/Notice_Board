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
    private HashSet<string> _likeUserIDList;        // 좋아요 누른 사람들 (유저의 고유 ID)
    public int LikeCount => _likeUserIDList.Count;  // 좋아요 수
    public int CommentCount { get; private set; }   // 댓글 수 (게시판에서는 댓글 수만 보이기 때문에 따로 저장)

    // 생성자
    public Post(string postId, string authorID, string content, DateTime createdAt)
    {
        if (string.IsNullOrEmpty(postId))
        {
            throw new Exception("포스트 아이디가 유효하지 않습니다.");
        }

        // 명세로 수정
        if (string.IsNullOrEmpty(authorID))
        {
            throw new Exception("작성자 정보가 유효하지 않습니다.");
        }

        if (string.IsNullOrEmpty(content))
        {
            throw new Exception("내용은 비워둘 수 없습니다.");
        }

        PostId = postId;
        AuthorID = authorID;
        Content = content;
        CreatedAt = createdAt;
        _likeUserIDList = new HashSet<string>();
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

    // 좋아요 추가 / 삭제
    public void SetLike(string userID)
    {
        // 명세
        if (string.IsNullOrWhiteSpace(userID))
        {
            throw new Exception("유효하지 않은 사용자 ID입니다.");
        }

        if (!_likeUserIDList.Add(userID))
        {
            _likeUserIDList.Remove(userID);
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

    public PostDto ToDto()
    {
        return new PostDto(this);
    }
}