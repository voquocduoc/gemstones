using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System;

namespace Gemstones.Board
{
    using Board = List<List<Gem>>;
    
    public class NoMoreMovesChecker
    {
        public bool isNoMoreMoves = false;
        public Point hintGem;
        private GameBoard gameBoard;

        public NoMoreMovesChecker(GameBoard gameBoard)
        {
            this.gameBoard = gameBoard;
        }

        public bool CheckForNoMoreMoves() {
            if (gameBoard.board == null)
                return false;

            Board boardClone = BoardClone(gameBoard.board);

            //Switch the board according to columns
            for (int i = 0; i < GameBoard.size - 1; i++)
                for (int j = 0; j < GameBoard.size; j++) {
                    boardClone[i][j].SwitchWithoutAnimation(boardClone[i + 1][j], boardClone);
                    if (!IsNoMoreMoves(boardClone)) {
                        hintGem = new Point(i, j);
                        isNoMoreMoves = false;
                        return false;
                    }
                    boardClone[i][j].SwitchWithoutAnimation(boardClone[i + 1][j], boardClone); //Switch back
                }
                
            //Switch the board according to rows
            for (int i = 0; i < GameBoard.size; i++)
                for (int j = 0; j < GameBoard.size - 1; j++) {
                    boardClone[i][j].SwitchWithoutAnimation(boardClone[i][j + 1], boardClone);
                    if (!IsNoMoreMoves(boardClone)) {
                        hintGem = new Point(i, j);
                        isNoMoreMoves = false;
                        return false;
                    }
                    boardClone[i][j].SwitchWithoutAnimation(boardClone[i][j + 1], boardClone); //Switch back
                }
                 
            isNoMoreMoves = true;
            return true;
        }

        private Board BoardClone(Board src) {
            Board result = new Board();

            for (int i = 0; i < src.Count; ++i)
            {
                List<Gem> col = new List<Gem>();
                for (int j = 0; j < src.Count; ++j)
                    col.Add(src[i][j].Clone());
                result.Add(col);
            }

            return result;
        }

        private HashSet<Gem> GetGemsToBeDestroyed(Board board) { 
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

        private bool IsNoMoreMoves(Board board) {
            HashSet<Gem> gemsToBeDestroyed = GetGemsToBeDestroyed(board);
            if (gemsToBeDestroyed.Count == 0 || gemsToBeDestroyed == null)
                return true;
            return false;
        }
    }
}
