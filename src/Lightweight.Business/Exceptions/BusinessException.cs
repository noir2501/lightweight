using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lightweight.Business.Exceptions
{
    public class BusinessException : Exception
    {
        public Guid CorrelationId { get; set; }

        private readonly BusinessExceptionType _type;
        public BusinessExceptionType Type
        {
            get { return _type; }
        }

        public BusinessException(string message, Exception innerException, BusinessExceptionType type)
            : base(message, innerException)
        {
            _type = type;
            CorrelationId = Guid.NewGuid();
        }

        public BusinessException(string message)
            : this(message, null, BusinessExceptionType.ERROR)
        {

        }

        public BusinessException(string message, Exception innerException)
            : this(message, innerException, BusinessExceptionType.ERROR)
        {

        }
    }
}
