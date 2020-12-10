/*-------------------------------------------------------------------------
  The MIT License (MIT)
  Copyright (c) 2013 Jayant Singh - www.j4jayant.com
  Copyright (c) 2019 Efferent Health, LLC
  Copyright (c) 2020 Province of British Columbia.
 -------------------------------------------------------------------------*/
namespace HL7.Dotnetcore
{
    using System.Collections.Generic;

    internal class ComponentCollection : List<Component>
    {
        /// <summary>
        /// Component indexer
        /// </summary>
        /// <param name="index">The index into this collection</param>
        internal new Component this[int index]
        {
            get
            {
                Component? component = null;

                if (index < this.Count)
                {
                    component = base[index];
                }

                return component!;
            }

            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Add Component at next position
        /// </summary>
        /// <param name="component">Component</param>
        internal new void Add(Component component)
        {
            base.Add(component);
        }

        /// <summary>
        /// Add component at specific position
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="position">Position</param>
        internal void Add(Component component, int position)
        {
            int listCount = this.Count;
            position = position - 1;

            if (position < listCount)
            {
                base[position] = component;
            }
            else
            {
                for (int comIndex = listCount; comIndex < position; comIndex++)
                {
                    Component blankComponent = new Component(component.Encoding);
                    blankComponent.Value = string.Empty;
                    base.Add(blankComponent);
                }

                base.Add(component);
            }
        }
    }
}
#pragma warning restore SA1633, SA1636
