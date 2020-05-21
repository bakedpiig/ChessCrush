﻿using BackEnd;
using BackEnd.Tcp;
using ChessCrush.OperationResultCode;
using ChessCrush.UI;
using LitJson;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ChessCrush.Game
{
    public class BackendDirector : SubDirector
    {
        private string roomToken;

        private void Awake()
        {
            Backend.Initialize(() =>
            {
                if (Backend.IsInitialized)
                {
                    SetBackendSetting();
                    gameObject.UpdateAsObservable().Subscribe(_ => Backend.Match.poll()).AddTo(gameObject);
                }
                else
                    MessageBoxUI.UseWithComponent("Failed to connect to server");
            });
        }

        private void SetBackendSetting()
        {
            string gameRoomToken = "";

            Backend.Match.OnJoinMatchMakingServer += args =>
            {
                if (args.ErrInfo != ErrorInfo.Success)
                    MessageBoxUI.UseWithComponent("Failed to join match making server");
            };
            Backend.Match.OnLeaveMatchMakingServer += args =>
            {

            };
            Backend.Match.OnMatchMakingResponse += args =>
            {
                switch (args.ErrInfo)
                {
                    case ErrorCode.Success:
                        Backend.Match.JoinGameServer(args.Address, args.Port, false, out var errorInfo);
                        gameRoomToken = args.Token;
                        break;
                    case ErrorCode.Match_InvalidMatchType:
                    case ErrorCode.Match_InvalidModeType:
                    case ErrorCode.InvalidOperation:
                        MessageBoxUI.UseWithComponent("Failed to do match making");
                        break;
                    default:
                        return;
                }
            };
            Backend.Match.OnException += args =>
            {
                MessageBoxUI.UseWithComponent("Network error");
                Debug.Log(args.ToString());
            };

            Backend.Match.OnSessionJoinInServer += args =>
            {
                Backend.Match.JoinGameRoom(gameRoomToken);
            };
            Backend.Match.OnSessionOnline += args => { };
            Backend.Match.OnSessionListInServer += args =>
            {

            };
            Backend.Match.OnMatchInGameAccess += args => { };
            Backend.Match.OnMatchInGameStart += () => { };
            Backend.Match.OnMatchRelay += args => { };
            Backend.Match.OnMatchChat += args => { };
            Backend.Match.OnMatchResult += args => { };
            Backend.Match.OnLeaveInGameServer += args => { };
            Backend.Match.OnSessionOffline += args => { };
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

        public void LoginWithBackendToken(Action successCallback, Action<string> failedCallback)
        {
            if (PlayerPrefs.HasKey("access_token"))
            {
                var bro = Backend.BMember.LoginWithTheBackendToken();
                if (bro.IsSuccess())
                    successCallback();
                else
                    failedCallback("Failed to log in");
            }
        }

        public void SignOut(Action successCallback, Action<string> failedCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.BMember.SignOut(c =>
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
                        failedCallback("Failed to sign out");

                    bro.Clear();
                    success.Dispose();
                }
            });
        }

        public void LogOut(Action successCallback, Action<string> failedCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.BMember.Logout(c =>
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
                        failedCallback("Failed to log out");

                    bro.Clear();
                    success.Dispose();
                }
            });
        }

        public void GetUserInfo(Action<JsonData> successCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.BMember.GetUserInfo(c =>
            {
                bro = c;
                success.Value = true;
            });

            success.Subscribe(value =>
            {
                if (value)
                {
                    var saveToken = Backend.BMember.SaveToken(bro);
                    if (saveToken.IsSuccess())
                    {
                        var infoJson = bro.GetReturnValuetoJSON()["row"];
                        successCallback(infoJson);
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

        public void UpdateNickname(string nickname, Action successCallback,Action<string> failedCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.BMember.UpdateNickname(nickname, c =>
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
                                failedCallback("Failed to update nickname: nickname doesn't fit");
                                break;
                            case SetNicknameCode.DuplicatedParameterException:
                                failedCallback("Failed to update nickname: duplicated nickname");
                                break;
                            case SetNicknameCode.Etc:
                                failedCallback("Failed to update nickname");
                                break;
                        }
                    }

                    bro.Clear();
                    success.Dispose();
                }
            });
        }

        public void GetFriendList(Action<JsonData> successCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.Social.Friend.GetFriendList(c =>
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
                    {
                        LitJson.JsonData jsonData = bro.GetReturnValuetoJSON();
                        successCallback(jsonData);
                    }

                    bro.Clear();
                    success.Dispose();
                }
            });
        }

        public void GetReceivedRequestList(Action<JsonData> successCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.Social.Friend.GetReceivedRequestList(c =>
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
                    {
                        JsonData jsonData = saveToken.GetReturnValuetoJSON();
                        successCallback(jsonData);

                    }

                    bro.Clear();
                    success.Dispose();
                }
            });
        }

        public void GetSentRequestList(Action<JsonData> successCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.Social.Friend.GetSentRequestList(c =>
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
                    {
                        JsonData jsonData = saveToken.GetReturnValuetoJSON();
                        successCallback(jsonData);
                    }
                }

                bro.Clear();
                success.Dispose();
            });
        }

        public void AcceptFriend(string friendInDate, Action successCallback, Action<string> failedCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.Social.Friend.AcceptFriend(friendInDate, c =>
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
                        failedCallback("Failed to accept friend");

                    bro.Clear();
                    success.Dispose();
                }
            });
    }

        public void RejectFriend(string friendInDate,Action successCallback,Action<string> failedCallback)
        {
            var success = new ReactiveProperty<bool>();
            var bro = new BackendReturnObject();

            Backend.Social.Friend.RejectFriend(friendInDate, c =>
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
                        failedCallback("Failed to reject friend");

                    bro.Clear();
                    success.Dispose();
                }
            });
        }
    }
}