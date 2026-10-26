using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.WinForms;
using System.Windows.Forms;

namespace EthereoPandora.Game;

public sealed class GameForm : Form
{
    private readonly Pandora3DViewport _viewport = new();
    private readonly Timer _timer = new() { Interval = 16 };
    private readonly Label _stats = new();
    private readonly ProgressBar _hp = new();
    private readonly ProgressBar _mana = new();
    private readonly Panel _inventory = new();
    private readonly Panel _quest = new();
    private readonly Panel _menu = new();

    public GameForm()
    {
        Text = "Ethereo Pandora — Ethereal Veni Runtime";
        ClientSize = new Size(1440, 900);
        MinimumSize = new Size(1100, 720);
        KeyPreview = true;
        BackColor = Color.FromArgb(5, 8, 13);
        BuildHud();
        Controls.Add(_viewport);
        _viewport.Dock = DockStyle.Fill;
        _viewport.BringToFront();
        BringHudToFront();
        KeyDown += OnKeyDown;
        _timer.Tick += (_, _) =>
        {
            _viewport.UpdateFrame(1f / 60f);
            _stats.Text = $"ETHEREO PANDORA   |   LEVEL 12 VANGUARD   |   VERDANT MARCH   |   ENEMIES: {_viewport.EnemyCount}";
            _hp.Value = Math.Clamp((int)_viewport.PlayerHp, 0, 100);
            _mana.Value = Math.Clamp((int)_viewport.PlayerMana, 0, 100);
        };
        Shown += (_, _) => { _viewport.Focus(); _timer.Start(); };
        FormClosed += (_, _) => _timer.Stop();
    }

    private void BuildHud()
    {
        _stats.Dock = DockStyle.Top;
        _stats.Height = 34;
        _stats.Padding = new Padding(16, 7, 0, 0);
        _stats.BackColor = Color.FromArgb(215, 8, 13, 22);
        _stats.ForeColor = Color.WhiteSmoke;
        _stats.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        _stats.Text = "ETHEREO PANDORA   |   LEVEL 12 VANGUARD   |   VERDANT MARCH";
        Controls.Add(_stats);

        _hp.Location = new Point(20, 48); _hp.Size = new Size(190, 18); _hp.Maximum = 100; _hp.Value = 100;
        _hp.Style = ProgressBarStyle.Continuous;
        _mana.Location = new Point(20, 70); _mana.Size = new Size(190, 18); _mana.Maximum = 100; _mana.Value = 100;
        _mana.Style = ProgressBarStyle.Continuous;
        Controls.Add(_hp); Controls.Add(_mana);

        AddButton("INVENTORY [I]", 20, 105, (_, _) => Toggle(_inventory));
        AddButton("QUESTS [Q]", 20, 140, (_, _) => Toggle(_quest));
        AddButton("MENU [ESC]", 20, 175, (_, _) => Toggle(_menu));

        BuildPanel(_inventory, "INVENTORY", "Astral Blade\nVerdant Mail\nHealing Flask x8\nMana Crystal x12\nAstral Sigil\nGold: 4,850", 360, 150, 470, 390);
        BuildPanel(_quest, "QUEST LOG", "THE FALLEN TEMPLE\n\nExplore the ruins and recover the Astral Sigil.\n\nProgress: 0 / 1\n\nReward\n2,500 XP • 450 Gold • Rare Cache", 400, 170, 560, 350);
        BuildPanel(_menu, "ETHEREO PANDORA", "RESUME\nSAVE GAME\nOPTIONS\nRETURN TO EDITOR\nEXIT", 500, 210, 440, 420);
    }

    private void AddButton(string text, int x, int y, EventHandler click)
    {
        var b = new Button { Text = text, Location = new Point(x, y), Size = new Size(160, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(35, 45, 60), ForeColor = Color.WhiteSmoke };
        b.Click += click; Controls.Add(b);
    }

    private static void BuildPanel(Panel panel, string title, string body, int x, int y, int w, int h)
    {
        panel.Visible = false; panel.Location = new Point(x, y); panel.Size = new Size(w, h); panel.BackColor = Color.FromArgb(240, 8, 13, 22); panel.BorderStyle = BorderStyle.FixedSingle;
        var t = new Label { Text = title, Dock = DockStyle.Top, Height = 55, Padding = new Padding(20, 14, 0, 0), Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.Gold };
        var c = new Label { Text = body, Dock = DockStyle.Fill, Padding = new Padding(22, 8, 20, 20), Font = new Font("Segoe UI", 12), ForeColor = Color.WhiteSmoke };
        panel.Controls.Add(c); panel.Controls.Add(t);
    }

    private void Toggle(Control panel) => panel.Visible = !panel.Visible;

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        _viewport.SetKey(e.KeyCode, true);
        if (e.KeyCode == Keys.I) Toggle(_inventory);
        if (e.KeyCode == Keys.Q) Toggle(_quest);
        if (e.KeyCode == Keys.Escape) Toggle(_menu);
        BringHudToFront();
    }

    private void BringHudToFront()
    {
        _stats.BringToFront(); _hp.BringToFront(); _mana.BringToFront(); _inventory.BringToFront(); _quest.BringToFront(); _menu.BringToFront();
        foreach (Control c in Controls) if (c is Button) c.BringToFront();
    }
}

public sealed class Pandora3DViewport : GLControl
{
    private readonly HashSet<Keys> _keys = new();
    private readonly List<EnemyActor> _enemies = new();
    private readonly Random _random = new(17);
    private int _shader;
    private int _vao;
    private int _vbo;
    private int _terrainVao;
    private int _terrainVbo;
    private int _terrainVertexCount;
    private int _modelLocation;
    private int _viewLocation;
    private int _projectionLocation;
    private Vector3 _player = new(0, 0, 1.2f);
    private float _yaw = 0.55f;
    private float _pitch = 0.62f;
    private float _distance = 16f;
    private float _hp = 100;
    private float _mana = 100;
    private float _attackCooldown;
    private Point _lastMouse;
    private bool _cameraDrag;

    public float PlayerHp => _hp;
    public float PlayerMana => _mana;
    public int EnemyCount => _enemies.Count;

    public Pandora3DViewport() : base(new GLControlSettings { API = ContextAPI.OpenGL, APIVersion = new Version(3, 3), Profile = ContextProfile.Core })
    {
        TabStop = true;
        BackColor = Color.Black;
        for (int i = 0; i < 14; i++)
            _enemies.Add(new EnemyActor(new Vector3((i % 7 - 3) * 5f, 8 + (i / 7) * 7f, 1.0f), i % 3));
        Load += (_, _) => InitializeRenderer();
        Resize += (_, _) => Invalidate();
        Paint += (_, _) => RenderFrame();
        MouseDown += OnMouseDown;
        MouseUp += (_, _) => _cameraDrag = false;
        MouseMove += OnMouseMove;
        MouseWheel += (_, e) => _distance = Math.Clamp(_distance - e.Delta * 0.01f, 7f, 28f);
        MouseClick += (_, e) => { if (e.Button == MouseButtons.Left) Attack(); };
    }

    public void SetKey(Keys key, bool down)
    {
        if (down) _keys.Add(key); else _keys.Remove(key);
        if (down && key == Keys.Space) Attack();
    }

    public void UpdateFrame(float dt)
    {
        var move = Vector2.Zero;
        if (_keys.Contains(Keys.W)) move.Y += 1;
        if (_keys.Contains(Keys.S)) move.Y -= 1;
        if (_keys.Contains(Keys.A)) move.X -= 1;
        if (_keys.Contains(Keys.D)) move.X += 1;
        if (move.LengthSquared > 0)
        {
            move = Vector2.Normalize(move);
            var forward = new Vector3(MathF.Sin(_yaw), MathF.Cos(_yaw), 0);
            var right = new Vector3(MathF.Cos(_yaw), -MathF.Sin(_yaw), 0);
            _player += (forward * move.Y + right * move.X) * (dt * 7f);
            _player.X = Math.Clamp(_player.X, -38, 38); _player.Y = Math.Clamp(_player.Y, -8, 55);
            _player.Z = TerrainHeight(_player.X, _player.Y) + 1.15f;
        }
        foreach (var e in _enemies)
        {
            var delta = _player - e.Position; delta.Z = 0;
            if (delta.LengthSquared > 4.5f * 4.5f)
                e.Position += Vector3.Normalize(delta) * dt * (1.15f + e.Type * .25f);
            else if (_random.NextDouble() < .02)
                _hp = Math.Max(0, _hp - (3 + e.Type));
            e.Position.Z = TerrainHeight(e.Position.X, e.Position.Y) + .9f;
        }
        _mana = Math.Min(100, _mana + dt * 4);
        _attackCooldown = Math.Max(0, _attackCooldown - dt);
        Invalidate();
    }

    private void Attack()
    {
        if (_attackCooldown > 0 || _mana < 12) return;
        _attackCooldown = .35f; _mana -= 12;
        var forward = new Vector3(MathF.Sin(_yaw), MathF.Cos(_yaw), 0);
        foreach (var enemy in _enemies)
        {
            var delta = enemy.Position - _player; delta.Z = 0;
            var d = delta.Length;
            if (d < 8 && d > .01f && Vector3.Dot(Vector3.Normalize(delta), forward) > -.15f)
                enemy.Hp -= 35;
        }
        _enemies.RemoveAll(x => x.Hp <= 0);
        while (_enemies.Count < 10)
            _enemies.Add(new EnemyActor(new Vector3(_random.Next(-35, 36), _random.Next(8, 50), 1), _random.Next(3)));
    }

    private void InitializeRenderer()
    {
        MakeShader(); MakeCube(); MakeTerrain();
        GL.Enable(EnableCap.DepthTest); GL.Enable(EnableCap.CullFace); GL.CullFace(TriangleFace.Back);
        GL.ClearColor(0.025f, 0.045f, 0.075f, 1f);
    }

    private void MakeShader()
    {
        const string vertex = "#version 330 core\nlayout(location=0) in vec3 aPosition; layout(location=1) in vec3 aColor; uniform mat4 uModel; uniform mat4 uView; uniform mat4 uProjection; out vec3 vColor; void main(){ vColor=aColor; gl_Position=uProjection*uView*uModel*vec4(aPosition,1.0); }";
        const string fragment = "#version 330 core\nin vec3 vColor; out vec4 FragColor; void main(){ FragColor=vec4(vColor,1.0); }";
        int vs = GL.CreateShader(ShaderType.VertexShader); GL.ShaderSource(vs, vertex); GL.CompileShader(vs); CheckShader(vs);
        int fs = GL.CreateShader(ShaderType.FragmentShader); GL.ShaderSource(fs, fragment); GL.CompileShader(fs); CheckShader(fs);
        _shader = GL.CreateProgram(); GL.AttachShader(_shader, vs); GL.AttachShader(_shader, fs); GL.LinkProgram(_shader);
        GL.DeleteShader(vs); GL.DeleteShader(fs);
        _modelLocation = GL.GetUniformLocation(_shader, "uModel"); _viewLocation = GL.GetUniformLocation(_shader, "uView"); _projectionLocation = GL.GetUniformLocation(_shader, "uProjection");
    }

    private static void CheckShader(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int ok);
        if (ok == 0) throw new InvalidOperationException(GL.GetShaderInfoLog(shader));
    }

    private void MakeCube()
    {
        float[] v =
        {
            -1,-1,-1, .18f,.25f,.30f, 1,-1,-1, .18f,.25f,.30f, 1,1,-1,.18f,.25f,.30f, -1,1,-1,.18f,.25f,.30f,
            -1,-1,1, .35f,.45f,.48f, 1,-1,1,.35f,.45f,.48f, 1,1,1,.35f,.45f,.48f, -1,1,1,.35f,.45f,.48f
        };
        uint[] i = {0,1,2,2,3,0, 4,6,5,6,4,7, 0,4,5,5,1,0, 2,6,7,7,3,2, 0,3,7,7,4,0, 1,5,6,6,2,1};
        var data = new float[i.Length * 6]; for(int n=0;n<i.Length;n++){ for(int k=0;k<6;k++) data[n*6+k]=v[i[n]*6+k]; }
        _vao = GL.GenVertexArray(); _vbo = GL.GenBuffer(); GL.BindVertexArray(_vao); GL.BindBuffer(BufferTarget.ArrayBuffer,_vbo); GL.BufferData(BufferTarget.ArrayBuffer,data.Length*sizeof(float),data,BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,6*sizeof(float),0); GL.EnableVertexAttribArray(0); GL.VertexAttribPointer(1,3,VertexAttribPointerType.Float,false,6*sizeof(float),3*sizeof(float)); GL.EnableVertexAttribArray(1); GL.BindVertexArray(0);
    }

    private void MakeTerrain()
    {
        var data = new List<float>();
        for(int y=-10;y<60;y++) for(int x=-45;x<45;x++)
        {
            AddTerrainTri(data,x,y,x+1,y,x+1,y+1); AddTerrainTri(data,x,y,x+1,y+1,x,y+1);
        }
        _terrainVertexCount=data.Count/6; _terrainVao=GL.GenVertexArray(); _terrainVbo=GL.GenBuffer(); GL.BindVertexArray(_terrainVao); GL.BindBuffer(BufferTarget.ArrayBuffer,_terrainVbo); GL.BufferData(BufferTarget.ArrayBuffer,data.Count*sizeof(float),data.ToArray(),BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0,3,VertexAttribPointerType.Float,false,6*sizeof(float),0); GL.EnableVertexAttribArray(0); GL.VertexAttribPointer(1,3,VertexAttribPointerType.Float,false,6*sizeof(float),3*sizeof(float)); GL.EnableVertexAttribArray(1); GL.BindVertexArray(0);
    }

    private static void AddTerrainTri(List<float> d,int ax,int ay,int bx,int by,int cx,int cy)
    {
        AddTerrainVertex(d,ax,ay); AddTerrainVertex(d,bx,by); AddTerrainVertex(d,cx,cy);
    }
    private static void AddTerrainVertex(List<float> d,int x,int y){ float z=TerrainHeight(x,y); float shade=.18f+Math.Clamp(z*.025f,0,.12f); d.AddRange(new[]{(float)x,(float)y,z,shade,.34f+shade,.23f+shade*.5f}); }
    private static float TerrainHeight(float x,float y)=> MathF.Sin(x*.17f)*.7f+MathF.Cos(y*.13f)*.55f+MathF.Sin((x+y)*.07f)*1.0f;

    private void RenderFrame()
    {
        if (_shader == 0) return;
        GL.Clear(ClearBufferMask.ColorBufferBit|ClearBufferMask.DepthBufferBit); GL.UseProgram(_shader);
        var aspect=Math.Max(1f,Width/(float)Math.Max(1,Height)); var projection=Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(58),aspect,.1f,180f);
        var target=_player+new Vector3(0,0,1); var horizontal=MathF.Cos(_pitch); var camera=target+new Vector3(MathF.Sin(_yaw)*horizontal,MathF.Cos(_yaw)*horizontal,MathF.Sin(_pitch))*_distance;
        var view=Matrix4.LookAt(camera,target,Vector3.UnitZ); GL.UniformMatrix4(_viewLocation,false,ref view); GL.UniformMatrix4(_projectionLocation,false,ref projection);
        GL.BindVertexArray(_terrainVao); var identity=Matrix4.Identity; GL.UniformMatrix4(_modelLocation,false,ref identity); GL.DrawArrays(PrimitiveType.Triangles,0,_terrainVertexCount); GL.BindVertexArray(0);
        DrawCube(new Vector3(0,18,3),new Vector3(9,6,6),new Vector3(.30f,.24f,.18f)); DrawCube(new Vector3(7,22,2.5f),new Vector3(3,3,5),new Vector3(.24f,.28f,.22f));
        for(int i=-35;i<40;i+=7){ DrawCube(new Vector3(i,30+(i%4)*2,TerrainHeight(i,30)+2.2f),new Vector3(.7f,.7f,2.2f),new Vector3(.22f,.12f,.07f)); DrawCube(new Vector3(i,30+(i%4)*2,TerrainHeight(i,30)+5.2f),new Vector3(2.5f,2.5f,2.5f),new Vector3(.08f,.32f,.18f)); }
        DrawCube(_player,new Vector3(1,1,1.15f),new Vector3(.12f,.35f,.70f)); DrawCube(_player+new Vector3(0,0,1.35f),new Vector3(.75f,.75f,.75f),new Vector3(.72f,.52f,.35f));
        foreach(var e in _enemies) DrawCube(e.Position,new Vector3(1.15f,1.15f,1.4f),e.Type switch{0=>new Vector3(.55f,.12f,.18f),1=>new Vector3(.22f,.14f,.55f),_=>new Vector3(.08f,.42f,.32f)});
        SwapBuffers();
    }

    private void DrawCube(Vector3 pos,Vector3 scale,Vector3 color)
    {
        GL.BindVertexArray(_vao); var model=Matrix4.CreateScale(scale)*Matrix4.CreateTranslation(pos); GL.UniformMatrix4(_modelLocation,false,ref model);
        GL.DrawArrays(PrimitiveType.Triangles,0,36); GL.BindVertexArray(0);
    }

    private void OnMouseDown(object? s,MouseEventArgs e){Focus();if(e.Button==MouseButtons.Right){_cameraDrag=true;_lastMouse=e.Location;}}
    private void OnMouseMove(object? s,MouseEventArgs e){if(!_cameraDrag)return;_yaw+=(e.X-_lastMouse.X)*.008f;_pitch=Math.Clamp(_pitch-(e.Y-_lastMouse.Y)*.004f,.25f,1.15f);_lastMouse=e.Location;}
}

public sealed class EnemyActor
{
    public Vector3 Position;
    public float Hp=100;
    public int Type;
    public EnemyActor(Vector3 position,int type){Position=position;Type=type;}
}
