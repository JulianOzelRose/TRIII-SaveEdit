/*
    Julian O. Rose
    Program.cs
    8-8-2023
*/

using System;
using System.Windows.Forms;

namespace TRIII_SaveEdit
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TR3Edit());
        }
    }
}
