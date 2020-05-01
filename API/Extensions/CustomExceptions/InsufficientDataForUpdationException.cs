using System;
using System.Runtime.Serialization;

namespace API.Extensions.CustomExceptions
{
    public class InsufficientDataForUpdationException : Exception
    {
        public SerializationInfo Info { get; set; }
        public StreamingContext Context { get; set; }
        public InsufficientDataForUpdationException()
        {
        }

        public InsufficientDataForUpdationException(string message) : base(message)
        {
        }

        public InsufficientDataForUpdationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InsufficientDataForUpdationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Info = info;
            Context = context;
        }
    }
}