using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectRegistration.Models;

public partial class User : IdentityUser
{

    public string? UserId { get; set; }

    [Display(Name = "Họ và tên")]
    public string? Fullname { get; set; }

    [DisplayFormat(DataFormatString = "{0:d}")]
    [Display(Name = "Ngày sinh")]
    public DateTime? DateOfBirth { get; set; }

    //[Display(Name = "Tên đăng nhập")]
    //public string? Username { get; set; }

    //[Display(Name = "Mật khẩu")]
    //public string? UserPassword { get; set; }

    [Display(Name = "Ảnh đại diện")]
    [ValidateNever]
    public string? ImagePath { get; set; }

    [Display(Name = "Khoa")]
    public int? DepartmentId { get; set; }

    [Display(Name = "Loại tài khoản")]
    public int? UserTypeId { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<ClassDetail> ClassDetails { get; set; } = new List<ClassDetail>();

    [Display(Name = "Khoa")]
    public virtual Department? Department { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<LecturerStat> LecturerStats { get; set; } = new List<LecturerStat>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Project> ProjectGradingLecturers { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectGuidingLecturers { get; set; } = new List<Project>();

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public virtual ICollection<StudentStat> StudentStats { get; set; } = new List<StudentStat>();
}
