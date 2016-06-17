using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.IO;

namespace Gemstones.Board
{
    using Board = List<List<Gem>>;
    
    public class GameBoard
    {
        public static readonly int size = 8;
        public static readonly int cellEdge = 70;
        public static readonly int pixelJump = 5;
        public static readonly Rectangle playField = new Rectangle(0, 0, cellEdge * size, cellEdge * size);
        public static readonly Point TranslationParameter = new Point(100, 100);

        public Gemstones.Scene.InGame inGame { get; private set; }
        public Gem currentlySelectedGem;
        public Board board { get; private set; }
        private Random random;
        public NoMoreMovesChecker noMoreMovesChecker { get; private set; }

        public GameBoard(Gemstones.Scene.InGame inGame) {
            this.inGame = inGame;
            random = new Random();

            noMoreMovesChecker = new NoMoreMovesChecker(this);
            do {
                InitializeBoard();
            } while(noMoreMovesChecker.CheckForNoMoreMoves());
        }

        public void Draw(Graphics g) {
            g.TranslateTransform(TranslationParameter.X, TranslationParameter.Y);
            //Draw board's border
            g.DrawRectangle(new Pen(Brushes.Gold, 5), playField);
            g.FillRectangle(new SolidBrush(Color.FromArgb(20, 20, 20)), playField);

            for (int i = 0; i < board.Count; i++)
                for (int j = 0; j < board.Count; j++)
                    board[i][j].Draw(g);

            g.ResetTransform();
        }

        private HashSet<Gem> GetGemsToBeDestroyed(){
            HashSet<Gem> gemsToBeDestroyed = new HashSet<Gem>();
            int rowCount = 1, colCount = 1;
            //Row checking
            for (int i = 0; i < board.Count; i++)
                for (int j = 1; j < board.Count; j++ ) {
                    if (!board[i][j].name.Equals(GemName.Nothing))
                        if (board[i][j].name.Equals(board[i][j - 1].name)) {
                            rowCount++;
                            if (j < board.Count - 1)
                                continue;
                            j++;
                        }
                    if (rowCount >= 3)
                        while (rowCount > 0) {
                            gemsToBeDestroyed.Add(board[i][j - rowCount]);
                            rowCount--;
                        }
                    rowCount = 1;
                }
            //Col checking
            for (int j = 0; j < board.Count; j++)
                for (int i = 1; i < board.Count; i++) {
                    if (!board[i][j].name.Equals(GemName.Nothing))
                        if (board[i][j].name.Equals(board[i - 1][j].name)) {
                            colCount++;
                            if (i < board.Count - 1)
                                continue;
                            i++;
                        }
                    if (colCount >= 3)
                        while (colCount > 0) {
                            gemsToBeDestroyed.Add(board[i - colCount][j]);
                            colCount--;
                        }
                    colCount = 1;
                }
            return gemsToBeDestroyed;
        }

        private void FillNothingWithNewGems() { 
            //Scan the board upwards, if there is an empty cell, move gems that are above that cell down
            GemListToBeMoved toBeMovedList = new GemListToBeMoved(board);
            
            for (int i = board.Count - 1; i >= 0; i--)
                for (int j = 0; j < board.Count; j++) {
                    try {
                        if (!board[i][j].name.Equals(GemName.Nothing) || board[i + 1][j].name.Equals(GemName.Nothing))
                            continue;
                    }
                    catch (ArgumentOutOfRangeException) {}

                    int tempRow = i;
                    List<Gem> nothingGemList = new List<Gem>();
                    while (board[tempRow][j].name.Equals(GemName.Nothing)) {
                        nothingGemList.Add(board[tempRow][j]); //Add in reverse order - bottom-up
                        --tempRow;
                        if (tempRow < 0)
                            break;
                    }
                    nothingGemList.Reverse();
                    ColToBeMoved col = new ColToBeMoved(tempRow >= 0 ? nothingGemList.Count : 0);
                    col.nothingGemsInCol = nothingGemList;

                    //Add the the gems to be moved in this column to the col's list
                    for (; tempRow >= 0; tempRow--)
                        col.gemsInCol.Add(board[tempRow][j]);
                    col.gemsInCol.Reverse(); //Reverse the order of the list
                    toBeMovedList.Add(col);
                }

            toBeMovedList.DoAnimation();

            //create new random gems
            HashSet<Gem> nothingList = new HashSet<Gem>();
            for (int i = 0; i < board.Count; i++)
                for (int j = 0; j < board.Count; j++)
                    if (board[i][j].name == GemName.Nothing)
                        nothingList.Add(board[i][j]);
            SetNewGemsFromList(nothingList);
            //Check for matching again
            CheckAndDoMatching();
        }
        
        private void SetNewGemsFromList(HashSet<Gem> nothingList)
        {
            foreach (Gem gem in nothingList)
                gem.name = (GemName)random.Next(6);
            for (decimal scale = 0.1m; scale <= 1; scale += 0.1m)
            {
                foreach (Gem gem in nothingList)
                    gem.SetScaleParameter((float)scale);
                System.Threading.Thread.Sleep(GameMain.frameRate);
            }
        }

        public bool CheckAndDoMatching() {
            HashSet<Gem> gemsToBeDestroyed = GetGemsToBeDestroyed();
            if (gemsToBeDestroyed.Count == 0 || gemsToBeDestroyed == null)
                return false;

            //Play sound
            //inGame.main.sm.PlaySound(SoundName.GemBreak);

            //Add some scores
            inGame.main.statistics.AddScore(gemsToBeDestroyed.Count);

            for (decimal scale = 1.0m; scale > 0; scale -= 0.1m)
            {
                foreach (Gem gem in gemsToBeDestroyed)
                    gem.SetScaleParameter((float)scale);
                System.Threading.Thread.Sleep(GameMain.frameRate);
            }
            ResetZeroScaleToNothing();
            FillNothingWithNewGems();
            return true;
        }

        public Point GetTranslatedCursor() {
            Point cursor = GameMain.cursorLocation;
            cursor.X -= TranslationParameter.X;
            cursor.Y -= TranslationParameter.Y;
            return cursor;
        }

        public Gem GetCurrentGem() {
            foreach(List<Gem> list in board)
                foreach(Gem gem in list)
                    if (Helpers.IsWithin(gem.bounds, GetTranslatedCursor()))
                        return gem;
            return null;
        }

        public void InitializeBoard() {
            board = new Board();

            for (int i = 0; i < size; i++)
            {
                List<Gem> rowList = new List<Gem>();
                for (int j = 0; j < size; j++)
                {
                    Rectangle bounds = new Rectangle(playField.X + Gem.size * j, playField.Y + Gem.size * i, Gem.size, Gem.size);
                    Point index = new Point(i, j);
                    GemName tempRand;
                    //Create a gem that does not create a triplet with adjacent gems
                    do
                    {
                        tempRand = (GemName)random.Next(6);
                        if (i < 2 && j < 2)
                            break;
                    } while ((j >= 2 && tempRand.Equals(rowList[j - 1].name) && tempRand.Equals(rowList[j - 2].name))
                         || (i >= 2 && tempRand.Equals(board[i - 1][j].name) && tempRand.Equals(board[i - 2][j].name)));
                    Gem gem = new Gem(this, tempRand, bounds, index);
                    rowList.Add(gem);
                }
                board.Add(rowList);
            }

            inGame.IsAbleToClick = true;
            inGame.main.statistics = new Statistics();
        }

        private void ResetZeroScaleToNothing() { 
            foreach(List<Gem> list in board)
                foreach (Gem gem in list) {
                    if (gem.scaleParameter.X < 1f)
                        gem.ResetToNothing();
                }
        }

        public void LoadBoardFromFile(string location) {
            board = new Board();
            List<GemName> gemsList = new List<GemName>();

            try {
                StreamReader sr = new StreamReader(location);

                while (!sr.EndOfStream)
                {
                    string temp = sr.ReadLine();
                    for (int i = 0; i < temp.Length; i++) {
                        if (temp[i] == ' ')
                            continue;
                        int readNum = temp[i] - 48;
                        gemsList.Add((GemName) readNum);
                    }
                }
                sr.Close();
            }
            catch (FileNotFoundException)
            {
                System.Windows.Forms.MessageBox.Show("The file could not be found.");
            }

            int listIndex = 0;
            for (int i = 0; i < size; i++)
            {
                List<Gem> rowList = new List<Gem>();
                for (int j = 0; j < size; j++)
                {
                    Rectangle bounds = new Rectangle(playField.X + Gem.size * j, playField.Y + Gem.size * i, Gem.size, Gem.size);
                    Point index = new Point(i, j);
                    GemName tempRand = gemsList[listIndex++];
                    //Create a gem that does not create a triplet with adjacent gems

                    Gem gem = new Gem(this, tempRand, bounds, index);
                    rowList.Add(gem);
                }
                board.Add(rowList);
            }
        }
    }

    class GemListToBeMoved : List<ColToBeMoved> {
        private static Board board;

        public GemListToBeMoved(Board board) : base() {
            GemListToBeMoved.board = board;
        }

        public void DoAnimation() {
            if (!Thread.CurrentThread.Name.Equals("SwitchGem"))
                return;

            while(true) {
                int moveParamCount = 0;

                for(int i = 0; i < this.Count; ++i) {
                    if (this[i].moveParameter == 0) {
                        moveParamCount++;
                        continue;
                    }

                    //Move each gem in the column down
                    for (int j = 0; j < this[i].gemsInCol.Count; ++j)
                        this[i].gemsInCol[j].bounds = new Rectangle(this[i].gemsInCol[j].bounds.X, this[i].gemsInCol[j].bounds.Y + GameBoard.pixelJump, this[i].gemsInCol[j].bounds.Width, this[i].gemsInCol[j].bounds.Height);

                    if(this[i].gemsInCol.Count > 0)
                        if (this[i].gemsInCol[0].bounds.Y % Gem.size == 0)
                            this[i].moveParameter--;
                }

                if (moveParamCount == this.Count)
                    break;
                Thread.Sleep((int) (GameMain.frameRate / 1.5));
            }
        
            //Reupdate indexes and positions in the board
            //Update nothing cells
            for (int i = 0; i < this.Count; ++i)
                for (int j = 0; j < this[i].nothingGemsInCol.Count; ++j) {
                    this[i].nothingGemsInCol[j].bounds = new Rectangle(
                        this[i].nothingGemsInCol[j].bounds.X, j * Gem.size, Gem.size, Gem.size);
                    int newX = j;
                    int newY = this[i].nothingGemsInCol[j].index.Y;
                    this[i].nothingGemsInCol[j].index = new Point(newX, newY);
                    board[newX][newY] = this[i].nothingGemsInCol[j];
                }
            //Update gem cells
            for(int i = 0; i < this.Count; ++i)
                for (int j = 0; j < this[i].gemsInCol.Count; ++j) {
                    int newX = this[i].nothingGemsInCol.Count + j;
                    int newY = this[i].gemsInCol[j].index.Y;
                    this[i].gemsInCol[j].index = new Point(newX, newY);
                    board[newX][newY] = this[i].gemsInCol[j];
                }
        }
    }

    class ColToBeMoved {
        public List<Gem> gemsInCol { get; set; }
        public List<Gem> nothingGemsInCol { get; set; }
        public int moveParameter { get; set; }

        public ColToBeMoved(int num) {
            gemsInCol = new List<Gem>();
            nothingGemsInCol = new List<Gem>();
            moveParameter = num;
        }
    }
}
