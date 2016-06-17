using System;
using System.Drawing;

namespace Gemstones
{
    public delegate uint LevelFormula(ushort x);

    public class Statistics
    {
        private LevelFormula levelFormula;
        public static readonly string gameOverStatus = "No more moves! Press ENTER key to return to Main Menu.";

        public uint score { get; private set; }
        public ushort level { get; private set; }
        public string status { get; private set; }

        public Statistics() {
            score = 0;
            level = 1;
            status = "";
            //levelFormula = x => (uint)(1000 * level + (6 / 5 * Math.Pow((double)x, 3.0) - 15 * Math.Pow((double)x, 2.0) + 100 * x - 140));
            levelFormula = x => (uint)(800 + Math.Exp(level));
        }

        public void AddScore(int numOfGemsCollapsed) {
            if (numOfGemsCollapsed <= 0)
                return;
            uint scoreAdded = (uint) Math.Round(Math.Exp(numOfGemsCollapsed * 1.1));
            score += scoreAdded;
            status = "Obtained " + scoreAdded + " points.";
            while (score >= levelFormula(level))
            {
                ++level;
                status = "You've reached level " + level;
            }
        }

        public void Draw(Graphics g) {
            using(Font font = new Font("Arial", 20)) {
                g.DrawString("Score: " + score, font, Brushes.Gold, 700, 150);
                g.DrawString("Level: " + level, font, Brushes.Gold, 700, 200);
                g.DrawString("To next level: " + (levelFormula(level) - score), font, Brushes.Gold, 700, 250);
                g.DrawString(status, font, !status.Equals(Statistics.gameOverStatus) ? Brushes.Gold : Brushes.Red, 100, 30);
            }
        }

        public void DisplayGameOverStatus() {
            status = Statistics.gameOverStatus;
        }
    }
}
