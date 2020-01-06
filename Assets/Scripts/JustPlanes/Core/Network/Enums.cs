
namespace JustPlanes.Core.Network
{

    public enum ServerPackets
    {
        SWelcomeMsg = 1,
        SGivePlayers = 2,
        SGiveUnits = 3,
        SPlayerJoined = 4,
        SUnitSpawned = 5,
        SUnitDied = 6,
        SUnitsDied = 7,
        SUnitsDamaged = 8,
        SUpdateMission = 9,
        SGiveMission = 10,
        SCompleteMission = 11,
        SPlayerLeft = 12,
        SLoginResp = 13,
    }
    
    public enum ClientPackets
    {
        CHelloServer = 1,
        CGiveMePlayers = 2,
        CGiveMeUnits = 3,
        CUnitDamaged = 4,
        CGiveMeMission = 5,
        CLoginReq = 6,
    }
}