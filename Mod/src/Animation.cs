using System.Collections.Generic;

namespace DuckGame.BadApple
{
    public sealed class Animation : Thing
    {
        public const string RequiredLevelName = "Bad Apple";

        private readonly List<Crate> _spawnedCrates = new List<Crate>();
        private int _frame;

        public Animation(float x, float y) : base(x, y)
        {

        }

        public override void Initialize()
        {
            Music.Play(Mod.GetPath<BadApple>("music.ogg"), false);
        }

        public override void Update()
        {
            if (!FrameData.TryGetPositions(_frame, out IEnumerable<Vec2> positions))
            {
                Level.Remove(this);
                return;
            }

            foreach (Crate crate in _spawnedCrates)
                Level.Remove(crate);

            _spawnedCrates.Clear();

            foreach (Vec2 position in positions)
            {
                var crate = new Crate(x + position.x, y + position.y)
                {
                    updatePhysics = false
                };

                _spawnedCrates.Add(crate);

                Level.Add(crate);
            }

            _frame++;
        }

        public override void Terminate()
        {
            Music.Stop();
        }
    }
}
