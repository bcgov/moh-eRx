/*-------------------------------------------------------------------------
  The MIT License (MIT)
  Copyright (c) 2013 Jayant Singh - www.j4jayant.com
  Copyright (c) 2019 Efferent Health, LLC
  Copyright (c) 2020 Province of British Columbia.
 -------------------------------------------------------------------------*/
namespace HL7.Dotnetcore
{
    using System;

    public class HL7Exception : Exception
    {
#pragma warning disable CA1707
        public const string REQUIRED_FIELD_MISSING = "Validation Error - Required field missing in message";
        public const string UNSUPPORTED_MESSAGE_TYPE = "Validation Error - Message Type not supported by this implementation";
        public const string BAD_MESSAGE = "Validation Error - Bad Message";
        public const string PARSING_ERROR = "Parsing Error";
        public const string SERIALIZATION_ERROR = "Serialization Error";
#pragma warning restore CA1707

        public HL7Exception()
        : base()
        {
            this.ErrorCode = null;
        }

        public HL7Exception(string message)
        : base(message)
        {
            this.ErrorCode = null;
        }

        public HL7Exception(string message, Exception innerException)
        : base(message, innerException)
        {
            this.ErrorCode = null;
        }

        public HL7Exception(string message, string code)
        : this(message)
        {
            this.ErrorCode = code;
        }

        public string? ErrorCode { get; set; }

        public override string ToString()
        {
            return this.ErrorCode + " : " + this.Message;
        }
    }
}