using System;
using System.Net.Sockets;

namespace EVEMon.Common.IgbService
{
    /// <summary>
    /// Event arguments triggered on client connect.
    /// </summary>
    public class ClientConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectedEventArgs"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public ClientConnectedEventArgs(TcpClient client)
        {
            TcpClient = client;
        }

        /// <summary>
        /// Gets or sets the TCP client.
        /// </summary>
        /// <value>The TCP client.</value>
        public TcpClient TcpClient { get; }
    }
}