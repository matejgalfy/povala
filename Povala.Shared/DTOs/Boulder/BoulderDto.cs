using Povala.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Povala.Shared.DTOs.Boulder;

public class BoulderDto
{
    public int Id { get; set; }

    public BoulderDifficulty Difficulty { get; set; } = BoulderDifficulty.V0;
    public BoulderColor Color { get; set; } = BoulderColor.Blue;

    public double CoordX { get; set; }
    public double CoordY { get; set; }
}

