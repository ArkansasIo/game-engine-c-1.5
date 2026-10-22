#pragma once
#include <algorithm>
#include <cstdint>
#include <string>
#include <unordered_map>
#include <vector>

namespace GoonzuGame {

struct Stats {
    int strength{1};
    int agility{1};
    int vitality{1};
    int wisdom{1};
    int dexterity{1};
    int luck{1};
};

struct Skill {
    std::string id;
    std::string name;
    int basicLevel{0};
    int experienceLevel{0};
    int totalLevel() const { return basicLevel + experienceLevel; }
};

struct Recipe {
    std::string id;
    std::string outputItemId;
    std::string skillId;
    int difficulty{1};
    std::unordered_map<std::string, int> materials;
};

struct MarketListing {
    std::uint64_t id{0};
    std::string sellerId;
    std::string itemId;
    std::int64_t unitPrice{0};
    std::int32_t quantity{0};
    std::int32_t townId{0};
};

class EconomySystem {
public:
    static double PriceMultiplier(double demand, double supply, double region, double tax) {
        const auto d = std::clamp(1.0 + demand, 0.25, 4.0);
        const auto s = std::clamp(1.0 - supply, 0.25, 2.0);
        const auto r = std::clamp(region, 0.50, 2.0);
        const auto t = std::clamp(1.0 + tax, 1.0, 2.0);
        return d * s * r * t;
    }

    void AddListing(MarketListing listing) { listings_.push_back(std::move(listing)); }
    const std::vector<MarketListing>& Listings() const { return listings_; }

private:
    std::vector<MarketListing> listings_;
};

class CraftingSystem {
public:
    static double SuccessChance(int skillLevel, int difficulty) {
        if (skillLevel >= difficulty + 5) return 1.0;
        return std::clamp(0.50 + ((skillLevel - difficulty) * 0.10), 0.05, 1.0);
    }
};

struct TownGovernment {
    std::int32_t townId{0};
    std::string name;
    std::string leaderPlayerId;
    std::int64_t treasury{0};
    double salesTax{0.0};
};

struct Election {
    std::string id;
    std::string office;
    std::int64_t opensAtUnix{0};
    std::int64_t closesAtUnix{0};
    std::unordered_map<std::string, std::int32_t> votes;
};

class PoliticalSystem {
public:
    void AddTown(TownGovernment town) { towns_.push_back(std::move(town)); }
    void AddElection(Election election) { elections_.push_back(std::move(election)); }

    const std::vector<TownGovernment>& Towns() const { return towns_; }
    const std::vector<Election>& Elections() const { return elections_; }

private:
    std::vector<TownGovernment> towns_;
    std::vector<Election> elections_;
};

} // namespace GoonzuGame
