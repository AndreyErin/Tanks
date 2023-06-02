using System.ComponentModel;
using System.Windows;

namespace Server.Model
{
    public abstract class WorldElement : INotifyPropertyChanged
    {

        public bool MessageSetON = false;


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
                    ChangeElement();
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
                    ChangeElement();                
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
                    ChangeElement();               
                }
            }
        }

        private VectorEnum _vectorElement;
        public VectorEnum VectorElement
    {
            get => _vectorElement;

            set
            {
                if (_vectorElement != value)
                {
                    _vectorElement = value;
                    ChangeElement();
                }
            }
        }

        //произошли изменения в элементе
        private void ChangeElement() 
        {
            if (MessageSetON == false)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CHANGE"));
                MessageSetON = true;
            }
        }

        //добавление себя на поле боя
        protected void AddMe() 
        { 
            try
            {
                GlobalDataStatic.Controller.Dispatcher.Invoke(() => GlobalDataStatic.BattleGroundCollection.TryAdd(ID, this));
                PropertyChanged += GlobalDataStatic.Controller.ChangedElement;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ADD"));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Добавление объекта" + ex.Message);
            }
        }
        public void RemoveMe()
        {
            try
            {
                GlobalDataStatic.Controller.Dispatcher.Invoke(() => GlobalDataStatic.BattleGroundCollection.TryRemove(ID, out var element));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("REMOVE"));
                PropertyChanged = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Удаление объекта" + ex.Message);
            }
        }

    }
}
