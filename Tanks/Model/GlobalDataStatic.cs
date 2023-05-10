using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Tanks.Model
{
    public static class GlobalDataStatic
    {
        public static BitmapImage PictureLogo { get; set; } = new BitmapImage(new Uri(@"Pictures\logo.png", UriKind.Relative));

        public static bool RespawnBotON { get; set; } //будут ли еще появляться танки врага
        public static List<Tank> PartyTanksOfPlayers { get; set; } = new List<Tank>(); //коллекция играков танков
        public static List<TankBot> PartyTankBots { get; set; } = new List<TankBot>(); //коллекция вражеский танков

        public static MediaPlayer SoundPlayer { get; set; } = new MediaPlayer();

        //музыка
        public static Uri menuSound = new Uri(@"Sounds\menuSound.mp3", UriKind.Relative);
        public static Uri bonusSound = new Uri(@"Sounds\bonus.mp3", UriKind.Relative);
        public static Uri mainSound = new Uri(@"Sounds\mainSound.mp3", UriKind.Relative);
        public static Uri ferumSoung = new Uri(@"Sounds\ferum.mp3", UriKind.Relative);
        public static Uri rockSound = new Uri(@"Sounds\rock.mp3", UriKind.Relative);
        public static Uri shotSoung = new Uri(@"Sounds\shot.mp3", UriKind.Relative);
        public static Uri shotTargetSound = new Uri(@"Sounds\brue.mp3", UriKind.Relative);

        //остальное
        public static Canvas cnvMap1 { get; set; }
        public static Dispatcher MainDispatcher { get; set; }
        public static Label lblStatisticTank { get; set; }
    }
}
