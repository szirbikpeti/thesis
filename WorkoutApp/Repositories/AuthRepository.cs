using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Abstractions;
using WorkoutApp.Data;
using WorkoutApp.Entities;

namespace WorkoutApp.Repositories
{
  public class AuthRepository : IAuthRepository
  {
    private readonly WorkoutDbContext _dbContext;

    public AuthRepository(WorkoutDbContext dbContext)
    {
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
        
    public async Task<UserEntity> SignUpAsync(
      UserEntity newUser, 
      string password, 
      CancellationToken cancellationToken)
    {
      (newUser.PasswordHash, 
        newUser.PasswordSalt) = CreateStorablePassword(password);
            
      await _dbContext.Users
        .AddAsync(newUser, cancellationToken)
        .ConfigureAwait(false);
            
      await _dbContext.SaveChangesAsync(cancellationToken)
        .ConfigureAwait(false);

      return newUser;
    }

    public async Task<UserEntity> SignInAsync(
      string userName, 
      string password, 
      CancellationToken cancellationToken)
    {
      var user = await _dbContext.Users
        .FirstOrDefaultAsync(_ => _
            .UserName.ToLower()
            .Equals(userName.ToLower().Trim()), 
          cancellationToken);

      if (user is null) {
        return null;
      }

      return 
        VerifyPassword(password, user.PasswordHash, user.PasswordSalt) 
          ? user 
          : null;
    }

    public async Task<bool> IsUserExistsAsync(string userName, CancellationToken cancellationToken)
    {
      return await _dbContext.Users
        .AnyAsync(_ => _.UserName.Equals(userName), cancellationToken)
        .ConfigureAwait(false);
    }

    private static (byte[], byte[]) CreateStorablePassword(string password)
    {
      using var hmac = new System.Security.Cryptography.HMACSHA512();

      var hash = hmac.ComputeHash(
        System.Text.Encoding.UTF8.GetBytes(password));
            
      return (hash, hmac.Key);
    }
    
    private static bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
    {
      using var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt);
      var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      
      return !computedHash
        .Where((hashChar, i) => 
          hashChar != passwordHash[i])
        .Any();
    }
  }
}