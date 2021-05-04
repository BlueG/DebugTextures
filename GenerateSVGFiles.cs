using System;
using System.IO;
using System.Text;


namespace CreateUVTileSVG
{
    class Program
    {
        const int ImageSize = 2048;
        const int NCells = 16;
        const int CellSize = ImageSize / NCells;

        const float ThinStrokeThickness = 2;
        const float NormalStrokeThickness = 4;
        const float ThickStrokeThickness = 6;

        static readonly Color Black = Color.FromRGBA(39, 39, 39);
        static readonly Color White = Color.FromRGBA(232, 232, 232);

        static readonly Color Gray1 = interpolate(Black, White, 0.25f);
        static readonly Color Gray2 = interpolate(Black, White, 0.50f);
        static readonly Color Gray3 = interpolate(Black, White, 0.75f);

        static readonly Color Red = Color.FromRGBA(180, 39, 39);
        static readonly Color Green = Color.FromRGBA(39, 180, 39);
        static readonly Color Blue = Color.FromRGBA(39, 80, 180);
        static readonly Color Yellow = Color.FromRGBA(220, 200, 39);

        TextWriter writer;


        static void Main() => new Program().Run();


        void Run()
        {
            CreateDebugGrid("tex_DebugGrid.svg");
            CreateDebugUVTiles("tex_DebugUVTiles.svg", generateCells: true);
            CreateDebugUVTiles("tex_DebugAlignment.svg", generateCells: false);
        }


        void CreateDebugGrid(string filename)
        {
            using (writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                //  Begin File
                {
                    writer.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                    writer.WriteLine($@"<svg width=""{ImageSize}"" height=""{ImageSize}"" version=""1.1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"">");
                    writer.WriteLine();
                }

                //  Definitions
                //
                //  Note: These are needed to clip lines and circles to the page
                {
                    writer.WriteLine(@"<defs>");
                    writer.WriteLine(@"  <mask id=""pageMask"">");
                    writer.WriteLine($@"    <rect x=""0"" y=""0"" width=""{ImageSize}"" height=""{ImageSize}"" fill=""white"" />");
                    writer.WriteLine(@"  </mask>");
                    writer.WriteLine(@"</defs>");
                }


                //  Background
                {
                    ColorRectangle(0, 0, ImageSize, ImageSize, Black);
                }


                //  Draw Grid Lines
                {
                    const int nMainCells = 8;
                    const int nSubCellsPerMainCell = 8;
                    const int nSubCells = nSubCellsPerMainCell * nMainCells;

                    const int mainCellSize = ImageSize / nMainCells;
                    const int subCellSize = ImageSize / nSubCells;

                    for (int i = 0; i <= nSubCells; i++)
                    {
                        if (i % nSubCellsPerMainCell != 0)
                        {
                            float p = i * subCellSize;

                            Line(0, p, ImageSize, p, ThinStrokeThickness, Gray2);
                            Line(p, 0, p, ImageSize, ThinStrokeThickness, Gray2);
                        }
                    }

                    for (int i = 0; i <= nMainCells; i++)
                    {
                        float p = i * mainCellSize;

                        Line(0, p, ImageSize, p, ThickStrokeThickness, Gray3);
                        Line(p, 0, p, ImageSize, ThickStrokeThickness, Gray3);
                    }
                }


                //  End File
                {
                    writer.WriteLine();
                    writer.WriteLine(@"</svg>");
                    writer.Close();
                }
            }
        }


        void CreateDebugUVTiles(string filename, bool generateCells)
        {
            using (writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                //  Begin File
                {
                    writer.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                    writer.WriteLine($@"<svg width=""{ImageSize}"" height=""{ImageSize}"" version=""1.1"" xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"">");
                    writer.WriteLine();
                }

                //  Definitions
                //
                //  Note: These are needed to clip lines and circles to the page
                {
                    writer.WriteLine(@"<defs>");
                    writer.WriteLine(@"  <mask id=""pageMask"">");
                    writer.WriteLine($@"    <rect x=""0"" y=""0"" width=""{ImageSize}"" height=""{ImageSize}"" fill=""white"" />");
                    writer.WriteLine(@"  </mask>");
                    writer.WriteLine(@"</defs>");
                }

                //  Draw Color Squares
                {
                    if (generateCells)
                    {
                        for (int x = 0; x < NCells; x++)
                        {
                            for (int y = 0; y < NCells; y++)
                            {
                                float tx = (float)x / (NCells - 1);
                                float ty = (float)y / (NCells - 1);

                                Color cx0 = interpolate(Green, Yellow, tx);
                                Color cx1 = interpolate(Blue, Red, tx);
                                Color c = interpolate(cx0, cx1, ty);

                                if ((x + y) % 2 == 1) c = interpolate(c, Black, 0.25f);

                                ColorRectangle(GetCell(x, y), c);
                            }
                        }
                    }
                    else
                    {
                        ColorRectangle(0, 0, ImageSize, ImageSize, Black);
                    }
                }


                //  Draw Cell Diagonals
                {
                    if (generateCells)
                    {
                        float o = ThickStrokeThickness / 2;

                        for (int i = 2; i < NCells; i += 2)
                        {
                            float p0 = 0;
                            float p1 = i * CellSize;

                            float mp0 = ImageSize - p0;
                            float mp1 = ImageSize - p1;

                            Line(p0, p1, p1, p0, NormalStrokeThickness, Gray3.WithA(0.5f));
                            Line(mp0, mp1, mp1, mp0, NormalStrokeThickness, Gray3.WithA(0.5f));

                            Line(p0, p1, mp1, mp0, NormalStrokeThickness, Gray3.WithA(0.5f));
                            Line(p1, p0, mp0, mp1, NormalStrokeThickness, Gray3.WithA(0.5f));
                        }
                    }
                }


                //  Draw Cell Interior Lines
                {
                    if (generateCells)
                    {
                        for (int i = 0; i < NCells * 4; i++)
                        {
                            if (i % 4 != 0)
                            {
                                float p = i * CellSize / 4;

                                Line(0, p, ImageSize, p, ThinStrokeThickness, Yellow.WithA(0.5f));
                                Line(p, 0, p, ImageSize, ThinStrokeThickness, Yellow.WithA(0.5f));
                            }
                        }
                    }
                }


                //  Draw Cell Borders
                {
                    for (int i = 1; i < NCells; i++)
                    {
                        float p = i * CellSize;

                        Line(0, p, ImageSize, p, NormalStrokeThickness, Gray2);
                        Line(p, 0, p, ImageSize, NormalStrokeThickness, Gray2);
                    }
                }


                //  Draw Extra Angle Lines
                {
                    float thickness = NormalStrokeThickness;
                    Color c = Gray3.WithA(0.75f);

                    float extent = ImageSize / 2f;

                    float p0 = extent - extent; //  for consitency (;-p)
                    float p1 = extent - extent * tan(30f);
                    float p2 = extent - extent * tan(15f);
                    //float p3 = extent;
                    float p4 = extent + extent * tan(15f);
                    float p5 = extent + extent * tan(30f);
                    float p6 = extent + extent;

                    Line(p0, p1, p6, p5, thickness, c);
                    Line(p0, p2, p6, p4, thickness, c);
                    Line(p0, p4, p6, p2, thickness, c);
                    Line(p0, p5, p6, p1, thickness, c);

                    Line(p1, p0, p5, p6, thickness, c);
                    Line(p2, p0, p4, p6, thickness, c);
                    Line(p4, p0, p2, p6, thickness, c);
                    Line(p5, p0, p1, p6, thickness, c);
                }


                //  Draw Corner and Center Circles
                {
                    Color c1 = Yellow;
                    Color c1b = interpolate(Black, c1, 0.5f);
                    Color c2 = Yellow.WithA(0.75f);

                    float extent = ImageSize / 2;
                    float thickness = NormalStrokeThickness;
                    float thicknessb = NormalStrokeThickness + 1;

                    //  Half markers

                    Circle(extent, extent, extent * 0.5f, thickness, c2);

                    Circle(extent - ImageSize, extent, extent * 1.5f, thickness, c2);
                    Circle(extent + ImageSize, extent, extent * 1.5f, thickness, c2);
                    Circle(extent, extent - ImageSize, extent * 1.5f, thickness, c2);
                    Circle(extent, extent + ImageSize, extent * 1.5f, thickness, c2);

                    Circle(extent - ImageSize, extent - ImageSize, extent * 1.5f, thickness, c2);
                    Circle(extent + ImageSize, extent - ImageSize, extent * 1.5f, thickness, c2);
                    Circle(extent - ImageSize, extent + ImageSize, extent * 1.5f, thickness, c2);
                    Circle(extent + ImageSize, extent + ImageSize, extent * 1.5f, thickness, c2);


                    // Corners

                    Circle(0, 0, extent, thickness, c1);
                    Circle(ImageSize, 0, extent, thickness, c1);
                    Circle(ImageSize, ImageSize, extent, thickness, c1);
                    Circle(0, ImageSize, extent, thickness, c1);


                    //  Outlines

                    Circle(extent, extent, extent, thicknessb, c1b);

                    Circle(extent - ImageSize, extent, extent * 2f, thicknessb, c1b);
                    Circle(extent + ImageSize, extent, extent * 2f, thicknessb, c1b);
                    Circle(extent, extent - ImageSize, extent * 2f, thicknessb, c1b);
                    Circle(extent, extent + ImageSize, extent * 2f, thicknessb, c1b);

                    Circle(extent - ImageSize, extent - ImageSize, extent * 2f, thicknessb, c1b);
                    Circle(extent + ImageSize, extent - ImageSize, extent * 2f, thicknessb, c1b);
                    Circle(extent - ImageSize, extent + ImageSize, extent * 2f, thicknessb, c1b);
                    Circle(extent + ImageSize, extent + ImageSize, extent * 2f, thicknessb, c1b);


                    //  Center

                    Circle(extent, extent, extent, thickness, c1);

                    Circle(extent - ImageSize, extent, extent * 2f, thickness, c1);
                    Circle(extent + ImageSize, extent, extent * 2f, thickness, c1);
                    Circle(extent, extent - ImageSize, extent * 2f, thickness, c1);
                    Circle(extent, extent + ImageSize, extent * 2f, thickness, c1);

                    Circle(extent - ImageSize, extent - ImageSize, extent * 2f, thickness, c1);
                    Circle(extent + ImageSize, extent - ImageSize, extent * 2f, thickness, c1);
                    Circle(extent - ImageSize, extent + ImageSize, extent * 2f, thickness, c1);
                    Circle(extent + ImageSize, extent + ImageSize, extent * 2f, thickness, c1);
                }


                //  Draw Main Diagonals
                {
                    Color c = Gray3;
                    float thickness = ThickStrokeThickness;

                    float p0 = 0;
                    float p1 = ImageSize / 2f;
                    float p2 = ImageSize;

                    Line(p0, p0, p2, p2, thickness, c);
                    Line(p0, p2, p2, p0, thickness, c);

                    Line(p0, p1, p1, p0, thickness, c);
                    Line(p1, p0, p2, p1, thickness, c);
                    Line(p2, p1, p1, p2, thickness, c);
                    Line(p1, p2, p0, p1, thickness, c);
                }


                //  Draw Image Borders and Center Lines
                {
                    Color c = Gray3;
                    float thickness = ThickStrokeThickness;

                    float p0 = 0;
                    float p1 = ImageSize / 2f;
                    float p2 = ImageSize;

                    Line(p0, p0, p2, p0, thickness, c);
                    Line(p0, p1, p2, p1, thickness, c);
                    Line(p0, p2, p2, p2, thickness, c);

                    Line(p0, p0, p0, p2, thickness, c);
                    Line(p1, p0, p1, p2, thickness, c);
                    Line(p2, p0, p2, p2, thickness, c);
                }


                //  Draw Cell Text
                {
                    if (generateCells)
                    {
                        string ColLets = "ABCDEFGHIJKLMNOP";

                        for (int x = 0; x < NCells; x++)
                        {
                            for (int y = 0; y < NCells; y++)
                            {
                                Box cell = GetCell(x, y);
                                Point center = cell.center;
                                string text = ColLets.Substring(x, 1) + (NCells - y - 1).ToString();

                                Text(center.x, center.y, text, cell.size.y * 0.3f, White.WithA(0.75f), Black.WithA(0.75f));
                            }
                        }
                    }
                }


                //  End File
                {
                    writer.WriteLine();
                    writer.WriteLine(@"</svg>");
                    writer.Close();
                }
            }
        }


        void Circle(float cx, float cy, float radius, float thickness, Color color)
        {
            writer.Write($"<circle cx=\"{cx}\" cy=\"{cy}\" r=\"{radius}\" fill=\"none\" mask=\"url(#pageMask)\" ");
            writer.Write($"style=\"stroke:{color.ToRGBString()}; stroke-width:{thickness}; stroke-opacity:{color.a:F3}\" />");
            writer.WriteLine();
        }


        //  old code kept for potential future use
        //
        //  was previously used for drawing circles clipped to page
        //  it works, but using masks to clip the circles make the code easier to understand
        //
        void Arc(float cx, float cy, float radius, float start, float end, float thickness, Color color)
        {
            Point startpt = FromPolar(cx, cy, radius, start);
            Point endpt = FromPolar(cx, cy, radius, end);

            int largeflag = end - start > 180.0f ? 1 : 0;

            writer.Write("<path ");
            writer.Write("d=\"");
            writer.Write($"M {endpt.x} {endpt.y} ");
            writer.Write($"A {radius} {radius} 0 {largeflag} 0 {startpt.x} {startpt.y} ");
            writer.Write("\" ");
            writer.Write($"style=\"fill: none; stroke:{color.ToRGBString()}; stroke-width:{thickness}; stroke-opacity:{color.a:F3}\"");
            writer.WriteLine("/>");
        }


        void ColorRectangle(Box box, Color color) => ColorRectangle(box.min.x, box.min.y, box.max.x, box.max.y, color);

        void ColorRectangle(float x1, float y1, float x2, float y2, Color color)
        {
            writer.Write($"<rect x=\"{x1}\" y=\"{y1}\" width=\"{x2 - x1}\" height=\"{y2 - y1}\" ");
            writer.Write($"style=\"stroke-width:0; fill:{color.ToRGBString()}; fill-opacity:{color.a:F3}\" />");
            writer.WriteLine();
        }


        void Line(float x1, float y1, float x2, float y2, float thickness, Color color)
        {
            writer.Write($"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" mask=\"url(#pageMask)\" ");
            writer.Write($"style=\"stroke:{color.ToRGBString()}; stroke-width:{thickness}; stroke-opacity:{color.a:F3}\" />");
            writer.WriteLine();
        }


        void Text(float cx, float cy, string text, float fontSize, Color fill, Color stroke)
        {
            writer.Write($"<text x=\"{cx}\" y=\"{cy}\" dominant-baseline=\"middle\" text-anchor=\"middle\" ");
            writer.Write($"style=\"font-family:sans-serif; font-size:{fontSize}; font-weight:bold; ");
            writer.Write($"fill:{fill.ToRGBString()}; fill-opacity:{fill.a:F3}; ");
            writer.Write($"stroke:{stroke.ToRGBString()}; stroke-opacity:{stroke.a:F3}\" >");
            writer.Write(text);
            writer.WriteLine("</text>");
        }


        static Box GetCell(int x, int y)
        {
            Point min = new Point(x * CellSize, y * CellSize);
            Point max = new Point(min.x + CellSize, min.y + CellSize);

            return new Box() { min = min, max = max };
        }


        static Point FromPolar(float cx, float cy, float r, float angle)
        {
            var rads = deg2rad(angle);

            return new Point
            {
                x = (float)(cx + r * Math.Cos(rads)),
                y = (float)(cy + r * Math.Sin(rads)),
            };
        }


        static Color interpolate(Color c1, Color c2, float t)
        {
            t = clamp(t);

            return new Color
            {
                r = c1.r + (c2.r - c1.r) * t,
                g = c1.g + (c2.g - c1.g) * t,
                b = c1.b + (c2.b - c1.b) * t,
                a = c1.a + (c2.a - c1.a) * t,
            };
        }


        static float clamp(float value, float min = 0f, float max = 1f)
            => value < min ? min : (value > max ? max : value);


        static double deg2rad(float value) => value * Math.PI / 180.0;

        static float cos(float value) => (float)Math.Cos(deg2rad(value));
        static float sin(float value) => (float)Math.Sin(deg2rad(value));
        static float tan(float value) => (float)Math.Tan(deg2rad(value));
    }


    struct Point
    {
        public float x;
        public float y;

        public Point(float x, float y) { this.x = x; this.y = y; }

        public static Point operator +(Point lhs, Point rhs) => new Point { x = lhs.x + rhs.x, y = lhs.y + rhs.y };
        public static Point operator -(Point lhs, Point rhs) => new Point { x = lhs.x - rhs.x, y = lhs.y - rhs.y };
    }


    struct Box
    {
        public Point min;
        public Point max;

        public Point center => new Point { x = (min.x + max.x) / 2, y = (min.y + max.y) / 2 };

        public Point size
        {
            get => max - min;
            set => max = min + value;
        }
    }


    struct Color
    {
        public float r;
        public float g;
        public float b;
        public float a;


        public static Color FromRGBA(int r, int g, int b, float a = 1f)
        {
            return new Color
            {
                r = (float)r / 255,
                g = (float)g / 255,
                b = (float)b / 255,
                a = a
            };
        }


        public string ToRGBString()
        {
            int rh = clamp((int)(r * 255), 0, 255);
            int gh = clamp((int)(g * 255), 0, 255);
            int bh = clamp((int)(b * 255), 0, 255);

            return $"rgb({rh},{gh},{bh})";
        }


        public Color WithA(float a) => new Color { r = r, g = g, b = b, a = a };


        private static int clamp(int value, int min, int max) => value < min ? min : (value > max ? max : value);
    }
}
