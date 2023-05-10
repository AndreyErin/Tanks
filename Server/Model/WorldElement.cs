﻿
//ред
namespace Server.Model
{
    public abstract class WorldElement : Img
    {
       
        //проверка является ли объект(танк или блок) за игрока или нет
        public bool IsPlayer { get; set; } = false;

        //проверка попадания по нашему обьекту
        //метод отвечает попали по нему или нет
        public bool HaveHit(System.Windows.Point posLedarL, System.Windows.Point posLedarR)
        {            
            if ((posLedarL.X >= _ePos.X) && (posLedarL.X <= (_ePos.X + Height))
                && (posLedarL.Y >= _ePos.Y) && (posLedarL.Y <= (_ePos.Y + Width)))
            {
                return true;
            }

            if ((posLedarR.X >= _ePos.X) && (posLedarR.X <= (_ePos.X + Height))
                && (posLedarR.Y >= _ePos.Y) && (posLedarR.Y <= (_ePos.Y + Width)))
            {
                return true;
            }
            return false;
            
        }

        //разрушение объекта
        protected virtual void DistroyMy()
        {
            GlobalDataStatic.BattleGroundCollection.Remove(this);            
        }

        public System.Windows.Point _ePos; // { get; set; }
        public int HP { get; set; } = 1;

        public abstract void GetDamage(int damage);//получение дамага
    }
}
