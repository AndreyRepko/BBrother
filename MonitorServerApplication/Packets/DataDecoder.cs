using System;
using MonitorServerApplication.PacketsDefinition;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;

namespace MonitorServerApplication.Packets
{
    static class DataDecoder
    {
        private static readonly KeyParameter Key;

        static DataDecoder()
        {
            byte[] bKey = System.Text.Encoding.ASCII.GetBytes("Implict error #56");
            var tiger = new TigerDigest();
            tiger.BlockUpdate(bKey, 0, bKey.Length);

            var bb = new byte[24];
            tiger.DoFinal(bb, 0);

            Key = new KeyParameter(bb);
        }

        public static void DoTestChiper()
        {
            var chiper = new CfbDCPCipher(new BlowfishEngine(), 8);
            chiper.Init(false, Key);
            //"RA3545RQa5d+xGPLAw==0"
            var b = System.Text.Encoding.ASCII.GetBytes("RA3545RQa5d+xGPLAw==");
            byte[] bDecoded = Base64.Decode(b);
            var decodedString2 = new byte[13];
            if (bDecoded != null)
                if (chiper.ProcessBlock(bDecoded, 0, decodedString2, 0) != 13)
                    throw new SystemException("Topor vsplil!");
                else
                    throw new SystemException("Decoded string is null!");


            var str2 = System.Text.Encoding.ASCII.GetString(decodedString2);
            if (str2 != "192.168.1.123")
                throw new SystemException("failed");

        }

        private static byte[] ReadBytes(ref int bt, Byte[] mem)
        {
            if (bt + sizeof(int) > mem.Length)
                throw new SystemException("Size of string is greater that expected");

            var chiper = new CfbDCPCipher(new BlowfishEngine(), 8);
            chiper.Init(false, Key);

            int stringSize = BitConverter.ToInt32(mem, bt);

            if (bt + sizeof(int) + stringSize > mem.Length)
                throw new SystemException("String is greater that expected");

            var b = new byte[stringSize - 1];
            Array.Copy(mem, bt + 4, b, 0, stringSize - 1);
            byte[] bDecoded = Base64.Decode(b);
            var decodedBytes = new byte[bDecoded.Length];
            chiper.ProcessBlock(bDecoded, 0, decodedBytes, 0);
            bt += stringSize + 4;
            return decodedBytes;
        }

        private static String ReadString(ref int bt, Byte[] mem)
        {
           return System.Text.Encoding.GetEncoding(1251).GetString(ReadBytes(ref bt, mem));
        }

        private static int ReadInt(ref int bt, Byte[] mem)
        {
            if (bt + sizeof(int) > mem.Length)
                throw new SystemException("Size of integer is greater that expected");

            int result = BitConverter.ToInt32(mem, bt);
            bt += sizeof(int);
            return result;
        }

        private static DateTime ReadDateTime(ref int bt, Byte[] mem)
        {
            const int sizeOfDateTime = 8; //sizeof (DateTime);
            if (bt + sizeOfDateTime > mem.Length)
                throw new SystemException("Size of integer is greater that expected");

            DateTime result = DateTime.FromOADate(BitConverter.ToDouble(mem, bt));
            bt += sizeOfDateTime;
            return result;
        }

        public static InfoMessage InfoMessageDecoder(byte[] data)
        {
            var msg = new InfoMessage();
            int bt = 0;

            msg.MessageType = ReadInt(ref bt, data);
            msg.Time = ReadDateTime(ref bt, data);
            msg.IP = ReadString(ref bt, data);
            msg.UserName = ReadString(ref bt, data);
            msg.Info = ReadString(ref bt, data);
            return msg;
        }

        public static InfoMessage PacketMessageDecoder(byte[] data)
        {
            var msg = new InfoMessage();
            int bt = 0;

            msg.MessageType = ReadInt(ref bt, data);
            msg.Time = ReadDateTime(ref bt, data);
            msg.IP = ReadString(ref bt, data);
            msg.UserName = ReadString(ref bt, data);
            msg.Info = ReadString(ref bt, data);
            return msg;
        }

        public static PacketData PacketDecoder(byte[] packetData)
        {
            var packet = new PacketData();
            int bt = 0;
            packet.TaskId = ReadInt(ref bt, packetData);
            packet.NameExe = ReadString(ref bt, packetData);
            packet.NewExe = ReadString(ref bt, packetData);
            packet.CurUser = ReadString(ref bt, packetData);
            packet.CompName = ReadString(ref bt, packetData);
            packet.CompAdr = ReadString(ref bt, packetData);
            packet.StartPeriod = ReadDateTime(ref bt, packetData);
            packet.EndPeriod = ReadDateTime(ref bt, packetData);
            packet.Keybs = ReadInt(ref bt, packetData);
            packet.Mouse = ReadInt(ref bt, packetData);
            packet.Caption = ReadString(ref bt, packetData);
            packet.Screenshot = ReadBytes(ref bt, packetData);
            packet.ExePath = ReadString(ref bt, packetData);
            packet.ActiveTime = ReadDateTime(ref bt, packetData);

            return packet;
        }
    }
}
