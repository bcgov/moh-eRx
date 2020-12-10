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
    using System.Linq;

    public class Field : MessageElement
    {
        public Field(HL7Encoding encoding)
        {
            this.ComponentList = new ComponentCollection();
            this.Encoding = encoding;
        }

        public Field(string value, HL7Encoding encoding)
        {
            this.ComponentList = new ComponentCollection();
            this.Encoding = encoding;
            this.Value = value;
        }

        public bool IsComponentized { get; set; }

        public bool HasRepetitions { get; set; }

        public bool IsDelimiters { get; set; }

        internal ComponentCollection ComponentList { get; set; }

        internal IList<Field> RepetitionList { get; set; } = new List<Field>();

        public bool AddNewComponent(Component com)
        {
            try
            {
                this.ComponentList.Add(com);
                return true;
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Unable to add new component Error - " + ex.Message);
            }
        }

        public bool AddNewComponent(Component component, int position)
        {
            try
            {
                this.ComponentList.Add(component, position);
                return true;
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Unable to add new component Error - " + ex.Message);
            }
        }

        public Component Components(int position)
        {
            position = position - 1;

            try
            {
                return this.ComponentList[position];
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Component not available Error - " + ex.Message);
            }
        }

        public IList<Component> Components()
        {
            return this.ComponentList;
        }

        public IList<Field> Repetitions()
        {
            if (this.HasRepetitions)
            {
                return this.RepetitionList;
            }

            return null!;
        }

        public Field Repetitions(int repetitionNumber)
        {
            if (this.HasRepetitions)
            {
                return this.RepetitionList[repetitionNumber - 1];
            }

            return null!;
        }

        public bool RemoveEmptyTrailingComponents()
        {
            try
            {
                for (var eachComponent = this.ComponentList.Count - 1; eachComponent >= 0; eachComponent--)
                {
                    if (string.IsNullOrEmpty(this.ComponentList[eachComponent].Value))
                    {
                        this.ComponentList.Remove(this.ComponentList[eachComponent]);
                    }
                    else
                    {
                        break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new HL7Exception("Error removing trailing components - " + ex.Message);
            }
        }

        protected override void ProcessValue()
        {
            if (this.IsDelimiters)
            {
                // Special case for the delimiters fields (MSH)
                var subcomponent = new SubComponent(this.Value, this.Encoding!);

                this.ComponentList = new ComponentCollection();
                Component component = new Component(this.Encoding, true);

                component.SubComponentList.Add(subcomponent);

                this.ComponentList.Add(component);
                return;
            }

            this.HasRepetitions = this.Value.Contains(this.Encoding!.RepeatDelimiter, StringComparison.Ordinal);

            if (this.HasRepetitions)
            {
                this.RepetitionList = new List<Field>();
                IList<string> individualFields = MessageHelper.SplitString(this.Value, this.Encoding.RepeatDelimiter);

                for (int index = 0; index < individualFields.Count; index++)
                {
                    Field field = new Field(individualFields[index], this.Encoding);
                    this.RepetitionList.Add(field);
                }
            }
            else
            {
                IList<string> allComponents = MessageHelper.SplitString(this.Value, this.Encoding.ComponentDelimiter);

                this.ComponentList = new ComponentCollection();

                foreach (string strComponent in allComponents)
                {
                    Component component = new Component(this.Encoding);
                    component.Value = strComponent;
                    this.ComponentList.Add(component);
                }

                this.IsComponentized = this.ComponentList.Count > 1;
            }
        }
    }
}
