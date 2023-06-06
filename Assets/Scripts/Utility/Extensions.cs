using System;

namespace InditeHappiness.LLM
{
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

        public static string SpaceBeforeUpperLetter(this string target)
        {
            string result = string.Empty;

            for (int i = 0; i < target.Length; i++)
            {
                if (char.IsUpper(target[i]) == true)
                {
                    result += " ";
                }

                result += target[i];
            }

            return result;
        }
    }
}