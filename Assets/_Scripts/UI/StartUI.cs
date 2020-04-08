﻿using ChessCrush.Game;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChessCrush.UI
{
    public class StartUI: MonoBehaviour
    {
        [SerializeField]
        private Button startButton;
        [SerializeField]
        private Button optionsButton;
        [SerializeField]
        private Button quitButton;
        [SerializeField]
        private GameObject loadingWidget;

        private void Awake()
        {
            startButton.OnClickAsObservable().Subscribe(_ => SubscribeStartButton()).AddTo(gameObject);
            optionsButton.OnClickAsObservable().Subscribe(_ => SubscribeOptionsButton()).AddTo(gameObject);
            quitButton.OnClickAsObservable().Subscribe(_ => SubscribeQuitButton()).AddTo(gameObject);
        }

        private void SubscribeStartButton()
        {
            loadingWidget.SetActive(true);
            Director.instance.GetSubDirector<ChessGameDirector>();
        }

        private void SubscribeOptionsButton()
        {

        }

        private void SubscribeQuitButton()
        {

        }
    }
}