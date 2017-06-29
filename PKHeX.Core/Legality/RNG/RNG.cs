﻿namespace PKHeX.Core
{
    public class RNG
    {
        public static readonly RNG LCRNG = new RNG(0x41C64E6D, 0x00006073, 0xEEB9EB65, 0x0A3561A1);
        public static readonly RNG XDRNG = new RNG(0x000343FD, 0x00269EC3, 0xB9B33155, 0xA170F641);
        public static readonly RNG ARNG  = new RNG(0x6C078965, 0x00000001, 0x9638806D, 0x69C77F93);

        private readonly uint Mult, Add, rMult, rAdd;
        private RNG(uint f_mult, uint f_add, uint r_mult, uint r_add)
        {
            Mult = f_mult;
            Add = f_add;
            rMult = r_mult;
            rAdd = r_add;
        }

        public uint Next(uint seed) => seed * Mult + Add;
        public uint Prev(uint seed) => seed * rMult + rAdd;

        public uint Advance(uint seed, int frames)
        {
            for (int i = 0; i < frames; i++)
                seed = Next(seed);
            return seed;
        }
        public uint Reverse(uint seed, int frames)
        {
            for (int i = 0; i < frames; i++)
                seed = Prev(seed);
            return seed;
        }
    }
}
