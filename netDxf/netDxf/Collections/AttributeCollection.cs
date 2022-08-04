namespace netDxf.Collections
{
    using netDxf.Entities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class AttributeCollection : IReadOnlyList<netDxf.Entities.Attribute>, IReadOnlyCollection<netDxf.Entities.Attribute>, IEnumerable<netDxf.Entities.Attribute>, IEnumerable
    {
        private readonly List<netDxf.Entities.Attribute> innerArray;

        public AttributeCollection()
        {
            this.innerArray = new List<netDxf.Entities.Attribute>();
        }

        public AttributeCollection(IEnumerable<netDxf.Entities.Attribute> attributes)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }
            this.innerArray = new List<netDxf.Entities.Attribute>(attributes);
        }

        public netDxf.Entities.Attribute AttributeWithTag(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                foreach (netDxf.Entities.Attribute attribute2 in this.innerArray)
                {
                    if ((attribute2.Definition > null) && string.Equals(tag, attribute2.Tag, StringComparison.OrdinalIgnoreCase))
                    {
                        return attribute2;
                    }
                }
            }
            return null;
        }

        public bool Contains(netDxf.Entities.Attribute item) => 
            this.innerArray.Contains(item);

        public void CopyTo(netDxf.Entities.Attribute[] array, int arrayIndex)
        {
            this.innerArray.CopyTo(array, arrayIndex);
        }

        public IEnumerator<netDxf.Entities.Attribute> GetEnumerator() => 
            this.innerArray.GetEnumerator();

        public int IndexOf(netDxf.Entities.Attribute item) => 
            this.innerArray.IndexOf(item);

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public int Count =>
            this.innerArray.Count;

        public static bool IsReadOnly =>
            true;

        public netDxf.Entities.Attribute this[int index] =>
            this.innerArray[index];
    }
}

