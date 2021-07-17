namespace WorkoutApp.Entities
{
  public class PostFileRelationEntity
  {
    public int PostId { get; set; }

    public int FileId { get; set; }

    public PostEntity Post { get; set; } = null!;

    public FileEntity File { get; set; } = null!;
  }
}