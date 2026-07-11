### **Slide 1: Title Slide**

* **Modern Distributed Applications with Aspire**
* **Cloud-Native Development Simplified**
* **Focus:** Architecture, Development Experience, AI Agents, and the Production Journey

---

### **Slide 2: Why Do We Need Aspire?**

* Setting up microservices usually requires many complex tools like Docker Compose.
* Developers waste time fixing ports and connections instead of writing business code.
* Aspire fixes this by setting up orchestration, service discovery, and monitoring out of the box.
* The main goal is to make building distributed systems feel as simple as building a single app.

---

### **Slide 3: What is Aspire?**

* It is an opinionated stack for building observable, production-ready applications.
* It is built on top of .NET, open-source tracking tools like OpenTelemetry, and cloud-native principles.
* It provides automatic dashboards, service finding, and infrastructure provisioning.
* It works perfectly with containers, APIs, workers, databases, and AI models.

---

### **Slide 4: Main Building Blocks**

* **AppHost:** The orchestrator that manages the distributed application.
* **ServiceDefaults:** Shared configuration and observability setup.
* **Dashboard:** A central place for logs, metrics, and traces.
* **Service Discovery:** Automatic endpoint resolution so services can find each other.
* **Integrations:** Connects easily to SQL Server, Redis, RabbitMQ, PostgreSQL, and more.

---

### **Slide 5: AppHost (The Starting Point)**

* It acts as the entry point for your whole system.
* It defines all your projects, containers, and infrastructure dependencies.
* It controls the exact startup order using `WaitFor()`.
* It automatically injects configuration and connection information.
* It provides a single-command startup for the entire application.

---

### **Slide 6: AppHost Code Example**

* Here is how simple it is to build and connect the application:

```csharp
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
var rabbitmq = builder.AddRabbitMQ("messaging").WithManagementPlugin();

// ──────────────────────────────────────────────
// Microservices
// ──────────────────────────────────────────────
var catalogService = builder.AddProject<Projects.AspireECommerceDemo_CatalogService>("catalogservice")
    .WithHttpEndpoint().WithReference(catalogDb).WithReference(redis)
    .WaitFor(catalogDb).WaitFor(redis);

var orderService = builder.AddProject<Projects.AspireECommerceDemo_OrderService>("orderservice")
    .WithHttpEndpoint().WithReference(orderDb).WithReference(rabbitmq)
    .WaitFor(orderDb).WaitFor(rabbitmq);

var paymentService = builder.AddProject<Projects.AspireECommerceDemo_PaymentService>("paymentservice")
    .WithHttpEndpoint().WithReference(paymentDb).WithReference(rabbitmq)
    .WaitFor(paymentDb).WaitFor(rabbitmq);

var notificationService = builder.AddProject<Projects.AspireECommerceDemo_NotificationService>("notificationservice")
    .WithHttpEndpoint().WithReference(notificationDb).WithReference(rabbitmq)
    .WaitFor(notificationDb).WaitFor(rabbitmq);

// API Gateway
var gateway = builder.AddProject<Projects.AspireECommerceDemo_ApiGateway>("apigateway")
    .WithReference(catalogService).WithReference(orderService)
    .WithReference(paymentService).WithReference(notificationService).WithReference(redis)
    .WaitFor(catalogService).WaitFor(orderService).WaitFor(paymentService)
    .WaitFor(notificationService).WaitFor(redis);

// Web Frontend
builder.AddProject<Projects.AspireECommerceDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints().WithReference(gateway).WaitFor(gateway);

builder.Build().Run();
```

---

### **Slide 7: Shared Settings (ServiceDefaults)**

* It gives every service centralized OpenTelemetry configuration automatically.
* It registers health checks so you know if a service goes down.
* It provides resilience defaults to ensure consistency across all services.
* It completely removes repetitive boilerplate configuration.

---

### **Slide 8: Finding Services Automatically (Service Discovery)**

* Services talk to each other using logical names rather than ports.
* Example: Use `http://catalogservice` instead of `localhost:5031`.
* Aspire automatically resolves addresses and injects endpoints.
* This makes your applications completely environment independent.

---

### **Slide 9: The Aspire Dashboard**

* It is a single place for observing the entire application.
* It provides clear visibility into logs, metrics, and distributed traces.
* It allows you to debug cross-service requests and see container resource usage.
* It eliminates the need for multiple monitoring tools during development.

---

### **Slide 10: Tracking Requests (Distributed Tracing)**

* A single user request generates spans that can be tracked across all services.
* Example Flow: Web -> API Gateway -> Order Service -> RabbitMQ -> Notification Service.
* OpenTelemetry automatically propagates the trace context.
* Developers can identify bottlenecks within seconds.

---

### **Slide 11: Easy Infrastructure Integrations**

* SQL Server integration sets up automatic connection strings.
* Redis integration handles caching and distributed state.
* RabbitMQ integration manages event-driven communication safely.
* All these containers are provisioned and started automatically by the AppHost.

---

### **Slide 12: How Developers Work Now (The Workflow)**

* 1. Run the AppHost.
* 2. Infrastructure containers start automatically.
* 3. Services start in their exact dependency order.
* 4. The Dashboard opens automatically.
* 5. Developers can start coding immediately.

---

### **Slide 13: Aspire and AI (LLMs & Agents)**

* Aspire makes it incredibly easy to add Artificial Intelligence components to your applications.
* It sets up secure connections to cloud AI models or locally hosted open-source LLMs.
* You can seamlessly run vector databases (like Qdrant, Milvus, or Redis) directly alongside your code.
* It manages your AI API keys and connection settings safely so nothing is leaked.

---

### **Slide 14: How Aspire Helps AI Developers**

* **Tracking Tokens:** You can see exactly how many tokens your LLM prompts use directly on the dashboard.
* **Speed Monitoring:** It tracks latency to show exactly how fast the LLM replies to user requests.
* **AI Agents:** You can easily build AI Agents that communicate directly with your microservices (e.g., querying the catalog or placing orders).
* If an AI Agent hallucinates or fails a task, the Aspire dashboard distributed tracing helps you find exactly where the logic broke down.

---

### **Slide 15: Aspire vs. Docker Compose**

* Docker Compose manages containers only.
* Aspire understands applications, services, and their deep dependencies.
* Aspire automatically provides observability, service discovery, and injects configuration.
* Docker Compose and Aspire can still coexist in production scenarios.

---

### **Slide 16: Good for Any App Size**

* **For Monoliths:** Useful even for modular monoliths. It can orchestrate APIs, caches, and databases while providing observability without microservices complexity.
* **For Microservices & AI:** Designed specifically for distributed systems, supporting service boundaries and independent deployments. It reduces onboarding time for new developers significantly.

---

### **Slide 17: Moving to Real Servers (Production Story)**

* Aspire is primarily a development orchestration platform.
* Applications can be easily deployed to Azure Container Apps, Kubernetes, or traditional containers.
* OpenTelemetry exporters send telemetry data directly to Azure Monitor, Grafana, Jaeger, or Prometheus.
* The exact same application architecture works locally and in production.

---

### **Slide 18: Best Practices to Follow**

* Use one database per microservice.
* Use asynchronous communication for long-running workflows.
* Declare every dependency clearly using `WithReference()`.
* Keep all infrastructure configuration inside the AppHost.
* Enable tracing and metrics from day one.

---

### **Slide 19: Main Benefits (Key Takeaways)**

* Aspire dramatically improves the developer experience.
* Distributed applications become much easier to run and debug.
* Observability becomes a default feature rather than an afterthought.
* The platform removes operational complexity from local development.
* Developers can focus entirely on business problems instead of infrastructure setup.

---

### **Slide 20: Questions & Discussion**

* Thank you for attending the session.
* **Discussion topics:** Adoption strategy, AI integrations, migration paths, and production deployment options.
