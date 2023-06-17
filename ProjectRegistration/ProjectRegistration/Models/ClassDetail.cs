using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class ClassDetail
{
    public int Id { get; set; }

    public int? ClassId { get; set; }

    public string? UserId { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    public virtual Class? Class { get; set; }

    public virtual User? User { get; set; }
}
