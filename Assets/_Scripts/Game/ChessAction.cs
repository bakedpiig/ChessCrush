﻿namespace ChessCrush.Game
{
    public class ChessAction
    {
        public int pieceId;
        public PieceType pieceType;
        public ChessBoardVector chessBoardVector;

        public ChessAction() { }

        public ChessAction(int pieceId,PieceType pieceType,ChessBoardVector vector)
        {
            this.pieceId = pieceId;
            this.pieceType = pieceType;
            chessBoardVector = vector;
        }

        public void Read(InputMemoryStream inputMemoryStream)
        {
            inputMemoryStream.Read(out pieceId);
            inputMemoryStream.Read(out int typeRes);
            pieceType = (PieceType)typeRes;
            inputMemoryStream.Read(out int x);
            inputMemoryStream.Read(out int y);
            chessBoardVector = new ChessBoardVector(x, y);
        }

        public void Write(OutputMemoryStream outputMemoryStream)
        {
            outputMemoryStream.Write(pieceId);
            outputMemoryStream.Write((int)pieceType);
            outputMemoryStream.Write(chessBoardVector.x);
            outputMemoryStream.Write(chessBoardVector.y);
        }
    }
}