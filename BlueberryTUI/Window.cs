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
        private static Dictionary<string, (char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Top, char Bottom, char Left, char Right)> BorderThemes = new()
        { 
            {"LIGHT", ('\u250c','\u2510','\u2514','\u2518','\u2500','\u2500','\u2502','\u2502')},
            {"HEAVY", ('\u250f','\u2513','\u2517','\u251B','\u2501','\u2501','\u2503','\u2503')},
            {"HASHTAG", ('#','#','#','#','#','#','#','#')},
            {"ASCIIART", ('.','.','^','^','-','-','|','|')}
        };
        private static List<string> ListOfStandardThemes = BorderThemes.Keys.ToList();
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
            int windowwidth,
            int windowheight,
            int windowleft,// Window drawing always starts at the top left
            int windowtop,
            string bordertheme = "LIGHT")
        {
            if (windowwidth < 2) windowwidth = 2;

            if (windowheight < 2) windowheight = 2;

            if (!BorderThemes.ContainsKey(bordertheme)) BorderTheme = "LIGHT";

            WindowWidth = windowwidth;
            WindowHeight = windowheight;
            WindowLeft = windowleft;
            WindowTop = windowtop;
            BorderTheme = bordertheme;
        }

        /// <summary>
        /// Draws the Window in the terminal.
        /// </summary>
        public void DrawWindow()
        {
            if (!(WindowLeft <= -WindowWidth || WindowLeft >= Console.WindowWidth || WindowTop <= -WindowHeight || WindowTop >= Console.WindowHeight))
            {
                (int Left, int Top) TempCursorPosition = Console.GetCursorPosition();

                (char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Top, char Bottom, char Left, char Right) BT = BorderThemes[BorderTheme];

                string InsideWindowSpace = String.Concat(Enumerable.Repeat(' ', WindowWidth - 2));
                string WindowTopLine = String.Concat(Enumerable.Repeat(BT.Top, WindowWidth - 2));
                string WindowBottomLine = String.Concat(Enumerable.Repeat(BT.Bottom, WindowWidth - 2));

                Console.SetCursorPosition(WindowLeft < 0 ? 0 : WindowLeft + 1 > Console.WindowWidth ? Console.WindowWidth : WindowLeft, WindowTop < 0 ? 0 : WindowTop + WindowHeight > Console.WindowHeight ? Console.WindowHeight - WindowHeight < 0 ? WindowTop : Console.WindowHeight - WindowHeight : WindowTop);

                if (WindowTop + WindowHeight > Console.WindowHeight && WindowHeight <= Console.WindowHeight)
                {
                    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + (WindowTop + WindowHeight > Console.WindowHeight ? WindowTop + WindowHeight - Console.WindowHeight : 0));
                }

                if (WindowTop >= 0)
                {
                    //Top line of the Window
                    Console.Write(WindowLineSubstring(BT.TopLeft + WindowTopLine + BT.TopRight));
                }

                bool FirstLoop = true;
                bool MoveLastLineDown = false;
                //Middle of the Window
                for (int i = 0 + (WindowTop < 0 ? Math.Abs(WindowTop + 1) : 0); i < WindowHeight - 2 - (WindowTop + WindowHeight > Console.WindowHeight ? WindowTop + WindowHeight - Console.WindowHeight - 1 : 0); i++)
                {
                    Console.SetCursorPosition(WindowLeft < 0 ? 0 : WindowLeft + 1 > Console.WindowWidth ? Console.WindowWidth : WindowLeft, Console.CursorTop + (FirstLoop && WindowTop < 0 ? 0 : 1));
                    Console.Write(WindowLineSubstring(BT.Left + InsideWindowSpace + BT.Right));
                    FirstLoop = false;
                    MoveLastLineDown = true;
                }

                if (WindowTop + WindowHeight <= Console.WindowHeight)
                {
                    //Bottom line of the Window
                    Console.SetCursorPosition(WindowLeft < 0 ? 0 : WindowLeft + 1 > Console.WindowWidth ? Console.WindowWidth : WindowLeft, Console.CursorTop + (MoveLastLineDown ? 1 : 0));
                    Console.Write(WindowLineSubstring(BT.BottomLeft + WindowBottomLine + BT.BottomRight));
                }

                Console.SetCursorPosition(TempCursorPosition.Left, TempCursorPosition.Top);
            }
        }

        /// <summary>
        /// Create a new BorderTheme.
        /// </summary>
        /// <param name="borderthemename"></param>
        /// <param name="borderchars"></param>
        /// <exception cref="Exception"></exception>
        public static void NewBorderTheme(string borderthemename, (char, char, char, char, char, char, char, char) borderchars)
        {
            if (!BorderThemes.ContainsKey(borderthemename))
            {
                BorderThemes.Add(borderthemename, borderchars);
            }
            else
            {
                throw new Exception($"New BorderTheme name: {borderthemename}. A theme with this name already exists. Use a different name or use the UpdateBorderTheme() method to update a theme.");
            }
        }

        /// <summary>
        /// Update an existing BorderTheme.
        /// </summary>
        /// <param name="borderthemename"></param>
        /// <param name="borderchars"></param>
        /// <exception cref="Exception"></exception>
        public static void UpdateBorderTheme(string borderthemename, (char, char, char, char, char, char, char, char) borderchars)
        {
            if (!ListOfStandardThemes.Contains(borderthemename) && BorderThemes.ContainsKey(borderthemename))
            {
                BorderThemes[borderthemename] = borderchars;
            }
            else if (ListOfStandardThemes.Contains(borderthemename))
            {
                throw new Exception($"Attempted to update the following BorderTheme: {borderthemename}. Can not update standard themes");
            }
            else
            {
                throw new Exception($"Attempted to update the following BorderTheme: {borderthemename}. This theme does not exist. Did you forget to make the theme?");
            }
        }

        /// <summary>
        /// Delete and existing BorderTheme.
        /// </summary>
        /// <param name="borderthemename"></param>
        /// <exception cref="Exception"></exception>
        public static void DeleteBorderTheme(string borderthemename)
        {
            if (!ListOfStandardThemes.Contains(borderthemename) && BorderThemes.ContainsKey(borderthemename))
            {
                BorderThemes.Remove(borderthemename);
            }
            else if (ListOfStandardThemes.Contains(borderthemename))
            {
                throw new Exception($"Attempted to delete the following BorderTheme: {borderthemename}. Can not delete standard themes");
            }
            else
            {
                throw new Exception($"Attempted to delete the following BorderTheme: {borderthemename}. This theme does not exist.");
            }
        }

        /// <summary>
        /// Makes a substring of the Window line that will be drawn so the application does not attempt to draw lines outside of the terminal dimensions.
        /// </summary>
        /// <param name="LineToSubstring"></param>
        /// <returns></returns>
        private string WindowLineSubstring(string LineToSubstring)
        {
            string SubstringFromLeft = LineToSubstring[(WindowLeft < 0 ? Math.Abs(WindowLeft) : 0)..];
            int Length = SubstringFromLeft.Length;
            return SubstringFromLeft[..(WindowLeft < 0 && SubstringFromLeft.Length > Console.WindowWidth ? Console.WindowWidth : WindowLeft < 0 ? SubstringFromLeft.Length : SubstringFromLeft.Length + WindowLeft > Console.WindowWidth ? Console.WindowWidth - WindowLeft : SubstringFromLeft.Length)];
        }
    }
}
