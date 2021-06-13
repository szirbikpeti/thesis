using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Entities;

namespace WorkoutApp.Repositories
{
  public class FileRepository : IFileRepository
  {
    private readonly WorkoutDbContext _dbContext;
    
    public FileRepository(WorkoutDbContext dbContext)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<FileEntity> DoGetAsync(int fileId, CancellationToken cancellationToken)
    {
      return await _dbContext.Files
        .AsNoTracking()
        .Where(_ => _.Id == fileId)
        .FirstAsync(cancellationToken)
        .ConfigureAwait(false);
    }

    public async Task<FileEntity> DoAddAsync(IFormFile file, CancellationToken cancellationToken)
    {
      var fileData = new byte[file.Length];

      await using var fileStream = file.OpenReadStream();
      await fileStream.ReadAsync(fileData, cancellationToken)
        .ConfigureAwait(false);

      var fileEntity = new FileEntity {
        Name = file.FileName,
        Size = fileData.Length,
        Data = fileData,
        UploadedOn = DateTimeOffset.Now
      };
      
      await _dbContext.Files
        .AddAsync(fileEntity, cancellationToken)
        .ConfigureAwait(false);
      
      await _dbContext.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      return fileEntity;
    }
    
    public async Task<FileEntity> DoUpdateAsync(int fileId, IFormFile file, CancellationToken cancellationToken)
    {
      var fileEntity = await _dbContext.Files
        .Where(_ => _.Id == fileId)
        .FirstOrDefaultAsync(cancellationToken)
        .ConfigureAwait(false);
      
      var fileData = new byte[file.Length];

      await using var fileStream = file.OpenReadStream();
      await fileStream.ReadAsync(fileData, cancellationToken)
        .ConfigureAwait(false);

      fileEntity!.Name = file.FileName;
      fileEntity!.Size = fileData.Length;
      fileEntity!.Data = fileData;
      fileEntity!.UploadedOn = DateTimeOffset.Now;

      _dbContext.Files
        .Update(fileEntity);
      
      await _dbContext.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      return fileEntity;
    }
  }
}