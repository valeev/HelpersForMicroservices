using System.Collections.Generic;

namespace Infrastructure.Api
{
    public class LogsConfig
    {
        public int LogsMaximumLength { get; set; }
        public List<PathInfo> IgnoredPaths { get; set; }
    }
}
