namespace Tama.ImageViewer
{
    [System.Serializable]
    public struct Color
    {
        public byte A; // Alpha channel
        public byte R; // Red channel
        public byte G; // Green channel
        public byte B; // Blue channel

        [System.NonSerialized]
        System.Drawing.Color SystemDrawingColor; // Backing field for System.Drawing.Color

        // Constructor for ARGB
        public Color(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;

            SystemDrawingColor = System.Drawing.Color.FromArgb(A, R, G, B);
        }

        // Constructor for RGB (Alpha defaults to 255)
        public Color(byte r, byte g, byte b)
        {
            A = byte.MaxValue;
            R = r;
            G = g;
            B = b;

            SystemDrawingColor = System.Drawing.Color.FromArgb(A, R, G, B);
        }

        // Constructor from System.Drawing.Color
        public Color(System.Drawing.Color systemDrawingColor)
        {
            A = systemDrawingColor.A;
            R = systemDrawingColor.R;
            G = systemDrawingColor.G;
            B = systemDrawingColor.B;

            SystemDrawingColor = systemDrawingColor;
        }

        // Override ToString for easier debugging
        public override string ToString()
        {
            return $"Color(A: {A}, R: {R}, G: {G}, B: {B})";
        }

        public System.Drawing.Color ToSystemDrawingColor()
        {
            SystemDrawingColor = System.Drawing.Color.FromArgb(A, R, G, B);

            return SystemDrawingColor;
        }
    }
}