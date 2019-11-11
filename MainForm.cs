using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace csFallingBlocks
{
	public partial class MainForm : Form
	{
		private int squareSize;
		
		public MainForm()
		{
			InitializeComponent();

			squareSize = 16;
			timer1.Interval = (int)(1.0 / 60.0 * 1000);
			timer1.Start();
		}

		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			DrawSquare(5, 5, 0, e);
		}

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{

		}

		private void DrawSquare(int x, int y, int type, PaintEventArgs e)
		{
			// Create a path that consists of a single ellipse.
			GraphicsPath path = new GraphicsPath();

			Rectangle rect = new Rectangle(x, y, squareSize, squareSize);

			path.AddRectangle(rect);

			// Use the path to construct a brush.
			PathGradientBrush pthGrBrush = new PathGradientBrush(path);

			// Set the color at the center of the path to blue.
			pthGrBrush.CenterColor = Color.FromArgb(255, 0, 0, 255);

			// Set the color along the entire boundary 
			// of the path to aqua.
			Color[] colors = { Color.FromArgb(255, 0, 255, 255) };
			pthGrBrush.SurroundColors = colors;

			e.Graphics.FillRectangle(pthGrBrush, rect);
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Escape)
			{
				this.Close();
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.Invalidate();
		}
	}
}
