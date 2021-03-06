﻿using ChessCrush.Game;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChessCrush.UI
{
    public class ChessGameUI: MonoBehaviour
    {
        public ObjectPool objectPool;
        public PlayerStatus myStatus;
        public PlayerStatus enemyStatus;
        public Text turnText;
        public ChessActionScroll chessActionScroll;
        [SerializeField]
        private Button inputButton;
        [SerializeField]
        private InputTimeCircle inputTimeCircle;
        [SerializeField]
        private GameObject waitingTextObject;
        [SerializeField]
        private ButtonInformationPopup buttonInformationPopup;
        [SerializeField]
        private ChessGameAlert chessGameAlert;
        public GameOverWidget gameOverWidget;

        public ButtonInformationPopup ButtonInformationPopup => buttonInformationPopup;

        private List<PieceSelectButton> pieceSelectButtons = new List<PieceSelectButton>();

        public float LessTime => inputTimeCircle.LessTime.Value;

        private ChessGameDirector chessGameDirector;

        private void Start()
        {
            chessGameDirector = Director.instance.GetSubDirector<ChessGameDirector>();
            chessGameDirector.turnCount.Subscribe(value =>
            {
                turnText.text = $"Turn {value.ToString()}";
                inputTimeCircle.gameObject.SetActive(true);
            }).AddTo(chessGameDirector);
            chessGameDirector.expectedActionDeleteSubject.Subscribe(value => pieceSelectButtons.Find(button => button.PieceId == value)?.gameObject.SetActive(true)).AddTo(chessGameDirector);
            inputButton.OnClickAsObservable().Subscribe(_ => chessGameDirector.inputCompleted = true).AddTo(gameObject);
        }

        private void OnDisable()
        {
            chessGameAlert.gameObject.SetActive(false);
            gameOverWidget.gameObject.SetActive(false);
        }

        public void SetSelectButtons(List<ChessPiece> pieces)
        {
            objectPool.DestroyAll();
            pieceSelectButtons.Clear();

            foreach(var piece in pieces)
                if(piece.IsMine)
                    pieceSelectButtons.Add(PieceSelectButton.UseWithComponent(piece.PieceId, piece.chessBoardVector, piece.PieceType));
        }

        public void SetInputAreaActive(bool active)
        {
            inputButton.gameObject.SetActive(active);
            inputTimeCircle.gameObject.SetActive(active);
            waitingTextObject.SetActive(!active);
        }

        public void SetInputTimeCircle(float time) => inputTimeCircle.LessTime.Value = time;

        public void UpdateDisconnectOpposite() => chessGameAlert.Appear("Opposite player disconnected");

        public void UpdateReconnectOpposite() => chessGameAlert.Disappear();
    }
}
