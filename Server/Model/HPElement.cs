


namespace Server.Model
{
    public abstract class HPElement : WorldElement
    {
        public int HP { get; set; } = 1;

        //проверка является ли объект(танк или блок) за игрока или нет
        public bool IsPlayer { get; set; } = false;

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

        //разрушение объекта
        protected virtual void DistroyMy()
        {
            GlobalDataStatic.BattleGroundCollection.Remove(this);
        }

        public abstract void GetDamage(int damage);//получение дамага
    }
}
