
namespace JustPlanes.Network
{

    public enum ServerPackets
    {
        SWelcomeMsg = 1,
        SGivePlayers = 2,
        SPlayerJoined = 3,
    }
    
    public enum ClientPackets
    {
        CHelloServer = 1,
        CGiveMePlayers = 2,
    }
}