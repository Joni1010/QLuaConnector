using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServiceMessage
{
    public class StackMsg
    {
        public long id = 0;
        public object Message = null;
        public StackMsg(long id, object msg)
        {
            this.id = id;
            this.Message = msg;
        }
    }


}