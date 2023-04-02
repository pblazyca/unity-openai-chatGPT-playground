using System;

public static class Extensions
{
    public static string LastPartOfTypeName(this Type type)
    {
        string[] typeNameFull = type.FullName?.Split('.');
        string typeNameLast = typeNameFull?[typeNameFull.Length - 1];
        return typeNameLast;
    }

    public static string ToLower(this Enum target)
    {
        return target.ToString().ToLower();
    }
}
