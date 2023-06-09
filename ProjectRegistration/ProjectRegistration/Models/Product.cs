﻿using System;
using System.Collections.Generic;

namespace ProjectRegistration.Models;

public partial class Product
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public string? StudentId { get; set; }

    public DateTime? CreatedDateTime { get; set; }

    public bool? Deleted { get; set; }

    public DateTime? DeletedDateTime { get; set; }

    public virtual ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();

    public virtual Project? Project { get; set; }

    public virtual User? Student { get; set; }
}
