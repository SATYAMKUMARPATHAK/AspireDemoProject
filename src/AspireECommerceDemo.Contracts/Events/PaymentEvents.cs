namespace AspireECommerceDemo.Contracts.Events;

public record PaymentSucceeded(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string TransactionId,
    DateTime ProcessedAt);

public record PaymentFailed(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string Reason,
    DateTime FailedAt);
