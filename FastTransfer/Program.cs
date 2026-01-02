using System;
using System.Windows.Forms;

namespace FastTransfer
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            
            if (args.Length == 0)
            {
                MessageBox.Show("This application should be launched from the right-click context menu.", "Fast Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Application.Run(new MainForm(args[0]));
        }
    }
}
