using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication1 {
    internal static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            SqlCommand comm = new SqlCommand();
            var now = DateTime.Now.AddYears(6000);
            double r = ConvertToTimestamp(now);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        static double ConvertToTimestamp(DateTime value) {
            TimeSpan elapsedTime = value - Epoch;
            return  elapsedTime.TotalSeconds;
        }
    }
}
