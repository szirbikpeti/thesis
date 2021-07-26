using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Abstractions;
using WorkoutApp.Dto;
using WorkoutApp.Entities;
using WorkoutApp.Extensions;

namespace WorkoutApp.Controllers
{
  [Authorize(Policies.ManagePosts)]
  [ApiController]
  [Route("api/post")]
  public class PostController : ControllerBase
  {
    
    private readonly IMapper _mapper;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IPostRepository _post;

    public PostController(IMapper mapper, UserManager<UserEntity> userManager, IPostRepository post)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _post = post ?? throw new ArgumentNullException(nameof(post));
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<GetPostDto>>> ListAsync(
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var fetchedPosts = await _post.DoListAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      var postDtoList = fetchedPosts.Select(CreatePostDto);

      return Ok(postDtoList);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<GetPostDto>> GetAsync(
      [FromRoute] [Required]int id,
      CancellationToken cancellationToken)
    {
      var fetchedPost = await _post.DoGetAsync(id, cancellationToken)
        .ConfigureAwait(false);

      if (fetchedPost is null) {
        return BadRequest($"There is no post with this id ({id})");
      }

      return Ok(CreatePostDto(fetchedPost));
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(
      [FromBody] [Required] PostAdditionDto newPostDto,
      CancellationToken cancellationToken)
    {
      if (newPostDto.FileIds.Count == 0) {
        return BadRequest("One file is must be selected.");
      }
      
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var mappedPost = _mapper.Map<PostEntity>(newPostDto);
      mappedPost.UserId = currentUserId;
      mappedPost.PostedOn = DateTimeOffset.Now;

      await _post.DoAddAsync(mappedPost, newPostDto.FileIds, cancellationToken)
        .ConfigureAwait(false);

      return Ok();
    }

    [HttpPost("{postId}/comment")]
    public async Task<IActionResult> AddCommentAsync(
      [FromRoute] [Required] int postId,
      [FromBody] [Required] CommentAdditionDto newCommentDto,
      CancellationToken cancellationToken)
    {
      // TODO - check that user posts includes postId
      var mappedComment = _mapper.Map<CommentEntity>(newCommentDto);
      mappedComment.PostId = postId;
      mappedComment.CommentedOn = DateTimeOffset.Now;
      mappedComment.ModifiedOn = DateTimeOffset.Now;

      var updatedPost = await _post.DoAddCommentAsync(postId, mappedComment, cancellationToken)
        .ConfigureAwait(false);
      
      return Ok(CreatePostDto(updatedPost!));
    }

    [HttpPost("like")]
    public async Task<IActionResult> AddLikeAsync(
      [FromBody] [Required] LikeDto newLikeDto,
      CancellationToken cancellationToken)
    {
      var mappedLike = _mapper.Map<LikeEntity>(newLikeDto);

      var updatedPost = await _post.DoAddLikeAsync(mappedLike, cancellationToken)
        .ConfigureAwait(false);

      return Ok(CreatePostDto(updatedPost!));
    }

    [HttpPut("{commentId}")]
    public async Task<IActionResult> UpdateCommentAsync(
      [FromRoute] [Required] int commentId,
      [FromBody] [Required] CommentModificationDto updatedCommentDto,
      CancellationToken cancellationToken)
    {
      var post = await _post.DoUpdateCommentAsync(commentId, updatedCommentDto, cancellationToken)
        .ConfigureAwait(false);

      if (post is null) {
        return NotFound();
      }

      return Ok(CreatePostDto(post));
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeleteAsync(
      [FromRoute] [Required] int postId,
      CancellationToken cancellationToken)
    {
      var result = await _post.DoDeleteAsync(postId, cancellationToken)
        .ConfigureAwait(false);

      if (!result) {
        return Problem("Save process is failed");
      }
      
      return Ok();
    }
    
    [HttpDelete("{postId}/comment/{commentId}")]
    public async Task<IActionResult> DeleteCommentAsync(
      [FromRoute] [Required] int postId,
      [FromRoute] [Required] int commentId,
      CancellationToken cancellationToken)
    {
      var relationEntity = new PostCommentRelationEntity {
        PostId = postId,
        CommentId = commentId
      };
      
      var updatedPost = await _post.DoDeleteCommentAsync(relationEntity, cancellationToken)
        .ConfigureAwait(false);

      if (updatedPost is null) {
        return NotFound($"Comment is not found with id: {commentId}");
      }
      
      return Ok(CreatePostDto(updatedPost!));
    }

    [HttpDelete("like")]
    public async Task<IActionResult> DeleteLikeAsync(
      [FromBody] [Required] LikeDto deletedLikeDto,
      CancellationToken cancellationToken)
    {
      var mappedLike = _mapper.Map<LikeEntity>(deletedLikeDto);

      var updatedPost = await _post.DoDeleteLikeAsync(mappedLike, cancellationToken)
        .ConfigureAwait(false);

      return Ok(CreatePostDto(updatedPost!));
    }

    private GetPostDto CreatePostDto(PostEntity post)
    {
      var postDto = _mapper.Map<GetPostDto>(post);

      postDto.Files = post.FileRelationEntities
        .Select(relation => _mapper.Map<GetFileDto>(relation.File))
        .ToImmutableList();
      
      postDto.Comments = post.CommentRelationEntities
        .Select(relation => _mapper.Map<GetCommentDto>(relation.Comment))
        .ToImmutableList();
          
      postDto.LikedUsers = post.LikingUsers
        .Select(relation => _mapper.Map<GetUserDto>(relation.User))
        .ToImmutableList();

      return postDto;
    }
  }
}