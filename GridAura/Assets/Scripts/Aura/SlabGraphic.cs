using UnityEngine;
using UnityEngine.UI;

namespace Aura
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class SlabGraphic : MaskableGraphic
    {
        public Color face = new Color(1f, 0.82f, 0.12f, 1f);
        public Color highlight = new Color(1f, 0.95f, 0.55f, 1f);
        public Color shade = new Color(0.75f, 0.55f, 0f, 1f);
        public float corner = 10f;
        public float bevel = 7f;
        public bool rim;
        public bool flat;

        public void SetColors(Color faceColor, Color highlightColor, Color shadeColor, bool lightRim)
        {
            face = faceColor;
            highlight = highlightColor;
            shade = shadeColor;
            rim = lightRim;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect rect = GetPixelAdjustedRect();
            float radius = Mathf.Clamp(corner, 2f, Mathf.Min(rect.width, rect.height) * 0.45f);
            if (flat)
            {
                AddRounded(vh, rect, radius, face, face);
                return;
            }
            float lip = Mathf.Min(bevel, rect.height * 0.22f);
            var body = new Rect(rect.x, rect.y + lip, rect.width, rect.height - lip);
            AddRounded(vh, new Rect(rect.x, rect.y, rect.width, rect.height - lip * 0.15f), radius, shade, shade);
            AddRounded(vh, body, radius, highlight, face);
            AddEdgeLight(vh, body, radius);
        }

        void AddEdgeLight(VertexHelper vh, Rect body, float radius)
        {
            float inset = Mathf.Clamp(radius * 0.28f, 2f, 6f);
            float thick = Mathf.Clamp(body.height * 0.035f, 1.5f, 3.5f);
            Color edge = new Color(1f, 1f, 1f, rim ? 0.7f : 0.85f);
            Color fade = new Color(1f, 1f, 1f, 0f);
            float top = body.yMax - inset;
            float left = body.xMin + inset;
            float right = body.xMax - inset;
            Quad(vh,
                new Vector2(left, top - thick), new Vector2(left, top),
                new Vector2(right, top), new Vector2(right, top - thick * 1.6f),
                edge, edge, fade, fade);
            float sideBottom = body.yMin + body.height * 0.42f;
            Quad(vh,
                new Vector2(left, sideBottom), new Vector2(left, top),
                new Vector2(left + thick, top), new Vector2(left + thick * 1.4f, sideBottom),
                fade, edge, edge, fade);
        }

        void AddRounded(VertexHelper vh, Rect rect, float radius, Color top, Color bottom)
        {
            radius = Mathf.Min(radius, rect.width * 0.5f, rect.height * 0.5f);
            var center = rect.center;
            int origin = vh.currentVertCount;
            vh.AddVert(center, ColorAt(rect, center.y, top, bottom), Vector2.zero);
            const int segments = 5;
            AddArc(vh, rect, new Vector2(rect.xMax - radius, rect.yMax - radius), radius, 0f, 90f, segments, top, bottom);
            AddArc(vh, rect, new Vector2(rect.xMin + radius, rect.yMax - radius), radius, 90f, 180f, segments, top, bottom);
            AddArc(vh, rect, new Vector2(rect.xMin + radius, rect.yMin + radius), radius, 180f, 270f, segments, top, bottom);
            AddArc(vh, rect, new Vector2(rect.xMax - radius, rect.yMin + radius), radius, 270f, 360f, segments, top, bottom);
            int count = vh.currentVertCount - origin - 1;
            for (int i = 0; i < count; i++)
                vh.AddTriangle(origin, origin + 1 + i, origin + 1 + ((i + 1) % count));
        }

        void AddArc(VertexHelper vh, Rect rect, Vector2 origin, float radius, float start, float end, int segments, Color top, Color bottom)
        {
            for (int i = 0; i <= segments; i++)
            {
                float t = Mathf.Lerp(start, end, i / (float)segments) * Mathf.Deg2Rad;
                var point = origin + new Vector2(Mathf.Cos(t), Mathf.Sin(t)) * radius;
                vh.AddVert(point, ColorAt(rect, point.y, top, bottom), Vector2.zero);
            }
        }

        static Color ColorAt(Rect rect, float y, Color top, Color bottom)
        {
            float t = Mathf.InverseLerp(rect.yMin, rect.yMax, y);
            return Color.Lerp(bottom, top, Mathf.Clamp01(t));
        }

        static void Quad(VertexHelper vh, Vector2 a, Vector2 b, Vector2 c, Vector2 d, Color ca, Color cb, Color cc, Color cd)
        {
            int i = vh.currentVertCount;
            vh.AddVert(a, ca, Vector2.zero);
            vh.AddVert(b, cb, Vector2.zero);
            vh.AddVert(c, cc, Vector2.zero);
            vh.AddVert(d, cd, Vector2.zero);
            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i, i + 2, i + 3);
        }
    }
}
