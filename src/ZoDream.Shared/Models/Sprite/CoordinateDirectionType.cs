using System;

namespace ZoDream.Shared.Models
{
    [Flags]
    public enum CoordinateDirectionType: byte
    {
        Left = 0b1000,
        Right = 0b0100,
        Up = 0b0010,
        Down = 0b0001,
        Normal = 0b0101,
        RightDown = 0b0101,
        LeftDown = 0b1001,
        RightUp = 0b0110,
        LeftUp = 0b1010,
    }
}