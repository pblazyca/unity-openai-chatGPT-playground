using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static string LastPartOfTypeName(this Type type)
    {
        string[] typeNameFull = type.FullName?.Split('.');
        string typeNameLast = typeNameFull?[typeNameFull.Length - 1];
        return typeNameLast;
    }
}
