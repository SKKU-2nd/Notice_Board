using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PostManager : MonoSingleton<PostManager>
{
    private List<PostDTO> _posts;
    private PostRepository _postRepository;

    // 게시글 목록 변경 시 UI에 알림
    public event Action OnDataChanged;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    private void Init()
    {
        _posts = new List<PostDTO>();
        //_postRepository = new PostRepository(Firebase.FirebaseFirestore.DefaultInstance);
    }

    // 게시글 생성
    public async Task CreatePost(string authorId, string content)
    {
        var docRef = _postRepository.CreateDocumentReference();
        string postId = docRef.Id;

        var post = new Post(postId, authorId, content, DateTime.UtcNow, new List<string>(), 0);
        var postDto = new PostDTO(post);

        await _postRepository.AddPost(docRef, postDto);
    }

    // 게시글 목록 조회
    public async Task GetPosts()
    {
        _posts = await _postRepository.GetPosts();
        OnDataChanged?.Invoke(); // UI 갱신 알림
    }

    // 게시글 단건 조회
    public async Task<PostDTO> GetPost(string postId)
    {
        return await _postRepository.GetPost(postId);
    }

    // 게시글 수정 (작성자 확인 포함)
    public async Task EditPost(string postId, string editorId, string newContent)
    {
        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);

        post.Edit(editorId, newContent);
        await _postRepository.UpdatePost(new PostDTO(post));
    }

    // 게시글 삭제
    public async Task DeletePost(string postId, string requesterId)
    {
        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);

        if (post.CanDelete(requesterId))
        {
            await _postRepository.DeletePost(postId);
        }
    }

    // 좋아요 토글
    public async Task<bool> ToggleLike(string postId, string userId)
    {
        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);

        bool isLiked = post.SetLike(userId);
        await _postRepository.UpdatePost(new PostDTO(post));

        return isLiked;
    }

    // 댓글 수 증가
    public async Task IncrementCommentCount(string postId)
    {
        var postDto = await _postRepository.GetPost(postId);
        var post = new Post(postDto);

        post.SetCommentCount(postDto.CommentCount + 1);
        await _postRepository.UpdatePost(new PostDTO(post));
    }

    // 외부에서 _posts 접근용
    public IReadOnlyList<PostDTO> GetCachedPosts() => _posts;
}
