

using System.Windows;

namespace Server.Model
{
    public abstract class WorldElement
    {
        protected double _height { get; set; }
        protected double _width { get; set; }
        protected SkinsEnum _skin { get; set; }
        public Point ePos { get; set; }
    }
}
