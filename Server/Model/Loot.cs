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
                    _skin = SkinsEnum.PictureLootTeer ;
                    break;
                    
                case 1:
                case 2:  //увеличиваем шанс выпадения               
                    typeUpgrade = BonusEnum.Speed;
                    _skin = SkinsEnum.PictureLootSpeed;
                    break;

                case >= 3:
                    return; //фокус не удался, дропа не будет                                      
            }           

            _height = 30;
            _width = 30;
            ePos = pos;
            GlobalDataStatic.BattleGroundCollection.Add(this);
        }
    }
}
