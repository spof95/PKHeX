﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public class EncounterArea
    {
        public int Location;
        public EncounterSlot[] Slots;
        public EncounterArea() { }

        private EncounterArea(byte[] data)
        {
            Location = BitConverter.ToUInt16(data, 0);
            Slots = new EncounterSlot[(data.Length - 2) / 4];
            for (int i = 0; i < Slots.Length; i++)
            {
                ushort SpecForm = BitConverter.ToUInt16(data, 2 + i * 4);
                Slots[i] = new EncounterSlot
                {
                    Species = SpecForm & 0x7FF,
                    Form = SpecForm >> 11,
                    LevelMin = data[4 + i * 4],
                    LevelMax = data[5 + i * 4],
                };
            }
        }

        private static EncounterSlot1[] getSlots1_GW(byte[] data, ref int ofs, SlotType t)
        {
            int rate = data[ofs++];
            return rate == 0 ? new EncounterSlot1[0] : readSlots(data, ref ofs, 10, t, rate);
        }
        private static EncounterSlot1[] getSlots1_F(byte[] data, ref int ofs)
        {
            int count = data[ofs++];
            return readSlots(data, ref ofs, count, SlotType.Super_Rod, -1);
        }
        
        private static EncounterSlot1[] getSlots2_GW(byte[] data, ref int ofs, SlotType t, int slotSets, int slotCount)
        {
            byte[] rates = new byte[slotSets];
            for (int i = 0; i < rates.Length; i++)
                rates[i] = data[ofs++];
            
            var slots = readSlots(data, ref ofs, slotSets * slotCount, t, rates[0]);
            for (int r = 1; r < slotSets; r++)
            {
                for (int i = 0; i < slotCount; i++)
                {
                    int index = i + r*slotCount;
                    slots[index].Rate = rates[r];
                    slots[index].SlotNumber = i;
                }
            }

            return slots;
        }

        private static EncounterSlot1[] getSlots2_F(byte[] data, ref int ofs, SlotType t)
        {
            // slot set ends in 0xFF 0x** 0x**
            var slots = new List<EncounterSlot1>();
            while (true)
            {
                int rate = data[ofs++];
                int species = data[ofs++];
                int level = data[ofs++];

                slots.Add(new EncounterSlot1
                {
                    Rate = rate,
                    Species = species,
                    LevelMin = level,
                    LevelMax = level,
                    Type = species == 0 ? SlotType.Special : t // day/night specific
                });

                if (rate == 0xFF)
                    break;
            }
            return slots.ToArray();
        }

        private static IEnumerable<EncounterArea> getAreas2(byte[] data, ref int ofs, SlotType t, int slotSets, int slotCount)
        {
            var areas = new List<EncounterArea>();
            while (data[ofs] != 0xFF) // end
            {
                areas.Add(new EncounterArea
                {
                    Location = data[ofs++] << 8 | data[ofs++],
                    Slots = getSlots2_GW(data, ref ofs, t, slotSets, slotCount),
                });
            }
            return areas;
        }
        private static IEnumerable<EncounterArea> getAreas2_F(byte[] data, ref int ofs, SlotType t)
        {
            var areas = new List<EncounterArea>();
            var types = new[] {SlotType.Old_Rod, SlotType.Good_Rod, SlotType.Super_Rod};
            while (data.Length < ofs)
            {
                int count = 0;
                while (ofs != 0x18D)
                {
                    areas.Add(new EncounterArea
                    {
                        Location = count++,
                        Slots = getSlots2_F(data, ref ofs, types[count%3]),
                    });
                }
            }
            // Read TimeFishGroups
            var dl = new List<DexLevel>();
            while (data.Length < ofs)
                dl.Add(new DexLevel {Species = data[ofs++], Level = data[ofs++]});

            // Add TimeSlots
            foreach (var area in areas)
            {
                var slots = area.Slots;
                for (int i = 0; i < slots.Length; i++)
                {
                    var slot = slots[i];
                    if (slot.Type != SlotType.Special)
                        continue;

                    Array.Resize(ref slots, slots.Length + 1);
                    Array.Copy(slots, i, slots, i+1, slots.Length - i);
                    slots[i+1] = slot.Clone(); // differentiate copied slot

                    int index = slot.LevelMin*2;
                    for (int j = 0; j < 2; j++) // load special slot info
                    {
                        var s = slots[i + j];
                        s.Species = dl[index + j].Species;
                        s.LevelMin = s.LevelMax = dl[index + j].MinLevel;
                        s.Type = slots[0].Type; // special slots are never first, so copy first slot type
                    }
                }
            }
            return areas;
        }

        /// <summary>
        /// RBY Format Slot Getter from data.
        /// </summary>
        /// <param name="data">Byte array containing complete slot data table.</param>
        /// <param name="ofs">Offset to start reading from.</param>
        /// <param name="count">Amount of slots to read.</param>
        /// <param name="t">Type of encounter slot.</param>
        /// <param name="rate">Slot type encounter rate.</param>
        /// <returns>Array of encounter slots.</returns>
        private static EncounterSlot1[] readSlots(byte[] data, ref int ofs, int count, SlotType t, int rate)
        {
            EncounterSlot1[] slots = new EncounterSlot1[count];
            for (int i = 0; i < count; i++)
            {
                int lvl = data[ofs++];
                int spec = data[ofs++];

                slots[i] = new EncounterSlot1
                {
                    LevelMax = lvl,
                    LevelMin = lvl,
                    Species = spec,
                    Type = t,
                    Rate = rate,
                    SlotNumber = i,
                };
            }
            return slots;
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 1 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray1_GW(byte[] data)
        {
            // RBY Format
            var ptr = new int[255];
            int count = 0;
            for (int i = 0; i < ptr.Length; i++)
            {
                ptr[i] = BitConverter.ToInt16(data, i*2);
                if (ptr[i] != -1)
                    continue;

                count = i;
                break;
            }

            EncounterArea[] areas = new EncounterArea[count];
            for (int i = 0; i < areas.Length; i++)
            {
                var grass = getSlots1_GW(data, ref ptr[i], SlotType.Grass);
                var water = getSlots1_GW(data, ref ptr[i], SlotType.Surf);
                areas[i] = new EncounterArea
                {
                    Location = i,
                    Slots = grass.Concat(water).ToArray()
                };
            }
            return areas.Where(area => area.Slots.Any()).ToArray();
        }
        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Pokémon Yellow (Generation 1) Fishing data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray1_FY(byte[] data)
        {
            const int size = 9;
            int count = data.Length/size;
            EncounterArea[] areas = new EncounterArea[count];
            for (int i = 0; i < count; i++)
            {
                int ofs = i*size + 1;
                areas[i] = new EncounterArea
                {
                    Location = data[i*size + 0],
                    Slots = readSlots(data, ref ofs, 4, SlotType.Super_Rod, -1)
                };
            }
            return areas;
        }
        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 1 Fishing data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray1_F(byte[] data)
        {
            var ptr = new int[255];
            var map = new int[255];
            int count = 0;
            for (int i = 0; i < ptr.Length; i++)
            {
                map[i] = data[i*3 + 0];
                if (map[i] == 0xFF)
                {
                    count = i;
                    break;
                }
                ptr[i] = BitConverter.ToInt16(data, i * 3 + 1);
            }

            EncounterArea[] areas = new EncounterArea[count];
            for (int i = 0; i < areas.Length; i++)
            {
                areas[i] = new EncounterArea
                {
                    Location = map[i],
                    Slots = getSlots1_F(data, ref ptr[i])
                };
            }
            return areas;
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 2 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray2_GW(byte[] data)
        {
            int ofs = 0;
            var areas = new List<EncounterArea>();
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Grass,     3, 7)); // Johto Grass
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Surf,      1, 3)); // Johto Water
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Grass,     3, 7)); // Kanto Grass
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Surf,      1, 3)); // Kanto Water
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Swarm,     3, 7)); // Swarm
            areas.AddRange(getAreas2(data, ref ofs, SlotType.Special,   1, 3)); // Union Cave
            return areas.ToArray();
        }

        /// <summary>
        /// Gets the encounter areas with <see cref="EncounterSlot"/> information from Generation 2 Grass/Water data.
        /// </summary>
        /// <param name="data">Input raw data.</param>
        /// <returns>Array of encounter areas.</returns>
        public static EncounterArea[] getArray2_F(byte[] data)
        {
            int ofs = 0;
            return getAreas2_F(data, ref ofs, SlotType.Any).ToArray();
        }

        public static EncounterArea[] getArray(byte[][] entries)
        {
            if (entries == null)
                return null;

            EncounterArea[] data = new EncounterArea[entries.Length];
            for (int i = 0; i < data.Length; i++)
                data[i] = new EncounterArea(entries[i]);
            return data;
        }
    }
}
