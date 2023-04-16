using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class Class
{
    public int Id { get; set; }

    public int? CourseId { get; set; }

    public int? Semester { get; set; }

    public int? Cyear { get; set; }

    public DateTime? CreatedDateTime { get; set; }
}
