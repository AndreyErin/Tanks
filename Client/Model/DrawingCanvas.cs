using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ImageTest
{
    public class DrawingCanvas : FrameworkElement
    {
        private VisualCollection CollectionVisualElements;

        public DrawingCanvas()
        {
            CollectionVisualElements = new VisualCollection(this);
        }

        //возвращаем объект по индексу
        protected override Visual GetVisualChild(int index)
        {
            return CollectionVisualElements[index];
        }

        //возвращаем количество элементов
        protected override int VisualChildrenCount {
            get 
            {
                return CollectionVisualElements.Count;
            }
        }

        //добавляем элемент
        public void AddVisual(Visual visual)
        {
            CollectionVisualElements.Add(visual);
        }

        //удаляем элемент
        public void DeleteVisual(Visual visual)
        {
            CollectionVisualElements.Remove(visual);
        }

        //получить количество элементов из вне
        public int Count() { return CollectionVisualElements.Count; }

        //получить доступ к объекту из вне
        public Visual GetElement(int index) { return CollectionVisualElements[index]; }
    }
}
