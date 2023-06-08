using System;
using System.Windows;
using System.Windows.Media;

namespace Client.Model
{
    public class WorldElement
    {
        public int ID { get; set; }
        public MyPoint ePos { get; set; }
        public VectorEnum Vector { get; set; }
        public SkinsEnum Skin { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
                
        public WorldElement()
        {
            ID = 0;
            ePos = null;
            Skin = SkinsEnum.None;
            Vector = VectorEnum.Top;
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

        //позиция и вектор
        public void PosAndVectorElement(double posX = -10, double posY = -10, VectorEnum vectorEnum = VectorEnum.Top) 
        {
            //позиция
            if (posX != -10)
            {
                if (posX < ePos.X)
                    Vector = VectorEnum.Top;
                else
                    Vector = VectorEnum.Down;

                ePos.X = posX;
            }

            if (posY != -10)
            {
                if (posY < ePos.Y)
                    Vector = VectorEnum.Left;
                else
                    Vector = VectorEnum.Right;

                ePos.Y = posY;
            }

            //вектор
            if (vectorEnum != VectorEnum.Top)           
                Vector = vectorEnum;
        }

        //удаление объекта
        public void DeleteMe() 
        {
            try
            {
                GlobalDataStatic.Controller.CollectionWorldElements.Remove(this);
                GlobalDataStatic.StackElements.Push(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Удаление элемента. Класс WorldElement - клиент\n" + ex.Message);
            }
           
        }

        public void AddMe(int id, MyPoint pos, SkinsEnum skin, VectorEnum vectorEnum) 
        {
            ID = id;
            ePos = pos;
            Skin = skin;
            Vector = vectorEnum;
            SizeElement(Skin);
            GlobalDataStatic.Controller.CollectionWorldElements.Add(this);
        }
    }
}
