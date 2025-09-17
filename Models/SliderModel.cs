﻿using Shopping.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping.Models
{
    public class SliderModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên thương hiệu")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả thương hiệu")]
        public string Description { get; set; }
        public int Status { get; set; }
        public string Image { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
