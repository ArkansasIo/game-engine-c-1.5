namespace GameEngine.V15.World;

public sealed class ProceduralSeed
{
    public int Seed {get;}
    public ProceduralSeed(int seed)=>Seed=seed;
    public float Sample(int x,int y,int z=0){unchecked{uint h=(uint)(x*374761393+y*668265263+z*2147483647+Seed);h^=h>>13;h*=1274126177;h^=h>>16;return (h%100000)/100000f;}}
}
