using UnityEngine;
using System;
using JustPlanes.Unity;
using JustPlanes.Unity.UI;

namespace JustPlanes
{
    public static class DebugLog
    {
        public static void Warning(string msg)
        {
            Debug.Log(msg);
        }

        public static void Info(string msg)
        {
            Debug.Log(msg);
        }

        public static void LogConn(string msg)
        {
            Console.WriteLine(msg);
        }

        internal static void Severe(string v)
        {
            throw new NotImplementedException();
        }

        internal static void Severe(MissionPanelPresenter missionPanelPresenter, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Severe(MissionPanelView missionPanelView, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finer(MissionPanelView missionPanelView, string v)
        {
            throw new NotImplementedException();
        }

        public static void LogPackets(string msg)
        {
            Debug.Log(msg);
        }

        internal static void Severe(PlayerListPanelOld playerListPanelOld, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Severe(UnitViewManager unitViewManager, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finer(string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finer(PlayerListPanelManager playerListPanelManager, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Fine(MissionHandlerManager missionHandlerManager, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finest(PlayerListPanelOld playerListPanelOld, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Severe(MissionUIView missionUIView, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finest(UnitViewManager unitViewManager, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Severe(MissionHandlerManager missionHandlerManager, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Severe(PlayerListPanelManager playerListPanelManager, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Fine(UnitViewManager unitViewManager, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finest(PlayerListPanelManager playerListPanelManager, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finest(MissionPanelView missionPanelView, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finer(PlayerListPanelOld playerListPanelOld, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finer(MissionUIView missionUIView, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Warning(MissionHandlerManager missionHandlerManager, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finer(MissionPanelPresenter missionPanelPresenter, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Finest(MissionHandlerManager missionHandlerManager, string v)
        {
            throw new NotImplementedException();
        }

        internal static void Warning(UnitViewManager unitViewManager, string v)
        {
            throw new NotImplementedException();
        }
    }
}