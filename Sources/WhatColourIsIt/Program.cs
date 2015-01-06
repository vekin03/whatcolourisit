﻿/*
 * Inspired by:
 * - Screen saver's tutorial by Frank McCown (Summer 2010)
 * - http://lehollandaisvolant.net/tout/tools/color-second/
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WhatColourIsIt
{
    static class Program
    {
        /// <summary>
        /// Main entry point of program.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0)
            {
                string firstArgument = args[0].ToLower().Trim();
                string secondArgument = null;

                // Handle cases where arguments are separated by colon. 
                // Example: /P:1234567
                if (firstArgument.Length > 2)
                {
                    secondArgument = firstArgument.Substring(3).Trim();
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (args.Length > 1)
                {
                    secondArgument = args[1];
                }

                if (firstArgument == "/c")           // Configuration mode
                {
                    ShowSettings();
                }
                else if (firstArgument == "/p")      // Preview mode
                {
                    if (secondArgument == null)
                    {
                        MessageBox.Show("Sorry, but the expected window handle was not provided.", "What colour is it?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    IntPtr previewWndHandle = new IntPtr(Int64.Parse(secondArgument));
                    Application.Run(new ScreenSaverForm(previewWndHandle));
                }
                else if (firstArgument == "/s")      // Full-screen mode
                {
                    ShowScreenSaver();
                    Application.Run();
                }
                else    // Undefined argument
                {
                    MessageBox.Show("Sorry, but the command line argument \"" + firstArgument + "\" is not valid.", "What colour is it?", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else    // No arguments - treat like /c
            {
                ShowSettings();
            }
        }

        /// <summary>
        /// Display the form on each of the computer's monitors.
        /// </summary>
        private static void ShowScreenSaver()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                ScreenSaverForm screensaver = new ScreenSaverForm(screen.Bounds);
                screensaver.Show();
            }
        }

        /// <summary>
        /// Display the settings form.
        /// </summary>
        private static void ShowSettings()
        {
            MessageBox.Show("This screen saver doesn't have settings!", "What colour is it?", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
