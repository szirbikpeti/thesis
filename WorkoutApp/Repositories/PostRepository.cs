using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Dto;
using WorkoutApp.Entities;
using WorkoutApp.Extensions;

namespace WorkoutApp.Repositories
{
  public class PostRepository : IPostRepository
  {
    private readonly UserManager<UserEntity> _userManager;
    private readonly WorkoutDbContext _dbContext;
    private readonly IMapper _mapper;

    public PostRepository(UserManager<UserEntity> userManager, WorkoutDbContext dbContext, IMapper mapper)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public async Task<ICollection<PostEntity>> DoListAsync(int currentUserId, CancellationToken cancellationToken)
    {
      var followedUserIds = await _userManager
        .ListFollowedUserIdsAsync(currentUserId, cancellationToken)
        .ConfigureAwait(false);

      return await _dbContext.Posts
        .AsNoTracking()
        .AsSplitQuery()
        .Where(_ => _.UserId == currentUserId || followedUserIds.Contains(_.UserId))
        .Include(_ => _.User)
        .Include(_ => _.Workout)
        .ThenInclude(_ => _.Exercises)
        .ThenInclude(_ => _.Sets)
        .Include(_ => _.FileRelationEntities)
        .ThenInclude(_ => _.File)
        .Include(_ => _.CommentRelationEntities)
        .Include(_ => _.LikingUsers)  // TODO - ThenInclude
        .ToListAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<PostEntity?> DoGetAsync(int postId, CancellationToken cancellationToken)
    {
      return await _dbContext.Posts
        .AsNoTracking()
        .AsSplitQuery()
        .Where(_ => _.Id == postId)
        .Include(_ => _.Workout)
        .ThenInclude(_ => _.Exercises)
        .ThenInclude(_ => _.Sets)
        .Include(_ => _.FileRelationEntities)
        .ThenInclude(_ => _.File)
        .Include(_ => _.CommentRelationEntities)
        .Include(_ => _.LikingUsers)  // TODO - ThenInclude
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task DoAddAsync(PostEntity newPost, IReadOnlyCollection<int> fileIds, CancellationToken cancellationToken)
    {
      await _dbContext.Posts
        .AddAsync(newPost, cancellationToken)
        .ConfigureAwait(false); // TODO - store return value to get Id 
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      if (fileIds.Count > 0) {
        var createdPost = await _dbContext.Posts
          .Where(_ => _.PostedOn == newPost.PostedOn)
          .FirstOrDefaultAsync(cancellationToken)
          .ConfigureAwait(false);
        
        var newEntities = fileIds.Select(_ => new PostFileRelationEntity {
          PostId = createdPost!.Id,
          FileId = _
        });

        await _dbContext.PostFileRelations
          .AddRangeAsync(newEntities, cancellationToken)
          .ConfigureAwait(false);

        await _dbContext
          .SaveChangesAsync(cancellationToken)
          .ConfigureAwait(false);
      }
    }

    public async Task<PostEntity?> DoAddCommentAsync(int postId, CommentEntity newComment, CancellationToken cancellationToken)
    {
      var createdComment  = await _dbContext.Comments
        .AddAsync(newComment, cancellationToken)
        .ConfigureAwait(false);

      var newRelationEntity = new PostCommentRelationEntity {
        PostId = postId,
        CommentId = createdComment.Entity.Id
      };

      await _dbContext.PostCommentRelations
        .AddAsync(newRelationEntity, cancellationToken)
        .ConfigureAwait(false);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
      
      return await DoGetAsync(postId, cancellationToken)
        .ConfigureAwait(false);
    }
    
    public async Task<PostEntity?> DoAddLikeAsync(LikeEntity like, CancellationToken cancellationToken)
    {
      await _dbContext.Likes
        .AddAsync(like, cancellationToken)
        .ConfigureAwait(false);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      return await DoGetAsync(like.PostId, cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<PostEntity?> DoUpdateCommentAsync(
      int commentId, 
      CommentModificationDto commentDto,
      CancellationToken cancellationToken)
    {
      var fetchedComment = await _dbContext.Comments
        .Where(_ => _.Id == commentId)
        .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

      if (fetchedComment is null) {
        return null;
      }

      _mapper.Map(commentDto, fetchedComment);
      
      fetchedComment.ModifiedOn = DateTimeOffset.Now;
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
      
      return await DoGetAsync(fetchedComment.PostId, cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<bool> DoDeleteAsync(int postId, CancellationToken cancellationToken)
    {
      var fetchedPost = await _dbContext.GetByIdAsync<PostEntity>(postId, cancellationToken)
        .ConfigureAwait(false);

      if (fetchedPost is null) {
        return false;
      }
      
      _dbContext.DoDelete(fetchedPost);

      return await _dbContext.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false) > 0;
    }

    public async Task<PostEntity?> DoDeleteCommentAsync(
      PostCommentRelationEntity deletedRelation,
      CancellationToken cancellationToken)
    {
      var deletedComment = await _dbContext.Comments
        .Where(_ => _.Id == deletedRelation.CommentId)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);

      if (deletedComment is null) {
        return null;
      }
      
      deletedComment.DeletedOn = DateTimeOffset.Now;

      _dbContext.PostCommentRelations.Remove(deletedRelation);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);
      
      return await DoGetAsync(deletedRelation.PostId, cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<PostEntity?> DoDeleteLikeAsync(LikeEntity like, CancellationToken cancellationToken)
    {
      _dbContext.Likes.Remove(like);
      
      await _dbContext
        .SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      return await DoGetAsync(like.PostId, cancellationToken)
        .ConfigureAwait(false);
    }
  }
}