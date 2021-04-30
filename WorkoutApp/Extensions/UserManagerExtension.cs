using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkoutApp.Entities;

namespace WorkoutApp.Extensions
{
  public static class UserManagerExtension
  {
    public static async Task<UserEntity> FindByIdWithAdditionalDataAsync(
      this UserManager<UserEntity> userManager, 
      int id, 
      CancellationToken cancellationToken)
    {
      return await userManager.Users
        .FirstOrDefaultAsync(_ => (_.Id == id) && (_.DeletedOn == null), cancellationToken)
        .ConfigureAwait(false);
    }
    
    public static async Task<UserEntity> FindByNameWithAdditionalDataAsync(
      this UserManager<UserEntity> userManager, 
      string userName, 
      CancellationToken cancellationToken)
    {
      var normalizedUserName = userManager.NormalizeName(userName);
      
      return await userManager.Users
        .FirstOrDefaultAsync(_ => 
          (_.NormalizedUserName == normalizedUserName) 
            && (_.DeletedOn == null), 
          cancellationToken)
        .ConfigureAwait(false);
    }
  }
}