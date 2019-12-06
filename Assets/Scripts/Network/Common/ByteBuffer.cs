using System;
using System.Text;
using System.Collections.Generic;

namespace JustPlanes.Network
{
    public class ByteBuffer : IDisposable
    {
        private List<byte> Buff;
        private byte[] readBuff;
        private int readPos;
        private bool buffUpdated = false;

        public ByteBuffer()
        {
            Buff = new List<byte>();
            readPos = 0;
        }

        public int GetReadPos()
        {
            return readPos;
        }

        public byte[] ToArray()
        {
            return Buff.ToArray();
        }

        public int Count()
        {
            return Buff.Count;
        }

        public int Length()
        {
            return Count() - readPos;
        }

        public void Clear()
        {
            Buff.Clear();
            readPos = 0;
        }

        #region Write operations

        public void WriteByte(byte input)
        {
            Buff.Add(input);
            buffUpdated = true;
        }

        public void WriteBytes(byte[] input)
        {
            Buff.AddRange(input);
            buffUpdated = true;
        }

        public void WriteShort(short input)
        {
            WriteBytes(BitConverter.GetBytes(input));
        }

        public void WriteInteger(int input)
        {
            WriteBytes(BitConverter.GetBytes(input));
        }

        public void WriteLong(long input)
        {
            WriteBytes(BitConverter.GetBytes(input));
        }

        public void WriteUnit(Unit unit)
        {
            WriteString(unit.ID);
            WriteInteger(unit.X);
            WriteInteger(unit.Y);
        }

        public void WritePlayer(Player player)
        {
            WriteString(player.Name);
            WriteInteger(player.X);
            WriteInteger(player.Y);
        }

        public void WriteFloat(float input)
        {
            WriteBytes(BitConverter.GetBytes(input));
        }

        public void WriteBool(bool input)
        {
            WriteBytes(BitConverter.GetBytes(input));
        }

        public void WriteString(string input)
        {
            WriteBytes(BitConverter.GetBytes(input.Length));
            WriteBytes(Encoding.ASCII.GetBytes(input));
        }

        #endregion

        #region Read operations

        public byte ReadByte(bool Peek = true)
        {
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                byte value = readBuff[readPos];
                if (Peek & Buff.Count > readPos)
                {
                    readPos += 1;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'BYTE'");
            }
        }

        public byte[] ReadBytes(int Length, bool Peek = true)
        {
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                byte[] value = Buff.GetRange(readPos, Length).ToArray();
                if (Peek)
                {
                    readPos += Length;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'BYTE[]'");
            }
        }

        public short ReadShort(bool Peek = true)
        {
            // TODO: readPos + 2? shouldn't it be that? same with the rest
            // also I don't like the duplicate code...
            // I don't like the control flow of ifs either...
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                short value = BitConverter.ToInt16(readBuff, readPos);
                if (Peek & Buff.Count > readPos)
                {
                    readPos += 2;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'SHORT'");
            }
        }

        public int ReadInteger(bool Peek = true)
        {
            if (Buff.Count >= readPos + 4)  // TODO: adjust other types to bigger than too
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

               // TODO: error here, test with wireshark?
                int value = BitConverter.ToInt32(readBuff, readPos);
                if (Peek & Buff.Count > readPos)
                {
                    readPos += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'INTEGER'");
            }
        }

        public long ReadLong(bool Peek = true)
        {
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                long value = BitConverter.ToInt64(readBuff, readPos);
                if (Peek & Buff.Count > readPos)
                {
                    readPos += 8;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'LONG'");
            }
        }

        public float ReadFloat(bool Peek = true)
        {
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                float value = BitConverter.ToSingle(readBuff, readPos);
                if (Peek & Buff.Count > readPos)
                {
                    readPos += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'FLOAT'");
            }
        }

        public bool ReadBool(bool Peek = true)
        {
            if (Buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }

                bool value = BitConverter.ToBoolean(readBuff, readPos);
                if (Peek & Buff.Count > readPos)
                {
                    readPos += 1;
                }

                return value;
            }
            else
            {
                throw new Exception("You are not trying to read out a 'BOOL'");
            }
        }

        public string ReadString(bool Peek = true)
        {
            try
            {
                int length = ReadInteger(true);
                if (buffUpdated)
                {
                    readBuff = Buff.ToArray();
                    buffUpdated = false;
                }
                string value = Encoding.ASCII.GetString(readBuff, readPos, length);

                if (Peek & Buff.Count > readPos)
                {
                    if (value.Length > 0)
                        readPos += length;
                }

                return value;
            }
            catch (Exception)
            {
                throw new Exception("You are not trying  to read out a 'STRING'");
            }
        }
        

        public Player ReadPlayer()
        {
            string name = ReadString();
            int x = ReadInteger();
            int y = ReadInteger();
            return new Player(name, x, y);
        }

        public Unit ReadUnit()
        {
            string id = ReadString();
            int x = ReadInteger();
            int y = ReadInteger();
            return new Unit(id, x, y);
        }

        #endregion

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Buff.Clear();
                    readPos = 0;
                }
                disposedValue = true;
            }

        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
