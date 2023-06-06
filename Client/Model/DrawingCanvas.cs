using System.Windows;
using System.Windows.Media;

namespace Client
{
    public class DrawingCanvas : FrameworkElement
    {             
        public VisualCollection Visual { get; set; }


        public DrawingCanvas()
        {
            
               Visual = new VisualCollection(this);
        }

        protected override int VisualChildrenCount
        {
            get { return Visual.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return Visual[index];
        }   
    }
}
