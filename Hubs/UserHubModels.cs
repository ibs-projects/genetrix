using System.Collections.Generic;

namespace genetrix.Hubs
{
    internal class UserHubModels
    {
        public string UserName { get; internal set; }
        public HashSet<string> ConnectionIds { get; internal set; }
    }
}