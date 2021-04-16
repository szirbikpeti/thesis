using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
    public class FileEntity : IIdentityAwareEntity
    {
        public int Id { get; set; }

        public int Size { get; set; }

        public byte[] Data { get; set; }
    }
}