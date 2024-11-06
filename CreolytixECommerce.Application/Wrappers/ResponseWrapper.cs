using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreolytixECommerce.Application.Wrappers
{
    public class ResponseWrapper<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T ResultDto { get; set; }
    }
}
