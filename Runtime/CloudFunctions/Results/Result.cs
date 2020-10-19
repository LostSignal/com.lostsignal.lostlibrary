//-----------------------------------------------------------------------
// <copyright file="CommonResult.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost.CloudFunctions
{
    using System;

    public class Result
    {
        protected Result()
        {
        }

        public bool Success { get; set; }

        public Exception Exception { get; set; }

        public static Result Ok()
        {
            return new Result
            {
                Success = true,
                Exception = null,
            };
        }

        public static Result Failure(Exception ex)
        {
            return new Result
            {
                Success = false,
                Exception = ex,
            };
        }
    }

    public class Result<T> : Result
    {
        public T ResultObject { get; set; }

        public static Result<T> Ok(T resultObject)
        {
            return new Result<T>
            {
                Success = true,
                Exception = null,
                ResultObject = resultObject,
            };
        }

        public static new Result<T> Failure(Exception ex)
        {
            return new Result<T>
            {
                Success = false,
                Exception = ex,
                ResultObject = default(T),
            };
        }
    }
}
