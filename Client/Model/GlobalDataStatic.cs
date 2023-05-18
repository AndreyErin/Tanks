using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Client.Model
{
    static class GlobalDataStatic
    {
        //музыка
        public static Dictionary<SoundsEnum, Uri> SoundDictionary = new Dictionary<SoundsEnum, Uri>() 
        {            
            [SoundsEnum.menuSound] = new Uri(@"Sounds\menuSound.mp3", UriKind.Relative),
            [SoundsEnum.bonusSound] = new Uri(@"Sounds\bonus.mp3", UriKind.Relative),
            [SoundsEnum.mainSound] = new Uri(@"Sounds\mainSound.mp3", UriKind.Relative),
            [SoundsEnum.ferumSoung] = new Uri(@"Sounds\ferum.mp3", UriKind.Relative),
            [SoundsEnum.rockSound] = new Uri(@"Sounds\rock.mp3", UriKind.Relative),
            [SoundsEnum.shotSoung] = new Uri(@"Sounds\shot.mp3", UriKind.Relative),
            [SoundsEnum.shotTargetSound] = new Uri(@"Sounds\brue.mp3", UriKind.Relative)
        };

        //скины
        public static Dictionary<SkinsEnum, BitmapImage> SkinDictionary = new Dictionary<SkinsEnum, BitmapImage>()
        {
            //каменные блоки
            [SkinsEnum.PictureBlock1] = new BitmapImage(new Uri(@"Pictures\blok.png", UriKind.Relative)),
            [SkinsEnum.PictureBlock2] = new BitmapImage(new Uri(@"Pictures\blok2.png", UriKind.Relative)),
            [SkinsEnum.PictureBlock3] = new BitmapImage(new Uri(@"Pictures\blok3.png", UriKind.Relative)),
            //железные блоки
            [SkinsEnum.PictureBlockFerum1] = new BitmapImage(new Uri(@"Pictures\ferum.png", UriKind.Relative)),
            [SkinsEnum.PictureBlockFerum2] = new BitmapImage(new Uri(@"Pictures\ferum2.png", UriKind.Relative)),
            [SkinsEnum.PictureBlockFerum3] = new BitmapImage(new Uri(@"Pictures\ferum3.png", UriKind.Relative)),
            //деревья
            [SkinsEnum.PictureWood1] = new BitmapImage(new Uri(@"Pictures\wood.png", UriKind.Relative)),
            //танк-бот
            [SkinsEnum.PictureTank1] = new BitmapImage(new Uri(@"Pictures\playertank.png", UriKind.Relative)),
            [SkinsEnum.PictureTank2] = new BitmapImage(new Uri(@"Pictures\playertank2.png", UriKind.Relative)),
            [SkinsEnum.PictureTank3] = new BitmapImage(new Uri(@"Pictures\playertank3.png", UriKind.Relative)),
            [SkinsEnum.PictureTank4] = new BitmapImage(new Uri(@"Pictures\playertank4.png", UriKind.Relative)),
            [SkinsEnum.PictureTankSpeed] = new BitmapImage(new Uri(@"Pictures\tankspeed.png", UriKind.Relative)),
            [SkinsEnum.PictureTankSpeed2] = new BitmapImage(new Uri(@"Pictures\tankspeed2.png", UriKind.Relative)),
            //бункер
            [SkinsEnum.PictureBunker] = new BitmapImage(new Uri(@"Pictures\bunker.png", UriKind.Relative)),
            [SkinsEnum.PictureBunker2] = new BitmapImage(new Uri(@"Pictures\bunker2.png", UriKind.Relative)),
            [SkinsEnum.PictureBunker3] = new BitmapImage(new Uri(@"Pictures\bunker3.png", UriKind.Relative)),
            //вражеский бункер
            [SkinsEnum.PictureBunkerEnamy] = new BitmapImage(new Uri(@"Pictures\bunkerEnamy.png", UriKind.Relative)),
            [SkinsEnum.PictureBunkerEnamy2] = new BitmapImage(new Uri(@"Pictures\bunkerEnamy2.png", UriKind.Relative)),
            [SkinsEnum.PictureBunkerEnamy3] = new BitmapImage(new Uri(@"Pictures\bunkerEnamy3.png", UriKind.Relative)),
            //разрушенный танк
            [SkinsEnum.PictureTankOfDestroy1] = new BitmapImage(new Uri(@"Pictures\tankofdestroy.png", UriKind.Relative)),
            [SkinsEnum.PictureTankOfDestroy2] = new BitmapImage(new Uri(@"Pictures\tankofdestroy2.png", UriKind.Relative)),
            [SkinsEnum.PictureTankOfDestroy3] = new BitmapImage(new Uri(@"Pictures\tankofdestroy3.png", UriKind.Relative)),
            [SkinsEnum.PictureTankOfDestroySpeed1] = new BitmapImage(new Uri(@"Pictures\tankofdestroyspeed1.png", UriKind.Relative)),
            [SkinsEnum.PictureTankOfDestroySpeed2] = new BitmapImage(new Uri(@"Pictures\tankofdestroyspeed2.png", UriKind.Relative)),
            //лут
            [SkinsEnum.PictureLootTeer] = new BitmapImage(new Uri(@"Pictures\Damage.png", UriKind.Relative)),
            [SkinsEnum.PictureLootSpeed] = new BitmapImage(new Uri(@"Pictures\Speed.png", UriKind.Relative)),
            //танк-бот
            [SkinsEnum.PictureTankBot1] = new BitmapImage(new Uri(@"Pictures\tankbot.png", UriKind.Relative)),
            [SkinsEnum.PictureTankBot2] = new BitmapImage(new Uri(@"Pictures\tankbot2.png", UriKind.Relative)),
            [SkinsEnum.PictureTankBot3] = new BitmapImage(new Uri(@"Pictures\tankbot3.png", UriKind.Relative)),
            [SkinsEnum.PictureTankBot4] = new BitmapImage(new Uri(@"Pictures\tankbot4.png", UriKind.Relative)),
            [SkinsEnum.PictureTankSpeedBot] = new BitmapImage(new Uri(@"Pictures\tankspeedbot.png", UriKind.Relative)),
            [SkinsEnum.PictureTankSpeedBot2] = new BitmapImage(new Uri(@"Pictures\tankspeedbot2.png", UriKind.Relative)),
            //стационарная пушка-бот
            [SkinsEnum.PictureLocationGun1] = new BitmapImage(new Uri(@"Pictures\LocationGun.png", UriKind.Relative)),
            [SkinsEnum.PictureLocationGun2] = new BitmapImage(new Uri(@"Pictures\LocationGun2.png", UriKind.Relative)),
            [SkinsEnum.PictureLocationGun3] = new BitmapImage(new Uri(@"Pictures\LocationGun3.png", UriKind.Relative)),
            //пуля
            [SkinsEnum.PictureBullet1] = new BitmapImage(new Uri(@"Pictures\bullet.png", UriKind.Relative)),
            [SkinsEnum.PictureBullet2] = new BitmapImage(new Uri(@"Pictures\bullet2.png", UriKind.Relative)),
            [SkinsEnum.PictureBullet3] = new BitmapImage(new Uri(@"Pictures\bullet3.png", UriKind.Relative)),
            [SkinsEnum.PictureBullet4] = new BitmapImage(new Uri(@"Pictures\bullet4.png", UriKind.Relative))
        };
    }
}
