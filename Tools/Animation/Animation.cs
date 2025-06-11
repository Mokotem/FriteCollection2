
namespace FriteCollection2.Tools.Animation
{
    public class Animation
    {
        public delegate void KeyFrame(float dt);

        private readonly KeyFrame[] frames;
        private readonly float[] durations;
        private float start;

        public Animation(KeyFrame[] frames, float[] durations, float startTime = 0f)
        {
            if (frames.Length < 1 || durations.Length != frames.Length)
                throw new System.Exception("Frame count should be the same as durations");

            this.frames = frames;
            this.durations = durations;
            this.start = startTime;
            currentKey = -1;
            b = 0f;
            a = 0f;
        }

        private int currentKey;
        private float a, b;

        public bool Loop = false;

        public bool Done => currentKey >= frames.Length;
        public int CurrentFrame => currentKey;

        public void Restart(float startTime = 0f)
        {
            this.start = startTime;
            currentKey = -1;
            b = 0f;
            a = 0f;
        }

        public void Animate(float timer)
        {
            while (currentKey < frames.Length
                && timer > start + b)
            {
                a = b;
                currentKey += 1;
                if (!Done)
                {
                    if (durations[currentKey] <= 0)
                    {
                        frames[currentKey](0);
                    }
                    else
                    {
                        b += durations[currentKey];
                    }
                }
            }
            if (!Done && currentKey >= 0)
            {
                frames[currentKey]((timer - a - start) / durations[currentKey]);
            }

            if (Loop && Done)
            {
                Restart(timer);
            }
        }
    }
}