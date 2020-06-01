﻿using DG.Tweening;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ChessCrush.Game
{
    public class ChessGameObjects : MonoBehaviour 
    {
        public ChessBoard chessBoard;
        public Transform chessBoardOrigin;
        public Transform attackIconPosition;

        private readonly float myTurnAttackIconPositionY = -1.7f;
        private readonly float enemyTurnAttackIconPositionY = 2.7f;

        private ChessGameDirector chessGameDirector;

        private void Start()
        {
            chessGameDirector = Director.instance.GetSubDirector<ChessGameDirector>();
            chessGameDirector.turnCount.Subscribe(value => MoveAttackIcon(value)).AddTo(chessGameDirector);
        }

        private void OnEnable()
        {
            attackIconPosition.gameObject.SetActive(false);
        }

        public void SetExpectedAction(List<ChessAction> actions)
        {
            chessBoard.ClearExpectedChessPieces();
            foreach(var action in actions)
                chessBoard.AddExpectedChessPiece(ChessPiece.UseWithComponent(action.pieceId, action.chessBoardVector.x, action.chessBoardVector.y, action.pieceType, true));
        }

        private void MoveAttackIcon(int turn)
        {
            if (turn <= 0) return;

            if (!attackIconPosition.gameObject.activeSelf)
            {
                if (chessGameDirector.player.IsWhite)
                    attackIconPosition.position = new Vector3(attackIconPosition.position.x, myTurnAttackIconPositionY);
                else
                    attackIconPosition.position = new Vector3(attackIconPosition.position.x, enemyTurnAttackIconPositionY);

                attackIconPosition.gameObject.SetActive(true);
            }
            else
            {
                if ((chessGameDirector.player.IsWhite && turn % 2 == 0) || (!chessGameDirector.player.IsWhite && turn % 2 != 0))
                    attackIconPosition.DOMoveY(enemyTurnAttackIconPositionY, 1.0f).SetEase(Ease.OutCirc);
                else
                    attackIconPosition.DOMoveY(myTurnAttackIconPositionY, 1.0f).SetEase(Ease.OutCirc);
            }
        }
    }
}