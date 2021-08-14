using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace MyAnimalDll
{
    public partial class MyCat : ICat
    {
        protected byte gPictState = 0;
        protected byte gPictVect = 255; //255 обозначает начальное состояние когда кошечка еще ни разу не бегала

        /// <summary>
        /// Направление кота
        /// </summary>
        protected enum PictVect
        {
            Down = 0,
            Up = 1,
            Left = 2,
            Right = 3,
            RightDown = 4,
            LeftDown = 5,
            RightUp = 6,
            LeftUp = 7,
            Assleep = 8,
            Sleep = 9,
            Sit = 10
        }

        private void TimerWalk_Tick(object sender)
        {
            if (gPictState < 3)
                gPictState++;
            else
                gPictState = 0;
            GetPict(ref gPictVect);
        }

        private void GetPict(ref byte curVect)
        {
            switch (curVect)
            {
                case 0:
                    HelpGetPict(ResPictMove.q, ResPictMove.w, ResPictMove.e);
                    break;
                case 1:
                    HelpGetPict(ResPictMove.r, ResPictMove.t, ResPictMove.y);
                    break;
                case 2:
                    HelpGetPict(ResPictMove.u, ResPictMove.i, ResPictMove.o);
                    break;
                case 3:
                    HelpGetPict(ResPictMove.p, ResPictMove.a, ResPictMove.s);
                    break;
                case 4:
                    HelpGetPict(ResPictMove.d, ResPictMove.f, ResPictMove.g);
                    break;
                case 5:
                    HelpGetPict(ResPictMove.h, ResPictMove.j, ResPictMove.k);
                    break;
                case 6:
                    HelpGetPict(ResPictMove.l, ResPictMove.z, ResPictMove.x);
                    break;
                case 7:
                    HelpGetPict(ResPictMove.c, ResPictMove.v, ResPictMove.b);
                    break;
                case 8:
                    HelpGetPict(ResPictRest.assleep1, ResPictRest.assleep2, ResPictRest.assleep3);
                    break;
                case 9:
                    HelpGetPict(ResPictRest.sleep_1, ResPictRest.sleep_2, ResPictRest.sleep_3);
                    break;
                case 10:
                    HelpGetPict(ResPictRest.sit1, ResPictRest.sit2, ResPictRest.sit3);
                    break;
                default:
                    throw new Exception("Number picture exeption!");
            }
        }

        private void HelpGetPict(Bitmap state1, Bitmap state2, Bitmap state3)
        {
            switch (gPictState)
            {
                case 0:
                    bm = state1;
                    break;
                case 1:
                    bm = state2;
                    break;
                case 2:
                    bm = state3;
                    break;
                default:
                    break;
            }
            CatUpdate(bm);
        }

        private void HelpRun(byte catVector, ref Point point, int offsetX, int offsetY)
        {            
            gPictVect = catVector;
            point.X += offsetX;
            point.Y += offsetY;
            SetControlPropertyThreadSafe(form, "Location", point);
            Thread.Sleep(IAnimal.SleepFormMove);
        }

        private void RunCat()
        {
            Point point = new Point
            {
                X = form.Location.X,
                Y = form.Location.Y
            };

            //Смещение от позиции курсора
            //Сколько кошечка не добегает до курсора (Что бы не мешаться юзеру)
            int a = 15;
            int b = 5;

            timerGame.Change(0, IAnimal.ChangePictMove);
            while (form.Location.X != Control.MousePosition.X + a || form.Location.Y != Control.MousePosition.Y + b)
            {
                if (Control.MousePosition.X + a < form.Location.X && Control.MousePosition.Y + b == form.Location.Y)
                {
                    HelpRun((byte)PictVect.Left, ref point, -IMove.offsetFormX, 0);
                }
                else if (Control.MousePosition.X + a > form.Location.X && Control.MousePosition.Y + b == form.Location.Y)
                {
                    HelpRun((byte)PictVect.Right, ref point, IMove.offsetFormX, 0);
                }
                else if (Control.MousePosition.Y + b < form.Location.Y && Control.MousePosition.X + a == form.Location.X)
                {
                    HelpRun((byte)PictVect.Up, ref point, 0, -IMove.offsetFormY);
                }
                else if (Control.MousePosition.Y + b > form.Location.Y && Control.MousePosition.X + a == form.Location.X)
                {
                    HelpRun((byte)PictVect.Down, ref point, 0, IMove.offsetFormY);
                }
                else if (Control.MousePosition.Y + b > form.Location.Y && Control.MousePosition.X + a > form.Location.X)
                {
                    HelpRun((byte)PictVect.RightDown, ref point, IMove.offsetFormX, IMove.offsetFormY);
                }
                else if (Control.MousePosition.Y + b > form.Location.Y && Control.MousePosition.X + a < form.Location.X)
                {
                    HelpRun((byte)PictVect.LeftDown, ref point, -IMove.offsetFormX, IMove.offsetFormY);
                }
                else if (Control.MousePosition.Y + b < form.Location.Y && Control.MousePosition.X + a > form.Location.X)
                {
                    HelpRun((byte)PictVect.RightUp, ref point, IMove.offsetFormX, -IMove.offsetFormY);
                }
                else if (Control.MousePosition.Y + b < form.Location.Y && Control.MousePosition.X + a < form.Location.X)
                {
                    HelpRun((byte)PictVect.LeftUp, ref point, -IMove.offsetFormX, -IMove.offsetFormY);
                }
            }
            Sleep();
        }
    }
}