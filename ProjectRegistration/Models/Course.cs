using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class Course
{
    public int Id { get; set; }

    public string? CourseName { get; set; }

    public int? Semester { get; set; }

    public int? Cyear { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public virtual ICollection<Project> ProjectCourseId2Navigations { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectCourses { get; set; } = new List<Project>();
}
