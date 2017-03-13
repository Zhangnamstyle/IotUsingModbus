﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IoTModbus
{
    /// <summary>
    /// Static Class for creating and decode Modbus Protocol Data Unit(PDU)
    /// </summary>
    static class ModbusPDU
    {
        // ------------------------------------------------------------------------
        // Supported Modbus Function Codes
        private const byte fctReadCoil = 1;
        private const byte fctReadDiscreteInputs = 2;
        private const byte fctReadHoldingRegister = 3;
        private const byte fctReadInputRegister = 4;

        private const byte fctWriteSingleCoil = 5;
        private const byte fctWriteSingleRegister = 6;
        private const byte fctWriteMultipleCoils = 15;
        private const byte fctWriteMultipleRegister = 16;

        private const byte fctDiagnostics = 8;
        private const byte fctReportSlaveId = 11;

        // ------------------------------------------------------------------------
        // Supported Modbus Exeption Codes



        // ------------------------------------------------------------------------
        /// <summary>Returns a PDU for Writing to registers</summary>
        /// <param name="funcNr">Modbus Function Code.</param>
        /// <param name="startAddress">Address from where the data write begins.</param>
        /// <param name="numBytes">Specifys number of bytes.</param>
        /// <param name="numData">Specifys number of Data.</param>
        /// <param name="values">Contains the bit information in byte format.</param>
        public static byte[] CreatePDU(byte funcNr,ushort startAddress,ushort numBytes,ushort numData,byte[] values)
        {

            byte[] pdu = new byte[numBytes + 4];
            pdu[0] = funcNr;
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startAddress));
            pdu[1] = _adr[0];
            pdu[2] = _adr[1];
            if(funcNr >= fctWriteMultipleCoils) //Larger or equal to function number for multiple coils
            {
                byte[] _cnt = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)numData));
                pdu[3] = _cnt[0];			// Number of bytes
                pdu[4] = _cnt[1];			// Number of bytes
                pdu[5] = (byte)(numBytes - 2);
                Array.Copy(values, 0, pdu, 6, numBytes);
            }
            else
            {
                pdu[3] = values[0];
                pdu[4] = values[1];
            }
            return pdu;
        }

        // ------------------------------------------------------------------------
        /// <summary>Returns a PDU for Reading from registers </summary>
        /// <param name="funcNr">Modbus Function Code.</param>
        /// <param name="startAddress">Address from where the data read begins.</param>
        /// <param name="length">Number of data to read</param>
        public static byte[] CreatePDU(byte funcNr, ushort startAddress,ushort length)
        {

            byte[] pdu = new byte[5];
            pdu[0] = funcNr;
            byte[] _adr = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)startAddress));
            pdu[1] = _adr[0];
            pdu[2] = _adr[1];
            byte[] _length = BitConverter.GetBytes((short)IPAddress.HostToNetworkOrder((short)length));
            pdu[3] = _length[0];			// Number of data to read
            pdu[4] = _length[1];			// Number of data to read
            return pdu;
        }

        public static byte[] ReadPDU(byte[] pdu,out byte _funcNr) //TODO: Add test for exeption code
        {
            byte[] data = new byte[pdu.Length - 1];
            byte funcNr = pdu[0];
            _funcNr = funcNr;
            if(funcNr > fctWriteMultipleRegister)
            {

            }
            Buffer.BlockCopy(pdu, 1, data, 0, data.Length);
            return data;
        }


    }
}
