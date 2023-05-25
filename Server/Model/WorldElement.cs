using System.ComponentModel;


namespace Server.Model
{
    public abstract class WorldElement : INotifyPropertyChanged
    {
        public WorldElement()
        {
            ID = GlobalDataStatic.IdNumberElement++;
        }

        //интерфейс INotifyPropertyChanged
        //наблюдаем за изменением свойств позиции и скина
        public event PropertyChangedEventHandler? PropertyChanged;

        public int ID { get; set; }

        protected double _height { get; set; }
        protected double _width { get; set; }

        //свойства вызывающие события при изменении
        private SkinsEnum _skin;
        public SkinsEnum Skin {
            get => _skin;

            set
            {
                if (_skin != value) 
                { 
                    _skin = value; 
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Skin)));
                }
            }
        }

        private MyPoint _ePos = new MyPoint();
        public MyPoint ePos {
            get => _ePos;
            set 
            {
                if (_ePos.X != value.X) 
                {
                    _ePos.X = value.X;
                    ePos.X = _ePos.X;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ePos.X)));
                }

                if (_ePos.Y != value.Y)
                {
                    _ePos.Y = value.Y;
                    ePos.Y = _ePos.Y;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ePos.Y)));
                }
            } 
        }

        //добавление себя на поле боя
        protected void AddMe() { GlobalDataStatic.BattleGroundCollection.Add(this); }


    }
}
