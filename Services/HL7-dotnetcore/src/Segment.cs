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

    public class Segment : MessageElement
    {
        public Segment(HL7Encoding encoding)
        {
            this.FieldList = new FieldCollection();
            this.Encoding = encoding;
            this.Name = string.Empty;
        }

        public Segment(string name, HL7Encoding encoding)
        {
            this.FieldList = new FieldCollection();
            this.Name = name;
            this.Encoding = encoding;
        }

        public string Name { get; set; }

        public IList<Field> Fields
        {
            get
            {
                return this.FieldList;
            }
        }

        internal FieldCollection FieldList { get; set; }

        internal short SequenceNo { get; set; }

        public Segment DeepCopy()
        {
            var newSegment = new Segment(this.Name, this.Encoding!);
            newSegment.Value = this.Value;

            return newSegment;
        }

        public void AddEmptyField()
        {
            this.AddNewField(string.Empty);
        }

        public void AddNewField(string content, int position = -1)
        {
            this.AddNewField(new Field(content, this.Encoding!), position);
        }

        public void AddNewField(string content, bool isDelimiters)
        {
            var newField = new Field(this.Encoding!);

            if (isDelimiters)
            {
                newField.IsDelimiters = true; // Prevent decoding
            }

            newField.Value = content;
            this.AddNewField(newField, -1);
        }

        public bool AddNewField(Field field, int position = -1)
        {
            try
            {
                if (position < 0)
                {
                    this.FieldList.Add(field);
                }
                else
                {
                    position = position - 1;
                    this.FieldList.Add(field, position);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Unable to add new field in segment " + this.Name + " Error - " + ex.Message);
            }
        }

        public Field FieldAt(int position)
        {
            position = position - 1;

            try
            {
                return this.FieldList[position];
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Field not available Error - " + ex.Message);
            }
        }

        protected override void ProcessValue()
        {
            IList<string> allFields = MessageHelper.SplitString(this.Value, this.Encoding!.FieldDelimiter);

            allFields.RemoveAt(0);

            for (int i = 0; i < allFields.Count; i++)
            {
                string strField = allFields[i];
                Field field = new Field(this.Encoding);

                if (this.Name == "MSH" && i == 0)
                {
                    field.IsDelimiters = true; // special case
                }

                field.Value = strField;
                this.FieldList.Add(field);
            }

            if (this.Name == "MSH")
            {
                var field1 = new Field(this.Encoding);
                field1.IsDelimiters = true;
                field1.Value = this.Encoding.FieldDelimiter.ToString();

                this.FieldList.Insert(0, field1);
            }
        }
    }
}
