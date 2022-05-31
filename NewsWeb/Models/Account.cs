using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace NewsWeb.Models
{
    public class Account
    {
        public int Id { set; get; }

        [DisplayName("Tên đăng nhập")]
        [Required(ErrorMessage ="{0} không được bỏ trống!")]
        [StringLength(20,MinimumLength = 6, ErrorMessage = "{0} từ 6-20 ký tự!")]
        public string UserName { set; get; }

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage ="{0} không được bỏ trống!")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage ="{0} từ 8-20 ký tự!")]
        public string Password { set; get; }

        [Required(ErrorMessage ="{0} không được bỏ trống!")]
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "{0} không hợp lệ!")]
        public string Email { set; get; }


        [Required(ErrorMessage ="{0} không được bỏ trống!")]
        [DisplayName("Tên đầy đủ")]
        public string FullName { set; get; }

        [Required(ErrorMessage ="{0} không được bỏ trống!")]
        [DisplayName("Địa chỉ")]
        public string Address { set; get; }

        [DisplayName("Là admin")]
        [DefaultValue(false)]
        public bool IsAdmin { get; set; } = false;


        [Required(ErrorMessage ="{0} không được bỏ trống!")]
        [DisplayName("SDT")]
        [RegularExpression("0\\d{9}", ErrorMessage = "STD không hợp lệ")]
        public string Phone { set; get; }

        [DisplayName("Ảnh đại diện")]
        [DefaultValue("default.png")]
        public string Avatar { set; get; } = "default.png";

        [NotMapped]
        [DisplayName("Ảnh đại diện")]
        public IFormFile AvatarFile { set; get; }

        //[DisplayName("Ngày đăng ký")]
        //public DateTime Date { get; set; }

        public List<Comment> Comments { set; get; }

        public List<SavedPost> SavedPosts { set; get; }

        public List<Favorite> Favorites { set; get; }

    }
}
