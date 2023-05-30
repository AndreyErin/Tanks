using Client.Model;
using System.Windows;
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
        public void DeleteElement(int id)
        {
            WorldElement? visual = GetElementByID(id);

            if (visual != null) CollectionVisualElements.Remove(visual);
        }

        //получить количество элементов из вне
        public int Count() { return CollectionVisualElements.Count; }

        public void ClearChildrens() { CollectionVisualElements.Clear(); }

        //получаем элемент по индексу
        private WorldElement? GetElementByID(int id) 
        {
            foreach (WorldElement visual in CollectionVisualElements) 
            {
                if(visual.ID == id) return visual;
            }

            return null;
        }

        //обновляем скин элемента
        public void SkinUpload(int id, SkinsEnum skin) 
        {
            WorldElement? visual = GetElementByID(id);

            if (visual != null) visual.SkinElement(skin);
        }

        //позиция элемента
        public void PosElement(int id, double x = -10, double y = -10) 
        {
            WorldElement? visual = GetElementByID(id);

            if (visual != null) visual.PosAndVectorElement(posX: x, posY: y);
        }

    }
}
