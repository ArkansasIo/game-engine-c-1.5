using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameEngine.GUI
{
    public class MainMenuForm : Form
    {
        private List<Button> menuButtons = new();
        private Label titleLabel;
        private Panel menuPanel;

        public MainMenuForm()
        {
            Text = "GOONZU";
            Size = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            titleLabel = new Label
            {
                Text = "GOONZU",
                Font = new Font("Georgia", 24, FontStyle.Bold),
                ForeColor = Color.Gold,
                AutoSize = true,
                Location = new Point(320, 20)
            };
            Controls.Add(titleLabel);

            menuPanel = new Panel
            {
                Location = new Point(250, 80),
                Size = new Size(300, 400),
                BackColor = Color.FromArgb(60, 40, 20)
            };
            Controls.Add(menuPanel);

            string[] options = { "Start Game", "My Character", "Inventory", "Quests", "Community", "Market", "Settings", "Exit Game" };
            for (int i = 0; i < options.Length; i++)
            {
                var btn = new Button
                {
                    Text = options[i],
                    Font = new Font("Georgia", 14, FontStyle.Bold),
                    Size = new Size(260, 40),
                    Location = new Point(20, 20 + i * 50),
                    BackColor = Color.SaddleBrown,
                    ForeColor = Color.Gold
                };
                btn.Click += (s, e) => MessageBox.Show($"Selected: {btn.Text}");
                menuPanel.Controls.Add(btn);
                menuButtons.Add(btn);
            }
        }
    }
}
