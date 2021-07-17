namespace WorkoutApp.Entities
{
  public class PostCommentRelationEntity
  {
    public int PostId { get; set; }

    public int CommentId { get; set; }

    public PostEntity Post { get; set; } = null!;

    public CommentEntity Comment { get; set; } = null!;
  }
}