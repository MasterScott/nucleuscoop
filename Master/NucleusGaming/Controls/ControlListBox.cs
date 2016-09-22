﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Nucleus.Gaming
{
    public partial class ControlListBox : Panel
    {
        public event Action<object, Control> SelectedChanged;
        public Size Offset { get; set; }

        public ControlListBox()
        {
            InitializeComponent();

            this.AutoScroll = true;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            for (int i = 0; i < this.Controls.Count; i++)
            {
                var con = Controls[i];
                con.Width = this.Width;
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            if (se.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                int barSize = SystemInformation.VerticalScrollBarWidth;
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    var con = Controls[i];
                    con.Width = this.Width - barSize;
                }
            }
            else if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                int barSize = SystemInformation.HorizontalScrollBarHeight;
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    var con = Controls[i];
                    con.Height = this.Height - barSize;
                }
            }
            base.OnScroll(se);
        }

        public void UpdateSizes()
        {
            //int barSize = SystemInformation.VerticalScrollBarWidth;
            for (int i = 0; i < this.Controls.Count; i++)
            {
                var con = Controls[i];
                con.Width = this.Width;
            }
        }

        private int totalHeight;
        private int border = 1;
        public int Border
        {
            get { return border; }
            set { border = value; }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (!this.DesignMode)
            {
                if (e.Control != null)
                {
                    Control c = e.Control;
                    c.Click += new EventHandler(c_Click);
                    if (c is IHighlightControl)
                    {
                        c.MouseMove += new MouseEventHandler(c_MouseMove);
                        c.MouseLeave += new EventHandler(c_MouseLeave);
                    }

                    int index = this.Controls.IndexOf(c);
                    Size s = c.Size;

                    c.Location = new Point(0, totalHeight);

                    totalHeight += s.Height + border;
                }
            }
        }

        public Control SelectedControl { get; protected set; }

        public void Deselect()
        {
            SelectedControl = null;
            c_Click(this, EventArgs.Empty);
        }

        void c_MouseMove(object sender, MouseEventArgs e)
        {
            Control parent = (Control)sender;

            if (parent != SelectedControl)
            {
                IHighlightControl high = (IHighlightControl)parent;
                high.SoftHighlight();
            }
        }
        void c_MouseLeave(object sender, EventArgs e)
        {
            Control parent = (Control)sender;

            if (parent != SelectedControl)
            {
                IHighlightControl high = (IHighlightControl)parent;
                high.Darken();
            }
        }

        private void c_Click(object sender, EventArgs e)
        {
            Control parent = (Control)sender;

            for (int i = 0; i < this.Controls.Count; i++)
            {
                Control c = this.Controls[i];
                if (c is IHighlightControl)
                {
                    IHighlightControl high = (IHighlightControl)c;
                    if (parent == c)
                    {
                        // highlight
                        high.Highlight();
                    }
                    else
                    {
                        high.Darken();
                    }
                }
            }

            if (parent != null &&
                parent != SelectedControl)
            {
                if (this.SelectedChanged != null)
                {
                    SelectedControl = parent;
                    this.SelectedChanged(SelectedControl, this);
                }
            }

            SelectedControl = parent;

            this.OnClick(e);
        }


    }
}
