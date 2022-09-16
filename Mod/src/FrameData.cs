using System;
using System.Collections.Generic;
using System.IO;

namespace DuckGame.BadApple
{
    public static class FrameData
    {
        private static List<IEnumerable<Vec2>> s_frames = new List<IEnumerable<Vec2>>();
        private static readonly Vec2 s_tileSize = new Vec2(16f);

        private static readonly double s_gameFrameRate = 61d;
        private static readonly double s_animationFrameRate = 30d; 

        public static void Load()
        {
            s_frames.Clear();

            using (FileStream stream = File.OpenRead(Mod.GetPath<BadApple>("frameData.dat")))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    while (reader.PeekChar() >= 0)
                    {
                        var positions = new List<Vec2>();

                        if (reader.ReadBoolean())
                        {
                            int intSize = sizeof(int);

                            int count = reader.ReadInt32();
                            byte[] data = reader.ReadBytes(count);

                            for (int i = 0; i < data.Length; i += intSize * 2)
                            {
                                float x = BitConverter.ToInt32(data, i);
                                float y = BitConverter.ToInt32(data, i + intSize);

                                positions.Add(new Vec2(x, y) * s_tileSize);
                            }
                        }

                        s_frames.Add(positions);
                    }
                }
            }
        }

        public static bool TryGetPositions(int frame, out IEnumerable<Vec2> positions)
        {
            frame = (int)(frame / (s_gameFrameRate / s_animationFrameRate));

            if (frame >= s_frames.Count)
            {
                positions = default(IEnumerable<Vec2>);
                return false;
            }

            positions = s_frames[frame];
            return true;
        }
    }
}
