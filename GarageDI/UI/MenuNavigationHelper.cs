using System;
using GarageDI.Contracts;

namespace GarageDI.UI
{
    public static class MenuNavigationHelper
    {
        public static int ShowSelectionMenu(
            IUI ui,
            string[] options,
            int startIndex = 0)
        {
            int selectedIndex = startIndex;
            ConsoleKeyInfo key;
            do
            {
                ui.Clear();
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        ui.SetBackgroundColor(ConsoleColor.Green);
                        ui.SetForegroundColor(ConsoleColor.Black);
                        ui.Print($"> {options[i]}");
                        ui.ResetColor();
                    }
                    else
                    {
                        ui.Print($"  {options[i]}");
                    }
                }
                key = ui.GetKeyInfo();
                if (key.Key == ConsoleKey.UpArrow && selectedIndex > 0)
                    selectedIndex--;
                else if (key.Key == ConsoleKey.DownArrow && selectedIndex < options.Length - 1)
                    selectedIndex++;
            } while (key.Key != ConsoleKey.Enter);

            return selectedIndex;
        }
    }
} 