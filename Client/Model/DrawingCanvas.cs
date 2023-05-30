using Client.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Client
{
    public class DrawingCanvas : FrameworkElement
    {
        private VisualCollection CollectionVisualElements;

        //конструктор
        public DrawingCanvas()
        {
            CollectionVisualElements = new VisualCollection(this);
            //Height = 720;
            //Width = 1320;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

        }

        //обязательные служебные
        protected override Visual GetVisualChild(int index)
        {
            return CollectionVisualElements[index];
        }

        //обязательные служебные
        protected override int VisualChildrenCount {
            get 
            {
                return CollectionVisualElements.Count;
            }
        }

        //добавляем элемент
        public void AddElement(Visual visual)
        {
            CollectionVisualElements.Add(visual);
        }

        //удаляем элемент
        public void DeleteElement(Visual visual)
        {
            CollectionVisualElements.Remove(visual);
        }

        //получить количество элементов из вне
        public int Count() { return CollectionVisualElements.Count; }

        //получить доступ к объекту из вне
        public Visual GetElement(int index) { return CollectionVisualElements[index]; }

        public void ClearChildrens() { CollectionVisualElements.Clear(); }
    }
}
