using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace _485刷机_Net_3._5
{
    static class Program
    {

        public static int  isValidUser = 0x00;//0x00:无  0x01:主控  0x02:dsp
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Application.Run(new Form6());
            Application.Run(new Form4());

            if (isValidUser == 0x01)
            {
                Application.Run(new Form3());
                
            }
            else if (isValidUser == 0x02)
            {
                Application.Run(new Form3());
            }
           
        }
    }
}
