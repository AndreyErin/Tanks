using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Client.Model
{
    public class WorldElement : Image
    {
        private WorldElement(){}

        public WorldElement(int id, MyPoint pos, SkinsEnum skin)
        {
            ID = id;
            ePos = pos;
            Source = GlobalDataStatic.SkinDictionary[skin];

            //размеры
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
                    _height = 40;
                    _width = 40;
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
                    _height = 30;
                    _width = 30;
                    break;
                //10
                case SkinsEnum.PictureBullet1:
                case SkinsEnum.PictureBullet2:
                case SkinsEnum.PictureBullet3:
                case SkinsEnum.PictureBullet4:
                    _height = 10;
                    _width = 10;
                    break;
            }
        }

        public int ID { get; set; }
        public double _height { get; set; }
        public double _width { get; set; }
        public SkinsEnum Skin { get; set; }      
        public MyPoint ePos { get; set; }

    }
}
