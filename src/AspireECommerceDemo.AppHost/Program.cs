var builder = DistributedApplication.CreateBuilder(args);

// ──────────────────────────────────────────────
// Infrastructure Resources
// ──────────────────────────────────────────────

var sql = builder.AddSqlServer("sql");

var catalogDb = sql.AddDatabase("catalogdb");
var orderDb = sql.AddDatabase("orderdb");
var paymentDb = sql.AddDatabase("paymentdb");
var notificationDb = sql.AddDatabase("notificationdb");

var redis = builder.AddRedis("redis");

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin();


// ──────────────────────────────────────────────
// Microservices
// ──────────────────────────────────────────────

// Catalog Service — product browsing, search, caching
var catalogService = builder.AddProject<Projects.AspireECommerceDemo_CatalogService>("catalogservice")
    .WithHttpEndpoint()
    .WithReference(catalogDb)
    .WithReference(redis)
    .WaitFor(catalogDb)
    .WaitFor(redis);

// Order Service — order placement, saga orchestration
var orderService = builder.AddProject<Projects.AspireECommerceDemo_OrderService>("orderservice")
    .WithHttpEndpoint()
    .WithReference(orderDb)
    .WithReference(rabbitmq)
    .WaitFor(orderDb)
    .WaitFor(rabbitmq);

// Payment Service — payment processing, event publishing
var paymentService = builder.AddProject<Projects.AspireECommerceDemo_PaymentService>("paymentservice")
    .WithHttpEndpoint()
    .WithReference(paymentDb)
    .WithReference(rabbitmq)
    .WaitFor(paymentDb)
    .WaitFor(rabbitmq);

// Notification Service — email/SMS via event consumers
var notificationService = builder.AddProject<Projects.AspireECommerceDemo_NotificationService>("notificationservice")
    .WithHttpEndpoint()
    .WithReference(notificationDb)
    .WithReference(rabbitmq)
    .WaitFor(notificationDb)
    .WaitFor(rabbitmq);

// API Gateway — YARP-based reverse proxy + rate limiting
var gateway = builder.AddProject<Projects.AspireECommerceDemo_ApiGateway>("apigateway")
    .WithReference(catalogService)
    .WithReference(orderService)
    .WithReference(paymentService)
    .WithReference(notificationService)
    .WithReference(redis)
    .WaitFor(catalogService)
    .WaitFor(orderService)
    .WaitFor(paymentService)
    .WaitFor(notificationService)
    .WaitFor(redis);

// Web Frontend — Blazor interactive UI
builder.AddProject<Projects.AspireECommerceDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(gateway)
    .WaitFor(gateway);

builder.Build().Run();
