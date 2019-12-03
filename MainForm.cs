using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace csFallingBlocks
{
	public partial class MainForm : Form
	{
		private int squareSize;
		TetEngine te;
		int score;
		int level;

		public MainForm(TetEngine te)
		{
			this.te = te;
			InitializeComponent();

			squareSize = 16;

			Font f = label1.Font;
			Font fn = new Font(f.FontFamily, squareSize, f.Style);
			label1.Font = fn;
			label2.Font = fn;
			label3.Font = fn;

			Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);
			int titleHeight = screenRectangle.Top - this.Top;

			int win_w = (int)((te.X_MAX + 1) * 2 * squareSize);
			// this weird mess for the height was determined experimentally
			int win_h = (int)((te.Y_MAX + 1.5) * squareSize + titleHeight);

			this.Width = win_w;
			this.Height = win_h;

			int x = (int)((te.X_MAX + 1) * 1.5 * squareSize - label1.Width / 2);
			int y = (int)((te.Y_MAX + 1.5) * squareSize * 0.25 - label1.Height / 2);

			label1.Left = x;
			label1.Top = y;

			score = te.score;

			label2.Text = "Score: " + score;

			x = (int)((te.X_MAX + 1) * 1.5 * squareSize - label2.Width / 2);
			y = (int)((te.Y_MAX + 1.5) * squareSize * 0.75 - label2.Height / 2);
			
			label2.Left = x;
			label2.Top = y;

			level = te.level;

			label3.Text = "Level: " + level;

			x = (int)((te.X_MAX + 1) * 1.5 * squareSize - label3.Width / 2);
			y = label2.Bottom + label3.Height;

			label3.Left = x;
			label3.Top = y;

			label1.ForeColor = Color.FromArgb(255, 0, 255, 0);
			label2.ForeColor = Color.FromArgb(255, 0, 255, 0);
			label3.ForeColor = Color.FromArgb(255, 0, 255, 0);

			timer1.Interval = (int)(1.0 / 60.0 * 1000);
			timer1.Start();

			te.update_game_array();
		}

		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			int w, h;
			w = squareSize;
			h = squareSize;

			int x, y;

			// draw the game array
			x = 0;
			for(y = 0; y != (te.Y_MAX + 1);)
			{
				if(te.game_array[x, y] >= 1)
				{
					DrawSquare(x * w, y * w, squareSize, te.game_array[x, y], e);
				}

				// move to the next position
				if(x >= te.X_MAX)
				{
					y++;
					x = 0;
				}
				else
				{
					x++;
				}
			}

			//x = 320; y = 160;
			x = (int)((te.X_MAX - 1) * 1.5 * squareSize);
			// this weird mess for the height was determined experimentally
			y = (int)((te.Y_MAX) * squareSize / 2);
			int i = 0, j = 0;

			//fnt->printf_xy(renderer, 320, 140, "Next Block");

			w = squareSize;
			h = squareSize;

			// draw the next block array
			while(j < 3)
			{
				// fill in squares
				while(i < 5)
				{
					if(te.next_block_array[i, j] >= 1)
						DrawSquare(
							x + i * w, // x pos
							y + j * w, // y pos
							squareSize,
							te.next_block_array[i, j],
							e);  // square type  
					++i;
				}
				i = 0;
				++j;
			}

			Pen greenPen = new Pen(Color.FromArgb(255, 0, 255, 0));
			greenPen.Width = 3;
			x = (int)((te.X_MAX + 1) * squareSize) + 1;
			e.Graphics.DrawLine(greenPen, x, 0, x, (int)((te.Y_MAX + 1.5) * squareSize));
			greenPen.Dispose();

			if(te.score != score)
			{
				score = te.score;

				label2.Text = "Score: " + score;
			}

			if(te.level != level)
			{
				level = te.level;

				label3.Text = "Level: " + level;
			}
		}

		private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
		{
			
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{

		}

		private void DrawSquare(int x, int y, int size, int type, PaintEventArgs e)
		{
			// Create a path that consists of a single ellipse.
			GraphicsPath path = new GraphicsPath();

			Rectangle rect = new Rectangle(x, y, size, size);

			path.AddRectangle(rect);

			// Use the path to construct a brush.
			PathGradientBrush pthGrBrush = new PathGradientBrush(path);

			Color color;

			if(type == 1)
			{
				// Set the color at the center of the path.
				pthGrBrush.CenterColor = Color.FromArgb(255, 0, 0, 255);

				// Set the color along the entire boundary of the path
				color = Color.FromArgb(255, 255, 0, 0);
			}
			else if(type == 2)
			{
				// Set the color at the center of the path.
				pthGrBrush.CenterColor = Color.FromArgb(255, 0, 255, 0);

				// Set the color along the entire boundary of the path
				color = Color.FromArgb(255, 0, 0, 255);
			}
			else if(type == 3)
			{
				// Set the color at the center of the path.
				pthGrBrush.CenterColor = Color.FromArgb(255, 255, 0, 0);

				// Set the color along the entire boundary of the path
				color = Color.FromArgb(255, 255, 255, 0);
			}
			else if(type == 4)
			{
				// Set the color at the center of the path.
				pthGrBrush.CenterColor = Color.FromArgb(255, 0, 0, 255);

				// Set the color along the entire boundary of the path
				color = Color.FromArgb(255, 0, 255, 0);
			}
			else if(type == 5)
			{
				// Set the color at the center of the path.
				pthGrBrush.CenterColor = Color.FromArgb(255, 0, 0, 255);

				// Set the color along the entire boundary of the path
				color = Color.FromArgb(255, 255, 255, 0);
			}
			else if(type == 6)
			{
				// Set the color at the center of the path.
				pthGrBrush.CenterColor = Color.FromArgb(255, 255, 0, 0);

				// Set the color along the entire boundary of the path
				color = Color.FromArgb(255, 0, 0, 255);
			}
			else if(type == 7)
			{
				// Set the color at the center of the path.
				pthGrBrush.CenterColor = Color.FromArgb(255, 0, 255, 0);

				// Set the color along the entire boundary of the path
				color = Color.FromArgb(255, 255, 255, 0);
			}
			else
			{
				// Set the color at the center of the path.
				pthGrBrush.CenterColor = Color.FromArgb(255, 0, 0, 255);

				// Set the color along the entire boundary of the path
				color = Color.FromArgb(255, 255, 0, 0);
			}

			Color[] colors = { color };
			pthGrBrush.SurroundColors = colors;

			e.Graphics.FillRectangle(pthGrBrush, rect);
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Escape)
			{
				this.Close();
			}
			else if(e.KeyCode == Keys.Down)
			{
				te.move_down();
				te.update_game_array();
			}
			else if(e.KeyCode == Keys.Up)
			{
				te.move_rot();
				te.update_game_array();
			}
			else if(e.KeyCode == Keys.Left)
			{
				te.move_left();
				te.update_game_array();
			}
			else if(e.KeyCode == Keys.Right)
			{
				te.move_right();
				te.update_game_array();
			}
			else if(e.KeyCode == Keys.Space)
			{
				// down all
			}
			else if(e.KeyCode == Keys.P)
			{
				// this is pause
			}
			else if(e.KeyCode == Keys.D1)
			{
				te.fill_rows(1, 5);
				te.update_game_array();
			}
			else if(e.KeyCode == Keys.D2)
			{
				te.fill_rows(2, 5);
				te.update_game_array();
			}
			else if(e.KeyCode == Keys.D3)
			{
				te.fill_rows(3, 5);
				te.update_game_array();
			}
			else if(e.KeyCode == Keys.D4)
			{
				te.fill_rows(4, 5);
				te.update_game_array();
			}
			else if(e.KeyCode == Keys.D5)
			{
				te.fill_rows(5, 5);
				te.update_game_array();
			}
			else if(e.KeyCode == Keys.D6)
			{
				te.fill_rows(6, 5);
				te.update_game_array();
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.Invalidate();
		}
	}
}
