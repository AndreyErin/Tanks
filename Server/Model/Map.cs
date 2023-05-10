using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Server.Model
{

    public class Map
    {

        //все открытые свойства будут сериализованны по умолчанию (простые поля не будут)
        [JsonInclude]
        public List<Point> rockBlocs { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> ironBlocs { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> woodBlocs { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> friendlyRockBlocs { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> respawnTankPlayer { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> respawnTankBots { get; set; } = new List<Point>();
        [JsonInclude]
        public List<Point> LocationGun { get; set; } = new List<Point>();
        [JsonInclude]
        public bool BunkerON { get; set; } = false;
        [JsonInclude]
        public Point BunkerPos { get; set; }
        [JsonInclude]
        public bool BunkerEnamyON { get; set; } = false;
        [JsonInclude]
        public Point BunkerEnamyPos { get; set; }
        [JsonInclude]
        public int RespawnEnamyCount { get; set; } = 10;


        //каменные блоки
        [JsonInclude]
        public static BitmapImage PictureBlock1 { get; set; } = new BitmapImage(new Uri(@"Pictures\blok.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBlock2 { get; set; } = new BitmapImage(new Uri(@"Pictures\blok2.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBlock3 { get; set; } = new BitmapImage(new Uri(@"Pictures\blok3.png", UriKind.Relative));

        //железные блоки
        [JsonInclude]
        public static BitmapImage PictureBlockFerum1 { get; set; } = new BitmapImage(new Uri(@"Pictures\ferum.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBlockFerum2 { get; set; } = new BitmapImage(new Uri(@"Pictures\ferum2.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBlockFerum3 { get; set; } = new BitmapImage(new Uri(@"Pictures\ferum3.png", UriKind.Relative));

        //деревья
        [JsonInclude]
        public static BitmapImage PictureWood1 { get; set; } = new BitmapImage(new Uri(@"Pictures\wood.png", UriKind.Relative));

        //танк игрока
        [JsonInclude]
        public static BitmapImage PictureTank1 { get; set; } = new BitmapImage(new Uri(@"Pictures\playertank.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTank2 { get; set; } = new BitmapImage(new Uri(@"Pictures\playertank2.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTank3 { get; set; } = new BitmapImage(new Uri(@"Pictures\playertank3.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTank4 { get; set; } = new BitmapImage(new Uri(@"Pictures\playertank4.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankSpeed { get; set; } = new BitmapImage(new Uri(@"Pictures\tankspeed.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankSpeed2 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankspeed2.png", UriKind.Relative));



        //бункер
        [JsonInclude]
        public static BitmapImage PictureBunker { get; set; } = new BitmapImage(new Uri(@"Pictures\bunker.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBunker2 { get; set; } = new BitmapImage(new Uri(@"Pictures\bunker2.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBunker3 { get; set; } = new BitmapImage(new Uri(@"Pictures\bunker3.png", UriKind.Relative));

        //вражеский бункер
        [JsonInclude]
        public static BitmapImage PictureBunkerEnamy { get; set; } = new BitmapImage(new Uri(@"Pictures\bunkerEnamy.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBunkerEnamy2 { get; set; } = new BitmapImage(new Uri(@"Pictures\bunkerEnamy2.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBunkerEnamy3 { get; set; } = new BitmapImage(new Uri(@"Pictures\bunkerEnamy3.png", UriKind.Relative));


        //разрушенный танк
        [JsonInclude]
        public static BitmapImage PictureTankOfDestroy1 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankofdestroy.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankOfDestroy2 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankofdestroy2.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankOfDestroy3 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankofdestroy3.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankOfDestroySpeed1 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankofdestroyspeed1.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankOfDestroySpeed2 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankofdestroyspeed2.png", UriKind.Relative));


        //лут
        [JsonInclude]
        public static BitmapImage PictureLootTeer { get; set; } = new BitmapImage(new Uri(@"Pictures\Damage.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureLootDefense { get; set; } = new BitmapImage(new Uri(@"Pictures\Defense.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureLootSpeed { get; set; } = new BitmapImage(new Uri(@"Pictures\Speed.png", UriKind.Relative));


        //танк-бот
        [JsonInclude]
        public static BitmapImage PictureTankBot1 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankbot.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankBot2 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankbot2.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankBot3 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankbot3.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankBot4 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankbot4.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankSpeedBot { get; set; } = new BitmapImage(new Uri(@"Pictures\tankspeedbot.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureTankSpeedBot2 { get; set; } = new BitmapImage(new Uri(@"Pictures\tankspeedbot2.png", UriKind.Relative));


        //стационарная пушка-бот
        [JsonInclude]
        public static BitmapImage PictureLocationGun1 { get; set; } = new BitmapImage(new Uri(@"Pictures\LocationGun.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureLocationGun2 { get; set; } = new BitmapImage(new Uri(@"Pictures\LocationGun2.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureLocationGun3 { get; set; } = new BitmapImage(new Uri(@"Pictures\LocationGun3.png", UriKind.Relative));


        //пуля
        [JsonInclude]
        public static BitmapImage PictureBullet1 { get; set; } = new BitmapImage(new Uri(@"Pictures\bullet.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBullet2 { get; set; } = new BitmapImage(new Uri(@"Pictures\bullet2.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBullet3 { get; set; } = new BitmapImage(new Uri(@"Pictures\bullet3.png", UriKind.Relative));
        [JsonInclude]
        public static BitmapImage PictureBullet4 { get; set; } = new BitmapImage(new Uri(@"Pictures\bullet4.png", UriKind.Relative));

    }
}
