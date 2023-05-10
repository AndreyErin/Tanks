using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace Server.Model
{
    public class Tree : System.Windows.Controls.Image
    {
        protected Tree() { }
        public Tree(System.Windows.Point ePos)
        {
            Width = 40;
            Height = 40;
            
            Source = Map.PictureWood1;

            Canvas.SetLeft(this, ePos.Y);
            Canvas.SetTop(this, ePos.X);
            Canvas.SetZIndex(this, 1); //- поверх остальных обьектов

            GlobalDataStatic.cnvMap1.Children.Add(this);           
        }
    }
}
