using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using MonitorServerApplication.DB;
using MonitorServerApplication.PacketsDefinition;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;

namespace MonitorServerApplication.Packets
{
    static class DataEncoder
    {
        private static readonly KeyParameter Key;

        static DataEncoder()
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

        private static byte[] EncodeBytes(Byte[] value, KeyParameter key)
        {
            var chiper = new CfbDCPCipher(new BlowfishEngine(), 8);
            if (key != null)
                chiper.Init(true, key);
            else
                chiper.Init(true, Key);

            var encodedBytes = new byte[value.Length];
            var totalBytes = chiper.ProcessBlock(value, 0, encodedBytes, 0);

            var b = new byte[totalBytes];
            Array.Copy(encodedBytes, 0, b, 0, totalBytes);
            var encoded = Base64.Encode(b);
            return encoded;
        }

        private static Byte[] EncodeString(string EncodeString, KeyParameter key)
        {
            var bytesToEncode = System.Text.Encoding.ASCII.GetBytes(EncodeString);
            return EncodeBytes(bytesToEncode, key);
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
            /*int bt = 0;

            msg.kod = ReadInt(ref bt, data);
            msg.time = ReadDateTime(ref bt, data);
            msg.IP = EncodeString(ref bt, data);
            msg.UserName = EncodeString(ref bt, data);
            msg.Info = EncodeString(ref bt, data);*/
            return msg;
        }

        public static InfoMessage PacketMessageDecoder(byte[] data)
        {
            var msg = new InfoMessage();
            /*int bt = 0;

            msg.kod = ReadInt(ref bt, data);
            msg.time = ReadDateTime(ref bt, data);
            msg.IP = EncodeString(ref bt, data);
            msg.UserName = EncodeString(ref bt, data);
            msg.Info = EncodeString(ref bt, data);*/
            return msg;
        }

        public static PacketData PacketDecoder(byte[] packetData)
        {
            var packet = new PacketData();
            /*int bt = 0;
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
            packet.ActiveTime = ReadDateTime(ref bt, packetData);*/

            return packet;
        }

        public static IEnumerable<byte[]> EncodeSettings(ClientSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            var ks = "ReadNewSettings2Connection_Request";

            byte[] bKey = System.Text.Encoding.ASCII.GetBytes(ks);
            var tiger = new TigerDigest();
            tiger.BlockUpdate(bKey, 0, bKey.Length);

            var bb = new byte[24];
            tiger.DoFinal(bb, 0);

            var key = new KeyParameter(bb);

            foreach (var sett in settings.Settings)
            {
                yield return DataEncoder.EncodeString(sett.FieldName, key);
                yield return DataEncoder.EncodeString(sett.GetStringValue(), key);
            }
        }
    }
}
