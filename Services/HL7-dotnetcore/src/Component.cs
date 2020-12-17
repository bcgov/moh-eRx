/*-------------------------------------------------------------------------
  The MIT License (MIT)
  Copyright (c) 2013 Jayant Singh - www.j4jayant.com
  Copyright (c) 2019 Efferent Health, LLC
  Copyright (c) 2020 Province of British Columbia.
 -------------------------------------------------------------------------*/
namespace HL7.Dotnetcore
{
    using System;
    using System.Collections.Generic;

    public class Component : MessageElement
    {
        public Component(HL7Encoding encoding, bool isDelimiter = false)
        {
            this.IsDelimiter = isDelimiter;
            this.SubComponentList = new List<SubComponent>();
            this.Encoding = encoding;
        }

        public Component(string pValue, HL7Encoding encoding)
        {
            this.SubComponentList = new List<SubComponent>();
            this.Encoding = encoding;
            this.Value = pValue;
        }

        public bool IsSubComponentized { get; set; }

        internal List<SubComponent> SubComponentList { get; set; }

        private bool IsDelimiter { get; set; }

        public SubComponent SubComponents(int position)
        {
            position = position - 1;

            try
            {
                return this.SubComponentList[position];
            }
            catch (Exception ex)
            {
                throw new HL7Exception("SubComponent not available Error-" + ex.Message);
            }
        }

        public IList<SubComponent> SubComponents()
        {
            return this.SubComponentList;
        }

        protected override void ProcessValue()
        {
            IList<string> allSubComponents;

            if (this.IsDelimiter)
            {
                allSubComponents = new List<string>(new[] { this.Value });
            }
            else
            {
                allSubComponents = MessageHelper.SplitString(this.Value, this.Encoding!.SubComponentDelimiter);
            }

            if (allSubComponents.Count > 1)
            {
                this.IsSubComponentized = true;
            }

            this.SubComponentList = new List<SubComponent>();

            foreach (string strSubComponent in allSubComponents)
            {
                SubComponent subComponent = new SubComponent(this.Encoding!.Decode(strSubComponent), this.Encoding);
                this.SubComponentList.Add(subComponent);
            }
        }
    }
}