using AutoMapper;
using WorkoutApp.Dto;
using WorkoutApp.Entities;

namespace WorkoutApp.Data
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<UserAdditionDto, UserEntity>();
            CreateMap<UpdateUserDto, UserEntity>();
            CreateMap<UserEntity, GetUserDto>()
                .ForMember(_ => _.IsBlocked,
                    config => config.MapFrom(source => source.LockoutEnd != null));
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
            CreateMap<PostEntity, GetPostDto>()
                .ForMember(_ => _.LikingUsers,
                    config => config.Ignore());
            CreateMap<CommentAdditionDto, CommentEntity>();
            CreateMap<CommentModificationDto, CommentEntity>();
            CreateMap<CommentEntity, GetCommentDto>();
            CreateMap<FeedbackAdditionDto, FeedbackEntity>();
            CreateMap<FeedbackEntity, GetFeedbackDto>();
        }
    }
}