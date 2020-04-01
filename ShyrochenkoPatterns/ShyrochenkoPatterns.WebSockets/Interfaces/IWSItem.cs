using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.WebSockets.Interfaces
{
    public interface IWSItem
    {
        int TokenId { get; set; }

        int UserId { get; set; }
    }
}
