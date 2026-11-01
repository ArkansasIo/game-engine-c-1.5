using EtherealVeni.Engine.Runtime;
using System.Windows.Forms;

namespace EtherealVeni.Editor;

public sealed class EditorForm : Form
{
    private readonly TreeView _hierarchy = new();
    private readonly PropertyGrid _inspector = new();
    private readonly TextBox _console = new();
    private readonly EngineRuntime _runtime = new();

    public EditorForm()
    {
        Text = "Ethereal Veni Editor — Ethereo Pandora";
        ClientSize = new Size(1500, 900);
        MinimumSize = new Size(1050, 680);
        BackColor = Color.FromArgb(18, 21, 27);
        BuildUi();
    }

    private void BuildUi()
    {
        var toolbar = new ToolStrip { GripStyle = ToolStripGripStyle.Hidden, BackColor = Color.FromArgb(28, 32, 40) };
        AddTool(toolbar, "▶ PLAY", (_, _) => Append("[Runtime] Play requested; launch the game executable separately."));
        AddTool(toolbar, "⏵ START ENGINE", (_, _) => { _runtime.Start(); Append("[Runtime] Engine started."); });
        AddTool(toolbar, "■ STOP ENGINE", (_, _) => { _runtime.Stop(); Append("[Runtime] Engine stopped."); });
        toolbar.Items.Add(new ToolStripSeparator());
        AddTool(toolbar, "SAVE", (_, _) => Append("[Scene] Save adapter is not configured yet."));
        AddTool(toolbar, "BUILD", (_, _) => Append("[Build] Use the repository build workflow."));
        toolbar.Dock = DockStyle.Top;
        Controls.Add(toolbar);

        var left = new Panel { Dock = DockStyle.Left, Width = 250, BackColor = Color.FromArgb(24, 28, 35), Padding = new Padding(8) };
        left.Controls.Add(new Label { Text = "WORLD HIERARCHY", Dock = DockStyle.Top, Height = 28, ForeColor = Color.Gold, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
        _hierarchy.Dock = DockStyle.Fill;
        _hierarchy.BackColor = Color.FromArgb(19, 23, 29);
        _hierarchy.ForeColor = Color.Gainsboro;
        var root = _hierarchy.Nodes.Add("EthereoPandora_World");
        root.Nodes.Add("DirectionalLight");
        root.Nodes.Add("Player_Vanguard");
        root.Nodes.Add("VerdantMarch_Terrain");
        root.Nodes.Add("Village_Elder");
        root.Nodes.Add("FallenTemple_Dungeon");
        _hierarchy.AfterSelect += (_, e) => _inspector.SelectedObject = new InspectorData(e.Node.Text);
        left.Controls.Add(_hierarchy);
        Controls.Add(left);

        var right = new Panel { Dock = DockStyle.Right, Width = 300, BackColor = Color.FromArgb(24, 28, 35), Padding = new Padding(8) };
        right.Controls.Add(new Label { Text = "INSPECTOR", Dock = DockStyle.Top, Height = 28, ForeColor = Color.Gold, Font = new Font("Segoe UI", 10, FontStyle.Bold) });
        _inspector.Dock = DockStyle.Fill;
        right.Controls.Add(_inspector);
        Controls.Add(right);

        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 170, BackColor = Color.FromArgb(13, 16, 21), Padding = new Padding(8) };
        bottom.Controls.Add(new Label { Text = "CONSOLE / ENGINE LOG", Dock = DockStyle.Top, Height = 24, ForeColor = Color.Gold });
        _console.Dock = DockStyle.Fill;
        _console.Multiline = true;
        _console.ReadOnly = true;
        _console.BackColor = Color.FromArgb(8, 10, 14);
        _console.ForeColor = Color.LightGreen;
        _console.BorderStyle = BorderStyle.None;
        _console.Text = "[Ethereal Veni] Editor initialized\r\n[Runtime] Engine runtime available\r\n";
        bottom.Controls.Add(_console);
        Controls.Add(bottom);

        var viewport = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(9, 14, 22) };
        viewport.Controls.Add(new Label
        {
            Text = "ETHEREAL VENI\r\n\r\nScene viewport adapter\r\n\r\nConnect an IGraphicsDevice backend to render the active world.",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(180, 195, 210),
            Font = new Font("Segoe UI", 14, FontStyle.Bold)
        });
        Controls.Add(viewport);
        viewport.BringToFront();
        bottom.BringToFront();
        left.BringToFront();
        right.BringToFront();
        toolbar.BringToFront();
    }

    private static void AddTool(ToolStrip bar, string text, EventHandler click)
    {
        var item = new ToolStripButton(text) { DisplayStyle = ToolStripItemDisplayStyle.Text, ForeColor = Color.WhiteSmoke };
        item.Click += click;
        bar.Items.Add(item);
    }

    private void Append(string text) => _console.AppendText(text + Environment.NewLine);

    private sealed class InspectorData(string entity)
    {
        public string Entity { get; } = entity;
        public string Components => "Transform, Mesh, Collider, Stats, AI";
        public string Status => "Editor prototype entity";
    }
}
