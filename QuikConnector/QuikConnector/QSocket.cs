using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace QuikControl
{
    public class QSocket
    {
        // State object for reading client data asynchronously
        public class StateObject
        {
            // Client  socket.
            public Socket wSocket = null;
            // Size of receive buffer.
            public int BufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = null;

            /// <summary>
            /// Флаг соединения с серверной частью.
            /// </summary>
            public bool ConnectedLua = false;

            public StateObject(int sizeBuff)
            {
                BufferSize = sizeBuff;
                buffer = new byte[BufferSize];
            }
        }

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public StateObject StateSock = null;

        private IPAddress ipAddress;
        private IPEndPoint remoteEP;

        public QSocket(int sizeBuff)
        {
            this.StateSock = new StateObject(sizeBuff);
            this.StateSock.ConnectedLua = false;
        }
        //Создает сокет
        public int CreateSocket(string host, int port, int sizeReceivBuffer)
        {
            try
            {
                IPHostEntry ipHostInfo = Dns.Resolve(host);//Dns.GetHostName()
                this.ipAddress = ipHostInfo.AddressList[0];
                this.remoteEP = new IPEndPoint(ipAddress, port);
                // Create a TCP/IP socket.
                this.StateSock.wSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                this.StateSock.wSocket.ReceiveBufferSize = sizeReceivBuffer;

                this.StateSock.wSocket.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), this.StateSock);
                connectDone.WaitOne();
                return 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return -1;
            }
        }
        //Обработчик события подключения клиента
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                StateObject stateObj = (StateObject)ar.AsyncState;
                if (stateObj.wSocket.Connected)
                {
                    // Complete the connection.  
                    stateObj.wSocket.EndConnect(ar);
                    stateObj.ConnectedLua = true;
                }
                connectDone.Set();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public delegate int _Receive(object baseObj, int byteRecv, byte[] recvData);
        public event _Receive OnReceive;
        /// <summary>
        /// Получает данные из сокета
        /// </summary>
        /// <param name="baseObj"></param>
        /// <returns></returns>
        public int Receive(object baseObj)
        {
            if (this.StateSock.wSocket == null) return 0;
            if (this.StateSock.wSocket.Available <= 0) return 0;

            int bytesRead = 0;
            try
            {
                if (this.StateSock.wSocket.IsBound)
                {
                    bytesRead = this.StateSock.wSocket.Receive(this.StateSock.buffer, 0, this.StateSock.buffer.Length, SocketFlags.Partial);
                    if (OnReceive != null)
                        OnReceive(baseObj, bytesRead, this.StateSock.buffer);
                }
            }
            catch (Exception e) { Qlog.Write(e.ToString()); };
            return bytesRead;
        }


        /// <summary>  Отправка сообщений  </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public int Send(string msg)
        {
            int bytesSend = 0;
            if (this.StateSock.wSocket.Connected && msg != "")
            {
                msg = msg + '\0';
                byte[] msgBytes = Encoding.GetEncoding(1251).GetBytes(msg);
                bytesSend = this.StateSock.wSocket.Send(msgBytes, msgBytes.Length, SocketFlags.None);
                if (OnSend != null)
                    OnSend(msgBytes, msgBytes.Length);
            }
            return bytesSend;
        }
        public delegate int _Send(byte[] sendData, int size);
        public event _Send OnSend;
        /*
        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();
            try
            {
                // Get the socket that handles the client request.
                //Socket listener = (Socket)ar.AsyncState;
                this.stateSock.wSocket = this.stateSock.Listener.EndAccept(ar);

                // Create the state object.
                //StateObject state = this.stateSock;
                // state.workSocket = handler;
                // byte[] buffer = new byte[3000];
                // int recv = handler.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                //string message = Encoding.GetEncoding(1251).GetString(buffer, 0, recv);

                //Console.WriteLine("{0} - {1}", message.Length, message);
                this.stateSock.wSocket.BeginReceive(this.stateSock.buffer, 0, this.stateSock.buffer.Length, 0,
                    new AsyncCallback(ReadCallback), this.stateSock);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }*/

        public void Send(byte[] Data)
        {
            // Convert the string data to byte data using ASCII encoding.
            //byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            this.StateSock.wSocket.BeginSend(Data, 0, Data.Length, 0,
                new AsyncCallback(SendCallback), this.StateSock);
        }
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                StateObject stateObj = (StateObject)ar.AsyncState;
                // Complete sending the data to the remote device.
                int bytesSent = stateObj.wSocket.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Qlog.Write(e.ToString());
            }
        }
        /// <summary>
        /// Закрывает подключение к сокету
        /// </summary>
        public void CloseSocket()
        {
            this.StateSock.ConnectedLua = false;
            if (this.StateSock != null)
            {
                if (this.StateSock.wSocket != null) this.StateSock.wSocket.Close();
            }
        }
    }
}
