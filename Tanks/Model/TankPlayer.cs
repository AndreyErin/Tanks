using System;
using System.Windows;

namespace Tanks.Model
{
    public class TankPlayer : Tank
    {
        //делегат сообщающий в Main что танк был уничтожен
        //нужен для того, чтобы определять уничтоженна ли группировка танков игроков или нет
        public event Action<Tank> DestroyPayerTank;

        public TankPlayer(Point tPos) : base(tPos)
        {
            IsPlayer = true;
            Source = Map.PictureTank1;
        }

        protected override void DistroyMy()
        {
            base.DistroyMy();

            DestroyPayerTank(this);           
        }
        //повышение уровня танка - визуализация
        protected override void UpgradeWiewTank(int teer)
        {
            Action action = ()=> { 
            switch (teer)
            {
                case 2:
                    Source = Map.PictureTank2;
                    break;
                case 3:
                    Source = Map.PictureTank3;
                    break;

            }
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);
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
