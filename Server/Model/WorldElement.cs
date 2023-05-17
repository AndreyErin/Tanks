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

        private MyPoint _ePos;
        public MyPoint ePos {
            get => _ePos;
            set 
            {
                if (_ePos != value) 
                {
                    _ePos = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ePos)));
                }
            } 
        }


    }
}
