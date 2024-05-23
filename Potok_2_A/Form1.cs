using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace Potok_2_A
{
    public partial class Form1 : Form
    {
        private Button button1;
        private Button button2;
        private Label label;
        private Thread thread1;
        private Thread thread2;
        private List<string> keyLogs = new List<string>();
        private bool isFormClosing = false;
        private object syncLock = new object();

        public Form1()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }
        private void InitializeCustomComponents()
        {
            // Створення лейблу
            label = new Label
            {
                Text = "Use Arrow Keys and WASD to move buttons",
                AutoSize = true,
                Location = new System.Drawing.Point(10, 10)
            };
            this.Controls.Add(label);

            // Створення першої кнопки
            button1 = new Button
            {
                Text = "Button 1",
                Location = new System.Drawing.Point(100, 100),
                Size = new System.Drawing.Size(100, 50)
            };
            this.Controls.Add(button1);

            // Створення другої кнопки
            button2 = new Button
            {
                Text = "Button 2",
                Location = new System.Drawing.Point(200, 100),
                Size = new System.Drawing.Size(100, 50)
            };
            this.Controls.Add(button2);

            // Створення та запуск потоків
            thread1 = new Thread(() => MoveButton(button1, Key.Left, Key.Right, Key.Up, Key.Down));
            thread1.SetApartmentState(ApartmentState.STA);
            thread1.Start();

            thread2 = new Thread(() => MoveButton(button2, Key.A, Key.D, Key.W, Key.S));
            thread2.SetApartmentState(ApartmentState.STA);
            thread2.Start();

            thread1 = new Thread(() => MoveButton(button2, Key.Left, Key.Right, Key.Up, Key.Down));
            thread1.SetApartmentState(ApartmentState.STA);
            thread1.Start();

            thread2 = new Thread(() => MoveButton(button1, Key.A, Key.D, Key.W, Key.S));
            thread2.SetApartmentState(ApartmentState.STA);
            thread2.Start();
        }

        private void MoveButton(Button button, Key left, Key right, Key up, Key down)
        {
            while (!isFormClosing)
            {
                bool moved = false;

                lock (syncLock)
                {
                    if (Keyboard.IsKeyDown(left))
                    {
                        Invoke(new Action(() => button.Left -= 5));
                        LogKey(left);
                        moved = true;
                    }
                    if (Keyboard.IsKeyDown(right))
                    {
                        Invoke(new Action(() => button.Left += 5));
                        LogKey(right);
                        moved = true;
                    }
                    if (Keyboard.IsKeyDown(up))
                    {
                        Invoke(new Action(() => button.Top -= 5));
                        LogKey(up);
                        moved = true;
                    }
                    if (Keyboard.IsKeyDown(down))
                    {
                        Invoke(new Action(() => button.Top += 5));
                        LogKey(down);
                        moved = true;
                    }
                }

                if (moved)
                {
                    Thread.Sleep(50); // Регулювання швидкості
                }
            }
        }

        private void LogKey(Key key)
        {
            lock (keyLogs)
            {
                keyLogs.Add(key.ToString());
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            isFormClosing = true;
            if (thread1 != null && thread1.IsAlive)
            {
                thread1.Join();
            }
            if (thread2 != null && thread2.IsAlive)
            {
                thread2.Join();
            }
        }
    }
}