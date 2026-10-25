#pragma once

#include <algorithm>
#include <cstdint>
#include <string>
#include <unordered_map>
#include <vector>

namespace GoonzuGame::Economy {

using ItemId = std::uint32_t;
using PlayerId = std::uint64_t;
using TownId = std::uint32_t;

struct MarketListing {
    std::uint64_t id{};
    PlayerId seller{};
    ItemId item{};
    std::int64_t quantity{};
    std::int64_t unitPrice{};
    TownId town{};
};

struct MarketSnapshot {
    double supplyIndex{1.0};
    double demandIndex{1.0};
    double averagePrice{0.0};
};

class Market {
public:
    void AddListing(const MarketListing& listing) { listings_.push_back(listing); }

    [[nodiscard]] MarketSnapshot Snapshot(ItemId item) const {
        std::int64_t quantity = 0;
        long double value = 0.0;
        for (const auto& listing : listings_) {
            if (listing.item != item || listing.quantity <= 0 || listing.unitPrice < 0) continue;
            quantity += listing.quantity;
            value += static_cast<long double>(listing.quantity) * listing.unitPrice;
        }
        if (quantity == 0) return {};
        const double average = static_cast<double>(value / quantity);
        return {static_cast<double>(quantity), 1.0, average};
    }

    [[nodiscard]] std::vector<MarketListing> ListingsFor(ItemId item) const {
        std::vector<MarketListing> result;
        for (const auto& listing : listings_) if (listing.item == item) result.push_back(listing);
        return result;
    }

private:
    std::vector<MarketListing> listings_;
};

struct Recipe {
    ItemId output{};
    std::int32_t outputQuantity{1};
    std::unordered_map<ItemId, std::int32_t> inputs;
    std::string profession;
    std::int32_t requiredSkill{};
    double baseSuccess{1.0};
};

struct CraftResult {
    bool success{false};
    double chance{0.0};
    std::int32_t quantity{0};
};

class Crafting {
public:
    [[nodiscard]] CraftResult Calculate(const Recipe& recipe, std::int32_t skill,
                                        std::int32_t artisanBonus = 0,
                                        std::int32_t blessingBonus = 0) const {
        const double skillBonus = std::clamp((skill - recipe.requiredSkill) * 0.01, -0.50, 0.35);
        const double chance = std::clamp(recipe.baseSuccess + skillBonus +
                                         artisanBonus * 0.01 + blessingBonus * 0.01, 0.05, 0.99);
        return {chance >= 0.50, chance, chance >= 0.50 ? recipe.outputQuantity : 0};
    }
};

struct TownTreasury {
    TownId town{};
    std::int64_t balance{};
    double taxRate{0.05};
};

struct Election {
    TownId town{};
    std::uint64_t electionId{};
    bool open{false};
    std::unordered_map<PlayerId, std::int64_t> votes;
};

class Politics {
public:
    void Open(Election election) { election.open = true; elections_[election.town] = std::move(election); }

    void Vote(TownId town, PlayerId candidate, std::int64_t weight = 1) {
        auto it = elections_.find(town);
        if (it != elections_.end() && it->second.open && weight > 0) it->second.votes[candidate] += weight;
    }

    [[nodiscard]] PlayerId Winner(TownId town) const {
        auto it = elections_.find(town);
        if (it == elections_.end()) return 0;
        PlayerId winner = 0;
        std::int64_t best = -1;
        for (const auto& [candidate, votes] : it->second.votes) {
            if (votes > best || (votes == best && candidate < winner)) { winner = candidate; best = votes; }
        }
        return winner;
    }

private:
    std::unordered_map<TownId, Election> elections_;
};

} // namespace GoonzuGame::Economy
