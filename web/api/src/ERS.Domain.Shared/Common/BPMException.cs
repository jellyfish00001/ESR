using System;

namespace ERS.Common
{
    public class BPMException : Exception
    {
        public BPMException(string message, Exception inner) : base(message, inner) { }
        public BPMException(string message) : base(message) { }
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }
    }
}
