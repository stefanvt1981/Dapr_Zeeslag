using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeeslagFrontEnd.Shared.Enums;

namespace ZeeslagFrontEnd.Shared.Model;

public class GameModel
{
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public Difficulty Difficulty { get; set; }

    public List<Records.Point> Shots { get; } = new List<Records.Point> { };
    public List<Records.Point> Hits { get; } = new List<Records.Point> { };
}
