using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WorkoutApp.Entities;

namespace WorkoutApp.Abstractions
{
  public interface IFileRepository
  {
    Task<FileEntity> DoGetAsync(int fileId, CancellationToken cancellationToken);
    
    Task<FileEntity?> DoAddAsync(IFormFile file, CancellationToken cancellationToken);
    
    Task<FileEntity> DoUpdateAsync(int fileId, IFormFile file, CancellationToken cancellationToken);
  }
}