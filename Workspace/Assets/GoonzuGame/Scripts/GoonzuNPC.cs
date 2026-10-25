using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    [System.Serializable]
    public class NPCDialogue
    {
        public string greeting;
        public string[] randomLines;
        public string farewell;
        public Dictionary<string, string> questDialogues = new Dictionary<string, string>();
        public Dictionary<string, string> tradeDialogues = new Dictionary<string, string>();
    }

    [System.Serializable]
    public class NPCShop
    {
        public List<GoonzuItem> itemsForSale = new List<GoonzuItem>();
        public float buyMultiplier = 1.2f; // NPC buys at 80% of value
        public float sellMultiplier = 1.5f; // NPC sells at 150% of value
    }

    [RequireComponent(typeof(GoonzuSpriteAnimator))]
    public class GoonzuNPC : MonoBehaviour
    {
        [Header("NPC Identity")]
        public string npcName;
        public NPCType npcType;
        public CharacterGender gender;

        [Header("NPC Settings")]
        public NPCDialogue dialogue = new NPCDialogue();
        public NPCShop shop = new NPCShop();
        public bool canTrade = false;
        public bool canQuest = false;
        public bool isHostile = false;

        [Header("AI Settings")]
        public float interactionRange = 2f;
        public Vector3[] waypoints;
        private int currentWaypoint = 0;
        public float moveSpeed = 2f;

        private GoonzuSpriteAnimator animator;
        private Rigidbody2D rb;
        private string assetName;
        private bool isInteracting = false;
        private GoonzuCharacter interactingPlayer;

        void Awake()
        {
            animator = GetComponent<GoonzuSpriteAnimator>();
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
                rb.freezeRotation = true;
            }
        }

        void Start()
        {
            InitializeNPC();
        }

        void Update()
        {
            UpdateAI();
            UpdateAnimations();
        }

        void InitializeNPC()
        {
            assetName = $"{npcType.ToString().ToLower()}_{gender.ToString().ToLower()}";

            // Load NPC animations
            animator.LoadCharacterAnimations(assetName);

            // Initialize dialogue
            InitializeDialogue();

            // Initialize shop if merchant
            if (npcType == NPCType.Merchant)
            {
                InitializeShop();
                canTrade = true;
            }

            // Initialize blacksmith shop
            if (npcType == NPCType.Blacksmith)
            {
                InitializeBlacksmithShop();
                canTrade = true;
            }

            // Set default animation
            animator.PlayAnimation($"{assetName}_idle");

            // Add to combat system if hostile
            if (isHostile)
            {
                GoonzuCombatManager.Instance.EnterCombat(this as GoonzuCharacter);
            }
        }

        void InitializeDialogue()
        {
            switch (npcType)
            {
                case NPCType.Merchant:
                    dialogue.greeting = "Welcome to my shop! Take a look at my wares.";
                    dialogue.randomLines = new string[] {
                        "Fine goods at fair prices!",
                        "Everything you need for your adventures.",
                        "Best deals in town, I guarantee it!"
                    };
                    dialogue.farewell = "Come back anytime!";
                    dialogue.tradeDialogues["buy"] = "A fine choice! That will be {price} gold.";
                    dialogue.tradeDialogues["sell"] = "I'll give you {price} gold for that.";
                    break;

                case NPCType.Blacksmith:
                    dialogue.greeting = "Need your weapons sharpened or armor repaired?";
                    dialogue.randomLines = new string[] {
                        "Fine craftsmanship is my specialty.",
                        "A good blade never goes out of style.",
                        "I can fix just about anything!"
                    };
                    dialogue.farewell = "Stay safe out there!";
                    dialogue.tradeDialogues["repair"] = "I can repair that for {price} gold.";
                    break;

                case NPCType.Guard:
                    dialogue.greeting = "All is quiet in the town.";
                    dialogue.randomLines = new string[] {
                        "Stay out of trouble.",
                        "The roads are safe... for now.",
                        "Report any suspicious activity."
                    };
                    dialogue.farewell = "Move along.";
                    break;

                case NPCType.Innkeeper:
                    dialogue.greeting = "Welcome to the inn! Need a room for the night?";
                    dialogue.randomLines = new string[] {
                        "Best beds in town!",
                        "Care for some ale?",
                        "Travelers from all over stay here."
                    };
                    dialogue.farewell = "Sleep well!";
                    break;

                case NPCType.Priest:
                    dialogue.greeting = "May the gods bless you.";
                    dialogue.randomLines = new string[] {
                        "Faith will guide you through dark times.",
                        "The temple is always open to those in need.",
                        "Prayer brings strength to the weary."
                    };
                    dialogue.farewell = "Go with the gods.";
                    break;

                case NPCType.Mayor:
                    dialogue.greeting = "Welcome to our fair town.";
                    dialogue.randomLines = new string[] {
                        "Our town prospers under good leadership.",
                        "We value hard work and honesty.",
                        "Together we build a better future."
                    };
                    dialogue.farewell = "Serve the town well.";
                    break;

                case NPCType.Farmer:
                    dialogue.greeting = "Hard day's work in the fields.";
                    dialogue.randomLines = new string[] {
                        "The crops are coming in nicely.",
                        "Fresh vegetables for sale!",
                        "Nothing beats farm-fresh food."
                    };
                    dialogue.farewell = "Back to work!";
                    break;

                case NPCType.Hunter:
                    dialogue.greeting = "The forest provides for those who respect it.";
                    dialogue.randomLines = new string[] {
                        "Fresh game meat available.",
                        "I know every trail in these woods.",
                        "Respect nature, and it will provide."
                    };
                    dialogue.farewell = "Safe hunting!";
                    break;

                case NPCType.Beggar:
                    dialogue.greeting = "Spare some coin for a poor soul?";
                    dialogue.randomLines = new string[] {
                        "Times are hard for everyone.",
                        "Every little bit helps.",
                        "Bless you for your kindness."
                    };
                    dialogue.farewell = "May fortune smile upon you.";
                    break;

                case NPCType.Noble:
                    dialogue.greeting = "Ah, a visitor of some refinement.";
                    dialogue.randomLines = new string[] {
                        "The nobility has its burdens.",
                        "One must maintain appearances.",
                        "Power comes with responsibility."
                    };
                    dialogue.farewell = "Until we meet again.";
                    break;
            }
        }

        void InitializeShop()
        {
            // Add common items for sale
            shop.itemsForSale.Add(GoonzuItemManager.Instance.CreateItem("health_potion", 1, GoonzuItem.ItemRarity.Common));
            shop.itemsForSale.Add(GoonzuItemManager.Instance.CreateItem("mana_potion", 1, GoonzuItem.ItemRarity.Common));
            shop.itemsForSale.Add(GoonzuItemManager.Instance.CreateItem("bread", 1, GoonzuItem.ItemRarity.Common));
            shop.itemsForSale.Add(GoonzuItemManager.Instance.CreateItem("dagger", 1, GoonzuItem.ItemRarity.Common));
            shop.itemsForSale.Add(GoonzuItemManager.Instance.CreateItem("leather_armor", 1, GoonzuItem.ItemRarity.Common));
            shop.itemsForSale.Add(GoonzuItemManager.Instance.CreateItem("cloth_bolt", 1, GoonzuItem.ItemRarity.Common));
        }

        void InitializeBlacksmithShop()
        {
            // Add weapons and armor for sale
            shop.itemsForSale.Add(GoonzuItemManager.Instance.CreateItem("broadsword", 1, GoonzuItem.ItemRarity.Common));
            shop.itemsForSale.Add(GoonzuItemManager.Instance.CreateItem("chainmail_armor", 1, GoonzuItem.ItemRarity.Common));
            shop.itemsForSale.Add(GoonzuItemManager.Instance.CreateItem("iron_ore", 1, GoonzuItem.ItemRarity.Common));
            shop.itemsForSale.Add(GoonzuItemManager.Instance.CreateItem("mithril_ore", 1, GoonzuItem.ItemRarity.Rare));
        }

        void UpdateAI()
        {
            if (isInteracting) return;

            // Patrol waypoints if available
            if (waypoints.Length > 0)
            {
                PatrolWaypoints();
            }
        }

        void PatrolWaypoints()
        {
            if (waypoints.Length == 0) return;

            Vector3 targetPoint = waypoints[currentWaypoint];
            float distance = Vector3.Distance(transform.position, targetPoint);

            if (distance < 0.5f)
            {
                // Move to next waypoint
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                targetPoint = waypoints[currentWaypoint];
            }

            MoveTowards(targetPoint);
        }

        void MoveTowards(Vector3 position)
        {
            Vector3 direction = (position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

            // Flip sprite based on movement direction
            if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        void UpdateAnimations()
        {
            bool isMoving = rb.velocity.magnitude > 0.1f;

            if (isMoving)
            {
                if (!animator.IsPlaying($"{assetName}_walk"))
                {
                    animator.PlayAnimation($"{assetName}_walk");
                }
            }
            else
            {
                if (!animator.IsPlaying($"{assetName}_idle"))
                {
                    animator.PlayAnimation($"{assetName}_idle");
                }
            }
        }

        // Interaction methods
        public void Interact(GoonzuCharacter player)
        {
            if (isInteracting) return;

            interactingPlayer = player;
            isInteracting = true;

            // Face the player
            Vector3 direction = (player.transform.position - transform.position).normalized;
            if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (direction.x < 0)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            // Show greeting
            ShowDialogue(dialogue.greeting);

            // Open appropriate interface
            if (canTrade)
            {
                GoonzuUIManager.Instance.ShowPanel("trade");
            }
        }

        public void EndInteraction()
        {
            if (!isInteracting) return;

            ShowDialogue(dialogue.farewell);
            isInteracting = false;
            interactingPlayer = null;

            GoonzuUIManager.Instance.HideAllPanels();
        }

        void ShowDialogue(string text)
        {
            Debug.Log($"{npcName}: {text}");
            // In a full implementation, this would show a dialogue UI
        }

        // Trading methods
        public void BuyItem(GoonzuItem item, GoonzuCharacter buyer)
        {
            if (!shop.itemsForSale.Contains(item)) return;

            int price = Mathf.RoundToInt(item.value * shop.sellMultiplier);

            // Check if buyer has enough gold (simplified)
            // In full implementation, check buyer's gold
            bool hasEnoughGold = true; // Placeholder

            if (hasEnoughGold)
            {
                buyer.AddToInventory(item);
                shop.itemsForSale.Remove(item);

                string dialogue = dialogue.tradeDialogues["buy"].Replace("{price}", price.ToString());
                ShowDialogue(dialogue);
            }
            else
            {
                ShowDialogue("You don't have enough gold!");
            }
        }

        public void SellItem(GoonzuItem item, GoonzuCharacter seller)
        {
            if (!seller.RemoveFromInventory(item)) return;

            int price = Mathf.RoundToInt(item.value * shop.buyMultiplier);
            // Add gold to seller (simplified)

            shop.itemsForSale.Add(item);

            string dialogue = dialogue.tradeDialogues["sell"].Replace("{price}", price.ToString());
            ShowDialogue(dialogue);
        }

        // Quest methods
        public void OfferQuest()
        {
            if (!canQuest) return;

            ShowDialogue("I have a task for someone with your skills...");
            // Quest system would be implemented here
        }

        // Combat interface (for hostile NPCs)
        public void TakeDamage(int damage)
        {
            // NPCs don't take damage unless hostile
            if (!isHostile) return;

            // Implement NPC health system if needed
            Debug.Log($"{npcName} takes {damage} damage!");
        }

        public void Attack() { }
        public void CastSpell() { }
        public void MoveTo(Vector3 position) { }
        public void GainExperience(int exp) { }
        public void Heal(int amount) { }
        public void AddToInventory(GoonzuItem item) { }
        public bool RemoveFromInventory(GoonzuItem item) { return false; }

        // Public API methods
        public string GetAssetName()
        {
            return assetName;
        }

        public bool CanInteract(Vector3 playerPosition)
        {
            return Vector3.Distance(transform.position, playerPosition) <= interactionRange;
        }

        public NPCType GetNPCType()
        {
            return npcType;
        }

        public bool IsHostile()
        {
            return isHostile;
        }

        void OnDestroy()
        {
            if (isHostile)
            {
                GoonzuCombatManager.Instance.ExitCombat(this as GoonzuCharacter);
            }
        }
    }
}