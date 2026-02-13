using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoonzuGame
{
    public class GoonzuUIManager : MonoBehaviour
    {
        private static GoonzuUIManager instance;
        public static GoonzuUIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GoonzuUIManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GoonzuUIManager");
                        instance = go.AddComponent<GoonzuUIManager>();
                    }
                }
                return instance;
            }
        }

        [Header("UI Panels")]
        public GameObject inventoryPanel;
        public GameObject characterPanel;
        public GameObject skillPanel;
        public GameObject questPanel;
        public GameObject mapPanel;
        public GameObject tradePanel;
        public GameObject craftingPanel;
        public GameObject guildPanel;
        public GameObject settingsPanel;

        [Header("UI Bars")]
        public Image healthBar;
        public Image manaBar;
        public Image experienceBar;
        public Image staminaBar;

        [Header("UI Icons")]
        public Image swordIcon;
        public Image shieldIcon;
        public Image potionIcon;
        public Image goldCoin;
        public Image manaCrystal;
        public Image healthHeart;
        public Image experienceStar;
        public Image questScroll;
        public Image mapMarker;
        public Image chatBubble;
        public Image friendIcon;
        public Image enemyIcon;
        public Image chestIcon;
        public Image keyIcon;
        public Image lockIcon;
        public Image arrowIcon;

        [Header("UI References")]
        public Text healthText;
        public Text manaText;
        public Text levelText;
        public Text goldText;
        public Text experienceText;

        private GoonzuCharacter currentCharacter;
        private Dictionary<string, GameObject> uiPanels = new Dictionary<string, GameObject>();
        private Dictionary<string, Image> uiIcons = new Dictionary<string, Image>();
        private Dictionary<string, Image> uiBars = new Dictionary<string, Image>();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeUI();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void InitializeUI()
        {
            // Initialize panel dictionary
            if (inventoryPanel) uiPanels["inventory"] = inventoryPanel;
            if (characterPanel) uiPanels["character"] = characterPanel;
            if (skillPanel) uiPanels["skill"] = skillPanel;
            if (questPanel) uiPanels["quest"] = questPanel;
            if (mapPanel) uiPanels["map"] = mapPanel;
            if (tradePanel) uiPanels["trade"] = tradePanel;
            if (craftingPanel) uiPanels["crafting"] = craftingPanel;
            if (guildPanel) uiPanels["guild"] = guildPanel;
            if (settingsPanel) uiPanels["settings"] = settingsPanel;

            // Initialize icon dictionary
            if (swordIcon) uiIcons["sword"] = swordIcon;
            if (shieldIcon) uiIcons["shield"] = shieldIcon;
            if (potionIcon) uiIcons["potion"] = potionIcon;
            if (goldCoin) uiIcons["gold"] = goldCoin;
            if (manaCrystal) uiIcons["mana"] = manaCrystal;
            if (healthHeart) uiIcons["health"] = healthHeart;
            if (experienceStar) uiIcons["experience"] = experienceStar;
            if (questScroll) uiIcons["quest"] = questScroll;
            if (mapMarker) uiIcons["map"] = mapMarker;
            if (chatBubble) uiIcons["chat"] = chatBubble;
            if (friendIcon) uiIcons["friend"] = friendIcon;
            if (enemyIcon) uiIcons["enemy"] = enemyIcon;
            if (chestIcon) uiIcons["chest"] = chestIcon;
            if (keyIcon) uiIcons["key"] = keyIcon;
            if (lockIcon) uiIcons["lock"] = lockIcon;
            if (arrowIcon) uiIcons["arrow"] = arrowIcon;

            // Initialize bar dictionary
            if (healthBar) uiBars["health"] = healthBar;
            if (manaBar) uiBars["mana"] = manaBar;
            if (experienceBar) uiBars["experience"] = experienceBar;
            if (staminaBar) uiBars["stamina"] = staminaBar;

            // Load UI sprites
            LoadUISprites();

            // Hide all panels initially
            foreach (var panel in uiPanels.Values)
            {
                if (panel) panel.SetActive(false);
            }
        }

        void LoadUISprites()
        {
            // Load sprites for UI elements
            foreach (var kvp in uiIcons)
            {
                Sprite sprite = GoonzuAssetManager.Instance.GetSprite($"UI/Icons/{kvp.Key}_icon");
                if (sprite != null)
                {
                    kvp.Value.sprite = sprite;
                }
            }

            foreach (var kvp in uiBars)
            {
                Sprite sprite = GoonzuAssetManager.Instance.GetSprite($"UI/Bars/{kvp.Key}_bar");
                if (sprite != null)
                {
                    kvp.Value.sprite = sprite;
                }
            }

            // Load panel backgrounds
            foreach (var kvp in uiPanels)
            {
                Image panelImage = kvp.Value.GetComponent<Image>();
                if (panelImage != null)
                {
                    Sprite sprite = GoonzuAssetManager.Instance.GetSprite($"UI/Panels/{kvp.Key}_panel");
                    if (sprite != null)
                    {
                        panelImage.sprite = sprite;
                    }
                }
            }
        }

        void Update()
        {
            if (currentCharacter != null)
            {
                UpdateCharacterUI();
            }
        }

        void UpdateCharacterUI()
        {
            // Update health bar
            if (healthBar)
            {
                healthBar.fillAmount = (float)currentCharacter.stats.currentHealth / currentCharacter.stats.maxHealth;
            }

            // Update mana bar
            if (manaBar)
            {
                manaBar.fillAmount = (float)currentCharacter.stats.currentMana / currentCharacter.stats.maxMana;
            }

            // Update experience bar
            if (experienceBar)
            {
                float expProgress = (float)currentCharacter.stats.experience / currentCharacter.stats.experienceToNext;
                experienceBar.fillAmount = expProgress;
            }

            // Update text displays
            if (healthText)
            {
                healthText.text = $"{currentCharacter.stats.currentHealth}/{currentCharacter.stats.maxHealth}";
            }

            if (manaText)
            {
                manaText.text = $"{currentCharacter.stats.currentMana}/{currentCharacter.stats.maxMana}";
            }

            if (levelText)
            {
                levelText.text = $"Level {currentCharacter.stats.level}";
            }

            if (experienceText)
            {
                experienceText.text = $"{currentCharacter.stats.experience}/{currentCharacter.stats.experienceToNext}";
            }
        }

        // Public API methods
        public void SetCurrentCharacter(GoonzuCharacter character)
        {
            currentCharacter = character;
        }

        public void ShowPanel(string panelName)
        {
            HideAllPanels();
            if (uiPanels.ContainsKey(panelName) && uiPanels[panelName])
            {
                uiPanels[panelName].SetActive(true);
                UpdatePanelContent(panelName);
            }
        }

        public void HidePanel(string panelName)
        {
            if (uiPanels.ContainsKey(panelName) && uiPanels[panelName])
            {
                uiPanels[panelName].SetActive(false);
            }
        }

        public void TogglePanel(string panelName)
        {
            if (uiPanels.ContainsKey(panelName) && uiPanels[panelName])
            {
                bool isActive = uiPanels[panelName].activeSelf;
                if (isActive)
                {
                    HidePanel(panelName);
                }
                else
                {
                    ShowPanel(panelName);
                }
            }
        }

        public void HideAllPanels()
        {
            foreach (var panel in uiPanels.Values)
            {
                if (panel) panel.SetActive(false);
            }
        }

        void UpdatePanelContent(string panelName)
        {
            if (currentCharacter == null) return;

            switch (panelName)
            {
                case "inventory":
                    UpdateInventoryPanel();
                    break;
                case "character":
                    UpdateCharacterPanel();
                    break;
                case "skill":
                    UpdateSkillPanel();
                    break;
            }
        }

        void UpdateInventoryPanel()
        {
            // This would populate the inventory grid with item icons
            // For now, just log the inventory contents
            Debug.Log("Inventory contents:");
            foreach (var item in currentCharacter.inventory)
            {
                Debug.Log($"- {item.itemName} (x{item.currentStack})");
            }
        }

        void UpdateCharacterPanel()
        {
            // Update character stats display
            Debug.Log($"Character: {currentCharacter.characterName}");
            Debug.Log($"Class: {currentCharacter.characterClass}");
            Debug.Log($"Race: {currentCharacter.race}");
            Debug.Log($"Level: {currentCharacter.stats.level}");
            Debug.Log($"Health: {currentCharacter.stats.currentHealth}/{currentCharacter.stats.maxHealth}");
            Debug.Log($"Mana: {currentCharacter.stats.currentMana}/{currentCharacter.stats.maxMana}");

            if (currentCharacter.weapon != null)
            {
                Debug.Log($"Weapon: {currentCharacter.weapon.itemName}");
            }

            if (currentCharacter.armor != null)
            {
                Debug.Log($"Armor: {currentCharacter.armor.itemName}");
            }
        }

        void UpdateSkillPanel()
        {
            // Display available skills based on class
            Debug.Log($"Skills for {currentCharacter.characterClass}:");

            switch (currentCharacter.characterClass)
            {
                case CharacterClass.Warrior:
                    Debug.Log("- Power Attack");
                    Debug.Log("- Shield Bash");
                    Debug.Log("- Battle Cry");
                    break;
                case CharacterClass.Mage:
                    Debug.Log("- Fireball");
                    Debug.Log("- Ice Lance");
                    Debug.Log("- Arcane Missile");
                    break;
                case CharacterClass.Rogue:
                    Debug.Log("- Backstab");
                    Debug.Log("- Stealth");
                    Debug.Log("- Poison Dagger");
                    break;
                case CharacterClass.Cleric:
                    Debug.Log("- Heal");
                    Debug.Log("- Divine Shield");
                    Debug.Log("- Holy Light");
                    break;
            }
        }

        public void ShowDamageNumber(int damage, Vector3 position, bool isCritical = false)
        {
            // Create floating damage text
            GameObject damageTextObj = new GameObject("DamageText");
            damageTextObj.transform.position = position + Vector3.up;

            Text damageText = damageTextObj.AddComponent<Text>();
            damageText.text = damage.ToString();
            damageText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            damageText.color = isCritical ? Color.red : Color.white;
            damageText.fontSize = isCritical ? 24 : 18;
            damageText.alignment = TextAnchor.MiddleCenter;

            // Add canvas for rendering
            Canvas canvas = damageTextObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            // Animate upward and fade out
            StartCoroutine(AnimateDamageText(damageTextObj, isCritical));
        }

        IEnumerator AnimateDamageText(GameObject textObj, bool isCritical)
        {
            Text text = textObj.GetComponent<Text>();
            Vector3 startPos = textObj.transform.position;
            Color startColor = text.color;
            float duration = 2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                // Move upward
                textObj.transform.position = startPos + Vector3.up * (progress * 2f);

                // Fade out
                Color newColor = startColor;
                newColor.a = 1f - progress;
                text.color = newColor;

                yield return null;
            }

            Destroy(textObj);
        }

        public void ShowItemTooltip(GoonzuItem item, Vector3 position)
        {
            // Create tooltip with item information
            GameObject tooltipObj = new GameObject("ItemTooltip");
            tooltipObj.transform.position = position;

            // Add tooltip components (simplified)
            Debug.Log($"Item Tooltip: {item.itemName}\n{item.GetDescription()}");
        }

        public void UpdateGoldDisplay(int gold)
        {
            if (goldText)
            {
                goldText.text = gold.ToString();
            }
        }

        public void FlashIcon(string iconName, Color flashColor, float duration = 0.5f)
        {
            if (uiIcons.ContainsKey(iconName))
            {
                StartCoroutine(FlashIconCoroutine(uiIcons[iconName], flashColor, duration));
            }
        }

        IEnumerator FlashIconCoroutine(Image icon, Color flashColor, float duration)
        {
            Color originalColor = icon.color;
            icon.color = flashColor;

            yield return new WaitForSeconds(duration);

            icon.color = originalColor;
        }
    }
}