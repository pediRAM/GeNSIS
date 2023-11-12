namespace GeNSIS.Core.Models
{
    public struct PosDim
    {
        public PosDim(int x, int y, int w, int h)
        {
            X = x; 
            Y = y; 
            W = w; 
            H = h;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        public int AddToX(int x) => X += x;
        public int AddToY(int y) => Y += y;
        public int GetNextX() => X + W + 1;

        public PosDim GetNextPosDim(int w) => new PosDim(X, Y, W+w, H);

        public override string ToString() => $"{X}% {Y}u {W}% {H}u";
    }
}
