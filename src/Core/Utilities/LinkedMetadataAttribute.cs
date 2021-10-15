using System;
using System.Runtime.CompilerServices;

namespace Bit.Core.Utilities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LinkedMetadataAttribute : Attribute
    {
        public int Id { get; set; }
        public string I18nKey { get; set; }

        public LinkedMetadataAttribute(int id, string i18nKey = null)
        {
            Id = id;
            I18nKey = i18nKey;
        }
    }
}
