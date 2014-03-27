using System;
using System.Collections.Generic;

namespace EVEMon.Common
{
    public interface IEveMessage
    {
        string Title { get; }
        string SenderName { get; }
        DateTime SentDate { get; }
        IEnumerable<string> Recipient { get; }
        string Text { get; }
    }
}