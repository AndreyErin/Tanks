using System.ComponentModel;

namespace Server.Model
{
    public abstract class WorldElement : INotifyPropertyChanged
    {
        protected double _height { get; set; }
        protected double _width { get; set; }
        public int ID { get; set; }
        public SkinsEnum Skin { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public VectorEnum VectorElement { get; set; }
        public bool MessageSetON = false;

        public WorldElement()
        {
            ID = GlobalDataStatic.IdNumberElement++;
        }
        
        //интерфейс INotifyPropertyChanged
        //наблюдаем за изменением свойств позиции и скина
        public event PropertyChangedEventHandler? PropertyChanged;

        //добавление себя на поле боя
        protected void AddMe() 
        {                                 
            GlobalDataStatic.BattleGroundCollection.TryAdd(ID, this);
            
            PropertyChanged += GlobalDataStatic.Controller.ChangedElement;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ADD"));                
        }

        public void RemoveMe()
        {           
            GlobalDataStatic.Controller.Dispatcher.Invoke(() => {

            GlobalDataStatic.BattleGroundCollection.TryRemove(ID, out var element);

            //возвращаем ненужный элемент в стак
            switch (element)
            {
                case BlockFerum:
                    GlobalDataStatic.StackBlocksFerum.Push((BlockFerum)this);
                    break;
                case BlockRock:
                    GlobalDataStatic.StackBlocksRock.Push((BlockRock)this);
                    break;
                case Bullet:
                    GlobalDataStatic.StackBullet.Push((Bullet)this);
                    break;
                case LocationGun:
                    GlobalDataStatic.StackLocationGun.Push((LocationGun)this);
                    break;
                case Loot:
                    GlobalDataStatic.StackLoot.Push((Loot)this);
                    break;
                case TankBot:
                    GlobalDataStatic.StackTankBot.Push((TankBot)this);
                    break;
                case TankOfDistroy:
                    GlobalDataStatic.StackTankOfDistroy.Push((TankOfDistroy)this);
                    break;
                case Tree:
                    GlobalDataStatic.StackTree.Push((Tree)this);
                    break;
            }

            });

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("REMOVE"));
            PropertyChanged = null;
        }

    }
}
