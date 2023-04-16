using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class LecturerStat
{
    public int Id { get; set; }

    public int? LecturerId { get; set; }

    public int? Semester { get; set; }

    public int? Syear { get; set; }

    public double? AvgGrade { get; set; }

    public int? TotalProjectsGuided { get; set; }

    public int? TotalProjectsGraded { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public virtual User? Lecturer { get; set; }
}
