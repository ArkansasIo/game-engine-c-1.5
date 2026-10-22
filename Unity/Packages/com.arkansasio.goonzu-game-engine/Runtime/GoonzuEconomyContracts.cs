using System;

namespace ArkansasIo.GoonzuGameEngine;

/// <summary>Transport-neutral request for a server-authoritative market purchase.</summary>
public readonly record struct GoonzuPurchaseRequest(
    long ListingId,
    long BuyerId,
    long Quantity,
    string IdempotencyKey);

/// <summary>Result returned by an authoritative economy implementation.</summary>
public readonly record struct GoonzuPurchaseResult(
    bool Success,
    string ErrorCode,
    long RemainingListingQuantity,
    long CurrencyCharged);

/// <summary>Boundary used by Unity UI/gameplay code for local or remote economy implementations.</summary>
public interface IGoonzuEconomyGateway
{
    GoonzuPurchaseResult Purchase(in GoonzuPurchaseRequest request);
}

public static class GoonzuEconomyErrors
{
    public const string InvalidRequest = "invalid_request";
    public const string ListingNotFound = "listing_not_found";
    public const string InsufficientStock = "insufficient_stock";
    public const string InsufficientFunds = "insufficient_funds";
    public const string DuplicateRequest = "duplicate_request";
    public const string ServerRejected = "server_rejected";
}
