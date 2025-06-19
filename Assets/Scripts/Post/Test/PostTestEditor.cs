using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;

public class PostTestEditor : EditorWindow
{
    private string authorId = "test_user";
    private string content = "테스트 게시글입니다.";
    private string targetPostId = "";
    private string editContent = "수정된 내용입니다.";

    [MenuItem("Tools/Post 기능 테스트")]
    public static void ShowWindow()
    {
        GetWindow<PostTestEditor>("Post 기능 테스트");
    }

    private void OnGUI()
    {
        GUILayout.Label("게시글 테스트", EditorStyles.boldLabel);

        authorId = EditorGUILayout.TextField("Author ID", authorId);
        content = EditorGUILayout.TextField("Post Content", content);

        if (GUILayout.Button("게시글 생성"))
        {
            RunAsync(() => PostManager.Instance.CreatePost(authorId, content));
        }

        if (GUILayout.Button("게시글 목록 조회"))
        {
            RunAsync(async () => {
                await PostManager.Instance.GetPosts();
                foreach (var post in PostManager.Instance.GetCachedPosts())
                {
                    Debug.Log(
                        $"PostID: {post.PostId}\n" +
                        $"AuthorID: {post.AuthorID}\n" +
                        $"Content: {post.Content}\n" +
                        $"CreatedAt: {post.CreatedAt}\n" +
                        $"LikeCount: {post.LikeCount}\n" +
                        $"CommentCount: {post.CommentCount}\n" +
                        $"LikedUsers: [{string.Join(", ", post.LikeUserIDList)}]"
                    );
                    Debug.Log("-----------------");
                }
            });
        }

        GUILayout.Space(10);
        GUILayout.Label("특정 게시글 조회 및 수정", EditorStyles.boldLabel);

        targetPostId = EditorGUILayout.TextField("Post ID", targetPostId);
        editContent = EditorGUILayout.TextField("Edit Content", editContent);

        if (GUILayout.Button("게시글 조회"))
        {
            RunAsync(async () => {
                var post = await PostManager.Instance.GetPost(targetPostId);
                Debug.Log(
                    $"PostID: {post.PostId}\n" +
                    $"AuthorID: {post.AuthorID}\n" +
                    $"Content: {post.Content}\n" +
                    $"CreatedAt: {post.CreatedAt}\n" +
                    $"LikeCount: {post.LikeCount}\n" +
                    $"CommentCount: {post.CommentCount}\n" +
                    $"LikedUsers: [{string.Join(", ", post.LikeUserIDList)}]"
                );
            });
        }

        if (GUILayout.Button("게시글 수정"))
        {
            RunAsync(() => PostManager.Instance.EditPost(targetPostId, authorId, editContent));
        }

        if (GUILayout.Button("게시글 삭제"))
        {
            RunAsync(() => PostManager.Instance.DeletePost(targetPostId, authorId));
        }

        if (GUILayout.Button("좋아요 토글"))
        {
            RunAsync(async () => {
                bool isLiked = await PostManager.Instance.ToggleLike(targetPostId, authorId);
                Debug.Log(isLiked ? "좋아요 추가됨" : "좋아요 제거됨");
            });
        }

        if (GUILayout.Button("댓글 수 증가"))
        {
            RunAsync(() => PostManager.Instance.IncrementCommentCount(targetPostId));
        }
    }

    private async void RunAsync(System.Func<Task> taskFunc)
    {
        try
        {
            await taskFunc();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"에러 발생: {ex.Message}");
        }
    }
}
