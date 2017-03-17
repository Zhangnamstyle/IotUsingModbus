﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Collections;

namespace IoTModbus
{
    
    class ComHandler 
    {        
        // ------------------------------------------------------------------------
        // Private declarations
        private ModbusTCP modbusTCP;
        private Report report;

        // ------------------------------------------------------------------------
        // Supported Modbus Exeption Codes
        private const byte exIllegalFunction = 1;
        private const byte exIllegalDataAddress = 2;
        private const byte exIllegalDataValue = 3;
        private const byte exSlaveDeviceFailure = 4;
        private const byte exAcknowledge = 5;
        private const byte exSlaveDeviceBusy = 6;
        private const byte exMemoryParityError = 8;
        private const byte exGatewayPathUnavailable = 10;
        private const byte exGatewayTargetNoResponse = 11;
        private const byte exTimeout = 12;

        // ------------------------------------------------------------------------
        /// <summary>Exception data event. This event is called when the data is incorrect</summary>
        public delegate void ExceptionData(ushort id, byte unit, byte function, string exMessage);
        /// <summary>Exception data event. This event is called when the data is incorrect</summary>
        public event ExceptionData OnException;
        /// <summary>Response data event. This event is called when new data arrives</summary>
        public delegate void ResponseData(ushort id, byte unit, byte function, byte[] data);
        /// <summary>Response data event. This event is called when new data arrives</summary>
        public event ResponseData OnResponseData;



        ushort cnt = 1;
        // ------------------------------------------------------------------------
        /// <summary>Returns the exeption type </summary>
        /// <param name="FuncEx">Modbus Exeption Code.</param>
        private string exMessage(byte funcEx)
        {
            string ex;
            switch (funcEx)
            {
                case exIllegalFunction:
                    ex = "Illegal Function";
                    break;
                case exIllegalDataAddress:
                    ex = "Illegal Data Address";
                    break;
                case exIllegalDataValue:
                    ex = "Illegal Data Value";
                    break;
                case exSlaveDeviceFailure:
                    ex = "Slave Devie Failure";
                    break;
                case exAcknowledge:
                    ex = "Acknowledge";
                    break;
                case exSlaveDeviceBusy:
                    ex = "Slave Device Busy";
                    break;
                case exMemoryParityError:
                    ex = "Memory Parity Error";
                    break;
                case exGatewayPathUnavailable:
                    ex = "Gateway Path Unavailable";
                    break;
                case exGatewayTargetNoResponse:
                    ex = "Gateway Target Device Failed to Respond";
                    break;
                default:
                    ex = "Unkown Exeption Code";
                    break;
            }

            return ex;
        }


        // ------------------------------------------------------------------------
        /// <summary>Constructor for Report class</summary>
        public ComHandler()
        {
            report = new Report();
            
        }

        // ------------------------------------------------------------------------
        /// <summary>Connects to the Modbus slave</summary>
        public void connect(string ip,int port)
        {

            modbusTCP = new ModbusTCP("192.168.1.101", 502, report);
            modbusTCP.OnResponseDataTCP += new ModbusTCP.ResponseDataTCP(ModbusTCP_OnResponseData);
            modbusTCP.OnExceptionTCP += new ModbusTCP.ExceptionDataTCP(ModbusTCP_OnException); 
        }

        private void ModbusTCP_OnException(ushort id, byte unit, byte function, byte exception)
        {
            string exM = exMessage(exception);
            disconnect();
            if (OnException != null) OnException(id, unit, function, exM);
        }

        private void ModbusTCP_OnResponseData(ushort id, byte unit, byte function, byte[] data)
        {
            if (OnResponseData != null) OnResponseData(id, unit, function, data);
        }

        // ------------------------------------------------------------------------
        /// <summary>Disconnect from the Modbus slave</summary>
        public void disconnect()
        {
            if(modbusTCP != null)
            {
                modbusTCP.disconnect();
                modbusTCP = null;
            }
        }

        // ------------------------------------------------------------------------
        /// <summary>Sends a write message to IoT Device </summary>
        /// <param name="funcNr">Modbus Function Code.</param>
        /// <param name="tId">Transaction Id. This needs to be unique</param>
        /// <param name="unit"></param>
        /// <param name="startAddress"></param>
        /// <param name="numBits"></param>
        /// <param name="values"></param>
        public void send(byte funcNr, ushort tId, byte unit, ushort startAddress, ushort numBits, byte[] values)
        {
            modbusTCP.sendTCP(funcNr, tId, unit, startAddress, numBits, values);
        }

        // ------------------------------------------------------------------------
        /// <summary>Sends a read message to IoT Device </summary>
        /// <param name="funcNr">Modbus Function Code.</param>
        /// <param name="tId">Transaction Id. This needs to be unique</param>
        /// <param name="unit"></param>
        /// <param name="startAddress"></param>
        /// <param name="numBits"></param>
        /// <param name="values"></param>
        public void send(byte funcNr, ushort tId, byte unit, ushort startAddress, ushort numInputs)
        {
            modbusTCP.sendTCP(funcNr, tId, unit, startAddress, numInputs);
        }

        public void reportSlaveID(byte tId,byte unit)
        {
            modbusTCP.reportSlaveID(tId, unit);
        }


    }
}
