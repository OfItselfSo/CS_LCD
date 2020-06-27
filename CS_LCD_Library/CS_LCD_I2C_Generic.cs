using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// +------------------------------------------------------------------------------------------------------------------------------+
/// ¦                                                   TERMS OF USE: MIT License                                                  ¦
/// +------------------------------------------------------------------------------------------------------------------------------¦
/// ¦Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation    ¦
/// ¦files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,    ¦
/// ¦modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software¦
/// ¦is furnished to do so, subject to the following conditions:                                                                   ¦
/// ¦                                                                                                                              ¦
/// ¦The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.¦
/// ¦                                                                                                                              ¦
/// ¦THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE          ¦
/// ¦WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR         ¦
/// ¦COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,   ¦
/// ¦ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                         ¦
/// +------------------------------------------------------------------------------------------------------------------------------+

namespace CS_LCD
{
    /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
    /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
    /// <summary>
    /// A class which handles the building of the byte sequences to control
    /// a generic HD44780 LCD display via I2C
    /// 
    /// This class inherits the basic functionality from CS_LCDBase and only 
    /// adds the necessary functionality to compose byte sequences suitable
    /// for a generic LCD display over I2C. In other words CS_LCDBase provides
    /// the basic bytes to control the display and this class reworks them
    /// to provide a byte stream suitable from interacting with a HD44780
    /// display via I2C
    /// 
    /// It is expected that a subsequent class designed for the specific 
    /// hardware platform will inherit this class and handle the actual send 
    /// of the data.
    /// 
    /// Acknowledgements: 
    /// 
    /// Based on, and largely ported from, the excellent python_lcd code of Dave Hylands
    /// https://github.com/dhylands/python_lcd
    /// </summary>
    /// 
    /// <history>
    ///    13 Jun 20  Cynic - Started
    /// </history>    
    public abstract class CS_LCD_I2C_Generic : CS_LCDBase
    {

        // some constants 
        private const byte MASK_RS = 0x01;
        private const byte MASK_RW = 0x02;
        private const byte MASK_E = 0x04;
        private const byte SHIFT_BACKLIGHT = 3;
        private const byte SHIFT_DATA = 4;
        
        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numRowsIn">the number of rows on the display</param>
        /// <param name="numColsIn">the number of cols on the display</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public CS_LCD_I2C_Generic(int numRowsIn, int numColsIn) : base(numRowsIn, numColsIn)
        {

        }

        // # ##################################################################
        // # ##### Abstract functions - we implement to support the parent class
        // # ##################################################################

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Allows the hal layer to turn the backlight on
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        protected override void HALBacklightOn()
        {
            SendByte((byte)(1 << SHIFT_BACKLIGHT));
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Allows the hal layer to turn the backlight off
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        protected override void HALBacklightOff()
        {
            SendByte((byte)(0));
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Write a command to the LCD
        /// </summary>
        /// <param name="cmdByte">the byte to write</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        protected override void HALWriteCommand(byte cmdByte)
        {
            // Data is latched on the falling edge of E. So each
            // outgoing nibble is sent twice
            byte byte2send = (byte)((BacklightState << SHIFT_BACKLIGHT) | (((cmdByte >> (byte)4) & (byte)0x0f) << SHIFT_DATA));
            SendByte((byte)(byte2send | MASK_E));
            SendByte((byte)byte2send);
            byte2send = (byte)((BacklightState << SHIFT_BACKLIGHT) | ((cmdByte & 0x0f) << SHIFT_DATA));
            SendByte((byte)(byte2send | MASK_E));
            SendByte(byte2send);
            if (cmdByte <= 3)
            {
                // The home and clear commands require a worst
                // case delay of 4.1 msec               
                Thread.Sleep(5);
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Write binary data to the LCD.
        /// </summary>
        /// <param name="dataByte">the byte to write</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        protected override void HALWriteData(byte dataByte)
        {
            byte byte2send = ((byte)(MASK_RS | (BacklightState << SHIFT_BACKLIGHT) | (((dataByte >> 4) & 0x0f) << SHIFT_DATA)));
            SendByte((byte)(byte2send | MASK_E));
            SendByte((byte)(byte2send));
            byte2send = ((byte)(MASK_RS | (BacklightState << SHIFT_BACKLIGHT) | ((dataByte & 0x0f) << SHIFT_DATA)));
            SendByte((byte)(byte2send | MASK_E));
            SendByte((byte)(byte2send));
        }


        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Send a byte with no interpretation
        /// </summary>
        /// <param name="dataByte">the byte to write</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        protected override void HALSendByte(byte byte2Send)
        {
            SendByte(byte2Send);
        }

        // # ##################################################################
        // # ##### Abstract functions - to be implemented by the child class
        // # ##################################################################

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Send an array of binary data
        /// </summary>
        /// <param name="byteArray">an array of bytes</param>
        public abstract void SendByteArray(byte[] byteArray);

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Send a single byte
        /// </summary>
        /// <param name="byteVal">a single byte of data</param>
        public abstract void SendByte(byte byteVal);

    }
}
