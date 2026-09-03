using System;

namespace MapCreator.Controls.UserSubmittedUtilities.TerrainGenerator
{
    public class PerlinNoise
    {
        private int[] permutation;

        public PerlinNoise()
        {
            permutation = new int[512];
            Random random = new Random();
            for (int i = 0; i < 256; i++)
                permutation[i] = i;
            Shuffle(permutation, random);
            for (int i = 0; i < 256; i++)
                permutation[256 + i] = permutation[i];
        }

        private void Shuffle(int[] array, Random random)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        private float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
        private float Lerp(float t, float a, float b) => a + t * (b - a);
        private float Grad(int hash, float x, float y)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : h == 12 || h == 14 ? x : 0;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        public float Noise(float x, float y)
        {
            int X = (int)Math.Floor(x) & 255;
            int Y = (int)Math.Floor(y) & 255;
            x -= (float)Math.Floor(x);
            y -= (float)Math.Floor(y);
            float u = Fade(x);
            float v = Fade(y);

            int A = permutation[X] + Y;
            int B = permutation[X + 1] + Y;

            return Lerp(v,
                Lerp(u, Grad(permutation[A], x, y), Grad(permutation[B], x - 1, y)),
                Lerp(u, Grad(permutation[A + 1], x, y - 1), Grad(permutation[B + 1], x - 1, y - 1)));
        }

        public float FBM(float x, float y, int octaves)
        {
            float total = 0;
            float frequency = 1;
            float amplitude = 1;
            float maxValue = 0;

            for (int i = 0; i < octaves; i++)
            {
                total += Noise(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= 0.5f;
                frequency *= 2;
            }

            return total / maxValue;
        }
    }
}
