using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace NewsWeb.Models
{
    public class Post
    {
        public int Id { set; get; }

        public int PostCategoryId { set; get; }

        [DisplayName("Loại bài viết")]
        public PostCategory PostCategory { set; get; }

        [Required(ErrorMessage ="{0} không được bỏ trống!")]
        public string Title { set; get; }

        [Required(ErrorMessage ="{0} không được bỏ trống!")]
        public string Author { set; get; }

        [Required(ErrorMessage ="{0} không được bỏ trống!")]
        public string Content { set; get; }

        public DateTime Date { set; get; }

        public string Imgage { set; get; }

        [NotMapped]
        public IFormFile ImageFile { set; get; }

        [DefaultValue(0)]
        public int View { set; get; } = 0;

        public List<Comment> Comments { set; get; }

        public List<SavedPost> SavedPosts { set; get; }
    }
}
