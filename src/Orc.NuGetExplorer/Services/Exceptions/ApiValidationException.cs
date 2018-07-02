// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiValidationException.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ApiValidationException : Exception
    {
        public ApiValidationException()
        {
        }

        public ApiValidationException(string message)
            : base(message)
        {
        }

        public ApiValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ApiValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
