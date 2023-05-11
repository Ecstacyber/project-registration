using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectRegistration.Models;

public partial class Course
{
    [Display(Name = "Id")]
    public int Id { get; set; }

    [Display(Name = "Mã môn")]
    public string? CourseId { get; set; }

    [Display(Name = "Tên môn học")]
    public string? CourseName { get; set; }

    [Display(Name = "Học kỳ")]
    public int? Semester { get; set; }

    [Display(Name = "Năm học")]
    public string? Cyear { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
