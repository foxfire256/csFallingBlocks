using System;
using System.Windows.Forms;
using System.Configuration;

namespace csFallingBlocks
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// will be null if the value isn't present
			string s;

			s = ConfigurationManager.AppSettings["square_size"];
			bool haveSquareSize = false;
			int squareSize = 0;
			if(s != null)
			{
				if(Int32.TryParse(s, out squareSize))
				{
					haveSquareSize = true;
				}
			}

			s = ConfigurationManager.AppSettings["game_array_width"];
			s = ConfigurationManager.AppSettings["game_array_height"];

			TetEngine te = null;
			te = new TetEngine();

			MainForm mf = null;
			if (haveSquareSize)
			{
				mf = new MainForm(te, squareSize);
			}
			else
			{
				mf = new MainForm(te);
			}

			Application.Run(mf);
		}
	}
}
