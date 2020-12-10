/*-------------------------------------------------------------------------
  The MIT License (MIT)
  Copyright (c) 2013 Jayant Singh - www.j4jayant.com
  Copyright (c) 2019 Efferent Health, LLC
  Copyright (c) 2020 Province of British Columbia.
 -------------------------------------------------------------------------*/
namespace HL7.Dotnetcore
{
    public abstract class MessageElement
    {
        private string mValue = string.Empty;

        public string Value
        {
            get
            {
                return this.mValue == this.Encoding!.PresentButNull ? string.Empty : this.mValue;
            }

            set
            {
                this.mValue = value;
                this.ProcessValue();
            }
        }

        public HL7Encoding Encoding { get; protected set; } = new HL7Encoding();

        protected abstract void ProcessValue();
    }
}