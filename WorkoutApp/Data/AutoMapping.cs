using AutoMapper;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Data
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<UserAdditionDto, UserEntity>();
            CreateMap<UpdateUserDto, UserEntity>();
            CreateMap<UserEntity, GetUserDto>();
            CreateMap<WorkoutAdditionDto, WorkoutEntity>();
            CreateMap<WorkoutModificationDto, WorkoutEntity>();
            CreateMap<WorkoutEntity, GetWorkoutDto>();
            CreateMap<ExerciseDto, ExerciseEntity>();
            CreateMap<ExerciseEntity, ExerciseDto>();
            CreateMap<SetDto, SetEntity>();
            CreateMap<SetEntity, SetDto>();
            CreateMap<GetFileDto, FileEntity>();
            CreateMap<FileEntity, GetFileDto>();
        }
    }
}