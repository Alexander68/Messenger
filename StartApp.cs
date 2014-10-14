using System;
using System.Threading;
using System.Windows.Forms;

namespace Messenger
{
    class StartApp
    {
        [STAThread]
        public static void Main(String[] args)
        {
            Messenger.ui.MainWindow window = new Messenger.ui.MainWindow();
            Application.Run(window);
        }
    }
}
