using System;
using System.Runtime.Serialization;

namespace API.Extensions.CustomExceptions
{
    public class AlreadyExistException : Exception
    {
        public SerializationInfo Info { get; set; }
        public StreamingContext Context { get; set; }
        
        public AlreadyExistException()
        {
        }

        public AlreadyExistException(string message) : base(message)
        {
        }

        public AlreadyExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Info = info;
            Context = context;
        }
    }
}