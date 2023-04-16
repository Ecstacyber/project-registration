using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class ProductDetail
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? ProductPath { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public virtual Product? Product { get; set; }
}
