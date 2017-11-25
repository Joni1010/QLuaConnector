using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServiceMessage
{
    public class ServiceMessage
    {
        /// <summary> ID </summary>
        public long id = 0;
        /// <summary> Сервисное сообщение </summary>
        public object Message = null;
        public ServiceMessage(object msg, long id = 0)
        {
            this.id = id;
            this.Message = msg;
        }
    }


}