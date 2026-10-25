namespace GameEngine.V15.Audio;

public sealed record AudioCue(string Id,string Path,float Volume=1,float Pitch=1);
public sealed class AudioSystem
{
    private readonly Dictionary<string,AudioCue> _cues=new();
    public void Register(AudioCue cue)=>_cues[cue.Id]=cue;
    public AudioCue? Get(string id)=>_cues.TryGetValue(id,out var cue)?cue:null;
}
