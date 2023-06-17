using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectRegistration.Models;

public partial class Class
{
    public int Id { get; set; }

    [Display(Name = "Mã môn")]
    public int? CourseId { get; set; }

    [Display(Name = "Mã lớp")]
    public string? ClassId { get; set; }

    [Display(Name = "Học kỳ")]
    public int? Semester { get; set; }

    [Display(Name = "Năm học")]
    public string? Cyear { get; set; }

    [Display(Name = "Trạng thái")]
    public string? RegOpen { get; set; }

    [Display(Name = "Thời gian bắt đầu đăng ký")]
    public DateTime? RegStart { get; set; }

    [Display(Name = "Thời gian kết thúc đăng ký")]
    public DateTime? RegEnd { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<ClassDetail> ClassDetails { get; set; } = new List<ClassDetail>();

    [Display(Name = "Mã môn học")]
    public virtual Course? Course { get; set; }

    public virtual ICollection<Project> ProjectClassId2Navigations { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectClasses { get; set; } = new List<Project>();
}
