using AutoMapper;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Data
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<AdditionUserDto, UserEntity>();
            CreateMap<UpdateUserDto, UserEntity>();
            CreateMap<UserEntity, GetUserDto>();
            CreateMap<WorkoutDto, WorkoutEntity>();
            CreateMap<WorkoutEntity, WorkoutDto>();
            CreateMap<GetFileDto, FileEntity>();
            CreateMap<FileEntity, GetFileDto>();
        }
    }
}