using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Abstractions;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Controllers
{
  [ApiController]
  [Route("api/file")]
  public class FileController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IFileRepository _file;
    private readonly UserManager<UserEntity> _userManager;

    public FileController(IFileRepository file, IMapper mapper, UserManager<UserEntity> userManager)
    {
      _file = file ?? throw new ArgumentNullException(nameof(file));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    [HttpPost]
    [RequestSizeLimit(256000000)]
    public async Task<ActionResult<GetFileDto>> AddAsync(IFormFile file, CancellationToken cancellationToken)
    {
      var fileEntity = await _file.DoAddAsync(file, cancellationToken)
        .ConfigureAwait(false);

      if (fileEntity is null) {
        return BadRequest("Maximum file size is 256MB");
      }

      var fileDto = _mapper.Map<GetFileDto>(fileEntity);

      return Ok(fileDto);
    }

    [HttpPatch]
    [Authorize]
    public async Task<ActionResult<GetFileDto>> UpdateAsync(IFormFile file, CancellationToken cancellationToken)
    {
      var currentUser = await _userManager.GetUserAsync(HttpContext.User)
        .ConfigureAwait(false);

      var fileEntity = await _file.DoUpdateAsync(currentUser.ProfilePictureId, file, cancellationToken)
        .ConfigureAwait(false);

      var fileDto = _mapper.Map<GetFileDto>(fileEntity);

      return Ok(fileDto);
    }
  }
}