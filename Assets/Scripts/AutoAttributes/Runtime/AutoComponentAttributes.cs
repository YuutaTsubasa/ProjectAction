using System;
using UnityEngine;

namespace ProjectAction.AutoAttributes
{
    public enum AutoComponentLookupMode
    {
        GetComponent,
        GetComponentInParent,
        GetComponentInChildren,
        GetComponentsInChildren
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public abstract class AutoComponentAttribute : PropertyAttribute
    {
        protected AutoComponentAttribute(bool includeInactive)
        {
            IncludeInactive = includeInactive;
        }

        public bool IncludeInactive { get; }
        public abstract AutoComponentLookupMode Mode { get; }
    }

    public sealed class GetComponentAttribute : AutoComponentAttribute
    {
        public GetComponentAttribute() : base(false)
        {
        }

        public override AutoComponentLookupMode Mode => AutoComponentLookupMode.GetComponent;
    }

    public sealed class GetComponentInParentAttribute : AutoComponentAttribute
    {
        public GetComponentInParentAttribute() : base(false)
        {
        }

        public GetComponentInParentAttribute(bool includeInactive) : base(includeInactive)
        {
        }

        public override AutoComponentLookupMode Mode => AutoComponentLookupMode.GetComponentInParent;
    }

    public sealed class GetComponentInChildrenAttribute : AutoComponentAttribute
    {
        public GetComponentInChildrenAttribute() : base(false)
        {
        }

        public GetComponentInChildrenAttribute(bool includeInactive) : base(includeInactive)
        {
        }

        public override AutoComponentLookupMode Mode => AutoComponentLookupMode.GetComponentInChildren;
    }

    public sealed class GetComponentsInChildrenAttribute : AutoComponentAttribute
    {
        public GetComponentsInChildrenAttribute() : base(false)
        {
        }

        public GetComponentsInChildrenAttribute(bool includeInactive) : base(includeInactive)
        {
        }

        public override AutoComponentLookupMode Mode => AutoComponentLookupMode.GetComponentsInChildren;
    }
}
