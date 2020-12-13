﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VideoUploader_ConfettiService
{
    public class Video
    {
        [Key]
        public int VideoId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime PostedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public string LinkURL { get; set; }

        [Required]
        public string BucketURL { get; set; }

        public int Views { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
