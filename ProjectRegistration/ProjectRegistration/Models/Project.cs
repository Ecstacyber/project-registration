using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectRegistration.Models;

public partial class Project
{
    public int Id { get; set; }

    [Display(Name = "Tên đề tài")]
    public string? Pname { get; set; }

    [Display(Name = "Mô tả đồ án")]
    public string? Info { get; set; }

    [Display(Name = "Khoa")]
    public int? DepartmentId { get; set; }

    [Display(Name = "Lớp")]
    public int? ClassId { get; set; }

    public int? ClassId2 { get; set; }

    [Display(Name = "Giảng viên hướng dẫn")]
    public string? GuidingLecturerId { get; set; }

    [Display(Name = "Giảng viên phúc đáp")]
    public string? GradingLecturerId { get; set; }

    [Display(Name = "Năm học")]
    public string? Pyear { get; set; }

    [Display(Name = "Học kỳ")]
    public int? Semester { get; set; }

    [Display(Name = "Trạng thái duyệt")]
    public bool? IsVerified { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    [Display(Name = "Lớp")]
    public virtual Class? Class { get; set; }

    public virtual Class? ClassId2Navigation { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    [Display(Name = "Giảng viên phúc đáp")]
    public virtual User? GradingLecturer { get; set; }

    [Display(Name = "Giảng viên hướng dẫn")]
    public virtual User? GuidingLecturer { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
}
