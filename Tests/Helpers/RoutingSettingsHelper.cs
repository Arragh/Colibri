using Colibri.Configuration;
using Colibri.Configuration.Models;

namespace Tests.Helpers;

public static class RoutingSettingsHelper
{
    public static RoutingSettings SingleCluster_SingleHost_SingleRoute_SimplePattern_NoParams()
    {
        return new RoutingSettings
        {
            Clusters =
            [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo:5000"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/test",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                }
            ]
        };
    }
}