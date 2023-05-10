using System;


namespace Server.Model
{
    public abstract class Img 
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public SkinsEnum Source { get; set; }
    }
}
