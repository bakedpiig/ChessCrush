﻿using ChessCrush.Game;
using UnityEngine;

namespace ChessCrush
{
    public struct ChessBoardVector
    {
        public int x;
        public int y;

        static Vector2 originPosition;
        private static readonly float ChessBoardBlockSize = 0.73f;

        public ChessBoardVector(int x, int y) { this.x = x; this.y = y; }

        public Vector3 ToWorldVector()
        {
            if (originPosition == default)
                originPosition = Director.instance.GetSubDirector<ChessGameDirector>().chessGameObjects.chessBoardOrigin.position;
            return new Vector3(x * ChessBoardBlockSize + originPosition.x, y * ChessBoardBlockSize + originPosition.y);
        }

        public ChessBoardVector ToMyBoardVector()
        {
            var result = new ChessBoardVector(x, y);
            result.x = 7 - result.x;
            result.y = 7 - result.y;
            return result;
        }

        public Vector3 ToWorldVectorOfCenter()
        {
            if (originPosition == default)
                originPosition = Director.instance.GetSubDirector<ChessGameDirector>().chessGameObjects.chessBoardOrigin.position;
            return new Vector3((x + 0.5f) * ChessBoardBlockSize + originPosition.x, (y + 0.5f) * ChessBoardBlockSize + originPosition.y);
        }
    }
}
