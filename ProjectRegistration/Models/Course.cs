using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectRegistration.Models;

public partial class Course
{
    public int Id { get; set; }

    [Display(Name = "Mã môn")]
    public string? CourseId { get; set; }

    [Display(Name = "Tên môn học")]
    public string? CourseName { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;
    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
