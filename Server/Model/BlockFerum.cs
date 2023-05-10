using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Server.Model
{
    //железный блок
    internal class BlockFerum : Block
    {
        public BlockFerum(System.Windows.Point ePos) : base(ePos)
        {
            Source = Map.PictureBlockFerum1;
            HP = 100;
        }

        //получение урона объктом
        public override void GetDamage(int damage) 
        {

            HP -= damage;

            Task.Factory.StartNew(()=>GetDamageView());
        }

        protected override void GetDamageView()
        {
            Action action = () => 
            {
                if (HP <= 60 && HP > 30) { Source = Map.PictureBlockFerum2; }
                if (HP <= 30 && HP > 0) { Source = Map.PictureBlockFerum3; }
                if (HP <= 0) { DistroyMy(); } //если нет хп, то объект уничтожается
            };
            GlobalDataStatic.MainDispatcher.Invoke(action);
        }
    }
}
