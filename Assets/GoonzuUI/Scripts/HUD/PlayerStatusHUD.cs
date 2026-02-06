using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GoonzuUI.Data;

namespace GoonzuUI.HUD
{
    public sealed class PlayerStatusHUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image hpBar;
        [SerializeField] private Image mpBar;
        [SerializeField] private Image expBar;
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private TMP_Text mpText;
        [SerializeField] private TMP_Text expText;

        public void Render(PlayerStats stats)
        {
            if (nameText) nameText.text = stats.Name;
            if (hpBar) hpBar.fillAmount = stats.HP / (float)stats.MaxHP;
            if (mpBar) mpBar.fillAmount = stats.MP / (float)stats.MaxMP;
            if (expBar) expBar.fillAmount = stats.EXP / (float)stats.MaxEXP;
            if (hpText) hpText.text = $"{stats.HP} / {stats.MaxHP}";
            if (mpText) mpText.text = $"{stats.MP} / {stats.MaxMP}";
            if (expText) expText.text = $"{stats.EXP} / {stats.MaxEXP}";
        }
    }
}
