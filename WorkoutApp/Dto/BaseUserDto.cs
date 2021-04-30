﻿using System;
using System.ComponentModel.DataAnnotations;

namespace WorkoutApp.Dto
{
    public class BaseUserDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateTimeOffset? Birthday { get; set; }
    }
}