using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CommentManagerEditor : EditorWindow
{
    private string postId = "";
    private string authorId = "";
    private string content = "";
    private string commentId = "";

    [MenuItem("Tools/댓글 테스트")]
    public static void ShowWindow()
    {
        GetWindow<CommentManagerEditor>("댓글 테스트");
    }

    private void OnGUI()
    {
        GUILayout.Label("댓글 테스트", EditorStyles.boldLabel);

        postId = EditorGUILayout.TextField("Post ID", postId);
        authorId = EditorGUILayout.TextField("Author ID", authorId);
        content = EditorGUILayout.TextField("Content", content);
        commentId = EditorGUILayout.TextField("Comment ID", commentId);

        GUILayout.Space(10);

        if (GUILayout.Button("댓글 작성"))
        {
            RunTask(async () =>
            {
                await CommentManager.Instance.CreateComment(postId, authorId, content);
                Debug.Log("댓글 작성 완료");
            });
        }

        if (GUILayout.Button("댓글 전체 조회"))
        {
            RunTask(async () =>
            {
                var comments = await CommentManager.Instance.GetComments(postId);
                foreach (var c in comments)
                {
                    Debug.Log(
                        $"[Comment] ID: {c.CommentID}, Author: {c.AuthorID}, Content: {c.Content}, CreatedAt: {c.CreatedAt}"
                    );
                }
            });
        }

        if (GUILayout.Button("댓글 단건 조회"))
        {
            RunTask(async () =>
            {
                var comment = await CommentManager.Instance.GetComment(postId, commentId);
                Debug.Log(
                    $"[Comment] ID: {comment.CommentID}, Author: {comment.AuthorID}, Content: {comment.Content}, CreatedAt: {comment.CreatedAt}"
                );
            });
        }

        if (GUILayout.Button("댓글 삭제"))
        {
            RunTask(async () =>
            {
                await CommentManager.Instance.DeleteComment(postId, commentId, authorId);
                Debug.Log("댓글 삭제 완료");
            });
        }
    }

    private void RunTask(Func<Task> taskFunc)
    {
        taskFunc().ContinueWith(t =>
        {
            if (t.Exception != null)
            {
                Debug.LogError(t.Exception.InnerException?.Message ?? t.Exception.Message);
            }
        });
    }
}
