using System;
using System.Windows;

namespace Server.Model
{
    public class TankPlayer : Tank
    {
        //делегат сообщающий в Main, что танк был уничтожен
        //нужен для того, чтобы определять уничтоженна ли группировка танков игроков или нет
        public event Action<TankPlayer> DestroyPayerTank;

        public TankPlayer(MyPoint tPos)
        {
            InitElementBase(tPos);
            IsPlayer = true;
            Skin = SkinsEnum.PictureTank1;
            AddMe();
        }

        protected override void DistroyMy()
        {
            base.DistroyMy();

            DestroyPayerTank?.Invoke(this);           
        }
        //повышение уровня танка - визуализация
        protected override void UpgradeWiewTank(int teer)
        {
            switch (teer)
            {
                case 2:
                    Skin = SkinsEnum.PictureTank2;
                    break;
                case 3:
                    Skin = SkinsEnum.PictureTank3;
                    break;

            }
        }

        protected override void UpgradeWiewTankSpeed(double speed)
        {
            switch (speed)
            {
                case 2.0:
                    Skin = SkinsEnum.PictureTankSpeed;
                    break;
                case 2.5:
                    Skin = SkinsEnum.PictureTankSpeed2;
                    break;
            }
        }

        //приводим характеристика танка в соответсвие с тиром
        public void UpdateHpForTeer()
        {
            switch (lvlTank)
            {
                case 2:
                    HP = 2;
                    break;
                case 3:
                case 4:
                    HP = 3;
                    break;
            }
        }
    }
}
