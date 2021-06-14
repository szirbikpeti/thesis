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
            CreateMap<ExerciseDto, ExerciseEntity>();
            CreateMap<ExerciseEntity, ExerciseDto>();
            CreateMap<SetDto, SetEntity>();
            CreateMap<SetEntity, SetDto>();
            CreateMap<GetFileDto, FileEntity>();
            CreateMap<FileEntity, GetFileDto>();
        }
    }
}