using System.ComponentModel;

namespace Server.Model
{
    public abstract class WorldElement : INotifyPropertyChanged
    {
        protected double _height { get; set; }
        protected double _width { get; set; }
        public int ID { get; set; }

        //флаг - был ли изменени элемент хоть раз
        //если да то будем отсылать его позицию
        public bool ElementIsChanget = false;
        private SkinsEnum _skin;
        public SkinsEnum Skin { get => _skin; set { _skin = value; ElementIsChanget = true; } }
        private double _x;
        public double X { get => _x; set { _x = value; ElementIsChanget = true; } }
        private double _y;
        public double Y { get => _y; set { _y = value ; ElementIsChanget = true; } }
        private VectorEnum _vectorElement;
        public VectorEnum VectorElement { get => _vectorElement; set { _vectorElement = value; ElementIsChanget = true; } }
        

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
            ElementIsChanget = false; //сбрасываем флаг изменения
        }

    }
}
