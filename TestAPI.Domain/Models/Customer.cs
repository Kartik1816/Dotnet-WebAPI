using System;
using System.Collections.Generic;

namespace TestAPI.Domain.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Address { get; set; }

    public string NickName { get; set; } = null!;

    public bool? IsDeleted { get; set; }
}
