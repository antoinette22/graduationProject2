using graduationProject.Dtos;
using graduationProject.DTOs.ReactDtos;

namespace graduationProject.Services
{
    public interface IPostService
    {
        Task<ReturnPostDto> CreatePost(AddPostDto post, string username);
        Task<ReturnPostDto> EditPost(PostDto post);
        Task<ResultDto> DeletePost(int id);
        Task<ReturnPostDto> GetPost(int id);
        Task<List<ReturnPostDto>> GetPostsByUser(string username);
        Task<ReturnPostsDto> GetAllPosts();
        Task<ReturnCommentDto> addComment(AddCommentDto comment, string username);
        Task<ResultDto> deleteComment(int id);
        Task<ReturnCommentDto> EditComment(CommentDto comment);
        Task<ReturnReplyDto> addReply(AddReplyDto reply, string username);
        Task<ResultDto> deleteReply(int id);
        Task<ReturnReplyDto> EditReply(ReplyDto reply);
        Task<List<ReturnCommentDto>> GetAllComments(int postId);
        Task<ReturnCommentDto> GetComment(int id);
        Task<List<ReturnReplyDto>> GetAllReplies(int commentId);
        Task<ReturnReplyDto> GetReply(int id);
        Task<ResultDto> AddReact(int postId, string userName);
        Task<ResultDto> DeleteReact(int postId, string userName);
        Task<ReturnReactsDto> GetPostReacts(int postId);
    }
}
