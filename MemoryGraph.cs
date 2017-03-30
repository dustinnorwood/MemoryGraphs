using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace MemoryGraphs
{
    public class MemoryGraph : IDisposable
    {
        Bitmap m_Bitmap;
        Graphics m_Graphics;

        public MemoryGraph()
        {
            m_Bitmap = null;
            m_Graphics = null;
        }

        public MemoryGraph(Image image)
        {
            CreateCopyFromImage(image);
        }

        public MemoryGraph(int width, int height)
        {
            Create(width, height);
        }

        public Bitmap Image
        {
            get { return m_Bitmap; }
        }

        public int Width { get { return m_Bitmap == null ? 0 : m_Bitmap.Width; } }
        public int Height { get { return m_Bitmap == null ? 0 : m_Bitmap.Height; } }
        public Rectangle Rect { get { return new Rectangle(0, 0, Width, Height); } }

        public Graphics Graphics
        {
            get { return m_Graphics; }
        }

        public Graphics Create(Size size)
        {
            return Create(size.Width, size.Height);
        }

        public Graphics Create(int width, int height, PixelFormat piexelFormat)
        {
            if (m_Bitmap != null)
                m_Bitmap.Dispose();
            if (m_Graphics != null)
                m_Graphics.Dispose();

            m_Bitmap = new Bitmap(width > 0 ? width : 1, height > 0 ? height : 1, piexelFormat);
            m_Graphics = Graphics.FromImage(m_Bitmap);
            return m_Graphics;
        }

        public Graphics Create(int width, int height)
        {
            if (m_Bitmap != null)
                m_Bitmap.Dispose();
            if (m_Graphics != null)
                m_Graphics.Dispose();

            m_Bitmap = new Bitmap(width > 0 ? width : 1, height > 0 ? height : 1, PixelFormat.Format16bppRgb555);
            m_Graphics = Graphics.FromImage(m_Bitmap);
            return m_Graphics;
        }

        public Graphics Select(Bitmap bitmap)
        {
            //			if (m_Bitmap != null)
            //				m_Bitmap.Dispose();
            if (m_Graphics != null)
                m_Graphics.Dispose();

            m_Bitmap = bitmap;
            m_Graphics = Graphics.FromImage(m_Bitmap);
            return m_Graphics;
        }

        public void CreateFromImage(Image image)
        {
            Create(image.Width, image.Height);
        }

        public void CreateCopyFromImage(Image image)
        {
            Create(image.Width, image.Height);
            CopyFromImage(image);
        }

        public void CopyFromImage(Image image)
        {
#if (WINCE)
			m_Graphics.DrawImage(image, 0, 0);
#else
            m_Graphics.DrawImage(image, 0, 0, image.Width, image.Height);
#endif
            //			m_Graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
        }

        public void CopyFromImage(Image image, int x, int y)
        {
            m_Graphics.DrawImage(image, new Rectangle(x, y, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
        }

        public void CopyFromScaledImage(Image image)
        {
            m_Graphics.DrawImage(image, Rect, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
        }

        public void CopyFromCroppedImage(Image image, int x, int y)
        {
            m_Graphics.DrawImage(image, 0, 0, new Rectangle(x, y, Width, Height), GraphicsUnit.Pixel);
        }

        public void CopyFromScaledCroppedImage(Image image, int x, int y, int w, int h)
        {
            m_Graphics.DrawImage(image, Rect, new Rectangle(x, y, w, h), GraphicsUnit.Pixel);
        }

        public void EraseBackground(Color color)
        {
            m_Graphics.Clear(color);
        }

        public void Paint(Graphics graphics, int x, int y)
        {
#if (WINCE)
//			graphics.DrawImage(m_Bitmap, x, y);
#else
            //			graphics.DrawImage(m_Bitmap, x, y, m_Bitmap.Width, m_Bitmap.Height);
#endif
            graphics.DrawImage(m_Bitmap, new Rectangle(x, y, Width, Height), new Rectangle(0, 0, Width, Height), GraphicsUnit.Pixel);
        }

        public void Paint(Graphics graphics, int x, int y, int w, int h)
        {
            graphics.DrawImage(m_Bitmap, new Rectangle(x, y, w, h), Rect, GraphicsUnit.Pixel);
        }

        public void PaintCropped(Graphics graphics, int x, int y, int croppedAreaWidth, int croppedAreaHeigh)
        {
            graphics.DrawImage(m_Bitmap, x, y, new Rectangle(0, 0, croppedAreaWidth, croppedAreaHeigh), GraphicsUnit.Pixel);
        }

        public void PaintCropped(Graphics graphics, int x, int y, int croppedX, int croppedY, int croppedAreaWidth, int croppedAreaHeigh)
        {
            graphics.DrawImage(m_Bitmap, x, y, new Rectangle(croppedX, croppedY, croppedAreaWidth, croppedAreaHeigh), GraphicsUnit.Pixel);
        }

        public void Paint(Graphics graphics, int x, int y, int w, int h, Color colorKey)
        {
            ImageAttributes attr = new ImageAttributes();
            attr.SetColorKey(colorKey, colorKey);
            graphics.DrawImage(m_Bitmap, new Rectangle(x, y, w, h), 0, 0, Width, Height, GraphicsUnit.Pixel, attr);
            attr.Dispose();
        }

        public void Paint(Graphics graphics, int x, int y, int w, int h, ImageAttributes attr)
        {
            graphics.DrawImage(m_Bitmap, new Rectangle(x, y, w, h), 0, 0, Width, Height, GraphicsUnit.Pixel, attr);
        }

        public void AutoSizeAndDrawString(string s, Font f, Color c, int width, int height, bool left)
        {
            SizeF sizef = this.Graphics.MeasureString(s, f);
            while (sizef.Width >= (float)(width))
            {
                f = new System.Drawing.Font(f.Name, f.Size - 1.0f, f.Style);
                sizef = this.Graphics.MeasureString(s, f);
            }

            using (SolidBrush brush = new SolidBrush(c))
            {
                if (left)
                {
                    this.Graphics.DrawString(s, f, brush, 1, (height - sizef.Height) / 2 + 1);
                }
                else
                {
                    this.Graphics.DrawString(s, f, brush, width / 2 - sizef.Width / 2, (height - sizef.Height) / 2 + 1);
                }
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (m_Bitmap != null)
                m_Bitmap.Dispose();
            if (m_Graphics != null)
                m_Graphics.Dispose();
            m_Graphics = null;
            m_Bitmap = null;
        }

        #endregion
    }
}
