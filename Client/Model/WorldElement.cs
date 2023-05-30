using System;
using System.Windows;
using System.Windows.Media;

namespace Client.Model
{
    public class WorldElement : DrawingVisual
    {
        public int ID { get; set; }
        public MyPoint ePos { get; set; }
        public VectorEnum vector { get; set; }
        private double vek { get; set; } = 0;

        private double Width { get; set; }
        private double Height { get; set; }

        private WorldElement(){}

        //конструктор
        public WorldElement(int id, MyPoint pos, SkinsEnum skin, VectorEnum vectorEnum = VectorEnum.Top)
        {            
            ID = id;
            ePos = pos;           
            //размер перед скином
            SizeElement(skin);

            SkinElement(skin);

            vector = vectorEnum;

            AddMe();
        }

        //размер
        protected void SizeElement(SkinsEnum skin) 
        {
            switch (skin)
            {
            //40
            case SkinsEnum.PictureBlock1:
            case SkinsEnum.PictureBlock2:
            case SkinsEnum.PictureBlock3:
            case SkinsEnum.PictureBlockFerum1:
            case SkinsEnum.PictureBlockFerum2:
            case SkinsEnum.PictureBlockFerum3:
            case SkinsEnum.PictureWood1:
            case SkinsEnum.PictureBunker:
            case SkinsEnum.PictureBunker2:
            case SkinsEnum.PictureBunker3:
            case SkinsEnum.PictureBunkerEnamy:
            case SkinsEnum.PictureBunkerEnamy2:
            case SkinsEnum.PictureBunkerEnamy3:
                Height = 40;
                Width = 40;
                break;
            //30
            case SkinsEnum.PictureTank1:
            case SkinsEnum.PictureTank2:
            case SkinsEnum.PictureTank3:
            case SkinsEnum.PictureTank4:
            case SkinsEnum.PictureTankSpeed:
            case SkinsEnum.PictureTankSpeed2:
            case SkinsEnum.PictureTankOfDestroy1:
            case SkinsEnum.PictureTankOfDestroy2:
            case SkinsEnum.PictureTankOfDestroy3:
            case SkinsEnum.PictureTankOfDestroySpeed1:
            case SkinsEnum.PictureTankOfDestroySpeed2:
            case SkinsEnum.PictureLootTeer:
            case SkinsEnum.PictureLootSpeed:
            case SkinsEnum.PictureTankBot1:
            case SkinsEnum.PictureTankBot2:
            case SkinsEnum.PictureTankBot3:
            case SkinsEnum.PictureTankBot4:
            case SkinsEnum.PictureTankSpeedBot:
            case SkinsEnum.PictureTankSpeedBot2:
            case SkinsEnum.PictureLocationGun1:
            case SkinsEnum.PictureLocationGun2:
            case SkinsEnum.PictureLocationGun3:
                Height = 30;
                Width = 30;
                break;
            //10
            case SkinsEnum.PictureBullet1:
            case SkinsEnum.PictureBullet2:
            case SkinsEnum.PictureBullet3:
            case SkinsEnum.PictureBullet4:
                Height = 10;
                Width = 10;
                break;
            }
        }

        //скин
        public void SkinElement(SkinsEnum skin)
        {
            DrawingContext dc = base.RenderOpen();
            dc.DrawImage(GlobalDataStatic.SkinDictionary[skin], new Rect(0, 0, Width, Height));
            dc.Close();

            TransformGroup Tgroup = new TransformGroup()
            {
                Children =
                {
                    new RotateTransform(vek, Width/2, Height/2),
                    new TranslateTransform(ePos.Y, ePos.X)
                }
            };
            base.Transform = Tgroup;
        }

        //позиция и вектор
        public void PosAndVectorElement(double posX = -10, double posY = -10, VectorEnum vectorEnum = VectorEnum.Top) 
        {
            //позиция
            if (posX != -10)
            {                
                if (posX < ePos.X)
                    vector = VectorEnum.Top;
                else
                    vector = VectorEnum.Down;

                ePos.X = posX;
            }

            if (posY != -10)
            {
                if (posY < ePos.Y)
                vector = VectorEnum.Left;
                else
                vector = VectorEnum.Right;

                ePos.Y = posY;
            }

            //вектор
            if (vectorEnum != VectorEnum.Top)           
                vector = vectorEnum;

                switch (vector)
                {
                    case VectorEnum.Top:
                        vek = 0;
                        break;
                    case VectorEnum.Down:
                        vek = 180;
                        break;
                    case VectorEnum.Left:
                        vek = 270;
                        break;
                    case VectorEnum.Right:
                        vek = 90;
                        break;
                }
            
            TransformGroup Tgroup = new TransformGroup()
            {
                Children =
                {
                    new RotateTransform(vek, Width/2, Height/2),
                    new TranslateTransform(ePos.Y, ePos.X)
                }
            };
            Transform = Tgroup;
        }

        //удаление объекта
        public void DeleteMe() 
        {
            try
            {
                GlobalDataStatic.Controller.cnvMain.DeleteElement(this);
                bool result = GlobalDataStatic.Controller.SearchElement.TryRemove(ID, out WorldElement worldElement);
                if (!result) { MessageBox.Show("Удаление из словаря не прокатило"); }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Удаление элемента. Класс WorldElement - клиент\n" + ex.Message);
            }
            
        }

        protected void AddMe() 
        {
            GlobalDataStatic.Controller.cnvMain.AddElement(this);

            bool result = GlobalDataStatic.Controller.SearchElement.TryAdd(ID, this);
            if (!result) { MessageBox.Show("Добавление в словарь не прокатило"); }
        }
    }
}
