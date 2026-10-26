using System.Windows.Forms;

namespace EtherealVeni.Editor;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new EditorForm());
    }
}
