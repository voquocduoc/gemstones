using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;

namespace Gemstones.Scene
{
    public class MainMenu : IScene
    {
        private readonly string name = "Main Menu";
        private GameMain game;

        private Thread t;
        private bool threadStarted = false;
        private Color color;
        private bool isIncreasing = true;
        
        public Bitmap background { get; private set; }

        public MainMenu(GameMain game) {
            this.game = game;
            //background = Helpers.LoadImage("mainbackground.png");
            background = Properties.Resources.mainbackground;
            color = Color.FromKnownColor(KnownColor.Gold);
        }

        public void Draw(Graphics g) {
            if (!threadStarted) {
                threadStarted = true;
                t = new Thread(LightEffect);
                t.Name = this.name;
                t.IsBackground = true;
                t.Start();
            }
            g.DrawImage(background, 0, 0);
            g.DrawString("PRESS START BUTTON", new Font("Elephant", 40, FontStyle.Bold | FontStyle.Italic), Brushes.Black, 103, 603);
            g.DrawString("PRESS START BUTTON", new Font("Elephant", 40, FontStyle.Bold | FontStyle.Italic), new SolidBrush(color), 100, 600);
        }

        public string GetName() {
            return name;
        }

        private void LightEffect() {
            if (!Thread.CurrentThread.Name.Equals(this.name))
                return;
            while (game.currentScene.GetName().Equals(this.name))
            {
                if (isIncreasing)
                {
                    if (color.B == 250)
                        isIncreasing = !isIncreasing;
                    else
                        color = Color.FromArgb(color.R, color.G, color.B + 10);
                }
                else
                {
                    if (color.B == 0)
                        isIncreasing = !isIncreasing;
                    else
                        color = Color.FromArgb(color.R, color.G, color.B - 10);
                }

                Thread.Sleep(50);
            }
            threadStarted = false;
        }
    }
}
