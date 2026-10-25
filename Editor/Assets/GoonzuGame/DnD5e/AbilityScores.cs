namespace GoonzuGame.DnD5e
{
    public class AbilityScores
    {
        public int STR { get; set; }
        public int DEX { get; set; }
        public int CON { get; set; }
        public int INT { get; set; }
        public int WIS { get; set; }
        public int CHA { get; set; }
        public AbilityScores(int str, int dex, int con, int intel, int wis, int cha)
        {
            STR = str;
            DEX = dex;
            CON = con;
            INT = intel;
            WIS = wis;
            CHA = cha;
        }
    }
}