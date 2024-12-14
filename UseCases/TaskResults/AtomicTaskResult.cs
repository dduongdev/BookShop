using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.TaskResults
{
    public class AtomicTaskResult
    {
        public AtomicTaskResultCodes ResultCode { get; }
        public string Message { get; } = string.Empty;

        public AtomicTaskResult(AtomicTaskResultCodes resultCode, string message) 
        {
            ResultCode = resultCode; 
            Message = message; 
        }

        public static AtomicTaskResult Success = new(AtomicTaskResultCodes.Success, string.Empty);
    }
}
