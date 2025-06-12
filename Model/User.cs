using System;
using System.Collections.Generic;

namespace StudentAPI.Model;

public partial class User
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string PassWord { get; set; } = null!;

    public string Role { get; set; } = null!;
}
