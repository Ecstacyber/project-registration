using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class Project
{
    public int Id { get; set; }

    public string? Pname { get; set; }

    public string? Info { get; set; }

    public int? DepartmentId { get; set; }

    public int? ClassId { get; set; }

    public int? ClassId2 { get; set; }

    public int? GuidingLecturerId { get; set; }

    public int? GradingLecturerId { get; set; }

    public int? MaxMember { get; set; }

    public int? Pyear { get; set; }

    public int? Semester { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    public virtual Class? Class { get; set; }

    public virtual Class? ClassId2Navigation { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual User? GradingLecturer { get; set; }

    public virtual User? GuidingLecturer { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
}
