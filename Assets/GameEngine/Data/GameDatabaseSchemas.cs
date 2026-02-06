namespace GameEngine.Data
{
    public static class GameDatabaseSchemas
    {
        public const string Players = @"CREATE TABLE IF NOT EXISTS Players (
            Id TEXT PRIMARY KEY,
            Name TEXT,
            Level INT,
            Exp INT,
            HP INT,
            MP INT,
            Gold INT
        );";

        public const string Inventory = @"CREATE TABLE IF NOT EXISTS Inventory (
            PlayerId TEXT,
            ItemId TEXT,
            Count INT,
            PRIMARY KEY (PlayerId, ItemId)
        );";

        public const string Items = @"CREATE TABLE IF NOT EXISTS Items (
            ItemId TEXT PRIMARY KEY,
            Name TEXT,
            Type TEXT,
            Rarity TEXT,
            Description TEXT
        );";

        public const string Skills = @"CREATE TABLE IF NOT EXISTS Skills (
            SkillId TEXT PRIMARY KEY,
            Name TEXT,
            Description TEXT,
            MaxLevel INT
        );";

        public const string PlayerSkills = @"CREATE TABLE IF NOT EXISTS PlayerSkills (
            PlayerId TEXT,
            SkillId TEXT,
            Level INT,
            PRIMARY KEY (PlayerId, SkillId)
        );";

        public const string Quests = @"CREATE TABLE IF NOT EXISTS Quests (
            QuestId TEXT PRIMARY KEY,
            Title TEXT,
            Description TEXT,
            RewardGold INT
        );";

        public const string PlayerQuests = @"CREATE TABLE IF NOT EXISTS PlayerQuests (
            PlayerId TEXT,
            QuestId TEXT,
            Status TEXT,
            PRIMARY KEY (PlayerId, QuestId)
        );";

        public const string CraftingRecipes = @"CREATE TABLE IF NOT EXISTS CraftingRecipes (
            RecipeId TEXT PRIMARY KEY,
            Name TEXT,
            OutputItemId TEXT,
            OutputCount INT
        );";

        public const string MarketListings = @"CREATE TABLE IF NOT EXISTS MarketListings (
            ListingId TEXT PRIMARY KEY,
            SellerId TEXT,
            ItemId TEXT,
            Price INT,
            Count INT
        );";

        public const string Parties = @"CREATE TABLE IF NOT EXISTS Parties (
            PartyId TEXT PRIMARY KEY,
            LeaderId TEXT
        );";

        public const string PartyMembers = @"CREATE TABLE IF NOT EXISTS PartyMembers (
            PartyId TEXT,
            PlayerId TEXT,
            PRIMARY KEY (PartyId, PlayerId)
        );";
    }
}
