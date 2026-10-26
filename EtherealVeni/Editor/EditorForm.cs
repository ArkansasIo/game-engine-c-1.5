using EthereoPandora.Game;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;

namespace EtherealVeni.Editor;

public sealed class EditorForm : Form
{
    private readonly EditorViewport _viewport = new();
    private readonly TreeView _hierarchy = new();
    private readonly PropertyGrid _inspector = new();
    private readonly TextBox _console = new();

    public EditorForm()
    {
        Text = "Ethereal Veni Editor — Ethereo Pandora";
        ClientSize = new Size(1600, 960);
        MinimumSize = new Size(1200, 760);
        BackColor = Color.FromArgb(18, 21, 27);
        BuildUi();
    }

    private void BuildUi()
    {
        var toolbar = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden, BackColor = Color.FromArgb(28, 32, 40), ForeColor = Color.White };
        var play = new ToolStripButton("▶ PLAY") { DisplayStyle = ToolStripItemDisplayStyle.Text, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        play.Click += (_, _) => new GameForm().Show();
        toolbar.Items.Add(play);
        toolbar.Items.Add(new ToolStripButton("⏸ PAUSE"));
        toolbar.Items.Add(new ToolStripButton("■ STOP"));
        toolbar.Items.Add(new ToolStripSeparator());
        toolbar.Items.Add(new ToolStripButton("SAVE"));
        toolbar.Items.Add(new ToolStripButton("BUILD"));
        toolbar.Items.Add(new ToolStripButton("PROFILER"));
        toolbar.Dock = DockStyle.Top;
        Controls.Add(toolbar);

        var hierarchyPanel = new Panel { Dock = DockStyle.Left, Width = 260, BackColor = Color.FromArgb(24, 28, 35), Padding = new Padding(8) };
        hierarchyPanel.Controls.Add(new Label { Text = "WORLD HIERARCHY", Dock = DockStyle.Top, Height = 28, ForeColor = Color.Gold, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
        _hierarchy.Dock = DockStyle.Fill;
        _hierarchy.BackColor = Color.FromArgb(19, 23, 29); _hierarchy.ForeColor = Color.Gainsboro; _hierarchy.BorderStyle = BorderStyle.None;
        var world = _hierarchy.Nodes.Add("EthereoPandora_World");
        world.Nodes.Add("DirectionalLight"); world.Nodes.Add("Player_Vanguard"); world.Nodes.Add("VerdantMarch_Terrain"); world.Nodes.Add("NPC_VillageElder"); world.Nodes.Add("Enemy_Shadewolf"); world.Nodes.Add("Enemy_AshKnight"); world.Nodes.Add("FallenTemple_Dungeon");
        _hierarchy.AfterSelect += (_, e) => _inspector.SelectedObject = new InspectorData(e.Node.Text);
        hierarchyPanel.Controls.Add(_hierarchy);
        Controls.Add(hierarchyPanel);

        var right = new Panel { Dock = DockStyle.Right, Width = 300, BackColor = Color.FromArgb(24, 28, 35), Padding = new Padding(8) };
        right.Controls.Add(new Label { Text = "INSPECTOR", Dock = DockStyle.Top, Height = 28, ForeColor = Color.Gold, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
        _inspector.Dock = DockStyle.Fill; _inspector.BackColor = Color.FromArgb(28, 32, 40); _inspector.ForeColor = Color.White;
        right.Controls.Add(_inspector); Controls.Add(right);

        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 170, BackColor = Color.FromArgb(13, 16, 21), Padding = new Padding(8) };
        bottom.Controls.Add(new Label { Text = "CONSOLE / ENGINE LOG", Dock = DockStyle.Top, Height = 24, ForeColor = Color.Gold });
        _console.Dock = DockStyle.Fill; _console.Multiline = true; _console.ReadOnly = true; _console.BackColor = Color.FromArgb(8, 10, 14); _console.ForeColor = Color.LightGreen; _console.BorderStyle = BorderStyle.None;
        _console.Text = "[EtherealVeni] Editor initialized\r\n[Renderer] Software 3D viewport online\r\n[World] EthereoPandora_World loaded\r\n[Game] PLAY launches the interactive prototype\r\n[Input] Select entities in the hierarchy to inspect components\r\n";
        bottom.Controls.Add(_console); Controls.Add(bottom);

        _viewport.Dock = DockStyle.Fill;
        Controls.Add(_viewport);
        _viewport.BringToFront();
        toolbar.BringToFront();
        hierarchyPanel.BringToFront(); right.BringToFront(); bottom.BringToFront();
    }

    private sealed class InspectorData
    {
        public string Entity { get; }
        public string Transform => "Position / Rotation / Scale";
        public string Components => "Transform, Mesh, Collider, Stats, AI";
        public string Status => "Editable prototype entity";
        public InspectorData(string entity) => Entity = entity;
    }
}

internal sealed class EditorViewport : Control
{
    private float _time;
    private readonly Timer _timer = new() { Interval = 33 };
    public EditorViewport()
    {
        DoubleBuffered = true; _timer.Tick += (_, _) => { _time += .033f; Invalidate(); }; _timer.Start();
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e); var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
        using var sky = new LinearGradientBrush(ClientRectangle, Color.FromArgb(24, 47, 76), Color.FromArgb(6, 9, 15), LinearGradientMode.Vertical); g.FillRectangle(sky, ClientRectangle);
        var horizon = Height * .52f;
        using var terrain = new SolidBrush(Color.FromArgb(43, 67, 54)); g.FillPolygon(terrain, new[] { new PointF(0, horizon), new PointF(Width, horizon), new PointF(Width, Height), new PointF(0, Height) });
        using var grid = new Pen(Color.FromArgb(60, 115, 105, 95), 1);
        for (int y = 0; y < 20; y++) { float yy = horizon + y * y * 2.1f; g.DrawLine(grid, 0, yy, Width, yy); }
        for (int x = -20; x <= 20; x++) { float xx = Width / 2f + x * 55; g.DrawLine(grid, Width / 2f, horizon, xx, Height); }
        DrawTree(g, Width * .23f, horizon + 20); DrawTree(g, Width * .78f, horizon + 50);
        DrawPlayer(g, Width / 2f, horizon + 20);
        using var label = new Font("Segoe UI", 11, FontStyle.Bold); g.DrawString("SCENE VIEWPORT — ETHEREO PANDORA / VERDANT MARCH", label, Brushes.WhiteSmoke, 18, 18);
        g.DrawString("Editor Camera • Perspective • Lit Prototype", SystemFonts.DefaultFont, Brushes.LightGray, 18, 44);
        using var gizmo = new Pen(Color.Red, 2); g.DrawLine(gizmo, Width - 100, Height - 70, Width - 50, Height - 70); using var gizmoY = new Pen(Color.LimeGreen, 2); g.DrawLine(gizmoY, Width - 100, Height - 70, Width - 100, Height - 20);
    }
    private static void DrawTree(Graphics g, float x, float y) { using var t = new SolidBrush(Color.FromArgb(70, 45, 28)); g.FillRectangle(t, x - 5, y - 90, 10, 90); using var l = new SolidBrush(Color.FromArgb(40, 105, 72)); g.FillEllipse(l, x - 42, y - 130, 84, 70); g.FillEllipse(l, x - 25, y - 165, 55, 65); }
    private static void DrawPlayer(Graphics g, float x, float y) { using var aura = new SolidBrush(Color.FromArgb(50, 80, 190, 255)); g.FillEllipse(aura, x - 30, y - 5, 60, 25); using var body = new SolidBrush(Color.FromArgb(55, 120, 190)); g.FillEllipse(body, x - 16, y - 65, 32, 62); using var head = new SolidBrush(Color.FromArgb(225, 184, 150)); g.FillEllipse(head, x - 12, y - 85, 24, 24); using var sword = new Pen(Color.LightSteelBlue, 5); g.DrawLine(sword, x + 15, y - 50, x + 55, y - 95); }
}
