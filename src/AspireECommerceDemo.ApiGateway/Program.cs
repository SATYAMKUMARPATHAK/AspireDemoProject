using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisClient("redis");

builder.Services.AddReverseProxy()
    .LoadFromMemory(
        routes: [
            new RouteConfig {
                RouteId = "catalog-route",
                ClusterId = "catalog-cluster",
                Match = new RouteMatch { Path = "/api/products/{**catch-all}" }
            },
            new RouteConfig {
                RouteId = "categories-route",
                ClusterId = "catalog-cluster",
                Match = new RouteMatch { Path = "/api/categories/{**catch-all}" }
            },
            new RouteConfig {
                RouteId = "orders-route",
                ClusterId = "orders-cluster",
                Match = new RouteMatch { Path = "/api/orders/{**catch-all}" }
            },
            new RouteConfig {
                RouteId = "payments-route",
                ClusterId = "payments-cluster",
                Match = new RouteMatch { Path = "/api/payments/{**catch-all}" }
            },
            new RouteConfig {
                RouteId = "notifications-route",
                ClusterId = "notifications-cluster",
                Match = new RouteMatch { Path = "/api/notifications/{**catch-all}" }
            }
        ],
        clusters: [
            new ClusterConfig {
                ClusterId = "catalog-cluster",
                Destinations = new Dictionary<string, DestinationConfig> {
                    { "destination1", new DestinationConfig { Address = "http://catalogservice" } }
                }
            },
            new ClusterConfig {
                ClusterId = "orders-cluster",
                Destinations = new Dictionary<string, DestinationConfig> {
                    { "destination1", new DestinationConfig { Address = "http://orderservice" } }
                }
            },
            new ClusterConfig {
                ClusterId = "payments-cluster",
                Destinations = new Dictionary<string, DestinationConfig> {
                    { "destination1", new DestinationConfig { Address = "http://paymentservice" } }
                }
            },
            new ClusterConfig {
                ClusterId = "notifications-cluster",
                Destinations = new Dictionary<string, DestinationConfig> {
                    { "destination1", new DestinationConfig { Address = "http://notificationservice" } }
                }
            }
        ]
    )
    .AddServiceDiscoveryDestinationResolver();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Correlation ID middleware
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("X-Correlation-ID"))
    {
        context.Request.Headers["X-Correlation-ID"] = Guid.NewGuid().ToString();
    }
    
    var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-Correlation-ID"] = correlationId;
        return Task.CompletedTask;
    });
    
    await next();
});

app.UseCors();
app.MapReverseProxy();

app.Run();
