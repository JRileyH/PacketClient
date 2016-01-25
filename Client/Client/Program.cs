using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client.Network;
using Client.Network.Packets;
using System.Net;
using System.Runtime.InteropServices;

namespace Client
{
    class Program
    {
        public static Boolean running = true;
        public static Listener listener;
        public static IPAddress myIP;
        static void Main(string[] args)
        {
	//Hey look I made a change! Aint that special!
            Console.WriteLine("Connect to IP: ");
            string host = Console.ReadLine();
            listener = new Listener(host);
            myIP = IPAddress.Parse(listener.getLocalIP());
            Console.WriteLine("Welcome, " + myIP);
            listener.sendPacket(new Packet000());
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                while (running)
                {
                    Thread.CurrentThread.IsBackground = true;
                    listener.listen();
                }
            }, null);
            while (running)
            {
                String msg = Console.ReadLine();
                if (msg[0] == '/')
                {
                    int point;
                    char command = msg[1];
                    point = msg.IndexOf(':') + 1;
                    string param = msg.Substring(point, msg.IndexOf(' ') - point);
                    point = msg.IndexOf(' ') + 1;
                    string content = msg.Substring(point, msg.Length - point);
                    switch(msg[1])
                    {
                        case 'w':
                            listener.sendPacket(new Packet002(content, param));
                            break;
                        case 'c':
                            listener.sendPacket(new Packet001(content, (ConsoleColor)Enum.Parse(typeof(ConsoleColor), param)));
                            break;
                    }
                    
                    
                }
                else
                {
                    listener.sendPacket(new Packet001(msg));
                }
            }
        }
        #region closing
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_CLOSE_EVENT:
                    running = false;
                    listener.sendPacket(new Packet999());
                    break;
            }
            return true;
        }
        
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        #endregion
    }
}
