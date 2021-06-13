using System;

namespace WorkoutApp.Dto
{
  public class GetFileDto
  {
    public int Id { get; set; }
        
    public string Name { get; set; } = null!;

    public int Size { get; set; }

    public byte[] Data { get; set; } = null!;

    public DateTimeOffset UploadedOn { get; set; }
  }
}