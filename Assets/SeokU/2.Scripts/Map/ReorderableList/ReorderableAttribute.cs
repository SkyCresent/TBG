using UnityEngine;
using System;

namespace Reorderable
{
    public class ReorderableAttribute : PropertyAttribute
    {
        public bool add;
        public bool remove;
        public bool draggable;              //드래그 가능
        public bool singleLine;
        public bool paginate;               //페이지 매기다
        public bool sortable;               //정렬 가능
        public int pageSize;
        public string elementNameProperty; 
        public string elementNameOverride;
        public string elementIconPath;
        public Type surrogateType;          //surrogate : 대리
        public string surrogateProperty;
		public ReorderableAttribute()
			: this(null)
		{
		}

		public ReorderableAttribute(string elementNameProperty)
			: this(true, true, true, elementNameProperty, null, null)
		{
		}

		public ReorderableAttribute(string elementNameProperty, string elementIconPath)
			: this(true, true, true, elementNameProperty, null, elementIconPath)
		{
		}

		public ReorderableAttribute(string elementNameProperty, string elementNameOverride, string elementIconPath)
			: this(true, true, true, elementNameProperty, elementNameOverride, elementIconPath)
		{
		}

		public ReorderableAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null, string elementIconPath = null)
			: this(add, remove, draggable, elementNameProperty, null, elementIconPath)
		{
		}

		public ReorderableAttribute(bool add, bool remove, bool draggable, string elementNameProperty = null, string elementNameOverride = null, string elementIconPath = null)
		{

			this.add = add;
			this.remove = remove;
			this.draggable = draggable;
			this.elementNameProperty = elementNameProperty;
			this.elementNameOverride = elementNameOverride;
			this.elementIconPath = elementIconPath;

			sortable = true;
		}
	}
}

