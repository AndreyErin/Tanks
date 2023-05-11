

namespace Server.Model
{
    //итерфейс для объектов способных издавать звук
    public interface ISoundsObjects
    {
        public delegate void SoundDeleg(SoundsEnum sound);
        public event SoundDeleg? SoundEvent;
        public SoundsEnum sound { get; set; }
    }
}
