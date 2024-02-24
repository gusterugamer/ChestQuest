using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using Random = UnityEngine.Random;

namespace GusteruStudio.Extensions
{
    public static class Extensions { }

    public static class GameObjectExtensions
    {
        public static void DestroyAllChildren(this GameObject go)
        {
            foreach (Transform child in go.transform)
            {
                child.gameObject.Destroy();
            }
        }

        public static void DestroyImmediateAllChildren(this GameObject go)
        {
            for (var i = go.transform.childCount - 1; i >= 0; i--)
            {
                go.transform.GetChild(i).gameObject.DestroyImmediate();
            }
        }

        public static void Destroy(this GameObject go)
        {
            UnityEngine.Object.Destroy(go);
        }

        public static void DestroyImmediate(this GameObject go)
        {
            #if UNITY_EDITOR
            UnityEngine.Object.DestroyImmediate(go);
            #else
            UnityEngine.Object.Destroy(go);
            #endif
        }
        
        public static void ShuffleChildren(this GameObject go)
        {
            var goTransform = go.transform;
            var childCount = goTransform.childCount;
            for (var i = 0; i < goTransform.childCount; i++)
            {
                goTransform.GetChild(i).SetSiblingIndex(Random.Range(0, childCount));
            }
        }
        
        public static string Hierarchy(this GameObject go)
        {
            var str = new List<string>();
            str.Add(go.name);
            var tempT = go.transform;
            while (tempT.parent != null)
            {
                var parent = tempT.parent;
                str.Add(parent.name);
                tempT = parent;
            }
            str.Reverse();
            return string.Join(" -> ", str.ToArray());
        }
    }

    public static class ActionExtensions
    {
        public static void Fire(this Action action)
        {
            if (action != null)
                action();
        }

        public static void Fire<T>(this Action<T> action, T t)
        {
            if (action != null)
                action(t);
        }

        public static void Fire<T, U>(this Action<T, U> action, T t, U u)
        {
            if (action != null)
                action(t, u);
        }

        public static void Fire<T, U, V>(this Action<T, U, V> action, T t, U u, V v)
        {
            if (action != null)
                action(t, u, v);
        }

        public static void Fire<T, U, V, W>(this Action<T, U, V, W> action, T t, U u, V v, W w)
        {
            if (action != null)
                action(t, u, v, w);
        }
    }

    public static class ListExtensions
    {
        public static T Random<T>(this IEnumerable<T> collection)
        {
            if (collection.Count() == 0)
            {
                Debug.Log(string.Format("Array length is 0. Returning default({0})", 0, default(T).GetType()));
                return default(T);
            }
            return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count()));
        }

        public static T Random<T>(this IEnumerable<T> collection, Func<T, bool> pred)
        {
            collection = collection.Where(pred);
            if (collection.Count() == 0)
            {
                Debug.Log(string.Format("Array length is 0. Returning default({0})", 0, default(T).GetType()));
                return default(T);
            }
            return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count()));
        }

        public static List<T> Random<T>(this List<T> list, int objects)
        {
            var temp = new List<T>(objects);
            if (list.Count == 0 || objects > list.Count)
            {
                Debug.Log(string.Format("(List count is 0 || objects < list.Count). Returning default({0})", 0, default(T).GetType()));
                return default(List<T>);
            }
            else
            {
                for (int i = 0; i < objects; i++)
                {
                    var random = list.Random();
                    while (temp.Contains(random))
                    {
                        random = list.Random();
                    }
                    temp.Add(random);
                }
            }
            return temp;
        }

        public static List<T> WithItems<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
            return list;
        }

        public static void AddUnique<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void RemoveExisting<T>(this List<T> list, T item)
        {
            if (list.Contains(item))
            {
                list.Remove(item);
            }
        }

        public static void RemoveExistings<T>(this List<T> list, params T[] items)
        {
            foreach (var item in items)
                if (list.Contains(item))
                {
                    list.Remove(item);
                }
        }

        public static bool HasAny<T>(this List<T> list, params T[] items)
        {
            foreach (var item in items)
            {
                if (list.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasAll<T>(this List<T> list, params T[] items)
        {
            foreach (var item in items)
            {
                if (!list.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool HasItems<T>(this List<T> list)
        {
            return list != null && list.Count > 0;
        }

        public static T GetAndRemoveFirst<T>(this IList<T> list)
        {
            lock (list)
            {
                var value = list[0];
                list.RemoveAt(0);
                return value;
            }
        }

        public static List<T> SwapObjectsAtIndexes<T>(this List<T> list, int x, int y)
        {
            if (list.Count > Mathf.Max(x, y))
            {
                var temp = list[x];
                list[x] = list[y];
                list[y] = temp;
            }
            else
            {
                Debug.LogWarning(string.Format("Could not swap indexes: [{0}, {1}]. List count too low: [{2}].", x, y, list.Count));
            }
            return list;
        }
    }

    public static class DictionaryExtensions
    {
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (source == null)
                throw new ArgumentNullException("source");
            foreach (var element in source)
                target.Add(element);
        }

        public static string Jsonize(this Dictionary<string, object> dict)
        {
            var entries = dict.Select(d =>
                string.Format("\"{0}\": [{1}]", d.Key, d.Value));
            return "{" + string.Join("\n", entries.ToArray()) + "}";
        }

        public static string Print(this Dictionary<string, object> dictionary)
        {
            return string.Join(Environment.NewLine, dictionary.Select(a => $"{a.Key}: {a.Value}"));
        }
    }

    public static class ArrayExtensions
    {
        public static void ForEach<T>(T[] array, Action<T> action)
        {
            Array.ForEach(array, action);
        }

        public static T[] SwapObjectsAtIndexes<T>(this T[] array, int x, int y)
        {
            if (array.Length > Mathf.Max(x, y))
            {
                var temp = array[x];
                array[x] = array[y];
                array[y] = temp;
            }
            else
            {
                Debug.LogWarning(string.Format("Could not swap indexes: [{0}, {1}]. List count too low: [{2}].", x, y, array.Length));
            }
            return array;
        }

        public static T Random<T>(this T[] array)
        {
            if (array.Length == 0)
            {
                Debug.Log(string.Format("Array length is 0. Returning default({0})", 0, default(T).GetType()));
                return default(T);
            }
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        public static T[] Random<T>(this T[] array, int objects)
        {
            var temp = new T[objects];
            if (array.Length == 0 || objects > array.Length)
            {
                Debug.Log(string.Format("(Array length is 0 || objects < array.Length). Returning default({0})", 0, default(T).GetType()));
                return default(T[]);
            }
            else
            {
                for(int i = 0; i < objects; i++)
                {
                    var random = array.Random();
                    while(temp.Contains(random))
                    {
                        random = array.Random();
                    }
                    temp[i] = random;
                }
            }
            return temp;
        }

        public static T Random<T>(this T[,] array2d)
        {
            return array2d[UnityEngine.Random.Range(0, array2d.GetLength(0)), UnityEngine.Random.Range(0, array2d.GetLength(1))];
        }

        public static T Find<T>(this T[] array, T toFind)
        {
            foreach (var item in array)
                if (item.Equals(toFind))
                    return item;

            return default(T);
        }

        public static bool Contains<T>(this T[] array, T toFind)
        {
            foreach (var item in array)
                if (item.Equals(toFind))
                    return true;

            return false;
        }

        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.OrderBy(x => UnityEngine.Random.value);
        }
    }

    public static class TransformExtensions
    {
        public static void SetX(this Transform t, float newX)
        {
            t.position = new Vector3(newX, t.position.y, t.position.z);
        }

        public static void SetY(this Transform t, float newY)
        {
            t.position = new Vector3(t.position.x, newY, t.position.z);
        }

        public static void SetZ(this Transform t, float newZ)
        {
            t.position = new Vector3(t.position.x, t.position.y, newZ);
        }

        public static float GetX(this Transform t)
        {
            return t.position.x;
        }

        public static float GetY(this Transform t)
        {
            return t.position.y;
        }

        public static float GetZ(this Transform t)
        {
            return t.position.z;
        }

        public static void IncreaseX(this Transform t, float amount)
        {
            t.SetX(t.GetX() + amount);
        }

        public static void IncreaseY(this Transform t, float amount)
        {
            t.SetY(t.GetY() + amount);
        }

        public static void IncreaseZ(this Transform t, float amount)
        {
            t.SetZ(t.GetZ() + amount);
        }
        
        public static Transform GetRandomChild(this Transform go)
        {
            var transforms = go.transform.GetAllChildren();
            Debug.Log(transforms.Count);
            return transforms[Random.Range(0, transforms.Count)];
        }
        
        
        public static List<Transform> GetAllChildren(this Transform parent, List<Transform> transformList = null)
        {
            if (transformList == null) transformList = new List<Transform>();
          
            foreach (Transform child in parent) {
                transformList.Add(child);
                GetAllChildren(child, transformList);
            }
            return transformList;
        }
    }

    public enum AnchorPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottonCenter,
        BottomRight,
        BottomStretch,

        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,

        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,

        StretchAll
    }

    public enum PivotPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public static class RectTransformExtensions
    {
        public static void SetAnchor(this RectTransform source, AnchorPresets allign, int offsetX = 0, int offsetY = 0)
        {
            source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

            switch (allign)
            {
                case (AnchorPresets.TopLeft):
                    {
                        source.anchorMin = new Vector2(0, 1);
                        source.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case (AnchorPresets.TopCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 1);
                        source.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case (AnchorPresets.TopRight):
                    {
                        source.anchorMin = new Vector2(1, 1);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case (AnchorPresets.MiddleLeft):
                    {
                        source.anchorMin = new Vector2(0, 0.5f);
                        source.anchorMax = new Vector2(0, 0.5f);
                        break;
                    }
                case (AnchorPresets.MiddleCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 0.5f);
                        source.anchorMax = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case (AnchorPresets.MiddleRight):
                    {
                        source.anchorMin = new Vector2(1, 0.5f);
                        source.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }

                case (AnchorPresets.BottomLeft):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(0, 0);
                        break;
                    }
                case (AnchorPresets.BottonCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 0);
                        source.anchorMax = new Vector2(0.5f, 0);
                        break;
                    }
                case (AnchorPresets.BottomRight):
                    {
                        source.anchorMin = new Vector2(1, 0);
                        source.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case (AnchorPresets.HorStretchTop):
                    {
                        source.anchorMin = new Vector2(0, 1);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }
                case (AnchorPresets.HorStretchMiddle):
                    {
                        source.anchorMin = new Vector2(0, 0.5f);
                        source.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }
                case (AnchorPresets.HorStretchBottom):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case (AnchorPresets.VertStretchLeft):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case (AnchorPresets.VertStretchCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 0);
                        source.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case (AnchorPresets.VertStretchRight):
                    {
                        source.anchorMin = new Vector2(1, 0);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case (AnchorPresets.StretchAll):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }
            }
        }

        public static void SetPivot(this RectTransform source, PivotPresets preset)
        {

            switch (preset)
            {
                case (PivotPresets.TopLeft):
                    {
                        source.pivot = new Vector2(0, 1);
                        break;
                    }
                case (PivotPresets.TopCenter):
                    {
                        source.pivot = new Vector2(0.5f, 1);
                        break;
                    }
                case (PivotPresets.TopRight):
                    {
                        source.pivot = new Vector2(1, 1);
                        break;
                    }

                case (PivotPresets.MiddleLeft):
                    {
                        source.pivot = new Vector2(0, 0.5f);
                        break;
                    }
                case (PivotPresets.MiddleCenter):
                    {
                        source.pivot = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case (PivotPresets.MiddleRight):
                    {
                        source.pivot = new Vector2(1, 0.5f);
                        break;
                    }

                case (PivotPresets.BottomLeft):
                    {
                        source.pivot = new Vector2(0, 0);
                        break;
                    }
                case (PivotPresets.BottomCenter):
                    {
                        source.pivot = new Vector2(0.5f, 0);
                        break;
                    }
                case (PivotPresets.BottomRight):
                    {
                        source.pivot = new Vector2(1, 0);
                        break;
                    }
            }
        }
    }

    public static class RectTransformExtensions2
    {
        public static void AnchorToCorners(this RectTransform transform)
        {
            if (transform == null)
                throw new ArgumentNullException("transform");
            if (transform.parent == null)
                return;
            var parent = transform.parent.GetComponent<RectTransform>();
            var newAnchorsMin = new Vector2(transform.anchorMin.x + transform.offsetMin.x / parent.rect.width, transform.anchorMin.y + transform.offsetMin.y / parent.rect.height);
            var newAnchorsMax = new Vector2(transform.anchorMax.x + transform.offsetMax.x / parent.rect.width, transform.anchorMax.y + transform.offsetMax.y / parent.rect.height);
            transform.anchorMin = newAnchorsMin;
            transform.anchorMax = newAnchorsMax;
            transform.offsetMin = transform.offsetMax = new Vector2(0, 0);
        }

        public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
        {
            trans.pivot = aVec;
            trans.anchorMin = aVec;
            trans.anchorMax = aVec;
        }

        public static void AnchorInPivot(this RectTransform trans)
        {
            var	myx = Mathf.Abs(trans.anchoredPosition.x) / (trans.parent as RectTransform).GetWidth() + trans.anchoredPosition.x >= 0 ? 0.5f : 0f;
            var	myy = Mathf.Abs(trans.anchoredPosition.y) / (trans.parent as RectTransform).GetHeight() + trans.anchoredPosition.y >= 0 ? 0.5f : 0f;
            var vector = new Vector2(myx, myy);
            trans.anchorMin = vector;
            trans.anchorMax = vector;
        }


        public static Vector2 GetSize(this RectTransform trans)
        {
            return trans.rect.size;
        }

        public static float GetWidth(this RectTransform trans)
        {
            return trans.rect.width;
        }

        public static float GetHeight(this RectTransform trans)
        {
            return trans.rect.height;
        }

        public static void SetSize(this RectTransform trans, Vector2 newSize)
        {
            var oldSize = trans.rect.size;
            var deltaSize = newSize - oldSize;
            trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
        }

        public static void SetWidth(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(newSize, trans.rect.size.y));
        }

        public static void SetHeight(this RectTransform trans, float newSize)
        {
            SetSize(trans, new Vector2(trans.rect.size.x, newSize));
        }

        public static void SetBottomLeftPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }

        public static void SetTopLeftPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }

        public static void SetBottomRightPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
        }

        public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
        {
            trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
        }

    }

    public static class UITextExtensions
    {
        public static void Arabicize(this UnityEngine.UI.Text myText)
        {
            var correctedLines = new List<string>();
            for (int i = 0; i < myText.cachedTextGenerator.lines.Count; i++)
            {
                var startIndex = myText.cachedTextGenerator.lines[i].startCharIdx;
                var endIndex = (i == myText.cachedTextGenerator.lines.Count - 1) ? myText.text.Length : myText.cachedTextGenerator.lines[i + 1].startCharIdx;
                var length = endIndex - startIndex;
                var lineText = myText.text.Substring(startIndex, length);
                var corrected = string.Join(" ", lineText.Split(' ').Reverse().ToArray());
                correctedLines.Add(corrected);
            }
            var endString = string.Join(Environment.NewLine, correctedLines.ToArray());
            myText.text = endString;
        }

        public static string[] GetLinesData(this UnityEngine.UI.Text myText)
        {
            var lineStrings = new string[2];
            for (int i = 0; i < myText.cachedTextGenerator.lines.Count; i++)
            {
                var startIndex = myText.cachedTextGenerator.lines[i].startCharIdx;
                var endIndex = (i == myText.cachedTextGenerator.lines.Count - 1) ? myText.text.Length : myText.cachedTextGenerator.lines[i + 1].startCharIdx;
                var length = endIndex - startIndex;
                var lineText = myText.text.Substring(startIndex, length);
                lineStrings[i] = lineText;
            }
            return lineStrings;
        }
    }

    public static class CompExtensions
    {
        public static T GetOrAddComponent<T>(this Component child) where T : Component
        {
            return child.GetComponent<T>() ?? child.gameObject.AddComponent<T>();
        }

        public static void Destroy(this Component com)
        {
            Destroy(com);
        }
    }
    
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Color IncreaseAlpha(this Color color, float amount)
        {
            return new Color(color.r, color.g, color.b, color.a + amount);
        }

        public static Color DecreaseAlpha(this Color color, float amount)
        {
            return new Color(color.r, color.g, color.b, color.a - amount);
        }

        public static Color HexStringToColor(this string hex)
        {
            hex = hex.Replace("#", "");
            var a = 255f;
            var r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            if (hex.Length > 6)
            {
                a = int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        public static string AsHexColoredText(this string text, string hex)
        {
            return "<color=" + hex + ">" + text + "</color>";
        }

        public static string ToRGBHex(this Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", c.r.AsByte(), c.g.AsByte(), c.b.AsByte());
        }

        public static byte AsByte(this float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

    }

    public static class RendererExtensions
    {
        public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }
    }

    public static class IConvertibleExtensions
    {
        public static T To<T>(this IConvertible obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public static T ToOrDefault<T>(this IConvertible obj)
        {
            try
            {
                return To<T>(obj);
            }
            catch
            {
                return default(T);
            }
        }

        public static bool ToOrDefault<T>(this IConvertible obj, out T newObj)
        {
            try
            {
                newObj = To<T>(obj);
                return true;
            }
            catch
            {
                newObj = default(T);
                return false;
            }
        }

        public static bool ToOrOther<T>(this IConvertible obj, out T newObj, T other)
        {
            try
            {
                newObj = To<T>(obj);
                return true;
            }
            catch
            {
                newObj = other;
                return false;
            }
        }

        public static T ToOrNull<T>(this IConvertible obj) where T : class
        {
            try
            {
                return To<T>(obj);
            }
            catch
            {
                return null;
            }
        }

        public static bool ToOrNull<T>(this IConvertible obj, out T newObj) where T : class
        {
            try
            {
                newObj = To<T>(obj);
                return true;
            }
            catch
            {
                newObj = null;
                return false;
            }
        }
    }

    public static class EnumExtensions
    {
        public static int? EnumToInt<T>(this T _enum) where T : IConvertible, IComparable, IFormattable
        {
            if (Enum.IsDefined(typeof(T), _enum))
            {
                var intEnum = (int)Convert.ChangeType(_enum, typeof(int));
                return intEnum;
            }
            else
            {
                return null;
            }
        }
    }

    public static class StringExtensions
    {
        public static bool EmptyOrNull(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool TryParse<T>(this string valueToParse, out T returnValue)
        {
            returnValue = default(T);
            if (Enum.IsDefined(typeof(T), valueToParse))
            {
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                returnValue = (T)converter.ConvertFromString(valueToParse);
                return true;
            }
            return false;
        }

        public static int? TryIntParse(this string valueToParse)
        {
            int val;
            if (int.TryParse(valueToParse, out val))
                return val;
            else
                return null;
        }

        public static Int64? TryLongParse(this string valueToParse)
        {
            Int64 val;
            if (Int64.TryParse(valueToParse, out val))
                return val;
            else
                return null;
        }

        public static float? TryFloatParse(this string valueToParse)
        {
            float val;
            if (float.TryParse(valueToParse, out val))
                return val;
            else
                return null;
        }

        public static double? TryDoubleParse(this string valueToParse)
        {
            double val;
            if (double.TryParse(valueToParse, out val))
                return val;
            else
                return null;
        }

        public static DateTime? TryDateTimeParse(this string valueToParse)
        {
            DateTime val;
            if (DateTime.TryParse(valueToParse, out val))
                return val;
            else
                return null;
        }

        public static bool TryParse(this string valueToParse, out double returnValue)
        {
            return double.TryParse(valueToParse, out returnValue);
        }

        public static bool TryParse(this string valueToParse, out float returnValue)
        {
            return float.TryParse(valueToParse, out returnValue);
        }

        public static bool TryParse(this string valueToParse, out int returnValue)
        {
            return int.TryParse(valueToParse, out returnValue);
        }

        public static int IntParse(this string val)
        {
            return val.TryIntParse() ?? 0;
        }

        public static float FloatParse(this string val)
        {
            return val.TryFloatParse() ?? 0;
        }

        public static double DoubleParse(this string val)
        {
            return val.TryDoubleParse() ?? 0;
        }

        public static T EnumParse<T>(this string valueToParse) where T : struct
        {
            if (Enum.IsDefined(typeof(T), valueToParse))
            {
                return (T)Enum.Parse(typeof(T), valueToParse);
            }
            Debug.Log("The passed in string: [" + valueToParse + "] is not an ENUM and you are trying to parse it. Use TryParse instead.");
            return default(T);
        }

        public static T NumberParse<T>(this string valueToParse) where T : class
        {
            var parseResult = valueToParse.TryIntParse() ?? valueToParse.TryFloatParse() ?? valueToParse.TryDoubleParse();

            if (parseResult.HasValue)
            {
                return parseResult as T;
            }
            throw new UnityException("The passed in string: [" + valueToParse + "] is not an int, float, or even double and you are trying to parse it. Use TryParse instead.");
        }

        public static string ReversedWords(this string str)
        {
            return string.Join(" ", str.Split(' ').Reverse().ToArray());
        }

        public static string FirstLetterToUpperCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }

    public static class IntExtensions
    {
        public static int roundUp5(this int n)
        {
            return (n + 4) / 5 * 5;
        }


        public static int Abs(this int val)
        {
            return Mathf.Abs (val);
        }
    }

    public static class FloatExtensions
    {
        public static int ToInt(this float obj)
        {
            return (int)Convert.ChangeType(obj, typeof(int));
        }

        public static float Abs(this float val)
        {
            return Mathf.Abs (val);
        }

        public static float RoundToDigit(this float val, int digit = 2)
        {
            return (float)Math.Round(val, digit);
        }
    }

    public static class DoubleExtensions
    {
        public static int ToInt(this double obj)
        {
            return (int)Convert.ChangeType(obj, typeof(int));
        }
    }

    public static class Vector2Extensions
    {
        public static Vector2 TextureCoordToLocalPosition(this Vector2 coord, Vector2 size)
        {
            var x = coord.x < size.x / 2 ? -size.x / 2 + coord.x : coord.x - size.x / 2;
            var y = coord.y < size.y / 2 ? -size.y / 2 + coord.y : coord.y - size.y / 2;
            return new Vector2(x, y);
        }

        public static Vector2 LocalPositionToTextureCoord(this Vector2 localPosition, Vector2 size)
        {
            var x = size.x / 2 + localPosition.x;
            var y = size.y / 2 + localPosition.y;
            return new Vector2(x, y);
        }

        public static float Random(this Vector2 vector)
        {
            return UnityEngine.Random.Range(vector.x, vector.y);
        }

        public static Vector2 SetX(this Vector2 vector, float value)
        {
            vector.x = value;
            return vector;
        }
        
        public static Vector2 SetY(this Vector2 vector, float value)
        {
            vector.y = value;
            return vector;
        }
    }

    public static class Vector3Extensions
    {
        public static Vector3 SetX(this Vector3 vector, float value)
        {
            vector.x = value;
            return vector;
        }
        
        public static Vector3 SetY(this Vector3 vector, float value)
        {
            vector.y = value;
            return vector;
        }
        
        public static Vector3 SetZ(this Vector3 vector, float value)
        {
            vector.z = value;
            return vector;
        }

        public static Vector3 Random(this Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(UnityEngine.Random.Range(vector1.x, vector2.x), UnityEngine.Random.Range(vector1.y, vector2.y), UnityEngine.Random.Range(vector1.z, vector2.z));
        }
    }
    
    public static class Vector3IntExtensions
    {
        public static Vector3Int ToVector3Int(this Vector3 val)
        {
            return new Vector3Int(val.x.ToInt(), val.y.ToInt(), val.z.ToInt());
        }
    }

    public static class SymbolExtensions
    {
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        public static MethodInfo GetMethodInfo(LambdaExpression expression)
        {
            var outermostExpression = expression.Body as MethodCallExpression;

            if (outermostExpression == null)
            {
                throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
            }

            return outermostExpression.Method;
        }
    }

    public static class GenericExtensions
    {
        public static bool IsBetween<T>(this T actual, T lower, T upper, bool excludeMax = false) where T : IComparable<T>
        {
            if (!excludeMax)
                return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) <= 0;
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }

        public static bool Has<T>(this T source, params T[] list)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            return list.Contains(source);
        }


        public static List<T> InList<T>(this T item)
        {
            return new List<T> { item };
        }

        public static T[] InArray<T>(this T item)
        {
            return new[] { item };
        }
    }

    public static class CharExtensions
    {
        public static bool IsLowerCase(this char ch)
        {
            return ch >= 'a' && ch <= 'z';
        }
    }
}