using Firebase.Firestore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PostRepository
{
    private const string COLLECTION_NAME = "BoardDB";
    
    private readonly FirebaseFirestore _firestore;

    public PostRepository(FirebaseFirestore firestore)
    {
        _firestore = firestore;
    }

    // 1. 빈 문서 ID 생성용
    public DocumentReference CreateDocumentReference()
    {
        return _firestore.Collection(COLLECTION_NAME).Document(); // ID 자동 생성
    }

    // 2. 게시글 추가
    public async Task AddPost(DocumentReference docRef, PostDTO postDto)
    {
        var saveData = new PostSaveData(postDto);
        await docRef.SetAsync(saveData);
    }

    // 3. 게시글 조회
    public async Task<PostDTO> GetPost(string postId)
    {
        var doc = await _firestore.Collection(COLLECTION_NAME).Document(postId).GetSnapshotAsync();
        if (!doc.Exists)
            return null;

        var saveData = doc.ConvertTo<PostSaveData>();
        return saveData.ToDto();
    }

    // 4. 게시글 목록 조회 (start, limit)
    public async Task<List<PostDTO>> GetPosts()
    {
        var query = _firestore.Collection(COLLECTION_NAME)
            .OrderByDescending("CreatedAt");

        var snapshot = await query.GetSnapshotAsync();

        var result = new List<PostDTO>();
        foreach (var doc in snapshot.Documents)
        {
            var saveData = doc.ConvertTo<PostSaveData>();
            result.Add(saveData.ToDto());
        }
        return result;
    }

    // 5. 게시글 수정 (전체 덮어쓰기 방식)
    public async Task UpdatePost(PostDTO postDto)
    {
        var saveData = new PostSaveData(postDto);
        var docRef = _firestore.Collection(COLLECTION_NAME).Document(postDto.PostId);
        await docRef.SetAsync(saveData, SetOptions.Overwrite);
    }

    // 6. 게시글 삭제
    public async Task DeletePost(string postId)
    {
        var docRef = _firestore.Collection(COLLECTION_NAME).Document(postId);
        await docRef.DeleteAsync();
    }
}