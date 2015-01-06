using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace WhatColourIsIt
{
    public partial class ScreenSaverForm : Form
    {
        private Point mouseLocation;
        private bool previewMode = false;
        private int middleScreenWidth, middleScreenHeight;
        private IntPtr previewWndHandle = IntPtr.Zero;
        private int padding = 50;

        public ScreenSaverForm()
        {
            InitializeComponent();
        }

        public ScreenSaverForm(Rectangle Bounds)
        {
            InitializeComponent();
            this.Bounds = Bounds;
        }

        public ScreenSaverForm(IntPtr previewWndHandle)
        {
            InitializeComponent();
            this.previewWndHandle = previewWndHandle;
            previewMode = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!previewMode)
            {
                Cursor.Hide();
            }
            else
            {
                // Set the preview window as the parent of this window
                Win32.SetParent(this.Handle, previewWndHandle);

                // Make this a child window so it will close when the parent dialog closes
                Win32.SetWindowLong(this.Handle, -16, Win32.GetWindowLong(this.Handle, -16) | Win32.WS_CHILD);

                // Place our window inside the parent
                Rectangle ParentRect;
                Win32.GetClientRect(previewWndHandle, out ParentRect);
                this.Size = ParentRect.Size;
                this.Location = new Point(0, 0);

                // Make text smaller (size / 6)
                labelTime.Font = new System.Drawing.Font("Trebuchet MS", 12F);
                labelTitle.Font = new System.Drawing.Font("Trebuchet MS", 4.375F);
                labelRGB.Font = new System.Drawing.Font("Trebuchet MS", 3.375F);
                labelHexColour.Font = new System.Drawing.Font("Trebuchet MS", 3.375F);

                padding /= 6;
            }
            
            this.TopMost = true;

            labelTitle.Location = new Point(this.Bounds.Width / 2 - labelTitle.Width / 2, padding);

            timer_Tick(null, null);

            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            labelTime.Text = now.ToString("HH : mm : ss", DateTimeFormatInfo.InvariantInfo);
            labelTime.Location = new Point(this.Bounds.Width / 2 - labelTime.Width / 2, this.Bounds.Height / 2 - labelTime.Height / 2);

            int r = (int)(now.Hour * 11.08);
            int g = (int)(now.Minute * 4.25);
            int b = (int)(now.Second * 4.25);

            labelHexColour.Text = GetHexColour(r, g, b);
            labelHexColour.Location = new Point(this.Bounds.Width / 2 - labelHexColour.Width / 2, this.Bounds.Height - labelHexColour.Height - padding);

            labelRGB.Text = String.Format("rgb({0}, {1}, {2})", r, g, b);
            labelRGB.Location = new Point(this.Bounds.Width / 2 - labelRGB.Width / 2, labelHexColour.Location.Y - labelRGB.Height - padding / 2);

            this.BackColor = Color.FromArgb(r, g, b);

            if (IsHighContrast(r, g, b))
            {
                labelRGB.ForeColor = Color.White;
                labelTime.ForeColor = Color.White;
                labelTitle.ForeColor = Color.White;
                labelHexColour.ForeColor = Color.White;
            }
            else
            {
                labelRGB.ForeColor = Color.Black;
                labelTime.ForeColor = Color.Black;
                labelTitle.ForeColor = Color.Black;
                labelHexColour.ForeColor = Color.Black;
            }
        }

        private bool IsHighContrast(int r, int g, int b)
        {
            int luminance = ((r * 299) + (g * 587) + (b * 114)) / 1000;

            return (luminance < 128) ? true : false;
        }

        private string GetHexColour(int r, int g, int b)
        {
            return "#" + (r * 256 * 256 + g * 256 + b).ToString("x3");
        }

        private void ScreenSaverForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!previewMode)
            {
                if (!mouseLocation.IsEmpty)
                {
                    // Terminate if mouse is moved a significant distance
                    if (Math.Abs(mouseLocation.X - e.X) > 5 || Math.Abs(mouseLocation.Y - e.Y) > 5)
                    {
                        Application.Exit();
                    }
                }

                // Update current mouse location
                mouseLocation = e.Location;
            }
        }

        private void ScreenSaverForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }

        private void ScreenSaverForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!previewMode)
                Application.Exit();
        }
    }
}
