using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IPostRepository
  {
    Task<ICollection<PostEntity>> DoListAsync(int currentUserId, bool isAdministrator, CancellationToken cancellationToken);
    
    Task<PostEntity?> DoGetAsync(int postId, CancellationToken cancellationToken);
    
    Task DoAddAsync(PostEntity newPost, IReadOnlyCollection<int> fileIds, CancellationToken cancellationToken);
    
    Task<PostEntity?> DoAddCommentAsync(int postId, CommentEntity newComment, CancellationToken cancellationToken);

    Task<PostEntity?> DoAddLikeAsync(LikeEntity like, CancellationToken cancellationToken);
    
    Task<PostEntity?> DoUpdateCommentAsync(int commentId, CommentModificationDto commentDto, CancellationToken cancellationToken);
    
    Task<bool> DoDeleteAsync(int postId, CancellationToken cancellationToken);
    
    Task<PostEntity?> DoDeleteCommentAsync(int postId, int commentId, CancellationToken cancellationToken);
    
    Task<PostEntity?> DoDeleteLikeAsync(LikeEntity like, CancellationToken cancellationToken);
  }
}