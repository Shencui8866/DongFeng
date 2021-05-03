// An highlighted block
using UnityEngine;

public static class RectTransformExtensions
{

    public static bool Overlaps(this RectTransform a, RectTransform b)
    {
        return a.WorldRect().Overlaps(b.WorldRect());
    }
    public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse)
    {
        return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
    }

    public static Rect WorldRect(this RectTransform rectTransform)
    {
        Vector2 sizeDelta = rectTransform.sizeDelta;
        float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
        float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

        Vector3 position = rectTransform.position;
        return new Rect(position.x + rectTransformWidth * rectTransform.pivot.x, position.y - rectTransformHeight * rectTransform.pivot.y, rectTransformWidth, rectTransformHeight);
    }

    public static bool Contains(this RectTransform rectTransform, Vector3 pos, Vector2 sizeData)
    {
        RectRange rectRangeA = new RectRange(rectTransform.position, rectTransform.sizeDelta);
        RectRange rectRangeB = new RectRange(pos, sizeData);
        return rectRangeA.Contains(rectRangeB.Range);
    }
    public static bool ContainsX(this RectTransform rectTransform, RectTransform a)
    {
        RectRangeAxis rectRangeA = new RectRangeAxis(rectTransform.position.x, rectTransform.sizeDelta);
        RectRangeAxis rectRangeB = new RectRangeAxis(a.position.x, a.sizeDelta);
        return rectRangeA.Contains(rectRangeB.Range);
    }
    public static bool ContainsY(this RectTransform rectTransform, RectTransform a)
    {
        RectRangeAxis rectRangeA = new RectRangeAxis(rectTransform.position.y, rectTransform.sizeDelta);
        RectRangeAxis rectRangeB = new RectRangeAxis(a.position.y, a.sizeDelta);
        return rectRangeA.Contains(rectRangeB.Range);
    }

    class RectRange
    {
        float minX, maxX, minY, maxY;

        public float[] Range { get { return new float[] { minX, maxX, minY, maxY }; } }

        public RectRange(Vector3 pos, Vector2 sizeData)
        {
            Vector2 sizeData2 = sizeData / 2;
            minX = pos.x - sizeData2.x;
            maxX = pos.x + sizeData2.x;
            minY = pos.y - sizeData2.y;
            maxY = pos.y + sizeData2.y;
        }

        public bool Contains(float[] range)
        {
            return (minX < range[0]) && (maxX > range[1]) && (minY < range[2]) && (maxY > range[3]);
        }
    }
    class RectRangeAxis
    {
        float minX, maxX;

        public float[] Range { get { return new float[] { minX, maxX}; } }

        public RectRangeAxis(float axis, Vector2 sizeData)
        {
            Vector2 sizeData2 = sizeData / 2;
            minX = axis - sizeData2.x;
            maxX = axis + sizeData2.x;
        }

        public bool Contains(float[] range)
        {
            return (minX < range[0]) && (maxX > range[1]);
        }
    }
}
