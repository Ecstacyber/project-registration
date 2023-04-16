using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Fullname { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Username { get; set; }

    public string? UserPassword { get; set; }

    public string? ImagePath { get; set; }

    public int? DepartmentId { get; set; }

    public int? UserTypeId { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<LecturerStat> LecturerStats { get; set; } = new List<LecturerStat>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Project> ProjectGradingLecturers { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectGuidingLecturers { get; set; } = new List<Project>();

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public virtual ICollection<StudentStat> StudentStats { get; set; } = new List<StudentStat>();
}
