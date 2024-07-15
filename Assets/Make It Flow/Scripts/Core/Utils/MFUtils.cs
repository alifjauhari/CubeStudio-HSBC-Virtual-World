using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MeadowGames.MakeItFlow
{
    // v1.1 - removed wrong inheritance from MFUtils
    public static class MFUtils
    {
        public static void AddToList<T>(T obj, List<T> list)
        {
            if (!list.Contains(obj))
            {
                list.Add(obj);
            }
        }

        public static void RemoveFromList<T>(T obj, List<T> list)
        {
            if (list.Contains(obj))
            {
                list.Remove(obj);
            }
        }

        public static Vector3 LerpWithoutClamp(Vector3 A, Vector3 B, float t)
        {
            return A + (B - A) * t;
        }

        public static float LerpWithoutClampAxis(float A, float B, float t)
        {
            return A + (B - A) * t;
        }

        // from https://stackoverflow.com/a/6137889/7781272
        public static string SplitCamelCase(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        // from https://stackoverflow.com/a/7170950/7781272
        public static string TrimEnd(this string source, string value, bool caseSensitive = false)
        {
            return source.EndsWith(value) ? source.Remove(source.LastIndexOf(value)) : source;
        }

        public static int SortByPriority(MFObject o1, MFObject o2)
        {
            return o2.MFCanvasManager.Priority.CompareTo(o1.MFCanvasManager.Priority);
        }

#if UNITY_EDITOR
        // from https://answers.unity.com/questions/1425758/how-can-i-find-all-instances-of-a-scriptable-objec.html?childToView=1460944#comment-1460944
        public static T[] FindAllInstances<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }
#endif
    }
}