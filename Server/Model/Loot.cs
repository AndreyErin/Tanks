using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Server.Model
{
    public class Loot : WorldElement
    {
        public BonusEnum typeUpgrade;

        public Loot(System.Windows.Point pos)
        {

            Random random = new Random();
            
            switch (random.Next(0, 10))
            {
                case 0:
                    typeUpgrade = BonusEnum.Teer;
                    Source = Map.PictureLootTeer;
                    break;
                    
                case 1:
                case 2:  //увеличиваем шанс выпадения               
                    typeUpgrade = BonusEnum.Speed;
                    Source = Map.PictureLootSpeed;
                    break;

                case >= 3:
                    return; //фокус не удался, дропа не будет                                      
            }           

            Height = 30;
            Width = 30;
            _ePos = pos;
            Canvas.SetTop(this, _ePos.X );
            Canvas.SetLeft(this, _ePos.Y );
            GlobalDataStatic.cnvMap1.Children.Add(this);
            //дописать варианты лута

        }

        public override void GetDamage(int damage)
        {
            //бонусы урон не получают
        }
    }
}
