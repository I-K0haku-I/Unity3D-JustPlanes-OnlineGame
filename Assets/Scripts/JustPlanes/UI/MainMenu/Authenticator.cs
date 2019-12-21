using System;
using JustPlanes.Network;
using JustPlanes.UI;
using UnityEngine.Events;

namespace JustPlanes
{
    class Authenticator : IAuthenticator
    {
        public UnityEvent<ILoginResponse> OnLogin()
        {
            throw new System.NotImplementedException();
        }

        public void TryLogin(string name)
        {
            NetworkManager.instance.net.Request(ClientPackets.CLoginReq, RespondToLogin);
        }


        private void RespondToLogin(ByteBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}