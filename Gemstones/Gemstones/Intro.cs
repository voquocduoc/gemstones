using System.Drawing;
using System.Threading;

namespace Gemstones.Scene
{
    public class Intro : IScene
    {
        private readonly string name = "Intro";
        private readonly int MAX_STAGE = 3;

        private GameMain gameMain;
        private int drawingStage;

        private Color color;
        private bool isIncreasing = true;

        private Thread t;
        public bool threadIsOver = false;

        public Intro(GameMain gameMain) {
            this.gameMain = gameMain;
            drawingStage = 0;
            color = Color.FromKnownColor(KnownColor.Gold);
            color = Color.FromArgb(0, color.R, color.G, color.B);
            t = new Thread(AdvanceAlphaColor);
            t.Name = name;
            t.IsBackground = true;
            t.Start();
        }

        ~Intro() {
            threadIsOver = true;
        }

        public string GetName() {
            return name;
        }

        public void Draw(Graphics g) {
            switch(drawingStage) {
            case 0:
                g.DrawString("RISING SUN ENTERTAINMENT", new Font("Arial", 40, FontStyle.Bold | FontStyle.Underline), new SolidBrush(color), 50, 200);
                g.DrawString("proudly presents", new Font("Arial", 30), new SolidBrush(color), 250, 280);
                break;
            case 1:
                g.DrawString("UNIVERSITY OF SCIENCE", new Font("Arial", 40, FontStyle.Bold | FontStyle.Underline), new SolidBrush(color), 50, 200);
                g.DrawString("Faculty of Mathematics and Computer Science", new Font("Arial", 30), new SolidBrush(color), 80, 280);
                break;
            case 2:
                g.DrawString("A PRODUCT BY", new Font("Arial", 40, FontStyle.Bold | FontStyle.Underline), new SolidBrush(color), 100, 200);
                g.DrawString("NGUYỄN QUỐC THÁI DƯƠNG", new Font("Arial", 30), new SolidBrush(color), 130, 280);
                g.DrawString("VÕ QUỐC ĐƯỢC", new Font("Arial", 30), new SolidBrush(color), 140, 330);
                g.DrawString("VŨ VĂN THANH", new Font("Arial", 30), new SolidBrush(color), 150, 380);
                break;
            }
        }

        private void AdvanceAlphaColor() {
            if(Thread.CurrentThread.Name != this.name)
                return;

            while (!threadIsOver)
            {
                if (color.A == 255 || color.A == 0)
                    Thread.Sleep(1000);

                if (isIncreasing)
                {
                    if (color.A == 255)
                        isIncreasing = !isIncreasing;
                    else
                        color = Color.FromArgb(color.A + 5, color.R, color.G, color.B);
                }
                else
                {
                    if (color.A == 0)
                    {
                        ++drawingStage;
                        if(drawingStage >= MAX_STAGE) {
                            threadIsOver = true;
                            gameMain.UpdateScene(gameMain.mainMenu);
                            gameMain.intro = null;
                            break;
                        }
                        isIncreasing = !isIncreasing;
                    }
                    else
                        color = Color.FromArgb(color.A - 5, color.R, color.G, color.B);
                }

                Thread.Sleep(25);
            }
        }

        private void RealDraw(object o) {
            Graphics g;
            if (o is Graphics)
                g = o as Graphics;
            else return;


        }
    }
}
