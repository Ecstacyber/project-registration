using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class ProjectMember
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public string? GroupName { get; set; }

    public int? StudentId { get; set; }

    public double? Grade { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? Student { get; set; }
}
