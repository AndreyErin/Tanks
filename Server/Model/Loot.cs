using System;
using System.Windows.Media.Media3D;


//ред
namespace Server.Model
{
    public class Loot : WorldElement
    {
        public BonusEnum typeUpgrade;

        public Loot(MyPoint pos)
        {
            Random random = new Random();
            //случайный лут
            switch (random.Next(0, 10))
            {
                case 0:
                    typeUpgrade = BonusEnum.Teer;
                    Skin = SkinsEnum.PictureLootTeer ;
                    break;
                    
                case 1:
                case 2:  //увеличиваем шанс выпадения               
                    typeUpgrade = BonusEnum.Speed;
                    Skin = SkinsEnum.PictureLootSpeed;
                    break;

                case >= 3:
                    return; //фокус не удался, дропа не будет                                      
            }           

            _height = 30;
            _width = 30;
            ePos = pos;
            AddMe();
        }

        //проверка попадания по нашему обьекту
        //метод отвечает попали по нему или нет
        public bool HaveHit(MyPoint posLedarL, MyPoint posLedarR)
        {
            if ((posLedarL.X >= ePos.X) && (posLedarL.X <= (ePos.X + _height))
                && (posLedarL.Y >= ePos.Y) && (posLedarL.Y <= (ePos.Y + _width)))
            {
                return true;
            }

            if ((posLedarR.X >= ePos.X) && (posLedarR.X <= (ePos.X + _height))
                && (posLedarR.Y >= ePos.Y) && (posLedarR.Y <= (ePos.Y + _width)))
            {
                return true;
            }
            return false;
        }
    }
}
