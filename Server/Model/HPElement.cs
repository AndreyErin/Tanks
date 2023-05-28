


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

        //разрушение объекта
        protected virtual void DistroyMy()
        {
            RemoveMe();
        }

        public abstract void GetDamage(int damage);//получение дамага
    }
}
