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
            CreateMap<ExerciseAdditionDto, ExerciseEntity>();
            CreateMap<ExerciseModificationDto, ExerciseEntity>();
            CreateMap<ExerciseEntity, GetExerciseDto>();
            CreateMap<SetAdditionDto, SetEntity>();
            CreateMap<SetModificationDto, SetEntity>();
            CreateMap<SetEntity, GetSetDto>();
            CreateMap<GetFileDto, FileEntity>();
            CreateMap<FileEntity, GetFileDto>();
            CreateMap<NotificationEntity, GetNotificationDto>();
            CreateMap<PostAdditionDto, PostEntity>();
            CreateMap<PostEntity, GetPostDto>();
            CreateMap<CommentAdditionDto, CommentEntity>();
            CreateMap<CommentModificationDto, CommentEntity>();
            CreateMap<CommentEntity, GetCommentDto>();
        }
    }
}