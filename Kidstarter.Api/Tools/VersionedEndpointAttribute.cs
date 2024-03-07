using System;

using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

namespace Kidstarter.Api.Tools
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VersionedEndpointAttribute : Attribute
    {
        public VersionedEndpointAttribute(string prefix)
        {
            Guard.ArgumentNotNullOrEmpty(prefix, nameof(prefix));

            this.Prefix = prefix;
        }

        public VersionedEndpointAttribute()
        {
        }

        public string Prefix { get; }
    }
}