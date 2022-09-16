using System.IO;
using System.Linq;

namespace DuckGame.BadApple
{
    internal static class Commands
    {
        public static void Initialize()
        {
            DevConsole.AddCommand(new CMD("badapple", () =>
            {
                Level currentLevel = Level.current;
                string requiredLevelName = Animation.RequiredLevelName;

                if (currentLevel is DuckGameTestArea)
                {
                    LevelData data = Editor._currentLevelData;

                    if (data is not null && Path.GetFileNameWithoutExtension(data.GetPath()) == requiredLevelName)
                    {
                        if (!currentLevel.things.OfType<Animation>().Any())
                            Level.Add(new Animation(-800f, -550f));

                        return;
                    }
                }

                DevConsole.Log($"Select level \"{requiredLevelName}\" in Editor to play the animation", Color.LightSkyBlue);
            }));
        }
    }
}
