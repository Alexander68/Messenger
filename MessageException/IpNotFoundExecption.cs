using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageEception
{
    class IpNotFoundExecption : ApplicationException
    {
        public IpNotFoundExecption() { }

        public IpNotFoundExecption(String msg) : base(msg) { }

        public IpNotFoundExecption(String msg, Exception cause) : base(msg, cause) { }
    }
}
