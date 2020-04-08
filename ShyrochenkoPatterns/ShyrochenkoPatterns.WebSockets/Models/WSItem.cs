using ShyrochenkoPatterns.WebSockets.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace ShyrochenkoPatterns.WebSockets.Models
{
    public class WSItem<ConnectedData> where ConnectedData : class, IWSItem
    {
        public WebSocket Socket { get; set; }

        public ConnectedData Data { get; set; }
    }
}
