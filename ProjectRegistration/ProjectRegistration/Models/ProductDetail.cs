using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class ProductDetail
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? UserId { get; set; }

    public string? Type { get; set; }

    public string? Info { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; } = false;

    public DateTime? DeletedDateTime { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
