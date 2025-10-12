using System;

namespace BlueberryTUI
{
    /// <summary>
    /// A Window object that has a certain X and Y size. BorderTheme defaults to "LIGHT"
    /// </summary>
    public class Window
    {
        public int WindowWidth;
        public int WindowHeight;
        public int WindowLeft;
        public int WindowTop;
        public bool KeepWindowOnScreen = true;
        private static Dictionary<string, (char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Top, char Bottom, char Left, char Right)> BorderThemes = new()
        { 
            {"LIGHT", ('\u250c','\u2510','\u2514','\u2518','\u2500','\u2500','\u2502','\u2502')},
            {"HEAVY", ('\u250f','\u2513','\u2517','\u251B','\u2501','\u2501','\u2503','\u2503')},
        };
        public string BorderTheme = "LIGHT";

        /// <summary>
        /// Window constructor. Both dimensions must be at least of size 2. Default BorderTheme is "LIGHT".
        /// </summary>
        /// <param name="xsize"></param>
        /// <param name="ysize"></param>
        /// <param name="xposition"></param>
        /// <param name="yposition"></param>
        /// <param name="bordertheme"></param>
        /// <param name="topleftcorner"></param>
        /// <param name="toprightcorner"></param>
        /// <param name="bottomleftcorner"></param>
        /// <param name="bottomrightcorner"></param>
        /// <param name="topline"></param>
        /// <param name="bottomline"></param>
        /// <param name="leftline"></param>
        /// <param name="rightline"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Window(
            int xsize,
            int ysize,
            int xposition,// Window drawing always starts at the top left
            int yposition,
            bool keepwindowonscreen = true,
            string bordertheme = "LIGHT")
        {
            if (xsize < 2 || ysize < 2)
            {
                xsize = 2;
                ysize = 2;
            }

            if (xposition < 0 || yposition < 0)
            {
                xposition = 0;
                yposition = 0;
            }

            if (!BorderThemes.ContainsKey(bordertheme))
            {
                BorderTheme = "LIGHT";
            }

            WindowWidth = xsize;
            WindowHeight = ysize;
            WindowLeft = xposition;
            WindowLeft = yposition;
            KeepWindowOnScreen = keepwindowonscreen;
            BorderTheme = bordertheme;
        }

        /// <summary>
        /// Draws the Window in the terminal.
        /// </summary>
        public void DrawWindow()// Make KeepWindowOnScreen and drawing outside of screen work as expected
        {

            (int Left, int Top) TempCursorPosition = Console.GetCursorPosition();

            (char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Top, char Bottom, char Left, char Right) BT = BorderThemes[BorderTheme];

            string InsideWindowSpace = String.Concat(Enumerable.Repeat(' ', WindowWidth - 2));
            string WindowTopLine = String.Concat(Enumerable.Repeat(BT.Top, WindowWidth - 2));
            string WindowBottomLine = String.Concat(Enumerable.Repeat(BT.Bottom, WindowWidth - 2));

            //Top line of the Window
            Console.SetCursorPosition(WindowLeft, WindowTop);
            Console.Write(BT.TopLeft + WindowTopLine + BT.TopRight);

            //Body of the Window
            for (int i = 0; i < WindowHeight - 2; i++)
            {
                Console.SetCursorPosition(Console.CursorLeft - WindowWidth, Console.CursorTop + 1);
                Console.Write(BT.Left + InsideWindowSpace + BT.Right);
            }

            //Bottom line of the Window
            if (WindowHeight + WindowTop <= Console.WindowHeight)
            {
                Console.SetCursorPosition(Console.CursorLeft - WindowWidth, Console.CursorTop + 1);
                Console.Write(BT.BottomLeft + WindowBottomLine + BT.BottomRight);
            }

            Console.SetCursorPosition(TempCursorPosition.Left, TempCursorPosition.Top);
        }

        /// <summary>
        /// Create a new BorderTheme.
        /// </summary>
        /// <param name="borderthemename"></param>
        /// <param name="borderchars"></param>
        public static void NewBorderTheme(string borderthemename, (char, char, char, char, char, char, char, char) borderchars)
        {
            BorderThemes.Add(borderthemename, borderchars);
        }
    }
}
