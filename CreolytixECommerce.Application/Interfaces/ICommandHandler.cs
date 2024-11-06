using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Interfaces
{
    public interface ICommandHandler<TCommand>
    {
        Task<bool> Handle(TCommand command);
    }
}
