using graduationProject.Dtos;
using graduationProject.DTOs.ReactDtos;
using graduationProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace graduationProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [Authorize(Roles = "User")]
        [HttpPost("AddPost")]
        public async Task<IActionResult> CreatePost([FromForm] AddPostDto post)
        {
            var username = User.Identity.Name;
            var result = await _postService.CreatePost(post, username);
            if (!result.IsSuccess)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [Authorize(Roles = "User")]
        [HttpPut("UpdatePost")]
        public async Task<IActionResult> EditPost([FromForm] PostDto post)
        {
            var result = await _postService.EditPost(post);
            if (!result.IsSuccess)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [Authorize(Roles = "User")]
        [HttpDelete("DeletePost")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var result = await _postService.DeletePost(id);
            if (!result.IsSuccess)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpGet("GetPost")]
        public async Task<IActionResult> GetPost(int id)
        {
            var result = await _postService.GetPost(id);
            if (!result.IsSuccess)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpGet("GetPostsByUser")]
        public async Task<IActionResult> GetPostsByUser(string username)
        {
            var result = await _postService.GetPostsByUser(username);
            return Ok(result);
        }
        [HttpGet("GetAllPosts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var result = await _postService.GetAllPosts();
            return Ok(result.Posts);
        }
        [HttpPost("AddComment")]
        public async Task<IActionResult> addComment([FromForm] AddCommentDto comment)
        {
            var username = User.Identity.Name;
            var result=await _postService.addComment(comment, username);
            if (!result.isSucces)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpDelete("DeleteComment")]
        public async Task<IActionResult> deleteComment(int id)
        {
            var result = await _postService.deleteComment(id);
            if (!result.IsSuccess)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpPut("UpdateComment")]
        public async Task<IActionResult> EditComment([FromForm] CommentDto comment)
        {
            var result = await _postService.EditComment(comment);
            if (!result.isSucces)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpPost("AddReply")]
        public async Task<IActionResult> addReply([FromForm] AddReplyDto reply)
        {
            var username = User.Identity.Name;
            var result = await _postService.addReply(reply, username);
            if (!result.isSucces)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpDelete("DeleteReply")]
        public async Task<IActionResult> deleteReply(int id)
        {
            var result = await _postService.deleteReply(id);
            if (!result.IsSuccess)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpPut("UpdateReply")]
        public async Task<IActionResult> EditReply([FromForm] ReplyDto reply)
        {
            var result = await _postService.EditReply(reply);
            if (!result.isSucces)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpGet("GetComments")]
        public async Task<IActionResult> GetComments(int postId)
        {
            var result = await _postService.GetAllComments(postId);
            return Ok(result);
        }
        [HttpGet("GetComment")]
        public async Task<IActionResult> GetComment(int id)
        {
            var result = await _postService.GetComment(id);
            if (!result.isSucces)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpGet("GetReplies")]
        public async Task<IActionResult> GetReplies(int commentId)
        {
            var result = await _postService.GetAllReplies(commentId);
            return Ok(result);
        }
        [HttpGet("GetReply")]
        public async Task<IActionResult> GetReply(int id)
        {
            var result = await _postService.GetReply(id);
            if (!result.isSucces)
            {
                var errors = new { errors = result.Errors };
                return BadRequest(errors);
            }
            return Ok(result);
        }
        [HttpPost("AddReact")]
        public async Task<IActionResult> AddReact(ReactDto reactDto)
        {
            var username = User.Identity.Name;
            var result =await _postService.AddReact(reactDto.postId, username);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpDelete("RemoveReact")]
        public async Task<IActionResult> RemoveReact(ReactDto reactDto)
        {
            var username = User.Identity.Name;
            var result = await _postService.DeleteReact(reactDto.postId, username);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
        [HttpGet("GetReacts/{id}")]
        public async Task<IActionResult> GetPostReacts(int id)
        {
            var result = await _postService.GetPostReacts(id);
            return Ok(result);
        }
    }
}
