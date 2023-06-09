using System;
using UnityEditor;
using UnityEngine;

namespace InditeHappiness.LLM
{
    public static class Utils
    {
        private const string NONE_SCRIPTABLE_OBJECT_ERROR_MESSAGE = "There is no Scriptable Object typeof {0}.";
        private const string NOT_UNIQUE_SCRIPTABLE_OBJECT_WARNING_MESSAGE = "Scriptable Object typeof {0} exist multiple times.";

        public static T LoadInstanceForEditorTools<T>(Type type) where T : ScriptableObject
        {
            string[] findAssets = AssetDatabase.FindAssets($"t:{type.Name}");

            if (findAssets == null || findAssets.Length == 0)
            {
                Debug.LogError(string.Format(NONE_SCRIPTABLE_OBJECT_ERROR_MESSAGE, type));
            }
            else if (findAssets.Length > 1)
            {
                Debug.LogWarning(string.Format(NOT_UNIQUE_SCRIPTABLE_OBJECT_WARNING_MESSAGE, type));
            }
            else
            {
                return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(findAssets[0]));
            }

            return null;
        }
    }
}