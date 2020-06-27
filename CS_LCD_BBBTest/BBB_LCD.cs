using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BBBCSIO;
using CS_LCD;

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

namespace CS_LCD_BBBTest
{

    /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
    /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
    /// <summary>
    /// A class to specifically handle the transmission of data via I2C on a
    /// BBB F7 board. Because we inherit from CS_I2C_LCD_Generic we will receive
    /// data in byte arrays suitable for a generic HD44780 LCD over I2C all we
    /// really have to do here is send it via the built-in I2C driver.
    /// </summary>
    /// <history>
    ///    13 Jun 20  Cynic - Started
    /// </history>
    public class BBB_LCD : CS_LCD_I2C_Generic
    {

        // this is passed in during construction. It is expected that the 
        // caller will close and dispose of it appropriately
        I2CPortFS i2cPeripheral = null;
        int i2cAddr = 0x27;

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="i2cPeripheralIn">the i2c object we use</param>
        /// <param name="numRowsIn">the number of rows on the display</param>
        /// <param name="numColsIn">the number of cols on the display</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public BBB_LCD(I2CPortFS i2cPeripheralIn, int i2cAddr, int numRowsIn, int numColsIn) : base(numRowsIn, numColsIn)
        {
            I2cPeripheral = i2cPeripheralIn;
            
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the I2cPeripheral. Never permits a null value to be returned
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public I2CPortFS I2cPeripheral 
        {
            get
            {
                if (i2cPeripheral == null) throw new Exception("The I2cPeripheral object is not set");
                return i2cPeripheral;
            }

            set
            {
                i2cPeripheral = value;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the i2cAddr
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public int I2CAddr
        {
            get
            {
                return i2cAddr;
            }
            set
            {
                i2cAddr = value;
            }
        }

        // # ##################################################################
        // # ##### Abstract functions - implemented to support the parent classes
        // # ##################################################################

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Send an array of binary data
        /// </summary>
        /// <param name="byteArray">an array of bytes</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public override void SendByteArray(byte[] byteArray)
        {
            if (byteArray == null) return;
            I2cPeripheral.Write(I2CAddr, byteArray, byteArray.Length);
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Send a single byte
        /// </summary>
        /// <param name="byteVal">a single byte of data</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public override void SendByte(byte byteVal)
        {
            I2cPeripheral.Write(I2CAddr, new byte[] { byteVal }, 1);
        }

    }

}
