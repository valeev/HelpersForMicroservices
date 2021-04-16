using System.Collections.Generic;

namespace Infrastructure.Api
{
    public class LoggingConfiguration
    {
        public int LogsMaximumLength { get; set; }
        public List<PathInfo> IgnoredPaths { get; set; }
    }
}
