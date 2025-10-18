using System;
using System.Text.RegularExpressions;

namespace BlueberryTUI
{
    /// <summary>
    /// Class for creating a Window object that has a certain size and position.
    /// </summary>
    public class Window
    {
        public int WindowWidth;//Test comment
        public int WindowHeight;
        public int WindowLeft;
        public int WindowTop;
        private static Dictionary<string, (char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Top, char Bottom, char Left, char Right)> OutlineStyles = new()
        { 
            {"LIGHT", ('\u250c','\u2510','\u2514','\u2518','\u2500','\u2500','\u2502','\u2502')},
            {"HEAVY", ('\u250f','\u2513','\u2517','\u251B','\u2501','\u2501','\u2503','\u2503')},
            {"HASHTAG", ('#','#','#','#','#','#','#','#')},
            {"ASCIIART", ('.','.','^','^','-','-','|','|')}
        };
        private static readonly List<string> ListOfStandardOutlineStyles = [.. OutlineStyles.Keys];
        public string OutlineStyle = "LIGHT";

        private static Dictionary<string, (string Box, string OutlineBackground, string OutlineForeground)> WindowThemes = new()
        {
            {"RED", ("255;150;150","255;0;0","255;0;0")},
            {"GREEN", ("150;255;150","0;255;0","150;255;0")},
            {"BLUE", ("150;150;255","0;0;255","0;0;255")},
            {"TEST", ("NONE","NONE","0;0;255")},
            {"NONE", ("NONE","NONE","NONE")}
        };
        private static readonly List<string> ListOfStandardWindowThemes = [.. WindowThemes.Keys];
        public string WindowTheme = "NONE";

        /// <summary>
        /// Window constructor.
        /// </summary>
        /// <param name="windowwidth"></param>
        /// <param name="windowheight"></param>
        /// <param name="windowleft"></param>
        /// <param name="windowtop"></param>
        /// <param name="outlinestyle"></param>
        /// <param name="windowtheme"></param>
        public Window(int windowwidth, int windowheight, int windowleft, int windowtop, string outlinestyle = "LIGHT", string windowtheme = "NONE")
        {
            if (windowwidth < 2) windowwidth = 2;

            if (windowheight < 2) windowheight = 2;

            if (!OutlineStyles.ContainsKey(outlinestyle)) OutlineStyle = "LIGHT";

            if (!WindowThemes.ContainsKey(windowtheme)) WindowTheme = "NONE";

            WindowWidth = windowwidth;
            WindowHeight = windowheight;
            WindowLeft = windowleft;
            WindowTop = windowtop;
            OutlineStyle = outlinestyle;
            WindowTheme = windowtheme;
        }

        /// <summary>
        /// Draws the window in the terminal.
        /// </summary>
        public void DrawWindow()
        {
            if (!(WindowLeft <= -WindowWidth || WindowLeft >= Console.WindowWidth || WindowTop <= -WindowHeight || WindowTop >= Console.WindowHeight))
            {
                (int Left, int Top) TempCursorPosition = Console.GetCursorPosition();

                (char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Top, char Bottom, char Left, char Right) = OutlineStyles[OutlineStyle];// Current OutlineStyle
                (string Box, string OutlineBackground, string OutlineForeground) = WindowThemes[WindowTheme];// Current WindowTheme

                List<string> WindowToDraw = [];

                string TopLineToWrite = Substring(TopLeft + String.Concat(Enumerable.Repeat(Top, WindowWidth - 2)) + TopRight, WindowLeft < 0 ? Math.Abs(WindowLeft) : 0, WindowLeft + WindowWidth > Console.WindowWidth ? Console.WindowWidth - WindowLeft - 1 : WindowWidth);
                if (WindowTheme != "NONE")
                {
                    if (OutlineForeground != "NONE")
                    {
                        TopLineToWrite = $"\u001b[38;2;{OutlineForeground}m{TopLineToWrite}\u001b[m";
                    }
                    if (OutlineBackground != "NONE")
                    {
                        TopLineToWrite = $"\u001b[48;2;{OutlineBackground}m{TopLineToWrite}\u001b[m";
                    }
                }
                WindowToDraw.Add(TopLineToWrite);

                for (int i = 0; i < WindowHeight - 2; i++)
                {
                    string LeftChar = Left.ToString();
                    string RightChar = Right.ToString();
                    string MiddleChars = String.Concat(Enumerable.Repeat(' ', WindowWidth - 2));
                    if (WindowTheme != "NONE")
                    {
                        if (OutlineForeground != "NONE")
                        {
                            LeftChar = $"\u001b[38;2;{OutlineForeground}m{LeftChar}\u001b[m";
                            RightChar = $"\u001b[38;2;{OutlineForeground}m{RightChar}\u001b[m";
                        }
                        if (OutlineBackground != "NONE")
                        {
                            LeftChar = $"\u001b[48;2;{OutlineBackground}m{LeftChar}\u001b[m";
                            RightChar = $"\u001b[48;2;{OutlineBackground}m{RightChar}\u001b[m";
                        }
                        if (Box != "NONE")
                        {
                            MiddleChars = $"\u001b[48;2;{Box}m{MiddleChars}\u001b[m";
                        }
                    }
                    WindowToDraw.Add(Substring(LeftChar + MiddleChars + RightChar, WindowLeft < 0 ? Math.Abs(WindowLeft) : 0, WindowLeft + WindowWidth > Console.WindowWidth ? Console.WindowWidth - WindowLeft - 1 : WindowWidth));
                }

                string BottomLineToWrite = Substring(BottomLeft + String.Concat(Enumerable.Repeat(Bottom, WindowWidth - 2)) + BottomRight, WindowLeft < 0 ? Math.Abs(WindowLeft) : 0, WindowLeft + WindowWidth > Console.WindowWidth ? Console.WindowWidth - WindowLeft - 1 : WindowWidth);
                if (WindowTheme != "NONE")
                {
                    if (OutlineForeground != "NONE")
                    {
                        BottomLineToWrite = $"\u001b[38;2;{OutlineForeground}m{BottomLineToWrite}\u001b[m";
                    }
                    if (OutlineBackground != "NONE")
                    {
                        BottomLineToWrite = $"\u001b[48;2;{OutlineBackground}m{BottomLineToWrite}\u001b[m";
                    }
                }
                WindowToDraw.Add(BottomLineToWrite);

                Console.SetCursorPosition(WindowLeft < 0 ? 0 : WindowLeft > Console.WindowWidth ? Console.WindowWidth : WindowLeft, WindowTop < 0 ? 0 : WindowTop > Console.WindowHeight ? Console.WindowHeight : WindowTop);
                Console.CursorVisible = false;

                for (int i = WindowTop < 0 ? Math.Abs(WindowTop) : 0; i < (WindowTop + WindowToDraw.Count > Console.WindowHeight ? Console.WindowHeight - WindowTop : WindowToDraw.Count); i++)
                {
                    Console.Write(WindowToDraw[i]);
                    Console.SetCursorPosition(WindowLeft < 0 ? 0 : WindowLeft > Console.WindowWidth ? Console.WindowWidth : WindowLeft, Console.CursorTop < Console.WindowHeight - 1 ? Console.CursorTop + 1: Console.CursorTop);
                }

                Console.CursorVisible = true;
                Console.SetCursorPosition(TempCursorPosition.Left, TempCursorPosition.Top);
            }
        }

        /// <summary>
        /// Create a new OutlineStyle.
        /// </summary>
        /// <param name="outlinestylename"></param>
        /// <param name="outlinestylechars"></param>
        /// <exception cref="Exception"></exception>
        public static void NewOutlineStyle(string OutlineStyleName, (char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Top, char Bottom, char Left, char Right) OutlineStyleChars)
        {
            if (!OutlineStyles.ContainsKey(OutlineStyleName))
            {
                OutlineStyles.Add(OutlineStyleName, OutlineStyleChars);
            }
            else
            {
                throw new Exception($"New OutlineStyle name: {OutlineStyleName}. An OutlineStyle with this name already exists. Use a different name or use the UpdateOutlineStyle() method to update an OutlineStyle.");
            }
        }

        /// <summary>
        /// Update a custom OutlineStyle.
        /// </summary>
        /// <param name="outlinestylename"></param>
        /// <param name="outlinestylechars"></param>
        /// <exception cref="Exception"></exception>
        public static void UpdateOutlineStyle(string OutlineStyleName, (char TopLeft, char TopRight, char BottomLeft, char BottomRight, char Top, char Bottom, char Left, char Right) OutlineStyleChars)
        {
            if (!ListOfStandardOutlineStyles.Contains(OutlineStyleName) && OutlineStyles.ContainsKey(OutlineStyleName))
            {
                OutlineStyles[OutlineStyleName] = OutlineStyleChars;
            }
            else if (ListOfStandardOutlineStyles.Contains(OutlineStyleName))
            {
                throw new Exception($"Attempted to update the following OutlineStyle: {OutlineStyleName}. Can not update standard OutlineStyles");
            }
            else
            {
                throw new Exception($"Attempted to update the following OutlineStyle: {OutlineStyleName}. This OutlineStyle does not exist. Did you forget to make the OutlineStyle?");
            }
        }

        /// <summary>
        /// Delete a custom OutlineStyle.
        /// </summary>
        /// <param name="outlinestylename"></param>
        /// <exception cref="Exception"></exception>
        public static void DeleteOutlineStyle(string OutlineStyleName)
        {
            if (!ListOfStandardOutlineStyles.Contains(OutlineStyleName) && OutlineStyles.ContainsKey(OutlineStyleName))
            {
                OutlineStyles.Remove(OutlineStyleName);
            }
            else if (ListOfStandardOutlineStyles.Contains(OutlineStyleName))
            {
                throw new Exception($"Attempted to delete the following OutlineStyle: {OutlineStyleName}. Can not delete standard OutlineStyles");
            }
            else
            {
                throw new Exception($"Attempted to delete the following OutlineStyle: {OutlineStyleName}. This OutlineStyle does not exist.");
            }
        }

        /// <summary>
        /// Create a new WindowTheme.
        /// </summary>
        /// <param name="WindowThemeName"></param>
        /// <param name="WindowThemeColors"></param>
        /// <exception cref="Exception"></exception>
        public static void NewWindowTheme(string WindowThemeName, (string Box, string OutlineBackground, string OutlineForeground) WindowThemeColors)
        {
            if (WindowThemes.ContainsKey(WindowThemeName))
            {
                WindowThemes.Add(WindowThemeName, WindowThemeColors);
            }
            else
            {
                throw new Exception($"New WindowTheme name: {WindowThemeName}. A WindowTheme with this name already exists. Use a different name or use the UpdateWindowTheme() method to update a WindowTheme.");
            }
        }

        /// <summary>
        /// Update a custom WindowTheme.
        /// </summary>
        /// <param name="WindowThemeName"></param>
        /// <param name="WindowThemeColors"></param>
        /// <exception cref="Exception"></exception>
        public static void UpdateWindowTheme(string WindowThemeName, (string Box, string OutlineBackground, string OutlineForeground) WindowThemeColors)
        {
            if (!ListOfStandardWindowThemes.Contains(WindowThemeName) && WindowThemes.ContainsKey(WindowThemeName))
            {
                WindowThemes[WindowThemeName] = WindowThemeColors;
            }
            else if (ListOfStandardWindowThemes.Contains(WindowThemeName))
            {
                throw new Exception($"Attempted to update the following WindowTheme: {WindowThemeName}. Can not update standard WindowThemes.");
            }
            else
            {
                throw new Exception($"Attempted to update the following WindowTheme: {WindowThemeName}. This WindowTheme does not exist. Did you forget to make the WindowTheme?");
            }
        }

        /// <summary>
        /// Delete a custom WindowTheme.
        /// </summary>
        /// <param name="WindowThemeName"></param>
        /// <exception cref="Exception"></exception>
        public static void DeleteWindowTheme(string WindowThemeName)
        {
            if (!ListOfStandardWindowThemes.Contains(WindowThemeName) && WindowThemes.ContainsKey(WindowThemeName))
            {
                WindowThemes.Remove(WindowThemeName);
            }
            else if (ListOfStandardWindowThemes.Contains(WindowThemeName))
            {
                throw new Exception($"Attempted to delete the following WindowTheme: {WindowThemeName}. Can not delete standard WindowThemes.");
            }
            else
            {
                throw new Exception($"Attempted to delete the following WindowTheme: {WindowThemeName}. This WindowTheme does not exist.");
            }
        }

        /// <summary>
        /// Returns a substring from the Input string. This method compensates for ANSI escape codes (currently only tested with 24 bit color escape codes) being present.
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <returns></returns>
        private string Substring(string Input, int Start, int End)
        {
            string Output = "";
            bool IgnoreChars = false;
            int CurrentChar = 0;
            for (int i = 0; i < Input.Length; i++)
            {
                if (Input[i] == '\u001b')
                {
                    IgnoreChars = true;
                }

                if (!IgnoreChars)
                {
                    if (CurrentChar >= Start && CurrentChar <= End)
                    {
                        Output += Input[i];
                    }
                    CurrentChar++;
                }
                else
                {
                    Output += Input[i];
                }

                if (IgnoreChars && Input[i] == 'm')
                {
                    IgnoreChars = false;
                }
            }
            return Regex.Replace(Output, @"(\u001b\[[0-9;]+m)+\u001b\[m", "");
        }
    }
}
