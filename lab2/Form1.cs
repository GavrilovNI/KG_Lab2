using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace lab2
{

    public partial class Form1 : Form
    {
        private static Form1 instance = null;
        public static Form1 GetInstance()
        {
            return instance;
        }

        private enum DrawType
        {
            Texture,
            QuadStrip
        }
        private DrawType drawType;

        public TrackBar minTB => trackBar2;
        public TrackBar maxTB => trackBar3;

        View view = new View();
        bool loaded = false;

        int frameCount = 0;
        DateTime nextFPSUpdate = DateTime.Now.AddSeconds(1);

        bool needReload = false;

        public Form1()
        {

            InitializeComponent();
            drawType = DrawType.Texture;
            textureToolStripMenuItem.Enabled = false;

            Application.Idle += Idle;

            instance = this;

            trackBar2.Value = 0;
            trackBar3.Value = 2000;
        }

        private void CountAndDisplayFPS()
        {
            if(DateTime.Now >= nextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", frameCount);
                nextFPSUpdate = nextFPSUpdate.AddSeconds(1);
                frameCount = 0;
            }
            frameCount++;
        }

        private void Idle(object sender, EventArgs e)
        {
            if(glControl1.IsIdle)
            {
                glControl1.Invalidate();
                CountAndDisplayFPS();
            }
        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if(loaded)
            {
                if(drawType==DrawType.Texture)
                {
                    if (needReload)
                    {
                        view.GenerateTextureImage(trackBar1.Value);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                }
                else if(drawType==DrawType.QuadStrip)
                {
                    view.DrawQuads(trackBar1.Value);
                }

                glControl1.SwapBuffers();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog()==DialogResult.OK)
            {
                string str = dialog.FileName;
                Bin.ReadBin(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
                trackBar1.Maximum = Bin.array.GetLength(2)-1;
                trackBar1.Value = Math.Min(trackBar1.Maximum, trackBar1.Value);
                needReload = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            Application.Idle -= Idle;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            needReload = true;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            view._min = trackBar2.Value;
            needReload = true;
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            view._max = trackBar3.Value;
            needReload = true;
        }

        private void textureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawType = DrawType.Texture;
            textureToolStripMenuItem.Enabled = false;
            quadTripToolStripMenuItem.Enabled = true;
        }

        private void quadTripToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawType = DrawType.QuadStrip;
            textureToolStripMenuItem.Enabled = true;
            quadTripToolStripMenuItem.Enabled = false;
        }
    }
}
