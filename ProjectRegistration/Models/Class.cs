using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class Class
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? Semester { get; set; }

    public string? Cyear { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<Project> ProjectClassId2Navigations { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectClasses { get; set; } = new List<Project>();
}
