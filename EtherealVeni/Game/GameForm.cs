using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Forms;

namespace EthereoPandora.Game;

public sealed class GameForm : Form
{
    private readonly GameViewport _viewport = new();
    private readonly Timer _timer = new() { Interval = 16 };

    public GameForm()
    {
        Text = "Ethereo Pandora — Ethereal Veni Runtime";
        ClientSize = new Size(1440, 900);
        MinimumSize = new Size(1024, 700);
        BackColor = Color.Black;
        KeyPreview = true;
        Controls.Add(_viewport);
        _viewport.Dock = DockStyle.Fill;
        KeyDown += (_, e) => _viewport.SetKey(e.KeyCode, true);
        KeyUp += (_, e) => _viewport.SetKey(e.KeyCode, false);
        _timer.Tick += (_, _) => _viewport.UpdateFrame(1f / 60f);
        Shown += (_, _) => { _viewport.Focus(); _timer.Start(); };
        FormClosed += (_, _) => _timer.Stop();
    }
}

public sealed class GameViewport : Control
{
    private readonly HashSet<Keys> _keys = new();
    private readonly List<Actor> _enemies = new();
    private readonly Random _random = new(7);
    private Vector3 _player = new(0, 0, 1.1f);
    private float _yaw = 0.15f;
    private float _pitch = 0.58f;
    private float _hp = 100;
    private float _mana = 100;
    private float _attackFlash;
    private bool _inventory;
    private bool _quest;
    private bool _menu;
    private Point _lastMouse;
    private bool _dragging;

    public GameViewport()
    {
        DoubleBuffered = true;
        BackColor = Color.FromArgb(8, 12, 20);
        TabStop = true;
        for (var i = 0; i < 12; i++)
            _enemies.Add(new Actor(new Vector3((i % 4 - 1.5f) * 7, (i / 4 + 1) * 7, 1.0f + (i % 3) * .2f), i % 3));
        MouseDown += OnMouseDown;
        MouseUp += (_, _) => _dragging = false;
        MouseMove += OnMouseMove;
        MouseWheel += (_, e) => _pitch = Math.Clamp(_pitch - e.Delta * .0005f, .35f, .95f);
        MouseClick += (_, e) => { if (e.Button == MouseButtons.Left) Attack(); };
    }

    public void SetKey(Keys key, bool down)
    {
        if (down) _keys.Add(key); else _keys.Remove(key);
        if (!down) return;
        if (key == Keys.I) _inventory = !_inventory;
        if (key == Keys.Q) _quest = !_quest;
        if (key == Keys.Escape) _menu = !_menu;
        if (key == Keys.Space) Attack();
    }

    public void UpdateFrame(float dt)
    {
        var move = Vector2.Zero;
        if (_keys.Contains(Keys.W)) move.Y += 1;
        if (_keys.Contains(Keys.S)) move.Y -= 1;
        if (_keys.Contains(Keys.A)) move.X -= 1;
        if (_keys.Contains(Keys.D)) move.X += 1;
        if (move.LengthSquared() > 0)
        {
            move = Vector2.Normalize(move);
            var forward = new Vector3(MathF.Sin(_yaw), MathF.Cos(_yaw), 0);
            var right = new Vector3(MathF.Cos(_yaw), -MathF.Sin(_yaw), 0);
            _player += (forward * move.Y + right * move.X) * (dt * 8f);
            _player.Z = 1.1f + Height(_player.X, _player.Y);
        }

        foreach (var enemy in _enemies)
        {
            var delta = _player - enemy.Position;
            delta.Z = 0;
            if (delta.LengthSquared() > 5)
                enemy.Position += Vector3.Normalize(delta) * dt * (1.1f + enemy.Type * .25f);
            else if (_random.NextDouble() < .015)
                _hp = Math.Max(0, _hp - 4);
        }
        _attackFlash = Math.Max(0, _attackFlash - dt * 4);
        _mana = Math.Min(100, _mana + dt * 3);
        Invalidate();
    }

    private void Attack()
    {
        if (_mana < 12) return;
        _mana -= 12;
        _attackFlash = 1;
        foreach (var enemy in _enemies.ToList())
        {
            var d = Vector3.Distance(new Vector3(_player.X, _player.Y, 0), new Vector3(enemy.Position.X, enemy.Position.Y, 0));
            if (d < 8) enemy.Hp -= 35;
        }
        _enemies.RemoveAll(e => e.Hp <= 0);
        while (_enemies.Count < 8)
            _enemies.Add(new Actor(new Vector3(_random.Next(-30, 31), _random.Next(8, 40), 1), _random.Next(3)));
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        Focus();
        if (e.Button == MouseButtons.Right) { _dragging = true; _lastMouse = e.Location; }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (!_dragging) return;
        _yaw += (e.X - _lastMouse.X) * .008f;
        _pitch = Math.Clamp(_pitch + (e.Y - _lastMouse.Y) * .004f, .35f, .95f);
        _lastMouse = e.Location;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        DrawSky(g);
        DrawTerrain(g);
        foreach (var enemy in _enemies.OrderByDescending(x => Depth(x.Position))) DrawEnemy(g, enemy);
        DrawPlayer(g);
        DrawHud(g);
        if (_inventory) DrawInventory(g);
        if (_quest) DrawQuest(g);
        if (_menu) DrawMenu(g);
    }

    private void DrawSky(Graphics g)
    {
        using var b = new LinearGradientBrush(ClientRectangle, Color.FromArgb(16, 31, 55), Color.FromArgb(3, 5, 10), LinearGradientMode.Vertical);
        g.FillRectangle(b, ClientRectangle);
        using var sun = new SolidBrush(Color.FromArgb(190, 220, 180));
        g.FillEllipse(sun, Width - 190, 90, 80, 80);
    }

    private void DrawTerrain(Graphics g)
    {
        var grid = new List<(float depth, PointF[] p)>();
        for (int y = -6; y < 65; y += 4)
        for (int x = -50; x <= 50; x += 4)
        {
            var a = new Vector3(x, y, Height(x, y));
            var b = new Vector3(x + 4, y, Height(x + 4, y));
            var c = new Vector3(x + 4, y + 4, Height(x + 4, y + 4));
            var d = new Vector3(x, y + 4, Height(x, y + 4));
            var pa = Project(a); var pb = Project(b); var pc = Project(c); var pd = Project(d);
            grid.Add((y, new[] { pa, pb, pc, pd }));
        }
        foreach (var cell in grid.OrderByDescending(x => x.depth))
        {
            var shade = Math.Clamp(24 + (int)(cell.depth * .55f), 24, 70);
            using var brush = new SolidBrush(Color.FromArgb(shade, shade + 22, shade + 14));
            g.FillPolygon(brush, cell.p);
            using var pen = new Pen(Color.FromArgb(18, 70, 60, 50), 1);
            g.DrawPolygon(pen, cell.p);
        }
        for (int i = -30; i < 45; i += 6)
        {
            DrawTree(g, new Vector3(i, 18 + (i % 5) * 3, Height(i, 18) + 1.5f));
            DrawTree(g, new Vector3(i + 38, 38 + (i % 4) * 2, Height(i + 38, 38) + 1.5f));
        }
    }

    private void DrawTree(Graphics g, Vector3 p)
    {
        var s = Project(p); var baseY = s.Y;
        using var trunk = new SolidBrush(Color.FromArgb(74, 45, 30));
        g.FillRectangle(trunk, s.X - 4, baseY - 35, 8, 35);
        using var leaves = new SolidBrush(Color.FromArgb(30, 95, 68));
        g.FillEllipse(leaves, s.X - 25, baseY - 75, 50, 48);
        g.FillEllipse(leaves, s.X - 15, baseY - 95, 38, 45);
    }

    private void DrawPlayer(Graphics g)
    {
        var p = Project(_player);
        using var aura = new SolidBrush(Color.FromArgb(45, 80, 190, 255));
        g.FillEllipse(aura, p.X - 25, p.Y - 14, 50, 28);
        using var body = new SolidBrush(Color.FromArgb(55, 115, 180));
        g.FillEllipse(body, p.X - 13, p.Y - 50, 26, 44);
        using var head = new SolidBrush(Color.FromArgb(222, 180, 145));
        g.FillEllipse(head, p.X - 10, p.Y - 70, 20, 20);
        using var cloak = new Pen(Color.FromArgb(120, 190, 235), 4);
        g.DrawLine(cloak, p.X - 12, p.Y - 48, p.X - 23, p.Y - 10);
        g.DrawLine(cloak, p.X + 12, p.Y - 48, p.X + 23, p.Y - 10);
        if (_attackFlash > 0)
        {
            using var arc = new Pen(Color.FromArgb((int)(255 * _attackFlash), 110, 220, 255), 6);
            g.DrawArc(arc, p.X - 65, p.Y - 75, 130, 110, 205, 130);
        }
    }

    private void DrawEnemy(Graphics g, Actor a)
    {
        var p = Project(a.Position);
        var size = 24 + a.Type * 4;
        using var shadow = new SolidBrush(Color.FromArgb(80, 0, 0, 0));
        g.FillEllipse(shadow, p.X - size, p.Y - 8, size * 2, 16);
        using var body = new SolidBrush(a.Type switch { 0 => Color.FromArgb(145, 60, 70), 1 => Color.FromArgb(95, 75, 135), _ => Color.FromArgb(45, 120, 95) });
        g.FillEllipse(body, p.X - size / 2, p.Y - size, size, size + 20);
        using var eye = new SolidBrush(Color.FromArgb(255, 190, 80));
        g.FillEllipse(eye, p.X - 8, p.Y - size + 8, 5, 5); g.FillEllipse(eye, p.X + 3, p.Y - size + 8, 5, 5);
        using var hp = new Pen(Color.FromArgb(210, 70, 70), 4);
        g.DrawLine(hp, p.X - 22, p.Y - size - 8, p.X + 22, p.Y - size - 8);
        using var hp2 = new Pen(Color.FromArgb(65, 200, 100), 3);
        g.DrawLine(hp2, p.X - 22, p.Y - size - 8, p.X - 22 + 44 * Math.Clamp(a.Hp / 100f, 0, 1), p.Y - size - 8);
    }

    private void DrawHud(Graphics g)
    {
        using var panel = new SolidBrush(Color.FromArgb(185, 7, 12, 20));
        g.FillRectangle(panel, 18, 18, 430, 92);
        using var title = new Font("Segoe UI", 14, FontStyle.Bold);
        using var small = new Font("Segoe UI", 10);
        using var text = new SolidBrush(Color.WhiteSmoke);
        g.DrawString("ETHEREO PANDORA", title, text, 34, 28);
        g.DrawString("Level 12  •  Vanguard  •  The Verdant March", small, Brushes.LightGray, 34, 55);
        Bar(g, 34, 78, 180, 12, _hp / 100f, Color.Firebrick, "HP");
        Bar(g, 230, 78, 180, 12, _mana / 100f, Color.DodgerBlue, "MP");
        g.DrawString("WASD Move   •   Right Mouse Camera   •   Space/LMB Attack   •   I Inventory   •   Q Quest", small, Brushes.WhiteSmoke, 24, Height - 32);
        using var mini = new SolidBrush(Color.FromArgb(190, 5, 9, 14));
        g.FillRectangle(mini, Width - 220, 18, 200, 150);
        g.DrawString("WORLD MAP", title, text, Width - 205, 28);
        g.DrawRectangle(Pens.SlateGray, Width - 205, 60, 165, 90);
        g.FillEllipse(Brushes.DeepSkyBlue, Width - 124, 99, 8, 8);
    }

    private static void Bar(Graphics g, int x, int y, int w, int h, float value, Color color, string label)
    {
        using var bg = new SolidBrush(Color.FromArgb(80, 0, 0, 0)); g.FillRectangle(bg, x, y, w, h);
        using var fg = new SolidBrush(color); g.FillRectangle(fg, x, y, (int)(w * Math.Clamp(value, 0, 1)), h);
        g.DrawString(label, SystemFonts.DefaultFont, Brushes.White, x + 5, y - 1);
    }

    private void DrawInventory(Graphics g)
    {
        using var b = new SolidBrush(Color.FromArgb(235, 9, 14, 23)); g.FillRectangle(b, 390, 150, 660, 510);
        using var f = new Font("Segoe UI", 20, FontStyle.Bold); g.DrawString("INVENTORY", f, Brushes.WhiteSmoke, 420, 175);
        for (int i = 0; i < 24; i++) { int x = 430 + (i % 8) * 72, y = 235 + (i / 8) * 95; g.DrawRectangle(Pens.SlateGray, x, y, 58, 70); g.FillEllipse(new SolidBrush(Color.FromArgb(45 + i * 3, 90, 130)), x + 14, y + 12, 30, 30); }
    }

    private void DrawQuest(Graphics g)
    {
        using var b = new SolidBrush(Color.FromArgb(235, 12, 16, 24)); g.FillRectangle(b, 430, 170, 580, 360);
        using var f = new Font("Segoe UI", 20, FontStyle.Bold); g.DrawString("QUEST LOG", f, Brushes.WhiteSmoke, 460, 200);
        using var s = new Font("Segoe UI", 12); g.DrawString("The Fallen Temple", s, Brushes.Gold, 460, 255); g.DrawString("Explore the ruins and recover the Astral Sigil.", s, Brushes.WhiteSmoke, 460, 290); g.DrawString("Progress: 0 / 1", s, Brushes.LightGray, 460, 330); g.DrawString("Reward: 2,500 XP • 450 Gold • Rare Cache", s, Brushes.LightGreen, 460, 380);
    }

    private void DrawMenu(Graphics g)
    {
        using var b = new SolidBrush(Color.FromArgb(220, 2, 5, 9)); g.FillRectangle(b, 0, 0, Width, Height);
        using var f = new Font("Segoe UI", 30, FontStyle.Bold); g.DrawString("ETHEREO PANDORA", f, Brushes.WhiteSmoke, 520, 230);
        using var s = new Font("Segoe UI", 16); g.DrawString("ESC Resume", s, Brushes.Gold, 625, 310); g.DrawString("I Inventory", s, Brushes.WhiteSmoke, 625, 350); g.DrawString("Q Quest Log", s, Brushes.WhiteSmoke, 625, 390);
    }

    private PointF Project(Vector3 world)
    {
        var dx = world.X - _player.X; var dy = world.Y - _player.Y; var dz = world.Z;
        var cy = MathF.Cos(-_yaw), sy = MathF.Sin(-_yaw);
        var x = dx * cy - dy * sy; var z = dx * sy + dy * cy;
        var cp = MathF.Cos(_pitch), sp = MathF.Sin(_pitch);
        var y = dz * cp - z * sp; var depth = dz * sp + z * cp + 18;
        var scale = 720f / Math.Max(3, depth);
        return new PointF(Width / 2f + x * scale, Height * .52f - y * scale);
    }

    private float Depth(Vector3 p) => Vector3.DistanceSquared(new Vector3(_player.X, _player.Y, 0), new Vector3(p.X, p.Y, 0));
    private static float Height(float x, float y) => MathF.Sin(x * .08f) * .35f + MathF.Cos(y * .07f) * .25f + MathF.Sin((x + y) * .035f) * .5f;

    private sealed class Actor
    {
        public Vector3 Position;
        public float Hp = 100;
        public int Type;
        public Actor(Vector3 position, int type) { Position = position; Type = type; }
    }
}
