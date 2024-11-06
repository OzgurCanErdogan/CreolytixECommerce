using CreolytixECommerce.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Interfaces.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string queueName, MessageWrapper<T> messageWrapper, string replyToQueue = null);

    }
}
