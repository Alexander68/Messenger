using System;
using System.Text;

namespace MessageException
{
    class IpNotFoundExecption : ApplicationException
    {
        public IpNotFoundExecption() { }

        public IpNotFoundExecption(String msg) : base(msg) { }

        public IpNotFoundExecption(String msg, Exception cause) : base(msg, cause) { }
    }
}
