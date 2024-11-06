using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Interfaces.Messaging
{
    public interface IMessageListener
    {
        Task StartListeningAsync<T>(string queueName, Func<T, Task> handleMessage);
        Task<T> WaitForResponseAsync<T>(string queueName, string correlationId);
    }
}
