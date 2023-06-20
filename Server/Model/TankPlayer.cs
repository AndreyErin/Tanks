using System;
using System.ComponentModel;
using System.Windows;

namespace Server.Model
{
    public class TankPlayer : Tank
    {
        public event PropertyChangedEventHandler? FragsEvent;
        

        //делегат сообщающий в Main, что танк был уничтожен
        //нужен для того, чтобы определять уничтоженна ли группировка танков игроков или нет
        public event Action<TankPlayer> DestroyPayerTank;

        public Frags _myFrags = new Frags() {lvl1 = 0, lvl2 =0, lvl3 = 0, lvl4 = 0, lvlSpeed1 = 0, lvlSpeed2 = 0, LocationGan = 0 };

        public TankPlayer(MyPoint tPos)
        {
            FragsEvent += GlobalDataStatic.Controller.ChangedElement;
            InitElementBase(tPos);
            IsPlayer = true;
            Skin = SkinsEnum.PictureTank1;
            AddMe();
        }

        //обновляем танк
        public void UploadTank(MyPoint myPoint) 
        {
            _myFrags.lvl1 = 0;
            _myFrags.lvl2 = 0;
            _myFrags.lvl3 = 0;
            _myFrags.lvl4 = 0;
            _myFrags.lvlSpeed1 = 0;
            _myFrags.lvlSpeed2 = 0;
            _myFrags.LocationGan = 0;

            VectorElement = VectorEnum.Top;
            X = myPoint.X;
            Y = myPoint.Y;
            UpdateHpForTeer();
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
            //обновляем HP танков игроков в клиенте
            PartyPlayers.One?.UploadHpPlayers();
            PartyPlayers.Two?.UploadHpPlayers();
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

            //обновляем HP танков игроков в клиенте
            PartyPlayers.One?.UploadHpPlayers();

            if (GlobalDataStatic.Controller.IsMultiPlayer)
                PartyPlayers.Two?.UploadHpPlayers();
        }

        //приводим характеристика танка в соответсвие с тиром
        private void UpdateHpForTeer()
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

            //обновляем HP танков игроков в клиенте
            PartyPlayers.One?.UploadHpPlayers();

            if(GlobalDataStatic.Controller.IsMultiPlayer)
                PartyPlayers.Two?.UploadHpPlayers();
        }

        public void Frag(int lvlTankFrag, double speedTankFrag) 
        {
            switch (speedTankFrag)
            {
                case 2d:
                    _myFrags.lvlSpeed1++;
                    return;
                case 2.5d:
                    _myFrags.lvlSpeed2++;
                    return;
            }

            switch (lvlTankFrag)
            {
                case 1:
                    _myFrags.lvl1++;
                    break;
                case 2:
                    _myFrags.lvl2++;
                    break;
                case 3:
                    _myFrags.lvl3++;
                    break;
                case 4:
                    _myFrags.lvl4++;
                    break;
            }
        }

        public void FragLocationGun() { _myFrags.LocationGan++; }

        public void GetFrags() 
        {           
            FragsEvent?.Invoke(this, new PropertyChangedEventArgs("FRAGS"));
        }

        public struct Frags 
        {
            public int lvl1;
            public int lvl2;
            public int lvl3;
            public int lvl4;
            public int lvlSpeed1;
            public int lvlSpeed2;
            public int LocationGan;
        }
    }
}
