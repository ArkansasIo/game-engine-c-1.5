using EthereoPandora.Game;
using System.Windows.Forms;

namespace EtherealVeni.Editor;

public sealed class EditorForm : Form
{
    private readonly Pandora3DViewport _viewport = new();
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
        var toolbar = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden, BackColor = Color.FromArgb(28, 32, 40) };
        AddTool(toolbar, "▶ PLAY", (_, _) => new GameForm().Show());
        AddTool(toolbar, "⏸ PAUSE", (_, _) => Append("[Runtime] Pause requested"));
        AddTool(toolbar, "■ STOP", (_, _) => Append("[Runtime] Stop requested"));
        toolbar.Items.Add(new ToolStripSeparator());
        AddTool(toolbar, "SAVE", (_, _) => Append("[Asset] Scene saved: EthereoPandora_World"));
        AddTool(toolbar, "BUILD", (_, _) => Append("[Build] Windows x64 build queued"));
        AddTool(toolbar, "PROFILER", (_, _) => Append("[Profiler] Renderer / gameplay counters opened"));
        toolbar.Dock = DockStyle.Top; Controls.Add(toolbar);

        var left = new Panel { Dock = DockStyle.Left, Width = 270, BackColor = Color.FromArgb(24, 28, 35), Padding = new Padding(8) };
        left.Controls.Add(new Label { Text = "WORLD HIERARCHY", Dock = DockStyle.Top, Height = 28, ForeColor = Color.Gold, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
        _hierarchy.Dock = DockStyle.Fill; _hierarchy.BackColor = Color.FromArgb(19, 23, 29); _hierarchy.ForeColor = Color.Gainsboro; _hierarchy.BorderStyle = BorderStyle.None;
        var world = _hierarchy.Nodes.Add("EthereoPandora_World");
        world.Nodes.Add("DirectionalLight"); world.Nodes.Add("Player_Vanguard"); world.Nodes.Add("VerdantMarch_Terrain"); world.Nodes.Add("Village_Elder"); world.Nodes.Add("Enemy_Shadewolf"); world.Nodes.Add("Enemy_AshKnight"); world.Nodes.Add("Enemy_VerdantWarden"); world.Nodes.Add("FallenTemple_Dungeon");
        _hierarchy.AfterSelect += (_, e) => { _inspector.SelectedObject = new InspectorData(e.Node.Text); Append($"[Selection] {e.Node.Text}"); };
        left.Controls.Add(_hierarchy); Controls.Add(left);

        var right = new Panel { Dock = DockStyle.Right, Width = 310, BackColor = Color.FromArgb(24, 28, 35), Padding = new Padding(8) };
        right.Controls.Add(new Label { Text = "INSPECTOR", Dock = DockStyle.Top, Height = 28, ForeColor = Color.Gold, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
        _inspector.Dock = DockStyle.Fill; _inspector.BackColor = Color.FromArgb(28, 32, 40); _inspector.ForeColor = Color.White; right.Controls.Add(_inspector); Controls.Add(right);

        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 175, BackColor = Color.FromArgb(13, 16, 21), Padding = new Padding(8) };
        bottom.Controls.Add(new Label { Text = "CONSOLE / ENGINE LOG", Dock = DockStyle.Top, Height = 24, ForeColor = Color.Gold });
        _console.Dock = DockStyle.Fill; _console.Multiline = true; _console.ReadOnly = true; _console.BackColor = Color.FromArgb(8, 10, 14); _console.ForeColor = Color.LightGreen; _console.BorderStyle = BorderStyle.None;
        _console.Text = "[EtherealVeni] Editor initialized\r\n[Renderer] OpenGL 3.3 viewport initialized\r\n[World] EthereoPandora_World loaded\r\n[Scene] Verdant March terrain ready\r\n[Game] PLAY launches the interactive 3D runtime\r\n[Editor] Hierarchy + inspector active\r\n";
        bottom.Controls.Add(_console); Controls.Add(bottom);

        _viewport.Dock = DockStyle.Fill; Controls.Add(_viewport);
        _viewport.BringToFront(); toolbar.BringToFront(); left.BringToFront(); right.BringToFront(); bottom.BringToFront();
    }

    private static void AddTool(ToolStrip bar, string text, EventHandler click)
    {
        var item = new ToolStripButton(text) { DisplayStyle = ToolStripItemDisplayStyle.Text, ForeColor = Color.WhiteSmoke };
        item.Click += click; bar.Items.Add(item);
    }

    private void Append(string text) => _console.AppendText(text + Environment.NewLine);

    private sealed class InspectorData
    {
        public string Entity { get; }
        public string Transform => "Position / Rotation / Scale";
        public string Components => "Transform, Mesh, Collider, Stats, AI";
        public string Renderer => "OpenGL 3.3 / Lit Forward Prototype";
        public string Status => "Editable runtime entity";
        public InspectorData(string entity) => Entity = entity;
    }
}
