using System;
using System.Collections.Generic;

namespace Commivoyager;

public partial class UserException
{
    public int Id { get; set; }

    public string Message { get; set; } = null!;

    public string TargetSite { get; set; } = null!;

    public DateTime DateTimeExc { get; set; }
}
