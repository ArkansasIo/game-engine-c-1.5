using GoonzuGame.GUI;
namespace GoonzuGame.GUI
{
    public class GoonzuHUD : UIWindow
    {
        public void ShowHUD()
        {
            Show();
            System.Console.WriteLine("HUD shown.");
        }
        public void UpdateHUD()
        {
            System.Console.WriteLine("HUD updated.");
        }
    }
}
