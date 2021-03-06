using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Box2DX.Common;

namespace JustPlanes.Core.Network
{
    public class NetworkData
    {
        public string ConnId;
    }

    public class AtServerCommand<T> : NetworkCommand<T> where T : NetworkData
    {
        public AtServerCommand(int packageId, int entityId, Action<T> action) : base(packageId, PackageTypes.AtServer, entityId, action) { }

        public override void HandleData(T data)
        {
            if (NetworkMagic.IsServer)
            {
                Action.Invoke(data);
            }
            else if (NetworkMagic.IsClient)
            {
                var buffer = BuildBuffer(data);
                SendBufferToServer(buffer);
            }
        }
    }

    public class AtClientCommand<T> : NetworkCommand<T> where T : NetworkData
    {
        public AtClientCommand(int packageId, int entityId, Action<T> action) : base(packageId, PackageTypes.AtClient, entityId, action) { }

        public override void HandleData(T data)
        {
            if (NetworkMagic.IsServer)
            {
                var buffer = BuildBuffer(data);
                SendBufferToClient(data.ConnId, buffer);
            }
            else if (NetworkMagic.IsClient)
            {
                Action.Invoke(data);
            }
        }
    }

    public class AtAllClientsCommand<T> : NetworkCommand<T> where T : NetworkData
    {
        public AtAllClientsCommand(int packageId, int entityId, Action<T> action) : base(packageId, PackageTypes.AtAllClients, entityId, action) { }

        public override void HandleData(T data)
        {
            if (NetworkMagic.IsServer)
            {
                // Action.Invoke(data);
                var buffer = BuildBuffer(data);
                SendBufferToAll(buffer);
            }
            else if (NetworkMagic.IsClient)
            {
                Action.Invoke(data);
            }
        }
    }

    public abstract class NetworkCommand<T> : IHandleCommand where T : NetworkData
    {
        public Action<T> Action;
        public int PackageId;
        public PackageTypes PackageType;

        // 1 is default and ignored
        public int EntityId;

        protected NetworkCommand(int packageId, PackageTypes packageType, int entityId, Action<T> action)
        {
            this.PackageId = packageId;
            this.Action = action;
            this.PackageType = packageType;
            this.EntityId = entityId;
        }

        public abstract void HandleData(T data);

        protected ByteBuffer BuildBuffer(T data)
        {
            var buffer = new ByteBuffer();
            buffer.WriteInteger((int)PackageType);
            buffer.WriteInteger(PackageId);
            buffer.WriteInteger(EntityId);
            DebugLog.LogPackets($"[NetworkMagic] Writing a packet of type {PackageType.ToString()}:");
            foreach (var field in typeof(T).GetFields())
            {
                if (field.Name == nameof(NetworkData.ConnId))
                    continue;

                var value = field.GetValue(data);
                switch (value)
                {
                    case string s:
                        DebugLog.LogPackets($"writing a string for {field.Name}: {s.ToString()}");
                        buffer.WriteString(s);
                        break;
                    case float f:
                        DebugLog.LogPackets($"writing a float for {field.Name}: {f.ToString()}");
                        buffer.WriteFloat(f);
                        break;
                    case bool b:
                        DebugLog.LogPackets($"writing a bool for {field.Name}: {b.ToString()}");
                        buffer.WriteBool(b);
                        break;
                    case int i:
                        DebugLog.LogPackets($"writing an int: {i.ToString()}");
                        buffer.WriteInteger(i);
                        break;
                    case PointF p:
                        DebugLog.LogPackets($"writing a PointF: {p.ToString()}");
                        buffer.WriteFloat(p.X);
                        buffer.WriteFloat(p.Y);
                        break;
                    case Vec2 p:
                        DebugLog.LogPackets($"writing a Vec2: X: {p.X}, Y: {p.Y}");
                        buffer.WriteFloat(p.X);
                        buffer.WriteFloat(p.Y);
                        break;
                    case List<string> list:
                        {
                            DebugLog.LogPackets($"writing a list of string: {string.Join(", ", list)}");
                            buffer.WriteInteger(list.Count);
                            foreach (var item in list)
                            {
                                buffer.WriteString(item);
                            }

                            break;
                        }
                    default:
                        DebugLog.Severe($"[NetworkMagic] You are trying to encrypt an unknown data type for {typeof(T).FullName}!");
                        break;
                }
            }
            DebugLog.LogPackets("Writing---");
            return buffer;
        }

        protected void SendBufferToAll(ByteBuffer buffer)
        {
            Server.ClientManager.SendDataToAll(buffer.ToArray());
        }

        protected void SendBufferToServer(ByteBuffer buffer)
        {
            Client.ClientTCP.SendData(buffer.ToArray());
        }

        protected void SendBufferToClient(string connId, ByteBuffer buffer)
        {
            Server.ClientManager.SendDataTo(connId, buffer.ToArray());
        }

        public void HandleBuffer(string connectionId, ByteBuffer buffer)
        {
            var data = (T)Activator.CreateInstance(typeof(T));
            DebugLog.LogPackets($"[NetworkMagic] Reading a packet {PackageType.ToString()} with data type {typeof(T).Name}:");
            foreach (var field in typeof(T).GetFields())
            {
                if (field.Name == nameof(NetworkData.ConnId))
                    continue;

                if (field.FieldType == typeof(string))
                {
                    var value = buffer.ReadString();
                    DebugLog.LogPackets($"Decrypted string: {value}");
                    field.SetValue(data, value);
                }
                else if (field.FieldType == typeof(float))
                {
                    var value = buffer.ReadFloat();
                    DebugLog.LogPackets($"Decrypted float: {value.ToString()}");
                    field.SetValue(data, value);
                }
                else if (field.FieldType == typeof(bool))
                {
                    var value = buffer.ReadBool();
                    DebugLog.LogPackets($"Decrypted bool: {value}");
                    field.SetValue(data, value);
                }
                else if (field.FieldType == typeof(List<string>))
                {
                    var someList = new List<string>();
                    var length = buffer.ReadInteger();
                    for (var i = 0; i < length; i++)
                    {
                        someList.Add(buffer.ReadString());
                    }
                    DebugLog.LogPackets($"Decrypted list of string: {string.Join(", ", someList)}");
                    field.SetValue(data, someList);
                }
                else if (field.FieldType == typeof(PointF))
                {
                    var value = new PointF();
                    value.X = buffer.ReadFloat();
                    value.Y = buffer.ReadFloat();
                    DebugLog.LogPackets($"Decrypted PointF: {value.ToString()}");
                    field.SetValue(data, value);
                }
                else if (field.FieldType == typeof(Vec2))
                {
                    var value = new Vec2();
                    value.X = buffer.ReadFloat();
                    value.Y = buffer.ReadFloat();
                    DebugLog.LogPackets($"Decrypted Vec2: X: {value.X}, Y: {value.Y}");
                    field.SetValue(data, value);
                }
                else
                    DebugLog.Severe($"[NetworkMagic] You are trying to decrypt an unknown data type for {typeof(T).FullName}!");
            }
            DebugLog.LogPackets("Reading---");
            data.ConnId = connectionId;
            HandleData(data);
        }

        public void HandleData(NetworkData data)
        {
            HandleData((T)data);
        }
    }
}