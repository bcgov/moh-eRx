/*-------------------------------------------------------------------------
  The MIT License (MIT)
  Copyright (c) 2013 Jayant Singh - www.j4jayant.com
  Copyright (c) 2019 Efferent Health, LLC
  Copyright (c) 2020 Province of British Columbia.
 -------------------------------------------------------------------------*/
namespace HL7.Dotnetcore
{
    using System.Collections.Generic;

    internal class FieldCollection : List<Field>
    {
        internal new Field this[int index]
        {
            get
            {
                Field? field = null;

                if (index < this.Count)
                {
                    field = base[index];
                }

                return field!;
            }

            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// add field at next position
        /// </summary>
        /// <param name="field">Field</param>
        internal new void Add(Field field)
        {
            base.Add(field);
        }

        /// <summary>
        /// Add field at specific position
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="position">position</param>
        internal void Add(Field field, int position)
        {
            int listCount = this.Count;

            if (position < listCount)
            {
                base[position] = field;
            }
            else
            {
                for (int fieldIndex = listCount; fieldIndex < position; fieldIndex++)
                {
                    Field blankField = new Field(string.Empty, field.Encoding);
                    base.Add(blankField);
                }

                base.Add(field);
            }
        }
    }
}