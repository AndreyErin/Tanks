using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client.Model
{
    public class WorldElement : Image
    {
        public int ID { get; set; }
        //protected double _height { get; set; }
        //protected double _width { get; set; }
        public MyPoint ePos { get; set; }
        public VectorEnum vector { get; set; }  

        private WorldElement(){}

        //4й параметр не обязателен
        public WorldElement(int id, MyPoint pos, SkinsEnum skin, VectorEnum vectorEnum = VectorEnum.Top)
        {            
            ID = id;
            ePos = pos;
            Source = GlobalDataStatic.SkinDictionary[skin];
            
            SizeElement(skin);

            vector = vectorEnum;
            if (vector != VectorEnum.Top)
                VectorElement(vector);

            Canvas.SetTop(this, ePos.X);
            Canvas.SetLeft(this, ePos.Y);
            GlobalDataStatic.Controller.cnvMain.Children.Add(this);
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

        //направление
        public void VectorElement(VectorEnum vectorEnum) 
        {
            switch (vectorEnum)
            {
            case VectorEnum.Top:
                this.LayoutTransform = new RotateTransform(0);
                break;
            case VectorEnum.Down:
                this.LayoutTransform = new RotateTransform(180);
                break;
            case VectorEnum.Left:
                this.LayoutTransform = new RotateTransform(270);
                break;
            case VectorEnum.Right:
                this.LayoutTransform = new RotateTransform(90);
                break;
            }
        }

        //скин
        public void SkinElement(SkinsEnum skin)
        {
            Source = GlobalDataStatic.SkinDictionary[skin];
        }

        //движение
        public void MoveElement(double x = -10, double y = -10 ) 
        {
           
            if (x != -10)
            {
                Canvas.SetTop(this, x);
                if (x < ePos.X)
                    vector = VectorEnum.Top;
                else
                    vector = VectorEnum.Down;

                ePos.X = x;
            }

            if (y != -10)
            {
                Canvas.SetLeft(this, y);
                if (y < ePos.Y)
                vector = VectorEnum.Left;
                else
                vector = VectorEnum.Right;

                ePos.Y = y;
            }

            //если направление сменилось
 
             VectorElement(vector);           
        }

        //удаление объекта
        public void DeleteElement() 
        {
            GlobalDataStatic.Controller.cnvMain.Children.Remove(this);
        }
    }
}
