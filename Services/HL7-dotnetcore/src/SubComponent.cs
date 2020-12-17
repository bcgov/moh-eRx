/*-------------------------------------------------------------------------
  The MIT License (MIT)
  Copyright (c) 2013 Jayant Singh - www.j4jayant.com
  Copyright (c) 2019 Efferent Health, LLC
  Copyright (c) 2020 Province of British Columbia.
 -------------------------------------------------------------------------*/
namespace HL7.Dotnetcore
{
    public class SubComponent : MessageElement
    {
        public SubComponent(string val, HL7Encoding encoding)
        {
            this.Encoding = encoding;
            this.Value = val;
        }

        protected override void ProcessValue()
        {
        }
    }
}
