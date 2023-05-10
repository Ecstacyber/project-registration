using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class Document
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public int? UserId { get; set; }

    public string? Info { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? User { get; set; }
}
