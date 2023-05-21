using System;
using System.Collections.Generic;

namespace Commivoyager;

public partial class Request
{
    public int Id { get; set; }

    public string Matrix { get; set; } = null!;

    public int Size { get; set; }

    public string BestPath { get; set; } = null!;

    public int BestDistance { get; set; }
}
