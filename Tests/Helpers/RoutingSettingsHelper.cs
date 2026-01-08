using Colibri.Configuration;
using Colibri.Configuration.Models;

namespace Tests.Helpers;

public static class RoutingSettingsHelper
{
    public static RoutingSettings MultipleClusters_MultipleRoutes()
    {
        return new RoutingSettings
        {
            Clusters =
            [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo1:5000",
                        "http://trololo1:5001"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/users",
                            DownstreamPattern = "/internal/users"
                        },
                        new RouteDto
                        {
                            Method = "POST",
                            UpstreamPattern = "/api/account",
                            DownstreamPattern = "/internal/account"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/users/{id}/info",
                            DownstreamPattern = "/internal/users/info?id={id}"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/users/new",
                            DownstreamPattern = "/internal/users/new"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/users/{id}",
                            DownstreamPattern = "/internal/users/{id}"
                        },
                        new RouteDto
                        {
                            Method = "DELETE",
                            UpstreamPattern = "/api/account/{login}/delete",
                            DownstreamPattern = "/internal/account/{login}/del"
                        },
                    ]
                },
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo2:5000"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/catalog",
                            DownstreamPattern = "/catalog"
                        },
                        new RouteDto
                        {
                            Method = "POST",
                            UpstreamPattern = "/catalog/remove",
                            DownstreamPattern = "/catalog/all"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/cart",
                            DownstreamPattern = "/shop/cart"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/cart/clear",
                            DownstreamPattern = "/shop/cart/clear"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/profile/{longParameter}/lol",
                            DownstreamPattern = "/internal/profile?id={id}"
                        },
                        new RouteDto
                        {
                            Method = "PATCH",
                            UpstreamPattern = "/profile/me/edit",
                            DownstreamPattern = "/internal/profile/{id}/edit"
                        },
                        
                    ]
                },
                new ClusterDto // Кластер для тестов RoutingEngine
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://escapepod:5000",
                        "http://escapepod:5001"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "POST",
                            UpstreamPattern = "/tests/users",
                            DownstreamPattern = "/internal/users"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/tests/users/{name}/info",
                            DownstreamPattern = "/profile/{login}/info"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/tests/users/me",
                            DownstreamPattern = "/api/clients"
                        }
                    ]
                },
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo3:5000",
                        "http://trololo3:5001"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "POST",
                            UpstreamPattern = "/api/register",
                            DownstreamPattern = "/internal/register"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/profile/{login}/info",
                            DownstreamPattern = "/profile/{login}/info"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/clients",
                            DownstreamPattern = "/api/clients"
                        },
                        new RouteDto
                        {
                            Method = "PUT",
                            UpstreamPattern = "/clients/{id}/edit",
                            DownstreamPattern = "/api/clients/{id}/edit"
                        }
                    ]
                }
            ]
        };
    }
}