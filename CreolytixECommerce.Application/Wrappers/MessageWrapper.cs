using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Wrappers
{
    public class MessageWrapper<T>
    {
        public string CorrelationId { get; set; }
        public T Payload { get; set; }

        public MessageWrapper(T payload)
        {
            Payload = payload;
            CorrelationId = Guid.NewGuid().ToString();  // Generate a unique CorrelationId
        }
    }
}
