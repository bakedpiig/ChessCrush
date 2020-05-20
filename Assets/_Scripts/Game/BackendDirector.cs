﻿using BackEnd;
using ChessCrush.OperationResultCode;
using ChessCrush.UI;
using System;
using UniRx;
using UniRx.Triggers;

namespace ChessCrush.Game
{
    public class BackendDirector: SubDirector
    {
        private string roomToken;

        private void Awake()
        {
            Backend.Initialize(() =>
            {
                if (Backend.IsInitialized)
                    gameObject.UpdateAsObservable().Subscribe(_ => Backend.Match.poll()).AddTo(gameObject);
                else
                    MessageBoxUI.UseWithComponent("Failed to connect to server");
            });
        }

        public void CustomSignUp(string id, string password, Action successCallback, Action<string> failedCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.BMember.CustomSignUp(id, password, c =>
            {
                bro = c;
                success.Value = true;
            });

            success.ObserveOnMainThread().Subscribe(value =>
            {
                if (value)
                {
                    var saveToken = Backend.BMember.SaveToken(bro);
                    if (saveToken.IsSuccess())
                        successCallback();
                    else   
                    {
                        switch ((SignUpCode)Convert.ToInt32(bro.GetStatusCode()))
                        {
                            case SignUpCode.DuplicatedParameterException:
                                failedCallback("Failed to sign up: Duplicated id");
                                break;
                            case SignUpCode.Etc:
                                failedCallback("Failed to sign up");
                                break;
                            default:
                                return;
                        }
                    }

                    bro.Clear();
                    success.Dispose();
                }
            });
        }

        public void CustomLogin(string id, string password, Action successCallback, Action<string> failedCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.BMember.CustomLogin(id, password, c =>
            {
                bro = c;
                success.Value = true;
            });

            success.ObserveOnMainThread().Subscribe(value =>
            {
                if (value)
                {
                    var saveToken = Backend.BMember.SaveToken(bro);
                    if (saveToken.IsSuccess())
                        successCallback();
                    else
                    {
                        switch ((SignInCode)Convert.ToInt32(bro.GetStatusCode()))
                        {
                            case SignInCode.BadUnauthorizedException:
                                failedCallback("Failed to sign in: wrong id or password");
                                break;
                            case SignInCode.Blocked:
                                failedCallback("Failed to sign in: blocked user");
                                break;
                            case SignInCode.Etc:
                                failedCallback("Failed to sign in");
                                return;
                        }
                    }

                    bro.Clear();
                    success.Dispose();
                }
            });
        }

        public void CreateNickname(string nickname, Action successCallback, Action<string> failedCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.BMember.CreateNickname(nickname, c =>
            {
                bro = c;
                success.Value = true;
            });

            success.ObserveOnMainThread().Subscribe(value =>
            {
                if (value)
                {
                    var saveToken = Backend.BMember.SaveToken(bro);
                    if (saveToken.IsSuccess())
                        successCallback();
                    else
                    {
                        switch ((SetNicknameCode)Convert.ToInt32(bro.GetStatusCode()))
                        {
                            case SetNicknameCode.BadParameterException:
                                failedCallback("Failed to set nickname: Nickname doesn't fit");
                                break;
                            case SetNicknameCode.DuplicatedParameterException:
                                failedCallback("Failed to set nickname: Duplicated nickname");
                                break;
                            case SetNicknameCode.Etc:
                                failedCallback("Failed to set nickname");
                                break;
                        }
                    }

                    bro.Clear();
                    success.Dispose();
                }
            });
        }
    }
}