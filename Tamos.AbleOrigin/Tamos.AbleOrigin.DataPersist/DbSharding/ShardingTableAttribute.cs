using System;

namespace Tamos.AbleOrigin.DataPersist
{
    public class ShardingTableAttribute : Attribute
    {
        public string Name { get; }

        public ShardingTableAttribute(string name)
        {
            Name = name;
        }
    }
}