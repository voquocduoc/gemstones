using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gemstones
{
    
    public partial class Form1 : Form
    {
        GameMain gameMain;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gameMain = new GameMain(this);
            gameMain.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (gameMain.statistics.status.Equals(Statistics.gameOverStatus) && gameMain.currentScene == gameMain.inGame)
                    {
                        gameMain.sm.PlayMainTheme();
                        gameMain.UpdateScene(gameMain.mainMenu);
                        break;
                    }
                    if (gameMain.currentScene == gameMain.mainMenu) {
                        gameMain.inGame.gameBoard.InitializeBoard();
                        gameMain.sm.PlayGameplay();
                        gameMain.UpdateScene(gameMain.inGame);
                    }
                    break;
                case Keys.D1:
                    gameMain.debuggerTools.GenerateGem(Board.GemName.Amethyst);
                    break;
                case Keys.D2:
                    gameMain.debuggerTools.GenerateGem(Board.GemName.Axinite);
                    break;
                case Keys.D3:
                    gameMain.debuggerTools.GenerateGem(Board.GemName.Diamond);
                    break;
                case Keys.D4:
                    gameMain.debuggerTools.GenerateGem(Board.GemName.Emerald);
                    break;
                case Keys.D5:
                    gameMain.debuggerTools.GenerateGem(Board.GemName.Ruby);
                    break;
                case Keys.D6:
                    gameMain.debuggerTools.GenerateGem(Board.GemName.Sapphire);
                    break;
                case Keys.D7:
                    gameMain.debuggerTools.GenerateGem(Board.GemName.Nothing);
                    break;
                case Keys.H:
                    gameMain.inGame.drawHint = !gameMain.inGame.drawHint;
                    break;
                case Keys.N:
                    gameMain.inGame.gameBoard.InitializeBoard();
                    gameMain.statistics = new Statistics();
                    break;
                case Keys.L:
                    gameMain.inGame.gameBoard.LoadBoardFromFile("NoMovesCase.txt");
                    break;
                case Keys.Escape:
                    if (gameMain.currentScene == gameMain.inGame) {
                        if (MessageBox.Show("Return to Main Menu?", "Gemstones", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        {
                            gameMain.sm.PlayMainTheme();
                            gameMain.UpdateScene(gameMain.mainMenu);
                        }
                        break;
                    }

                    if (MessageBox.Show("Close the game?", "Gemstones", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        gameMain.Cleanup();
                        Application.Exit();
                    }
                    break;
                case Keys.F12:
                    if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    else
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                    break;
            }
        }
    }
}