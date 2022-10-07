using UnityEngine;

namespace UnityArtNetDemo.Attributes
{
    public class IPAttribute : PropertyAttribute
    {
        public string Prefix { get; }

        public IPAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}