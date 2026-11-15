using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class DreamTempleSky
{
    const int Width = 2048;
    const int Height = 1024;
    const string ImagePath = "Assets/Project/Png/DreamTempleSky.png";
    static Color32[] pixels;

    static float Smooth(float a, float b, float x)
    {
        float t = Mathf.Clamp01((x - a) / (b - a));
        return t * t * (3f - 2f * t);
    }

    static float WrapDistance(float a, float b)
    {
        float d = Mathf.Abs(a - b);
        return Mathf.Min(d, 1f - d);
    }

    static Color Mix(Color a, Color b, float t) => Color.Lerp(a, b, Mathf.Clamp01(t));

    static void Blend(int x, int y, Color color, float alpha)
    {
        if (y < 0 || y >= Height) return;
        x = (x % Width + Width) % Width;
        int i = y * Width + x;
        pixels[i] = Mix(pixels[i], color, alpha);
    }

    static void Ellipse(float cx, float cy, float rx, float ry, Color color, float opacity, float feather)
    {
        int x0 = Mathf.FloorToInt((cx - rx * 1.15f) * Width);
        int x1 = Mathf.CeilToInt((cx + rx * 1.15f) * Width);
        int y0 = Mathf.Max(0, Mathf.FloorToInt((cy - ry * 1.15f) * Height));
        int y1 = Mathf.Min(Height - 1, Mathf.CeilToInt((cy + ry * 1.15f) * Height));
        for (int y = y0; y <= y1; y++)
        for (int x = x0; x <= x1; x++)
        {
            float dx = (x / (float)Width - cx) / rx;
            float dy = (y / (float)Height - cy) / ry;
            float d = Mathf.Sqrt(dx * dx + dy * dy);
            Blend(x, y, color, opacity * (1f - Smooth(1f - feather, 1f, d)));
        }
    }

    static void Line(Vector2 a, Vector2 b, Color color, float opacity, float width)
    {
        float dx = (b.x - a.x) * Width;
        float dy = (b.y - a.y) * Height;
        int steps = Mathf.CeilToInt(Mathf.Sqrt(dx * dx + dy * dy) * 1.5f);
        for (int i = 0; i <= steps; i++)
        {
            float t = steps == 0 ? 0f : i / (float)steps;
            Ellipse(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t),
                width / Width, width / Height, color, opacity, 0.8f);
        }
    }

    static void Polygon(Vector2[] p, Color color, float opacity)
    {
        float minY = 1f, maxY = 0f;
        foreach (var v in p) { minY = Mathf.Min(minY, v.y); maxY = Mathf.Max(maxY, v.y); }
        float[] intersections = new float[p.Length];
        int y0 = Mathf.Max(0, Mathf.FloorToInt(minY * Height));
        int y1 = Mathf.Min(Height - 1, Mathf.CeilToInt(maxY * Height));
        for (int y = y0; y <= y1; y++)
        {
            float py = (y + 0.5f) / Height;
            int n = 0;
            for (int i = 0, j = p.Length - 1; i < p.Length; j = i++)
            {
                var a = p[j]; var b = p[i];
                if ((a.y > py) != (b.y > py))
                    intersections[n++] = (a.x + (py - a.y) * (b.x - a.x) / (b.y - a.y)) * Width;
            }
            Array.Sort(intersections, 0, n);
            for (int i = 0; i + 1 < n; i += 2)
                for (int x = Mathf.CeilToInt(intersections[i]); x < intersections[i + 1]; x++)
                    Blend(x, y, color, opacity);
        }
    }

    static void Mobius(float cx, float cy, float scale, Color edge)
    {
        Func<float, float, Vector2> point = (t, w) =>
        {
            float radius = 1f + w * Mathf.Cos(t * .5f);
            float x = radius * Mathf.Cos(t);
            float y = radius * Mathf.Sin(t);
            float z = w * Mathf.Sin(t * .5f);
            return new Vector2(cx + x * .014f * scale, cy + (y * .010f + z * .024f) * scale);
        };
        for (int i = 0; i < 80; i++)
        {
            float a = Mathf.PI * 2f * i / 80f, b = Mathf.PI * 2f * (i + 1) / 80f;
            Vector2 a1 = point(a, -.27f), a2 = point(a, .27f);
            Vector2 b1 = point(b, -.27f), b2 = point(b, .27f);
            Color shade = Mix(new Color(.21f, .29f, .40f), edge, .3f + .2f * Mathf.Cos(a));
            Polygon(new[] { a1, b1, b2, a2 }, shade, .94f);
            Line(a1, b1, edge, .8f, .48f);
            Line(a2, b2, edge, .8f, .48f);
        }
    }

    static void Island(float cx, float cy, float scale, bool warm, bool ribbon)
    {
        Color edge = warm ? new Color(.93f, .73f, .52f) : new Color(.60f, .77f, .85f);
        Color top = warm ? new Color(.58f, .50f, .51f) : new Color(.34f, .43f, .58f);
        Color dark = new Color(.12f, .18f, .29f);
        Color mid = warm ? new Color(.30f, .31f, .40f) : new Color(.23f, .31f, .43f);
        Func<float, float, Vector2> v = (x, y) => new Vector2(cx + x * scale, cy + y * scale);
        Vector2 a = v(-.039f, 0), b = v(-.024f, .018f), c = v(.018f, .021f);
        Vector2 d = v(.039f, .004f), e = v(.014f, -.014f), f = v(-.006f, -.054f);
        Vector2 g = v(-.028f, -.018f), center = v(-.002f, -.004f);
        Polygon(new[] { a, b, c, d, e, f, g }, dark, .94f);
        Polygon(new[] { a, b, c, d, center }, top, .92f);
        Polygon(new[] { a, center, g }, mid, .95f);
        Polygon(new[] { g, center, f }, dark, .85f);
        Polygon(new[] { center, d, e, f }, mid, .75f);
        Line(a, b, edge, .65f, .7f); Line(b, c, edge, .65f, .7f);
        Line(c, d, edge, .65f, .7f); Line(a, center, edge, .35f, .5f);
        Line(center, d, edge, .35f, .5f); Line(center, f, edge, .30f, .5f);

        if (ribbon)
        {
            Line(v(-.004f, .018f), v(-.004f, .042f), edge, .55f, .6f);
            Mobius(cx - .004f * scale, cy + .052f * scale, scale, edge);
        }
        else
        {
            Vector2 peak = v(-.004f, warm ? .079f : .061f), left = v(-.020f, .026f);
            Vector2 right = v(.012f, .026f), basePoint = v(-.004f, .010f);
            Polygon(new[] { peak, left, basePoint }, Mix(top, edge, .23f), .95f);
            Polygon(new[] { peak, right, basePoint }, mid, .95f);
            Line(peak, left, edge, .7f, .6f); Line(peak, right, edge, .7f, .6f);
            Line(left, right, edge, .45f, .5f); Line(peak, basePoint, edge, .5f, .5f);
        }
        Vector2 q1 = v(.020f, .040f), q2 = v(.026f, .022f), q3 = v(.020f, .016f), q4 = v(.014f, .022f);
        Polygon(new[] { q1, q2, q3, q4 }, mid, .9f);
        Line(q1, q2, edge, .75f, .5f); Line(q1, q4, edge, .75f, .5f);

        Vector2 prev = v(-.047f, .012f);
        for (int i = 1; i <= 36; i++)
        {
            float t = Mathf.PI * 2f * i / 36f;
            Vector2 point = v(-.047f * Mathf.Cos(t), .012f + .014f * Mathf.Sin(t));
            Line(prev, point, edge, .38f, .42f);
            prev = point;
        }
        Ellipse(cx - .034f * scale, cy - .031f * scale, .002f * scale, .004f * scale, edge, .55f, .4f);
        Ellipse(cx + .031f * scale, cy - .018f * scale, .0014f * scale, .0028f * scale, edge, .5f, .4f);
    }

    public static string Generate()
    {
        pixels = new Color32[Width * Height];
        var zenith = new Color(.035f, .055f, .14f);
        var high = new Color(.12f, .17f, .30f);
        var dusk = new Color(.32f, .35f, .48f);
        var dawn = new Color(.92f, .67f, .46f);
        var water = new Color(.11f, .20f, .30f);
        for (int y = 0; y < Height; y++)
        {
            float v = y / (float)(Height - 1);
            for (int x = 0; x < Width; x++)
            {
                float u = x / (float)(Width - 1);
                float sunrise = Mathf.Exp(-Mathf.Pow(WrapDistance(u, .75f) / .16f, 2f));
                float horizon = Mathf.Exp(-Mathf.Pow((v - .51f) / .17f, 2f));
                Color sky = Mix(dusk, high, Smooth(.52f, .78f, v));
                sky = Mix(sky, zenith, Smooth(.75f, 1f, v));
                sky = Mix(sky, dawn, sunrise * horizon * .85f);
                if (v < .48f)
                {
                    float reflection = Mathf.Exp(-Mathf.Pow(WrapDistance(u, .75f) / .14f, 2f)) *
                        Mathf.Exp(-Mathf.Pow((v - .40f) / .13f, 2f));
                    Color below = Mix(water, dawn, reflection * .43f);
                    below = Mix(below, dusk, Smooth(.24f, .49f, v) * .55f);
                    sky = Mix(sky, below, 1f - Smooth(.44f, .50f, v));
                }
                float cloudWave = .012f * Mathf.Sin(u * Mathf.PI * 6f + .4f) +
                                  .006f * Mathf.Sin(u * Mathf.PI * 14f + 1.1f);
                float cloud1 = Mathf.Exp(-Mathf.Pow((v - .445f - cloudWave) / .023f, 2f));
                float cloud2 = Mathf.Exp(-Mathf.Pow((v - .385f + cloudWave * .7f) / .035f, 2f));
                float cloud3 = Mathf.Exp(-Mathf.Pow((v - .31f - cloudWave * 1.1f) / .042f, 2f));
                float folds = .68f + .32f * Mathf.Sin(u * Mathf.PI * 18f + Mathf.Sin(u * Mathf.PI * 6f));
                Color cloudColor = Mix(new Color(.48f, .57f, .65f), new Color(.95f, .75f, .58f), sunrise * .7f);
                sky = Mix(sky, cloudColor, (cloud1 * .34f + cloud2 * .29f + cloud3 * .19f) * folds);
                pixels[y * Width + x] = sky;
            }
        }

        var rng = new System.Random(81273);
        for (int row = 0; row < 3; row++)
        {
            float cy = .305f + row * .052f;
            for (int i = 0; i < 34; i++)
            {
                float u = (i + (float)rng.NextDouble()) / 34f;
                float v = cy + ((float)rng.NextDouble() - .5f) * .026f;
                float rx = .013f + (float)rng.NextDouble() * .018f;
                float ry = .010f + (float)rng.NextDouble() * .013f;
                float warm = Mathf.Exp(-Mathf.Pow(WrapDistance(u, .75f) / .16f, 2f));
                Color shadow = Mix(new Color(.25f, .33f, .45f), new Color(.56f, .46f, .47f), warm);
                Color light = Mix(new Color(.59f, .67f, .73f), new Color(.98f, .77f, .61f), warm);
                Ellipse(u, v - .006f, rx * 1.15f, ry * 1.3f, shadow, .16f, .9f);
                Ellipse(u, v, rx, ry, light, .21f, .9f);
                Ellipse(u - rx * .25f, v + ry * .35f, rx * .55f, ry * .48f, light, .09f, .9f);
            }
        }
        for (int i = 0; i < 430; i++)
        {
            float u = (float)rng.NextDouble();
            float v = .66f + (float)rng.NextDouble() * .28f;
            float r = rng.NextDouble() < .08 ? 1.6f : .65f;
            float alpha = .2f + (float)rng.NextDouble() * .45f;
            Ellipse(u, v, r / Width, r / Height, new Color(.91f, .91f, .82f), alpha, .7f);
        }

        Ellipse(.75f, .548f, .048f, .096f, new Color(1f, .72f, .40f), .18f, .96f);
        Ellipse(.75f, .548f, .014f, .028f, new Color(1f, .88f, .67f), .83f, .13f);
        Ellipse(.25f, .615f, .039f, .078f, new Color(.64f, .77f, .94f), .15f, .95f);
        Ellipse(.25f, .615f, .020f, .040f, new Color(.82f, .85f, .82f), .9f, .08f);
        Ellipse(.244f, .623f, .003f, .006f, new Color(.58f, .64f, .70f), .25f, .7f);
        Ellipse(.257f, .604f, .004f, .008f, new Color(.58f, .64f, .70f), .22f, .7f);
        Ellipse(.250f, .607f, .002f, .004f, new Color(.58f, .64f, .70f), .24f, .7f);
        Ellipse(.75f, .416f, .019f, .020f, new Color(.96f, .76f, .57f), .19f, .9f);
        Ellipse(.25f, .397f, .016f, .018f, new Color(.72f, .81f, .85f), .12f, .9f);

        Island(.105f, .503f, .65f, false, false);
        Island(.385f, .505f, .82f, false, true);
        Island(.585f, .505f, .72f, true, true);
        Island(.895f, .502f, .92f, true, false);

        var texture = new Texture2D(Width, Height, TextureFormat.RGB24, false, false);
        try
        {
            texture.SetPixels32(pixels);
            texture.Apply();
            File.WriteAllBytes(ImagePath, texture.EncodeToPNG());
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(texture);
            pixels = null;
        }
        AssetDatabase.ImportAsset(ImagePath, ImportAssetOptions.ForceSynchronousImport);
        var importer = AssetImporter.GetAtPath(ImagePath) as TextureImporter;
        if (importer == null) throw new InvalidOperationException("Sky texture import failed: " + ImagePath);
        importer.textureType = TextureImporterType.Default;
        importer.sRGBTexture = true;
        importer.wrapModeU = TextureWrapMode.Repeat;
        importer.wrapModeV = TextureWrapMode.Clamp;
        importer.maxTextureSize = Width;
        importer.mipmapEnabled = true;
        importer.SaveAndReimport();
        return ImagePath;
    }
}
