using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace lab2
{
    class View
    {
        Bitmap textureImage;
        int VBOTexture;

        public int _min = 0;
        public int _max = 2000;

        public void SetupView(int width, int height)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.x, 0, Bin.y, -1, 1);
            GL.Viewport(0, 0, width, height);
        }

        public void Load2DTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, VBOTexture);
            BitmapData data = textureImage.LockBits(
                new System.Drawing.Rectangle(0, 0, textureImage.Width, textureImage.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, data.Scan0);

            textureImage.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);

            ErrorCode Er = GL.GetError();
            string str = Er.ToString();
        }

        public void GenerateTextureImage(int layerNumber)
        {
            textureImage = new Bitmap(Bin.x, Bin.y);
            for (int i = 0; i < Bin.x; i++)
            {
                for (int j = 0; j < Bin.y; j++)
                {
                    textureImage.SetPixel(i, j, Transfer(Bin.array[i, j, layerNumber]));
                }
            }
        }

        public void DrawTexture()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, VBOTexture);

            GL.Begin(BeginMode.Quads);
            GL.Color3(Color.White);

            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 0);
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, Bin.y);
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(Bin.x, Bin.y);
            GL.TexCoord2(1f, 0f);
            GL.Vertex2(Bin.x, 0);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
        }

        public Color Transfer(short value)
        {
            //int _min = Form1.GetInstance().minTB.Value;//0;
            //int _max = Form1.GetInstance().maxTB.Value;//2000;
            int _newMin = 0;
            int _newMax = 255;

            int newVal = Clamp((value - _min) * (_newMax - _newMin) / (_max - _min) + _newMin, _newMin, _newMax);
            return Color.FromArgb(255, newVal, newVal, newVal);
        }

        public void DrawQuads(int layerNumber)
        {
            /*GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Begin(BeginMode.Quads);
            for (int x_coord = 0; x_coord < Bin.x - 1; x_coord++)
            {
                for (int y_coord = 0; y_coord < Bin.y - 1; y_coord++)
                {
                    GL.Color3(Transfer(Bin.array[x_coord, y_coord, layerNumber]));
                    GL.Vertex2(x_coord, y_coord);

                    GL.Color3(Transfer(Bin.array[x_coord, y_coord + 1, layerNumber]));
                    GL.Vertex2(x_coord, y_coord+1);

                    GL.Color3(Transfer(Bin.array[x_coord + 1, y_coord + 1, layerNumber]));
                    GL.Vertex2(x_coord + 1, y_coord + 1);

                    GL.Color3(Transfer(Bin.array[x_coord + 1, y_coord, layerNumber]));
                    GL.Vertex2(x_coord + 1, y_coord);
                }
            }
            GL.End();*/

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            for (int y_coord = 0; y_coord < Bin.y-1; y_coord++)
            {
                GL.Begin(BeginMode.QuadStrip);

                for (int x_coord = 0; x_coord < Bin.x; x_coord++)
                {
                    GL.Color3(Transfer(Bin.array[x_coord, y_coord, layerNumber]));
                    GL.Vertex2(x_coord, y_coord);

                    GL.Color3(Transfer(Bin.array[x_coord, y_coord + 1, layerNumber]));
                    GL.Vertex2(x_coord, y_coord + 1);
                }
                GL.End();
            }
        }

        private int Clamp(int val, int min, int max)
        {
            if (val > max) return max;
            if (val < min) return min;
            return val;
        }
    }
}
