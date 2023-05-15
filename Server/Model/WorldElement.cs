﻿

using System.ComponentModel;
using System.Windows;

namespace Server.Model
{
    public abstract class WorldElement : INotifyPropertyChanged
    {
        //интерфейс INotifyPropertyChanged
        //наблюдаем за изменением свойств позиции и скина
        public event PropertyChangedEventHandler? PropertyChanged;

        protected double _height { get; set; }
        protected double _width { get; set; }

        //свойства вызывающие события при изменении
        protected SkinsEnum _skin;
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

        protected MyPoint _ePos;
        public MyPoint ePos {
            get => ePos;
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
