using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class Department
{
    public int Id { get; set; }

    public string? Dname { get; set; }

    public string? Info { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
