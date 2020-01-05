using System;
using JustPlanes.Network;

namespace JustPlanes
{

    public class Authenticator
    {
        private PlayerManager playerManager;
        private Action<NameNetworkData> tryLogin;
        private Action<MessageResponseNetworkData> handleLogin;
        public bool isAuthenticated = false;

        public delegate void Login(string msg);
        public event Login OnLoginSucceeded;
        public event Login OnLoginFailed;

        public Authenticator(PlayerManager playerManager)
        {
            this.playerManager = playerManager;
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
            if (playerManager.IsNameTaken(data.Name))
            {
                resp.Message = "Name already taken.";
                handleLogin(resp);
                return;
            }

            playerManager.AddPlayer(data.Name);
            resp.IsSuccess = true;
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