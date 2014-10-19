using System;
using System.Text;

namespace MessageException
{
    public class InitilizationException : ApplicationException
    {
        public InitilizationException() { }

        public InitilizationException(string msg) : base(msg) { }
    }
}
