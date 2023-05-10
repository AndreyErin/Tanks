using System;


//ред
namespace Server.Model
{
    public class Loot : WorldElement
    {
        public BonusEnum typeUpgrade;

        public Loot(System.Windows.Point pos)
        {
            Random random = new Random();
            //случайный лут
            switch (random.Next(0, 10))
            {
                case 0:
                    typeUpgrade = BonusEnum.Teer;
                    Source = SkinsEnum.PictureLootTeer ;
                    break;
                    
                case 1:
                case 2:  //увеличиваем шанс выпадения               
                    typeUpgrade = BonusEnum.Speed;
                    Source = SkinsEnum.PictureLootSpeed;
                    break;

                case >= 3:
                    return; //фокус не удался, дропа не будет                                      
            }           

            Height = 30;
            Width = 30;
            _ePos = pos;
            GlobalDataStatic.BattleGroundCollection.Add(this);
        }

        public override void GetDamage(int damage)
        {
            //бонусы урон не получают
        }
    }
}
