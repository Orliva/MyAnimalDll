namespace MyAnimalDll
{
    public interface IMove
    {
        public const int offsetFormX = 1; ///Скорость формы (смещение за каждый интервал времени)
        public const int offsetFormY = 1; ///Скорость формы (смещение за каждый интервал времени)
        public const int ChangePictMove = 70; ///Время смены кадров (котик переставляет лапки)
        public const int SleepFormMove = 30; ///Тормазим форму, что бы задать скорость передвижению формы по экрану
        public void Move();
    }
    public interface ISit
    {
        //   const int timePause = 3000;
        public const int timeChangePictSit = 900; ///Время смены кадров отдыха

        public void Sit();
    }
    public interface ISleep
    {
        public const int timePauseAssleep = 4000;
        public const int timePauseWakeUp = 2000;
        public const int timeChangePictAssleep = 900;
        public const int timeChangePictSleep = 1500; ///Время смены кадров отдыха

        public void Sleep();

    }
    public interface IAnimal : IMove, ISit, ISleep
    {
        public delegate void AnimalMoveHandle();
        public delegate void AnimalSitHandle();
        public delegate void AnimalSleepHandle();

        public event AnimalMoveHandle AnimalMoveEvent;
        public event AnimalSitHandle AnimalSitEvent;
        public event AnimalSleepHandle AnimalSleepEvent;

        public enum ModeAnimal
        {
            UncompetitiveAnimal = 0,
            CompetitiveAnimal = 1
        }
    }
    public interface ICat : IAnimal
    {
        ///Возможно что то специфичное для кошечки добавить
        ///От анимала можно наследовать любых животных и добавлять индивидуальные фичи
    }
}
