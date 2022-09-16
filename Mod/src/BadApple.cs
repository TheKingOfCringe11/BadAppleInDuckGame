using System.Reflection;

[assembly: AssemblyTitle("|WHITE|BadApple")]
[assembly: AssemblyCompany("|GREEN|TheKingOfCringe|RED|11")]
[assembly: AssemblyVersion("1.0.0.0")]


namespace DuckGame.BadApple
{
    public class BadApple : Mod
    {
        protected override void OnPostInitialize()
        {
            FrameData.Load();
            Commands.Initialize();
        }
    }
}
