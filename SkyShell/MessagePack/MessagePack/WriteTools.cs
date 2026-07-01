using System;
using System.IO;

namespace MessagePackLib.MessagePack
{
    class WriteTools
    {
        public static void WriteNull(Stream ms)
        {
            ms.WriteByte(0xC0);
        }

        public static void WriteString(Stream ms, String strVal)
        {

            byte[] rawBytes = BytesTools.GetUtf8Bytes(strVal);
            byte[] lenBytes = null;
            int len = rawBytes.Length;
            byte b = 0;
            if (len <= 31)
            {
                b = (byte)(0xA0 + (byte)len);
                ms.WriteByte(b);
            }
            else if (len <= 255)
            {
                b = 0xD9;
                ms.WriteByte(b);
                b = (byte)len;
                ms.WriteByte(b);
            }
            else if (len <= 65535)
            {
                b = 0xDA;
                ms.WriteByte(b);

                lenBytes = BytesTools.SwapBytes(BitConverter.GetBytes((Int16)len));
                ms.Write(lenBytes, 0, lenBytes.Length);
            }
            else
            {
                b = 0xDB;
                ms.WriteByte(b);

                lenBytes = BytesTools.SwapBytes(BitConverter.GetBytes((Int32)len));
                ms.Write(lenBytes, 0, lenBytes.Length);
            }
            ms.Write(rawBytes, 0, rawBytes.Length);
        }
        public static void WriteBinary(Stream ms, byte[] rawBytes)
        {

            byte[] lenBytes = null;
            int len = rawBytes.Length;
            byte b = 0;
            if (len <= 255)
            {
                b = 0xC4;
                ms.WriteByte(b);
                b = (byte)len;
                ms.WriteByte(b);
            }
            else if (len <= 65535)
            {
                b = 0xC5;
                ms.WriteByte(b);

                lenBytes = BytesTools.SwapBytes(BitConverter.GetBytes((Int16)len));
                ms.Write(lenBytes, 0, lenBytes.Length);
            }
            else
            {
                b = 0xC6;
                ms.WriteByte(b);

                lenBytes = BytesTools.SwapBytes(BitConverter.GetBytes((Int32)len));
                ms.Write(lenBytes, 0, lenBytes.Length);
            }
            ms.Write(rawBytes, 0, rawBytes.Length);
        }

        public static void WriteFloat(Stream ms, Double fVal)
        {
            ms.WriteByte(0xCB);
            ms.Write(BytesTools.SwapDouble(fVal), 0, 8);
        }

        public static void WriteSingle(Stream ms, Single fVal)
        {
            ms.WriteByte(0xCA);
            ms.Write(BytesTools.SwapBytes(BitConverter.GetBytes(fVal)), 0, 4);
        }

        public static void WriteBoolean(Stream ms, Boolean bVal)
        {
            if (bVal)
            {
                ms.WriteByte(0xC3);
            }
            else
            {
                ms.WriteByte(0xC2);
            }
        }

        public static void WriteUInt64(Stream ms, UInt64 iVal)
        {
            ms.WriteByte(0xCF);
            byte[] dataBytes = BitConverter.GetBytes(iVal);
            ms.Write(BytesTools.SwapBytes(dataBytes), 0, 8);
        }

        public static void WriteInteger(Stream ms, Int64 iVal)
        {
            if (iVal >= 0)
            {   
                if (iVal <= 127)
                {
                    ms.WriteByte((byte)iVal);
                }
                else if (iVal <= 255)
                {  
                    ms.WriteByte(0xCC);
                    ms.WriteByte((byte)iVal);
                }
                else if (iVal <= (UInt32)0xFFFF)
                {  
                    ms.WriteByte(0xCD);
                    ms.Write(BytesTools.SwapInt16((Int16)iVal), 0, 2);
                }
                else if (iVal <= (UInt32)0xFFFFFFFF)
                {  
                    ms.WriteByte(0xCE);
                    ms.Write(BytesTools.SwapInt32((Int32)iVal), 0, 4);
                }
                else
                {  
                    ms.WriteByte(0xD3);
                    ms.Write(BytesTools.SwapInt64(iVal), 0, 8);
                }
            }
            else
            {  
                if (iVal <= Int32.MinValue)  
                {
                    ms.WriteByte(0xD3);
                    ms.Write(BytesTools.SwapInt64(iVal), 0, 8);
                }
                else if (iVal <= Int16.MinValue)   
                {
                    ms.WriteByte(0xD2);
                    ms.Write(BytesTools.SwapInt32((Int32)iVal), 0, 4);
                }
                else if (iVal <= -128)   
                {
                    ms.WriteByte(0xD1);
                    ms.Write(BytesTools.SwapInt16((Int16)iVal), 0, 2);
                }
                else if (iVal <= -32)
                {
                    ms.WriteByte(0xD0);
                    ms.WriteByte((byte)iVal);
                }
                else
                {
                    ms.WriteByte((byte)iVal);
                }
            }  
        }
    }
}