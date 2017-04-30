using System;

namespace FileManager.BL.Workers
{
    public class ResultDto
    {
        public ResultDto(Result result) : this(result, null) { }

        public ResultDto(Result result, Exception ex)
        {
            Result = result;
            Exception = ex;
        }

        public Result Result { get; }

        public Exception Exception { get; }
    }
}
