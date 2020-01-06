using System;
using JustPlanes.Core.Network;

namespace JustPlanes.Core
{

    public class Authenticator
    {
        private Action<NameNetworkData> tryLogin;
        private Action<MessageResponseNetworkData> handleLogin;
        public bool isAuthenticated = false;

        public delegate void Login(string msg);
        public event Login OnLoginSucceeded;
        public event Login OnLoginFailed;

        public Authenticator()
        {
            tryLogin = NetworkMagic.RegisterCommand<NameNetworkData>(1, CmdTryLogin);
            handleLogin = NetworkMagic.RegisterTargeted<MessageResponseNetworkData>(1, TargetedRPCHandleLogin);
        }

        public void TryLogin(string name)
        {
            if (isAuthenticated)
                return;
            tryLogin(new NameNetworkData { Name = name });
        }

        private void CmdTryLogin(NameNetworkData data)
        {
            MessageResponseNetworkData resp = new MessageResponseNetworkData { connId = data.connId };
            if (Network.Server.GameRunner.Game.AddPlayerName(data.connId, data.Name))
                resp.IsSuccess = true;
            else
                resp.Message = "Name already taken.";
            handleLogin(resp);
        }

        // private void HandleLogin(LoginResponse loginResponse)
        // {
        //     handleLogin.Invoke(loginResponse);
        // }

        private void TargetedRPCHandleLogin(MessageResponseNetworkData loginResponse)
        {
            if (loginResponse.IsSuccess)
            {
                isAuthenticated = true;
                OnLoginSucceeded(loginResponse.Message);
            }
            else
                OnLoginFailed(loginResponse.Message);
        }
    }

    internal class MessageResponseNetworkData : NetworkData
    {
        public bool IsSuccess = false;
        public string Message = "0";
    }

    internal class NameNetworkData : NetworkData
    {
        public string Name = "Guest";
    }
}