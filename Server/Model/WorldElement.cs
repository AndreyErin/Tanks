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

        private double _x;
        public double X {
            get { return _x; }
            set 
            { 
                if (_x != value) 
                {
                    _x = value;
                    PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(nameof(X)));
                }
            }
        }

        private double _y;
        public double Y
        {
            get { return _y; }
            set
            {
                if (_y != value)
                {
                    _y = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Y)));
                }
            }
        }


        //добавление себя на поле боя
        protected void AddMe() { GlobalDataStatic.BattleGroundCollection.Add(this); }


    }
}
