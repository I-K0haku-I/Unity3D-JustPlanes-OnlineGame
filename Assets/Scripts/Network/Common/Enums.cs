
namespace JustPlanes.Network
{

    public enum ServerPackets
    {
        SWelcomeMsg = 1,
        SGivePlayers = 2,
        SGiveUnits = 3,
        SPlayerJoined = 4,
        SUnitSpawned = 5,
        SUnitDied = 6,
    }
    
    public enum ClientPackets
    {
        CHelloServer = 1,
        CGiveMePlayers = 2,
        CGiveMeUnits = 3,
        CUnitDamaged = 4,
    }
}