using System;
using System.Windows.Forms;

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

			TetEngine te = new TetEngine();
			MainForm mf = new MainForm(te);

			Application.Run(mf);
		}
	}
}
