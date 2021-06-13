﻿using System;
using WorkoutApp.Abstractions;

namespace WorkoutApp.Entities
{
    public class FileEntity : IIdentityAwareEntity
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = null!;

        public int Size { get; set; }

        public byte[] Data { get; set; } = null!;

        public DateTimeOffset UploadedOn { get; set; }
        
        public UserEntity? ProfilePictureOfUser { get; set; }
    }
}