using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectRegistration.Models;

public partial class Course
{
    [Display(Name = "Id")]
    public int Id { get; set; }

    [Display(Name = "Name")]
    public string? CourseName { get; set; }

    [Display(Name = "Semester")]
    public int? Semester { get; set; }

    [Display(Name = "Year")]
    public int? Cyear { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public virtual ICollection<Project> ProjectCourseId2Navigations { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectCourses { get; set; } = new List<Project>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
