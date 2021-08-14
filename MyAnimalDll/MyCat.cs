using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Threading.Tasks;

namespace MyAnimalDll
{
    public partial class MyCat : ICat
    {
        private readonly Form form; //Родительская форма
        public Task wakeUpTask;
        public Task taskWalk;
        private Image clearBackgroundImage;
        private bool statusGame = false;

        /// <summary>
        /// Таймер для отрисовки Bitmap'ов
        /// </summary>
        public System.Threading.Timer timerGame;
        public Bitmap bm = ResPictRest.sit1; //Стартовый bitmap котика

        public event IAnimal.AnimalMoveHandle AnimalMoveEvent;// = Walk;
        public event IAnimal.AnimalSitHandle AnimalSitEvent;// = SitRest;
        public event IAnimal.AnimalSleepHandle AnimalSleepEvent;// = Sleep;

        ///Вызов пользовательских реализаций Move(), Sit(), Rest()
        ///p.s.Программист подписывает соответствующее событие на свою реализацию и вызывает соответственный метод
        public virtual void OnAnimalMove() { AnimalMoveEvent = Walk; AnimalMoveEvent?.Invoke(); }
        public virtual void OnAnimalSit() { AnimalSitEvent?.Invoke(); }
        public virtual void OnAnimalSleep() { AnimalSleepEvent?.Invoke(); }

        //Изменение свойств Control из другого потока
        ///при изменении в другом потоке, может попадать на dispose и крашится
        private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);
        private void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
        {
            try
            {
                if (control.InvokeRequired)
                    control?.Invoke(new SetControlPropertyThreadSafeDelegate(SetControlPropertyThreadSafe),
                                    new object[] { control, propertyName, propertyValue });
                else
                    control.GetType()?.InvokeMember(propertyName, BindingFlags.SetProperty, null, control, new object[] { propertyValue });
            }
            catch { }
        }

        ///Конструкторы
        public MyCat(Form form_)
        {
            form = form_;

            ////Возможно передавать как аргумент таймер
            timerGame = new System.Threading.Timer(TimerWalk_Tick, null, Timeout.Infinite, 0);
        }
        public MyCat(Form form_, IAnimal.AnimalMoveHandle moveHandle)
            : this(form_) { AnimalMoveEvent = moveHandle; }
        public MyCat(Form form_, IAnimal.AnimalMoveHandle moveHandle, IAnimal.AnimalSitHandle SitHandle)
            : this(form_, moveHandle) { AnimalSitEvent = SitHandle; }
        public MyCat(Form form_, IAnimal.AnimalMoveHandle moveHandle, IAnimal.AnimalSitHandle SitHandle, IAnimal.AnimalSleepHandle SleepHandle)
            : this(form_, moveHandle, SitHandle) { AnimalSleepEvent = SleepHandle; }

        /// <param name="form_"></param>
        /// <param name="mode">Будет ли форма принадлежать только коту или на форме будет что-то еще</param>
        public MyCat(Form form_, IAnimal.ModeAnimal mode) : this(form_)
        {
            if (mode == IAnimal.ModeAnimal.UncompetitiveAnimal)
                InitParrentForm();
        }

        protected virtual void InitParrentForm()
        {
            form.FormBorderStyle = FormBorderStyle.None;
            form.BackgroundImage = bm;
            form.BackgroundImageLayout = ImageLayout.None;
            form.AllowTransparency = true;
            form.TransparencyKey = Color.White;
            form.BackColor = Color.White;

            form.TopMost = true;
            form.Focus();

            System.Drawing.Drawing2D.GraphicsPath myPath = new System.Drawing.Drawing2D.GraphicsPath();
            form.Size = new Size(60, 60);

            myPath.AddRectangle(new Rectangle(0, 0, 40, 40));
            Region myRegion = new Region(myPath);
            form.Region = myRegion;
        }

        /// <summary>
        /// Обновить отрисовку родительской формы.
        /// Обновляется ParentForm.BackgroundImage.
        /// </summary>
        /// <param name="bm"></param>
        public void CatUpdate(Bitmap bm)
        {
            clearBackgroundImage = form.BackgroundImage;
            form.BackgroundImage = bm;
            clearBackgroundImage.Dispose();
        }

        //Вызов дефолтных реализаций Move(), Sit(), Rest()
        /// <summary>
        /// Дефолтная реализация IMove::Move()
        /// Для собственной реализации Move() используйте OnAnimalMove()
        /// </summary>
        public void Move()
        {
            if (statusGame == false)
            {
                statusGame = true;
                if (gPictVect == (byte)PictVect.Sleep || gPictVect == 255)
                {
                    wakeUpTask = new Task(WakeUp);
                    wakeUpTask.Start();
                    wakeUpTask.Wait();
                }
                Walk();
            }
        }

        /// <summary>
        /// Дефолтная реализация ISit::Sit()
        /// Для собственной реализации Sit() используйте OnAnimalSit()
        /// </summary>
        public void Sit()
        {
            timerGame.Change(0, IAnimal.timeChangePictSit);
            SitRest();
        }

        /// <summary>
        /// Дефолтная реализация ISleep::Sleep()
        /// Для собственной реализации Sleep() используйте OnAnimalSleep()
        /// </summary>
        public void Sleep() 
        {
            timerGame.Change(0, IAnimal.timeChangePictAssleep);
            AssleepRest();
            Thread.Sleep(IAnimal.timePauseAssleep);
            timerGame.Change(0, IAnimal.timeChangePictSleep);
            SleepRest();
            statusGame = false;
        }

        protected virtual void WakeUp()
        {
            timerGame.Change(0, IAnimal.timeChangePictAssleep);
            AssleepRest();
            Thread.Sleep(IAnimal.timePauseWakeUp);
        }

        //Дефолтная реализация Move(), Sit(), Sleep()
        protected virtual void Walk()
        {
            //Код для бегания кошечки
            taskWalk = new Task(RunCat);
            taskWalk.Start();
        }

        protected virtual void SitRest()
        {
            //Код когда кошечка сидит
            gPictState = 0;
            gPictVect = (byte)PictVect.Sit;
        }

        protected virtual void AssleepRest()
        {            
            //Код перед сном кошечки
            gPictState = 0;
            gPictVect = (byte)PictVect.Assleep; //Assleep set pict
        }

        protected virtual void SleepRest()
        {
            //Код для сна кошечки
            gPictState = 0;
            gPictVect = (byte)PictVect.Sleep; //Sleep set pict
        }
    }
}
