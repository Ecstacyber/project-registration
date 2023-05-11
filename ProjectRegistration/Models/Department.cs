using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectRegistration.Models;

public partial class Department
{
    [Display(Name = "ID")]
    public int Id { get; set; }

    [Display(Name = "Mã khoa")]
    public string? Dname { get; set; }

    [Display(Name = "Tên khoa")]
    public string? Info { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
