using System;
using System.Runtime.Serialization;

namespace API.Extensions.CustomExceptions
{
    public class CodeNotFoundException : Exception
    {
        public SerializationInfo Info { get; set; }
        public StreamingContext Context { get; set; }
        
        public CodeNotFoundException()
        {
        }

        public CodeNotFoundException(string message) : base(message)
        {
        }

        public CodeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CodeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Info = info;
            Context = context;
        }
    }
}