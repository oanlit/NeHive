using Avalonia;
using Avalonia.Platform;
using Avalonia.Media;

using System.Xml.Linq;
using System.Text;

namespace NeHive.UI.Avalonia.Utils;

public static class SvgUtil
{
    public static string LoadSvgString(string uri)
    {
        if (!uri.StartsWith("~/") && !uri.StartsWith("avares://")) return File.ReadAllText(uri);

        if (uri.StartsWith("~/"))
            uri = $"{NeHiveContext.ProjBaseUri}{uri[2..]}";

        var stream = AssetLoader.Open(new Uri(uri));
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
    
    public static Geometry? ParseGeometry(string svg)
    {
        var doc = XElement.Parse(svg);

        var geometries = new GeometryGroup();

        foreach (var element in doc.Descendants())
        {
            switch (element.Name.LocalName)
            {
                case "path":
                    AddPath(element, geometries);
                    break;

                case "rect":
                    AddRect(element, geometries);
                    break;

                case "circle":
                    AddCircle(element, geometries);
                    break;

                case "line":
                    AddLine(element, geometries);
                    break;

                case "polyline":
                    AddPolyline(element, geometries);
                    break;

                case "polygon":
                    AddPolygon(element, geometries);
                    break;
            }
        }

        return geometries.Children.Count > 0
            ? geometries
            : null;
    }
    
    private static void AddPath(
        XElement e,
        GeometryGroup group)
    {
        var d = e.Attribute("d")?.Value;

        if (!string.IsNullOrWhiteSpace(d))
        {
            group.Children.Add(
                Geometry.Parse(d));
        }
    }
    
    private static void AddRect(
        XElement e,
        GeometryGroup group)
    {
        var x = GetDouble(e, "x");
        var y = GetDouble(e, "y");
        var w = GetDouble(e, "width");
        var h = GetDouble(e, "height");

        var rect = new Rect(x, y, w, h);

        var rx = GetDouble(e, "rx");
        var ry = GetDouble(e, "ry");

        Geometry geometry;

        if (rx > 0 || ry > 0)
        {
            geometry = new RectangleGeometry(rect, rx, ry);
        }
        else
        {
            geometry = new RectangleGeometry(rect);
        }

        group.Children.Add(geometry);
    }
    
    private static void AddLine(
        XElement e,
        GeometryGroup group)
    {
        var x1 = GetDouble(e, "x1");
        var y1 = GetDouble(e, "y1");
        var x2 = GetDouble(e, "x2");
        var y2 = GetDouble(e, "y2");

        group.Children.Add(
            Geometry.Parse($"M{x1},{y1} L{x2},{y2}"));
    }
    
    private static void AddCircle(
        XElement e,
        GeometryGroup group)
    {
        var cx = GetDouble(e, "cx");
        var cy = GetDouble(e, "cy");
        var r = GetDouble(e, "r");

        group.Children.Add(
            new EllipseGeometry(
                new Rect(
                    cx - r,
                    cy - r,
                    r * 2,
                    r * 2)));
    }
    
    private static void AddPolyline(
        XElement e,
        GeometryGroup group)
    {
        var points = ParsePoints(
            e.Attribute("points")?.Value);

        if (points.Count == 0)
            return;

        var sb = new StringBuilder();

        sb.Append($"M{points[0].X},{points[0].Y}");

        for (int i = 1; i < points.Count; i++)
        {
            sb.Append($" L{points[i].X},{points[i].Y}");
        }

        group.Children.Add(
            Geometry.Parse(sb.ToString()));
    }
    
    private static void AddPolygon(
        XElement e,
        GeometryGroup group)
    {
        var points = ParsePoints(
            e.Attribute("points")?.Value);

        if (points.Count == 0)
            return;

        var sb = new StringBuilder();

        sb.Append($"M{points[0].X},{points[0].Y}");

        for (int i = 1; i < points.Count; i++)
        {
            sb.Append($" L{points[i].X},{points[i].Y}");
        }

        sb.Append(" Z");

        group.Children.Add(
            Geometry.Parse(sb.ToString()));
    }
    
    private static double GetDouble(
        XElement e,
        string name)
    {
        return double.TryParse(
            e.Attribute(name)?.Value,
            out var v)
            ? v
            : 0;
    }
    
    private static List<Point> ParsePoints(string? points)
    {
        var result = new List<Point>();

        if (string.IsNullOrWhiteSpace(points))
            return result;

        var tokens = points
            .Replace(",", " ")
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < tokens.Length - 1; i += 2)
        {
            if (double.TryParse(tokens[i], out var x) &&
                double.TryParse(tokens[i + 1], out var y))
            {
                result.Add(new Point(x, y));
            }
        }

        return result;
    }
}