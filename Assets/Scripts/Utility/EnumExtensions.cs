using System;

public static class EnumExtensions
{
    public static string ToLower(this Enum target)
    {
        return target.ToString().ToLower();
    }
}
