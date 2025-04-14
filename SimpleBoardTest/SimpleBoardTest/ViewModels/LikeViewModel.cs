namespace SimpleBoardTest.ViewModels
{
    public class LikeViewModel
    {
        public string TargetType { get; set; } = null!; // Post or Comment
        public int TargetId { get; set; } // 게시글 ID 또는 댓글 ID
        public int LikeCount { get; set; } // 좋아요 수
        public bool IsLiked { get; set; } // 현재 사용자가 좋아요를 눌렀는지 여부
    }
}
