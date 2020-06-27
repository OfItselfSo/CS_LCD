using System;
using System.CodeDom;
using System.Threading;
using BBBCSIO;

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
    /// A class to test the operation of an I2C LCD (HD44780 compatible) display
    /// via the Beaglebone Black board.
    /// </summary>
    /// <history>
    ///    13 Jun 20  Cynic - Started
    /// </history>
    public class BBBLCDTest 
    {

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Constructor
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public BBBLCDTest()
        {
            // call this on startup, uncomment for the type of display
            //LCD_Test_2x16();
            LCD_Test_4x20();
        }


        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// A function to provide the calls to test the LCD. 2 rows by 16 col version
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void LCD_Test_2x16()
        {

            I2CPortFS i2cPeripheral = new I2CPortFS(I2CPortEnum.I2CPORT_2);

            try
            {
                BBB_LCD lcdAPI = new BBB_LCD(i2cPeripheral, 0x27, 2, 16);


                // initialize now
                Console.WriteLine("Begin LCD_Test");
                lcdAPI.InitLCD();

                // title bit
                lcdAPI.MoveTo(0, 1);
                lcdAPI.WriteString("CS_LCD Library");
                lcdAPI.MoveTo(1, 2);
                lcdAPI.WriteString("Tests Begin");

                Thread.Sleep(3000);
                lcdAPI.Clear();

                // set values in all four corners
                lcdAPI.MoveTo(1, 2);
                lcdAPI.WriteString("TestCorners");

                lcdAPI.MoveTo(0, 0);
                lcdAPI.WriteChar('A');
                lcdAPI.MoveTo(1, 0);
                lcdAPI.WriteChar('B');

                lcdAPI.MoveTo(0, 15);
                lcdAPI.WriteChar('1');
                lcdAPI.MoveTo(1, 15);
                lcdAPI.WriteChar('2');

                Thread.Sleep(3000);
                lcdAPI.Clear();

                lcdAPI.MoveTo(2, 2);
                lcdAPI.WriteString("AutoWrapTest");
                Thread.Sleep(2000);

                lcdAPI.MoveTo(0, 14);
                lcdAPI.WriteString("ABC");
                Thread.Sleep(1000);
                lcdAPI.MoveTo(1, 14);
                lcdAPI.WriteString("DEF");
                Thread.Sleep(1000);

                Thread.Sleep(3000);
                lcdAPI.Clear();

                // now test the backlight
                lcdAPI.MoveTo(1, 1);
                lcdAPI.WriteString("BacklightOff+On");
                Thread.Sleep(2000);
                lcdAPI.BacklightOff();
                Thread.Sleep(2000);
                lcdAPI.BacklightOn();

                Thread.Sleep(3000);
                lcdAPI.Clear();

                // now test the display
                lcdAPI.MoveTo(1, 1);
                lcdAPI.WriteString("DisplayOff+On");
                Thread.Sleep(2000);
                lcdAPI.DisplayOff();
                Thread.Sleep(2000);
                lcdAPI.DisplayOn();

                Thread.Sleep(3000);
                lcdAPI.Clear();

                // now test the display
                lcdAPI.MoveTo(1, 1);
                lcdAPI.WriteString("Show Cursor");
                Thread.Sleep(2000);
                lcdAPI.ShowCursor();
                Thread.Sleep(2000);
                lcdAPI.MoveTo(1, 1);
                lcdAPI.WriteString("Blink Cursor");
                lcdAPI.BlinkCursorOn();
                Thread.Sleep(2000);
                lcdAPI.MoveTo(1, 1);
                lcdAPI.WriteString("Cursor Off  ");
                lcdAPI.HideCursor();

                Thread.Sleep(3000);
                lcdAPI.Clear();

                lcdAPI.MoveTo(1, 0);
                lcdAPI.WriteString("Custom Char Test");
                lcdAPI.HideCursor();

                Thread.Sleep(2000);

                // now custom char test
                byte[] happyFace = new byte[] { 0x00, 0x0A, 0x00, 0x04, 0x00, 0x11, 0x0E, 0x00 };
                byte[] sadFace = new byte[] { 0x00, 0x0A, 0x00, 0x04, 0x00, 0x0E, 0x11, 0x00 };
                byte[] grinFace = new byte[] { 0x00, 0x00, 0x0A, 0x00, 0x1F, 0x11, 0x0E, 0x00 };
                byte[] shockFace = new byte[] { 0x0A, 0x00, 0x04, 0x00, 0x0E, 0x11, 0x11, 0x0E };
                byte[] mehFace = new byte[] { 0x00, 0x0A, 0x00, 0x04, 0x00, 0x1F, 0x00, 0x00 };
                byte[] angryFace = new byte[] { 0x11, 0x0A, 0x11, 0x04, 0x00, 0x0E, 0x11, 0x00 };
                byte[] tongueFace = new byte[] { 0x00, 0x0A, 0x00, 0x04, 0x00, 0x1F, 0x05, 0x02 };

                lcdAPI.CustomChar(0, happyFace);
                lcdAPI.CustomChar(1, sadFace);
                lcdAPI.CustomChar(2, grinFace);
                lcdAPI.CustomChar(3, shockFace);
                lcdAPI.CustomChar(4, mehFace);
                lcdAPI.CustomChar(5, angryFace);
                lcdAPI.CustomChar(6, tongueFace);

                lcdAPI.MoveTo(0, 2);
                lcdAPI.WriteChar((char)0x00);
                lcdAPI.WriteChar((char)0x01);
                lcdAPI.WriteChar((char)0x02);
                lcdAPI.WriteChar((char)0x03);
                lcdAPI.WriteChar((char)0x04);
                lcdAPI.WriteChar((char)0x05);
                lcdAPI.WriteChar((char)0x06);

                Thread.Sleep(3000);
                lcdAPI.Clear();

                lcdAPI.MoveTo(0, 1);
                lcdAPI.WriteString("CS_LCD Library");
                lcdAPI.MoveTo(1, 1);
                lcdAPI.WriteString("Test Complete");

                Console.WriteLine("End LCD_Test");
            }
            finally
            {
                // close the port
                i2cPeripheral.ClosePort();
                i2cPeripheral.Dispose();
            }

        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// A function to provide the calls to test the LCD
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void LCD_Test_4x20()
        {
            I2CPortFS i2cPeripheral = new I2CPortFS(I2CPortEnum.I2CPORT_2);

            try
            {
                BBB_LCD lcdAPI = new BBB_LCD(i2cPeripheral, 0x27, 4, 20);

                // initialize now
                Console.WriteLine("Begin LCD_Test");
                lcdAPI.InitLCD();

                // title bit
                lcdAPI.MoveTo(1, 3);
                lcdAPI.WriteString("CS_LCD Library");
                lcdAPI.MoveTo(2, 4);
                lcdAPI.WriteString("Tests Begin");

                Thread.Sleep(3000);
                lcdAPI.Clear();

                // set values in all four corners
                lcdAPI.MoveTo(2, 4);
                lcdAPI.WriteString("TestCorners");

                lcdAPI.MoveTo(0, 0);
                lcdAPI.WriteChar('A');
                lcdAPI.MoveTo(1, 0);
                lcdAPI.WriteChar('B');
                lcdAPI.MoveTo(2, 0);
                lcdAPI.WriteChar('C');
                lcdAPI.MoveTo(3, 0);
                lcdAPI.WriteChar('D');

                lcdAPI.MoveTo(0, 19);
                lcdAPI.WriteChar('1');
                lcdAPI.MoveTo(1, 19);
                lcdAPI.WriteChar('2');
                lcdAPI.MoveTo(2, 19);
                lcdAPI.WriteChar('3');
                lcdAPI.MoveTo(3, 19);
                lcdAPI.WriteChar('4');

                Thread.Sleep(3000);
                lcdAPI.Clear();

                lcdAPI.MoveTo(2, 4);
                lcdAPI.WriteString("AutoWrapTest");
                Thread.Sleep(2000);

                lcdAPI.MoveTo(0, 18);
                lcdAPI.WriteString("ABC");
                Thread.Sleep(1000);
                lcdAPI.MoveTo(1, 18);
                lcdAPI.WriteString("DEF");
                Thread.Sleep(1000);
                lcdAPI.MoveTo(2, 18);
                lcdAPI.WriteString("GHI");
                Thread.Sleep(1000);
                lcdAPI.MoveTo(3, 18);
                lcdAPI.WriteString("JKL");

                Thread.Sleep(3000);
                lcdAPI.Clear();

                // now test the backlight
                lcdAPI.MoveTo(2, 3);
                lcdAPI.WriteString("BacklightOff+On");
                Thread.Sleep(2000);
                lcdAPI.BacklightOff();
                Thread.Sleep(2000);
                lcdAPI.BacklightOn();

                Thread.Sleep(3000);
                lcdAPI.Clear();

                // now test the display
                lcdAPI.MoveTo(2, 3);
                lcdAPI.WriteString("DisplayOff+On");
                Thread.Sleep(2000);
                lcdAPI.DisplayOff();
                Thread.Sleep(2000);
                lcdAPI.DisplayOn();

                Thread.Sleep(3000);
                lcdAPI.Clear();

                // now test the display
                lcdAPI.MoveTo(2, 3);
                lcdAPI.WriteString("Show Cursor");
                Thread.Sleep(2000);
                lcdAPI.ShowCursor();
                Thread.Sleep(2000);
                lcdAPI.MoveTo(2, 3);
                lcdAPI.WriteString("Blink Cursor");
                lcdAPI.BlinkCursorOn();
                Thread.Sleep(2000);
                lcdAPI.MoveTo(2, 3);
                lcdAPI.WriteString("Cursor Off  ");
                lcdAPI.HideCursor();

                Thread.Sleep(3000);
                lcdAPI.Clear();

                lcdAPI.MoveTo(2, 2);
                lcdAPI.WriteString("Custom Char Test");
                lcdAPI.HideCursor();

                Thread.Sleep(2000);

                // now custom char test
                byte[] happyFace = new byte[] { 0x00, 0x0A, 0x00, 0x04, 0x00, 0x11, 0x0E, 0x00 };
                byte[] sadFace = new byte[] { 0x00, 0x0A, 0x00, 0x04, 0x00, 0x0E, 0x11, 0x00 };
                byte[] grinFace = new byte[] { 0x00, 0x00, 0x0A, 0x00, 0x1F, 0x11, 0x0E, 0x00 };
                byte[] shockFace = new byte[] { 0x0A, 0x00, 0x04, 0x00, 0x0E, 0x11, 0x11, 0x0E };
                byte[] mehFace = new byte[] { 0x00, 0x0A, 0x00, 0x04, 0x00, 0x1F, 0x00, 0x00 };
                byte[] angryFace = new byte[] { 0x11, 0x0A, 0x11, 0x04, 0x00, 0x0E, 0x11, 0x00 };
                byte[] tongueFace = new byte[] { 0x00, 0x0A, 0x00, 0x04, 0x00, 0x1F, 0x05, 0x02 };

                lcdAPI.CustomChar(0, happyFace);
                lcdAPI.CustomChar(1, sadFace);
                lcdAPI.CustomChar(2, grinFace);
                lcdAPI.CustomChar(3, shockFace);
                lcdAPI.CustomChar(4, mehFace);
                lcdAPI.CustomChar(5, angryFace);
                lcdAPI.CustomChar(6, tongueFace);

                lcdAPI.MoveTo(0, 7);
                lcdAPI.WriteChar((char)0x00);
                lcdAPI.WriteChar((char)0x01);
                lcdAPI.WriteChar((char)0x02);
                lcdAPI.WriteChar((char)0x03);
                lcdAPI.WriteChar((char)0x04);
                lcdAPI.WriteChar((char)0x05);
                lcdAPI.WriteChar((char)0x06);

                Thread.Sleep(3000);
                lcdAPI.Clear();

                lcdAPI.MoveTo(1, 3);
                lcdAPI.WriteString("CS_LCD Library");
                lcdAPI.MoveTo(2, 3);
                lcdAPI.WriteString("Test Complete");

                Console.WriteLine("End LCD_Test");
            }
            finally
            {
                // close the port
                i2cPeripheral.ClosePort();
                i2cPeripheral.Dispose();
            }
}
    }
}
