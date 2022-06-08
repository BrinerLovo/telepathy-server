using System;

namespace Lovatto.Chicas
{
    class ServerConsole
    {
        internal static TelepathySocket socket = new TelepathySocket();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Test.DoTest();

            ChicasSocket.Active = socket;
            socket.Connect();

            // automatically close the connection when close the console.
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnAppExit);
        }

        /// <summary>
        /// 
        /// </summary>
        static void OnAppExit(object sender, EventArgs e)
        {
            if (socket != null) socket.Stop();
        }
    }
}