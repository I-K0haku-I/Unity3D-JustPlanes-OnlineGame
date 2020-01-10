using System;
using JustPlanes.Core.Network;

namespace JustPlanes.Core
{

    public class Authenticator
    {
        private Action<NameNetworkData> tryLogin;
        private Action<MessageResponseNetworkData> handleLogin;
        public bool IsAuthenticated = false;

        public delegate void Login(string msg);
        public event Login OnLoginSucceeded;
        public event Login OnLoginFailed;

        private int entityId;

        public Authenticator()
        {
            entityId = 23135;
            tryLogin = NetworkMagic.RegisterAtServer<NameNetworkData>(1, TryLogin_AtServer, entityId);
            handleLogin = NetworkMagic.RegisterAtClient<MessageResponseNetworkData>(1, HandleLogin_AtClient, entityId);
        }

        public void TryLogin(string name)
        {
            if (IsAuthenticated)
                return;

            DebugLog.Warning("[Auth] Attempting to log in...");
            tryLogin(new NameNetworkData { Name = name });
        }

        private void TryLogin_AtServer(NameNetworkData data)
        {
            MessageResponseNetworkData resp = new MessageResponseNetworkData { ConnId = data.ConnId };
            if (Network.Server.GameRunner.Game.AddPlayerName(data.ConnId, data.Name))
                resp.IsSuccess = true;
            else
                resp.Message = "Name already taken.";
            DebugLog.Warning($"[Auth] Login response: IsSuccess: {resp.IsSuccess.ToString()}; Msg: {resp.Message}");
            handleLogin(resp);
        }

        // private void HandleLogin(LoginResponse loginResponse)
        // {
        //     handleLogin.Invoke(loginResponse);
        // }

        private void HandleLogin_AtClient(MessageResponseNetworkData loginResponse)
        {
            if (loginResponse.IsSuccess)
            {
                IsAuthenticated = true;
                DebugLog.Warning("[Auth] Login successfully.");
                OnLoginSucceeded?.Invoke(loginResponse.Message);
            }
            else
            {
                DebugLog.Warning("[Auth] Log-in failed.");
                OnLoginFailed?.Invoke(loginResponse.Message);
            }
        }
    }

    public class MessageResponseNetworkData : NetworkData
    {
        public bool IsSuccess = false;
        public string Message = "0";
    }

    public class NameNetworkData : NetworkData
    {
        public string Name = "Guest";
    }
}