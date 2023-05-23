using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Client.Model
{
    static class GlobalDataStatic
    {
        public static MainWindow Controller { get; set; }

        //музыка
        public static Dictionary<SoundsEnum, Uri> SoundDictionary = new Dictionary<SoundsEnum, Uri>() 
        {            
            [SoundsEnum.menuSound] = new Uri(@"Model\Sounds\menuSound.mp3", UriKind.Relative),
            [SoundsEnum.bonusSound] = new Uri(@"Model\Sounds\bonus.mp3", UriKind.Relative),
            [SoundsEnum.mainSound] = new Uri(@"Model\Sounds\mainSound.mp3", UriKind.Relative),
            [SoundsEnum.ferumSoung] = new Uri(@"Model\Sounds\ferum.mp3", UriKind.Relative),
            [SoundsEnum.rockSound] = new Uri(@"Model\Sounds\rock.mp3", UriKind.Relative),
            [SoundsEnum.shotSoung] = new Uri(@"Model\Sounds\shot.mp3", UriKind.Relative),
            [SoundsEnum.shotTargetSound] = new Uri(@"Model\Sounds\brue.mp3", UriKind.Relative)
        };

        //скины
        public static Dictionary<SkinsEnum, BitmapImage> SkinDictionary = new Dictionary<SkinsEnum, BitmapImage>()
        {
            //каменные блоки
            [SkinsEnum.PictureBlock1] = new BitmapImage(new Uri(@"Model\Pictures\blok.png", UriKind.Relative)),
            [SkinsEnum.PictureBlock2] = new BitmapImage(new Uri(@"Model\Pictures\blok2.png", UriKind.Relative)),
            [SkinsEnum.PictureBlock3] = new BitmapImage(new Uri(@"Model\Pictures\blok3.png", UriKind.Relative)),
            //железные блоки
            [SkinsEnum.PictureBlockFerum1] = new BitmapImage(new Uri(@"Model\Pictures\ferum.png", UriKind.Relative)),
            [SkinsEnum.PictureBlockFerum2] = new BitmapImage(new Uri(@"Model\Pictures\ferum2.png", UriKind.Relative)),
            [SkinsEnum.PictureBlockFerum3] = new BitmapImage(new Uri(@"Model\Pictures\ferum3.png", UriKind.Relative)),
            //деревья
            [SkinsEnum.PictureWood1] = new BitmapImage(new Uri(@"Model\Pictures\wood.png", UriKind.Relative)),
            //танк-бот
            [SkinsEnum.PictureTank1] = new BitmapImage(new Uri(@"Model\Pictures\playertank.png", UriKind.Relative)),
            [SkinsEnum.PictureTank2] = new BitmapImage(new Uri(@"Model\Pictures\playertank2.png", UriKind.Relative)),
            [SkinsEnum.PictureTank3] = new BitmapImage(new Uri(@"Model\Pictures\playertank3.png", UriKind.Relative)),
            [SkinsEnum.PictureTank4] = new BitmapImage(new Uri(@"Model\Pictures\playertank4.png", UriKind.Relative)),
            [SkinsEnum.PictureTankSpeed] = new BitmapImage(new Uri(@"Model\Pictures\tankspeed.png", UriKind.Relative)),
            [SkinsEnum.PictureTankSpeed2] = new BitmapImage(new Uri(@"Model\Pictures\tankspeed2.png", UriKind.Relative)),
            //бункер
            [SkinsEnum.PictureBunker] = new BitmapImage(new Uri(@"Model\Pictures\bunker.png", UriKind.Relative)),
            [SkinsEnum.PictureBunker2] = new BitmapImage(new Uri(@"Model\Pictures\bunker2.png", UriKind.Relative)),
            [SkinsEnum.PictureBunker3] = new BitmapImage(new Uri(@"Model\Pictures\bunker3.png", UriKind.Relative)),
            //вражеский бункер
            [SkinsEnum.PictureBunkerEnamy] = new BitmapImage(new Uri(@"Model\Pictures\bunkerEnamy.png", UriKind.Relative)),
            [SkinsEnum.PictureBunkerEnamy2] = new BitmapImage(new Uri(@"Model\Pictures\bunkerEnamy2.png", UriKind.Relative)),
            [SkinsEnum.PictureBunkerEnamy3] = new BitmapImage(new Uri(@"Model\Pictures\bunkerEnamy3.png", UriKind.Relative)),
            //разрушенный танк
            [SkinsEnum.PictureTankOfDestroy1] = new BitmapImage(new Uri(@"Model\Pictures\tankofdestroy.png", UriKind.Relative)),
            [SkinsEnum.PictureTankOfDestroy2] = new BitmapImage(new Uri(@"Model\Pictures\tankofdestroy2.png", UriKind.Relative)),
            [SkinsEnum.PictureTankOfDestroy3] = new BitmapImage(new Uri(@"Model\Pictures\tankofdestroy3.png", UriKind.Relative)),
            [SkinsEnum.PictureTankOfDestroySpeed1] = new BitmapImage(new Uri(@"Model\Pictures\tankofdestroyspeed1.png", UriKind.Relative)),
            [SkinsEnum.PictureTankOfDestroySpeed2] = new BitmapImage(new Uri(@"Model\Pictures\tankofdestroyspeed2.png", UriKind.Relative)),
            //лут
            [SkinsEnum.PictureLootTeer] = new BitmapImage(new Uri(@"Model\Pictures\Damage.png", UriKind.Relative)),
            [SkinsEnum.PictureLootSpeed] = new BitmapImage(new Uri(@"Model\Pictures\Speed.png", UriKind.Relative)),
            //танк-бот
            [SkinsEnum.PictureTankBot1] = new BitmapImage(new Uri(@"Model\Pictures\tankbot.png", UriKind.Relative)),
            [SkinsEnum.PictureTankBot2] = new BitmapImage(new Uri(@"Model\Pictures\tankbot2.png", UriKind.Relative)),
            [SkinsEnum.PictureTankBot3] = new BitmapImage(new Uri(@"Model\Pictures\tankbot3.png", UriKind.Relative)),
            [SkinsEnum.PictureTankBot4] = new BitmapImage(new Uri(@"Model\Pictures\tankbot4.png", UriKind.Relative)),
            [SkinsEnum.PictureTankSpeedBot] = new BitmapImage(new Uri(@"Model\Pictures\tankspeedbot.png", UriKind.Relative)),
            [SkinsEnum.PictureTankSpeedBot2] = new BitmapImage(new Uri(@"Model\Pictures\tankspeedbot2.png", UriKind.Relative)),
            //стационарная пушка-бот
            [SkinsEnum.PictureLocationGun1] = new BitmapImage(new Uri(@"Model\Pictures\LocationGun.png", UriKind.Relative)),
            [SkinsEnum.PictureLocationGun2] = new BitmapImage(new Uri(@"Model\Pictures\LocationGun2.png", UriKind.Relative)),
            [SkinsEnum.PictureLocationGun3] = new BitmapImage(new Uri(@"Model\Pictures\LocationGun3.png", UriKind.Relative)),
            //пуля
            [SkinsEnum.PictureBullet1] = new BitmapImage(new Uri(@"Model\Pictures\bullet.png", UriKind.Relative)),
            [SkinsEnum.PictureBullet2] = new BitmapImage(new Uri(@"Model\Pictures\bullet2.png", UriKind.Relative)),
            [SkinsEnum.PictureBullet3] = new BitmapImage(new Uri(@"Model\Pictures\bullet3.png", UriKind.Relative)),
            [SkinsEnum.PictureBullet4] = new BitmapImage(new Uri(@"Model\Pictures\bullet4.png", UriKind.Relative))
        };
    }
}
