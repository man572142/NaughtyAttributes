using System;

namespace NaughtyAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class LayerAttribute : DrawerAttribute
    {
        public bool UseFlags { get; private set; }

        public LayerAttribute(bool useFlags = true)
        {
            UseFlags = useFlags;
        }
    }
}
