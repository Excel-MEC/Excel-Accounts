using System;
using System.Runtime.Serialization;

namespace API.Extensions.CustomExceptions
{
    public class OneTimeUseException : Exception
    {
        public SerializationInfo Info { get; set; }
        public StreamingContext Context { get; set; }
        
        public OneTimeUseException()
        {
        }

        public OneTimeUseException(string message) : base(message)
        {
        }

        public OneTimeUseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OneTimeUseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Info = info;
            Context = context;
        }
    }
}