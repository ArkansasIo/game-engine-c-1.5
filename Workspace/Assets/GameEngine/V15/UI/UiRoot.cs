namespace GameEngine.V15.UI;

public readonly record struct UiRect(float X,float Y,float Width,float Height);
public sealed class UiWidget
{
    public string Id {get;}
    public UiRect Rect {get;set;}
    public bool Visible {get;set;}=true;
    public UiWidget(string id)=>Id=id;
}
public sealed class UiRoot
{
    private readonly Dictionary<string,UiWidget> _widgets=new();
    public void Add(UiWidget widget)=>_widgets[widget.Id]=widget;
    public UiWidget? Find(string id)=>_widgets.TryGetValue(id,out var w)?w:null;
}
