using System;
using System.Windows.Media.Media3D;


//ред
namespace Server.Model
{
    public class Loot : WorldElement
    {
        public BonusEnum typeUpgrade;

        public Loot()
        {
            //прописать добавление в стек
        }

        public void InitElement(MyPoint pos)
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
                    //возвращаем неиспользованный лут в стак
                    GlobalDataStatic.StackLoot.Push(this);
                    return; //фокус не удался, дропа не будет                                      
            }

            _height = 30;
            _width = 30;
            X = pos.X;
            Y = pos.Y;
            AddMe();
        }

        //проверка попадания по нашему обьекту
        //метод отвечает попали по нему или нет
        public bool HaveHit(MyPoint posLedarL, MyPoint posLedarR)
        {
            if ((posLedarL.X >= X) && (posLedarL.X <= (X + _height))
                && (posLedarL.Y >= Y) && (posLedarL.Y <= (Y + _width)))
            {
                return true;
            }

            if ((posLedarR.X >= X) && (posLedarR.X <= (X + _height))
                && (posLedarR.Y >= Y) && (posLedarR.Y <= (Y + _width)))
            {
                return true;
            }
            return false;
        }
    }
}
