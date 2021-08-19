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
using Microsoft.Extensions.Logging;
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
    private readonly INotificationRepository _notification;
    private readonly IFileRepository _file;

    public PostController(
      IMapper mapper,
      UserManager<UserEntity> userManager,
      IPostRepository post,
      INotificationRepository notification,
      IFileRepository file)
    {
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _post = post ?? throw new ArgumentNullException(nameof(post));
      _notification = notification ?? throw new ArgumentNullException(nameof(notification));
      _file = file ?? throw new ArgumentNullException(nameof(file));
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<GetPostDto>>> ListAsync(
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var fetchedPosts = await _post.DoListAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      var postDtoList = fetchedPosts.Select(post => CreatePostDto(post, cancellationToken).Result);

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

      return Ok(CreatePostDto(fetchedPost, cancellationToken).Result);
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
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      var now = DateTimeOffset.Now;
      
      // TODO - check that user posts includes postId
      var mappedComment = _mapper.Map<CommentEntity>(newCommentDto);
      mappedComment.UserId = currentUserId;
      mappedComment.PostId = postId;
      mappedComment.CommentedOn = now;
      mappedComment.ModifiedOn = now;

      var updatedPost = await _post.DoAddCommentAsync(postId, mappedComment, cancellationToken)
        .ConfigureAwait(false);
      
      return Ok(CreatePostDto(updatedPost!, cancellationToken).Result);
    }

    [HttpPost("like")]
    public async Task<IActionResult> AddLikeAsync(
      [FromBody] [Required] LikeDto newLikeDto,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var likeEntity = new LikeEntity {
        PostId = newLikeDto.PostId,
        UserId = currentUserId
      };

      var updatedPost = await _post.DoAddLikeAsync(likeEntity, cancellationToken)
        .ConfigureAwait(false);
      
      var notification = new NotificationEntity {
        Type = NotificationType.AddLike,
        SentByUserId = currentUserId,
        ReceivedUserId = updatedPost!.UserId,
        TriggeredOn = DateTimeOffset.Now  // TODO - add post to notification
      };
      
      await _notification.DoAddAsync(notification, cancellationToken)
        .ConfigureAwait(false);
      
      await _notification.DoBroadcastFollowNotifications(updatedPost!.UserId)
        .ConfigureAwait(false);

      return Ok(CreatePostDto(updatedPost!, cancellationToken).Result);
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

      return Ok(CreatePostDto(post, cancellationToken).Result);
    }

    [HttpDelete("{postId}")]
    public async Task<IActionResult> DeleteAsync(
      [FromRoute] [Required] int postId,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var post = await _post.DoGetAsync(postId, cancellationToken)
        .ConfigureAwait(false);

      if (post!.UserId != currentUserId) {
        return Unauthorized("Cannot be delete other's post");
      }
      
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
      var updatedPost = await _post.DoDeleteCommentAsync(postId, commentId, cancellationToken)
        .ConfigureAwait(false);

      if (updatedPost is null) {
        return NotFound($"Comment is not found with id: {commentId}");
      }
      
      return Ok(CreatePostDto(updatedPost!, cancellationToken).Result);
    }

    [HttpDelete("like")]
    public async Task<ActionResult<GetPostDto>> DeleteLikeAsync(
      [FromBody] [Required] LikeDto deletedLikeDto,
      CancellationToken cancellationToken)
    {
      var currentUserId = _userManager.GetUserIdAsInt(HttpContext.User);
      
      var likeEntity = new LikeEntity {
        PostId = deletedLikeDto.PostId,
        UserId = currentUserId
      };

      var updatedPost = await _post.DoDeleteLikeAsync(likeEntity, cancellationToken)
        .ConfigureAwait(false);

      return Ok(CreatePostDto(updatedPost!, cancellationToken).Result);
    }

    private async Task<GetPostDto> CreatePostDto(PostEntity post, CancellationToken cancellationToken)
    {
      var postDto = _mapper.Map<GetPostDto>(post);

      postDto.Files = post.FileRelationEntities
        .Select(relation => _mapper.Map<GetFileDto>(relation.File))
        .ToImmutableList();
      
      foreach (var likingUser in post.LikingUsers) {
        likingUser.User.ProfilePicture = await _file.DoGetAsync(likingUser.User.ProfilePictureId, cancellationToken)
          .ConfigureAwait(false);
      }
      
      postDto.LikingUsers = post.LikingUsers
        .Select(relation => _mapper.Map<GetUserDto>(relation.User))
        .ToImmutableList();

      return postDto;
    }
  }
}