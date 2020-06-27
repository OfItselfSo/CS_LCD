using System;
using System.Text;
using System.Threading;

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
    /// The base class of the CS_LCD library. 
    /// 
    /// This code generates the byte sequences designed to control an LCD display
    /// using the HD44780 chipset (or derivative). Since there are numerous 
    /// hardware and software mechanisms to send the data to the LCD this code
    /// has no "send the data" functionality. Users of this code are expected to
    /// implement the HAL_* functions below in the derived class 
    /// in order to transmit the data.
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
    public abstract class CS_LCDBase
    {

        // constants derived from python_lcd
        private const int LCD_CLR = 0x01;              // DB0: clear display
        private const int LCD_HOME = 0x02;             // DB1: return to home position

        private const int LCD_ENTRY_MODE = 0x04;       // DB2: set entry mode
        private const int LCD_ENTRY_INC = 0x02;        // --DB1: increment
        private const int LCD_ENTRY_SHIFT = 0x01;      // --DB0: shift

        private const int LCD_ON_CTRL = 0x08;          // DB3: turn lcd/cursor on
        private const int LCD_ON_DISPLAY = 0x04;       // --DB2: turn display on
        private const int LCD_ON_CURSOR = 0x02;        // --DB1: turn cursor on
        private const int LCD_ON_BLINK = 0x01;         // --DB0: blinking cursor

        private const int LCD_MOVE = 0x10;             // DB4: move cursor/display
        private const int LCD_MOVE_DISP = 0x08;        // --DB3: move display (0-> move cursor)
        private const int LCD_MOVE_RIGHT = 0x04;       // --DB2: move right (0-> left)

        private const int LCD_FUNCTION = 0x20;         // DB5: function set
        private const int LCD_FUNCTION_8BIT = 0x10;    // --DB4: set 8BIT mode (0->4BIT mode)
        private const int LCD_FUNCTION_2LINES = 0x08;  // --DB3: two lines (0->one line)
        private const int LCD_FUNCTION_10DOTS = 0x04;  // --DB2: 5x10 font (0->5x7 font)
        private const int LCD_FUNCTION_RESET = 0x30;   // See "Initializing by Instruction" section

        private const int LCD_CGRAM = 0x40;            // DB6: set CG RAM address
        private const int LCD_DDRAM = 0x80;            // DB7: set DD RAM address

        private const int LCD_RS_CMD = 0;
        private const int LCD_RS_DATA = 1;

        private const int LCD_RW_WRITE = 0;
        private const int LCD_RW_READ = 1;


        public const int MIN_NUM_ROWS = 0;      // 1  zero based
        public const int MIN_NUM_COLS = 19;     // 20 zero based
        public const int MAX_NUM_ROWS = 3;      // 4 zero based
        public const int MAX_NUM_COLS = 39;     // 40 zero based
        public const int DEFAULT_NUM_ROWS = 1;  // 2 zero based
        public const int DEFAULT_NUM_COLS = 39; // 40 zero based

        // these are stored as zero based - makes the addressing
        // much easier
        private int numRows = DEFAULT_NUM_ROWS;
        private int numCols = DEFAULT_NUM_COLS;

        // these are always zero based 
        public const int DEFAULT_CURSOR_ROW = 0;
        public const int DEFAULT_CURSOR_COL = 0;
        private int currentCursorRow = DEFAULT_CURSOR_ROW;
        private int currentCursorCol = DEFAULT_CURSOR_COL;

        // state of the backlight 1=on, 0=off
        private byte backlightState = 1;


        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numRowsIn">the number of rows on the display</param>
        /// <param name="numColsIn">the number of cols on the display</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        protected CS_LCDBase(int numRowsIn, int numColsIn)
        {
            // zero base and set this now
            NumRows = numRowsIn - 1;
            NumCols = numColsIn - 1;

            // the most we can accept is 4 rows and 40 cols. We sanity check
            // them now these have already been zero based
            if (NumRows > MAX_NUM_ROWS) NumRows = MAX_NUM_ROWS;
            if (NumCols > MAX_NUM_COLS) NumCols = MAX_NUM_COLS;
            if (NumRows < 0) NumRows = DEFAULT_NUM_ROWS;
            if (NumCols < 0) NumCols = DEFAULT_NUM_COLS;
        }

        #region Base Library Calls

        // # ##################################################################
        // # #####  Base Library Calls
        // # ##################################################################

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Initializes the LCD
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void InitLCD()
        {
 
            CurrentCursorCol = 0;
            CurrentCursorRow = 0;
            BacklightState = 1;
            DisplayOff();
            BacklightOn();
            Clear();
            HALWriteCommand((byte)(LCD_ENTRY_MODE | LCD_ENTRY_INC));
            HideCursor();
            DisplayOn();

            // this second Clear() seems to be necessary for some 
            // displays to reset properly
            Thread.Sleep(100);
            Clear();
            Thread.Sleep(100);
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Clears the LCD display and moves the cursor to the top left corner.
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void Clear()
        {
            HALWriteCommand(LCD_CLR);
            HALWriteCommand(LCD_HOME);
            CurrentCursorCol = 0;
            CurrentCursorRow = 0;
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Causes the cursor to be made visible.
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void ShowCursor()
        {
            HALWriteCommand(LCD_ON_CTRL | LCD_ON_DISPLAY | LCD_ON_CURSOR);
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Causes the cursor to be hidden.
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void HideCursor()
        {
            HALWriteCommand(LCD_ON_CTRL | LCD_ON_DISPLAY);
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Turns on the cursor, and makes it blink.
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void BlinkCursorOn()
        {
            HALWriteCommand(LCD_ON_CTRL | LCD_ON_DISPLAY | LCD_ON_CURSOR | LCD_ON_BLINK);
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Turns on the cursor, and makes it not blink (i.e. be solid)
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void BlinkCursorOff()
        {
            HALWriteCommand(LCD_ON_CTRL | LCD_ON_DISPLAY | LCD_ON_CURSOR);
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Turns on (i.e. unblanks) the LCD.
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void DisplayOn()
        {
            HALWriteCommand(LCD_ON_CTRL | LCD_ON_DISPLAY);
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Turns off (i.e. blanks) the LCD.
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void DisplayOff()
        {
            HALWriteCommand(LCD_ON_CTRL);
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Turns the backlight on.
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void BacklightOn()
        {
            //This isn't really an LCD command, but some modules have backlight
            //controls, so this allows the HAL to pass through the command.

            BacklightState = 1;
            HALBacklightOn();
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Turns the backlight off.
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void BacklightOff()
        {
            // This isn't really an LCD command, but some modules have backlight
            // controls, so this allows the HAL to pass through the command.
            BacklightState = 0;
            HALBacklightOff();
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Writes the indicated character to the LCD at the current cursor
        //     position, and advances the cursor by one position
        /// </summary>
        /// <param name="inChar">the character to write</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void WriteChar(char inChar)
        {
            if (inChar != '\n')
            {
                HALWriteData(Convert.ToByte(inChar));
                CurrentCursorCol += 1;
                // note, the cursor autoincrements on the display here
            }

            // if we have exceeded the number of columns in the row or the user sent a newline
            // we need to reset the cursor manually. The auto increment in this case 
            // will not point at the correct position on the display
            if ((CurrentCursorCol > NumCols) || (inChar == '\n'))
            {
                CurrentCursorCol = 0;
                CurrentCursorRow += 1;
                if (CurrentCursorRow > NumRows)
                {
                    CurrentCursorRow = 0;
                }
                MoveTo(CurrentCursorRow, CurrentCursorCol);
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Write the indicated string to the LCD at the current cursor
        /// position and advances the cursor position appropriately.
        /// </summary>
        /// <param name="inStr">the string to write</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void WriteString(string inStr)
        {

            if (inStr == null) return;
            foreach (char c in inStr)
            {
                WriteChar(c);
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Write a character to one of the 8 CGRAM locations, available
        ///    as 0x00 through 0x07
        ///  
        /// Note: Once the custom char is set you use it by writing 0x00 to 0x07
        ///       as the character. IE instead of sending 'A' (or 0x41) to draw 
        ///       an 'A' on the screen you would send 0x00 to display the custom
        ///       character placed in that location by this call.       
        ///       
        /// </summary>
        /// <param name="location">the location, this can only be 0x00 to 0x07</param>
        /// <param name="byteArr">eight bytes which describe the pixels in the character
        /// only the bottom 5 bits are significant</param>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void CustomChar(byte location, byte[] byteArr)
        {
            location &= 0x7;
            HALWriteCommand((byte)(LCD_CGRAM | (location << 3)));
            Thread.Sleep(40);
            foreach (byte b in byteArr)
            {
                HALWriteData(b);
                Thread.Sleep(40);
            }
            MoveTo(CurrentCursorRow, CurrentCursorCol);
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Sets the cursor position. Note the Arduino Liquid_Crystal library uses
        /// a reverse ordering (col, row) of the parameters in its equivalent function
        /// This is irritating and so was modified here.
        ///
        /// Note: if the position is set out of bounds the cursor will be set to the
        /// closest boundary. The cursor position is zero based (i.e. col == 0 indicates 
        /// the first column)
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public void MoveTo(int row, int col)
        {
            // boundary test now
            if (row < 0) row = 0;
            if (col < 0) col = 0;
            if (row > NumRows) row = NumRows;
            if (col > NumCols) col = NumCols;

            CurrentCursorRow = row;
            CurrentCursorCol = col;

            byte addr = (byte)(CurrentCursorCol & 0x3f);
            if ((CurrentCursorRow & 0x01) != 0)
            {
                addr += 0x40;    // Lines 1 & 3 add 0x40
            }
            if ((CurrentCursorRow & 0x2) != 0)
            {
                addr += 0x14;   // Lines 2 & 3 add 0x14
            }

            // send the data
            HALWriteCommand((byte)(LCD_DDRAM | addr));
        }

        #endregion

        #region Supporting code

        // # ##################################################################
        // # ##### Supporting code
        // # ##################################################################

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the currentCursorRow on the LCD.  Cursor is always
        /// zero based. High bounds are not checked so the calling code can implement
        /// wrapping as desired.
        ///  
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public int CurrentCursorRow
        {
            get
            {
                return currentCursorRow;
            }
            set
            {
                if (value < 0) currentCursorRow = 0;
                else currentCursorRow = value;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the currentCursorCol on the LCD.  Cursor is always
        /// zero based. High bounds are not checked so the calling code can implement
        /// wrapping as desired.
        ///  
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public int CurrentCursorCol
        {
            get
            {
                return currentCursorCol;
            }
            set
            {
                if (value < 0) currentCursorCol = 0;
                else currentCursorCol = value;
            }
        }


        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the number of rows on the LCD. Zero based.
        /// 
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public int NumRows
        {
            get
            {
                return numRows;
            }
            set
            {
                numRows = value;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the number of cols on the LCD. Zero based.
        ///  
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        public int NumCols
        {
            get
            {
                return numCols;
            }
            set
            {
                numCols = value;
            }
        }

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Gets/Sets the state of the backlight. Only permits 1 or 0 to be stored
        /// </summary>
        /// <history>
        ///    13 Jun 20  Cynic - Started
        /// </history>
        protected byte BacklightState
        {
            get
            {
                if (backlightState > 1) backlightState = 1;
                return backlightState;
            }
            set
            {
                backlightState = value;
                if (backlightState > 1) backlightState = 1;
            }
        }
        #endregion

        #region Abstract To Be implemented

        // # ##################################################################
        // # ##### Abstract functions - to be implemented by the child class
        // # ##################################################################

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Allows the hal layer to turn the backlight on
        /// </summary>
        protected abstract void HALBacklightOn();

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Allows the hal layer to turn the backlight off
        /// </summary>
        protected abstract void HALBacklightOff();

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Write a command to the LCD
        /// </summary>
        /// <param name="cmdByte">the byte to write</param>
        protected abstract void HALWriteCommand(byte cmdByte);

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Write binary data to the LCD.
        /// </summary>
        /// <param name="cmdByte">the byte to write</param>
        protected abstract void HALWriteData(byte cmdByte);

        /// +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=
        /// <summary>
        /// Send a byte with no interpretation
        /// </summary>
        /// <param name="dataByte">the byte to write</param>
        protected abstract void HALSendByte(byte byte2Send);

        #endregion  

    }
}
