﻿using System;
using System.Linq;

namespace PKHeX.Core
{
    public abstract class PKM
    {
        public static readonly string[] Extensions = PKX.getPKMExtensions();
        public abstract int SIZE_PARTY { get; }
        public abstract int SIZE_STORED { get; }
        public virtual string Extension => "pk" + Format;
        public abstract PersonalInfo PersonalInfo { get; }

        // Internal Attributes set on creation
        public byte[] Data; // Raw Storage
        public string Identifier; // User or Form Custom Attribute
        public int Box { get; set; } = -1; // Batch Editor
        public int Slot { get; set; } = -1; // Batch Editor

        public virtual byte[] EncryptedPartyData => Encrypt().Take(SIZE_PARTY).ToArray();
        public virtual byte[] EncryptedBoxData => Encrypt().Take(SIZE_STORED).ToArray();
        public virtual byte[] DecryptedPartyData => Write().Take(SIZE_PARTY).ToArray();
        public virtual byte[] DecryptedBoxData => Write().Take(SIZE_STORED).ToArray();
        public virtual bool Valid { get { return ChecksumValid && Sanity == 0; } set { if (!value) return; Sanity = 0; RefreshChecksum(); } }

        protected virtual ushort CalculateChecksum()
        {
            ushort chk = 0;
            switch (Format)
            {
                case 3:
                    for (int i = 32; i < SIZE_STORED; i += 2)
                        chk += BitConverter.ToUInt16(Data, i);
                    return chk;
                default: // 4+
                    for (int i = 8; i < SIZE_STORED; i += 2)
                        chk += BitConverter.ToUInt16(Data, i);
                    return chk;
            }
        }
        public abstract byte[] Encrypt();
        public abstract int Format { get; }
        public byte[] Write()
        {
            RefreshChecksum();
            return Data;
        }

        // Surface Properties
        public abstract int Species { get; set; }
        public abstract string Nickname { get; set; }
        public abstract int HeldItem { get; set; }
        public abstract int Gender { get; set; }
        public abstract int Nature { get; set; }
        public abstract int Ability { get; set; }
        public abstract int CurrentFriendship { get; set; }
        public abstract int AltForm { get; set; }
        public abstract bool IsEgg { get; set; }
        public abstract bool IsNicknamed { get; set; }
        public abstract uint EXP { get; set; }
        public abstract int TID { get; set; }
        public abstract string OT_Name { get; set; }
        public abstract int OT_Gender { get; set; }
        public abstract int Ball { get; set; }
        public abstract int Met_Level { get; set; }

        // Battle
        public abstract int Move1 { get; set; }
        public abstract int Move2 { get; set; }
        public abstract int Move3 { get; set; }
        public abstract int Move4 { get; set; }
        public abstract int Move1_PP { get; set; }
        public abstract int Move2_PP { get; set; }
        public abstract int Move3_PP { get; set; }
        public abstract int Move4_PP { get; set; }
        public abstract int Move1_PPUps { get; set; }
        public abstract int Move2_PPUps { get; set; }
        public abstract int Move3_PPUps { get; set; }
        public abstract int Move4_PPUps { get; set; }
        public abstract int EV_HP { get; set; }
        public abstract int EV_ATK { get; set; }
        public abstract int EV_DEF { get; set; }
        public abstract int EV_SPE { get; set; }
        public abstract int EV_SPA { get; set; }
        public abstract int EV_SPD { get; set; }
        public abstract int IV_HP { get; set; }
        public abstract int IV_ATK { get; set; }
        public abstract int IV_DEF { get; set; }
        public abstract int IV_SPE { get; set; }
        public abstract int IV_SPA { get; set; }
        public abstract int IV_SPD { get; set; }
        public abstract int Stat_Level { get; set; }
        public abstract int Stat_HPMax { get; set; }
        public abstract int Stat_HPCurrent { get; set; }
        public abstract int Stat_ATK { get; set; }
        public abstract int Stat_DEF { get; set; }
        public abstract int Stat_SPE { get; set; }
        public abstract int Stat_SPA { get; set; }
        public abstract int Stat_SPD { get; set; }

        // Hidden Properties
        public abstract int Version { get; set; }
        public abstract int SID { get; set; }
        public abstract int PKRS_Strain { get; set; }
        public abstract int PKRS_Days { get; set; }
        public abstract int CNT_Cool { get; set; }
        public abstract int CNT_Beauty { get; set; }
        public abstract int CNT_Cute { get; set; }
        public abstract int CNT_Smart { get; set; }
        public abstract int CNT_Tough { get; set; }
        public abstract int CNT_Sheen { get; set; }

        public abstract uint EncryptionConstant { get; set; }
        public abstract uint PID { get; set; }
        public abstract ushort Sanity { get; set; }
        public abstract ushort Checksum { get; set; }

        // Misc Properties
        public abstract int Language { get; set; }
        public abstract bool FatefulEncounter { get; set; }
        public abstract int TSV { get; }
        public abstract int PSV { get; }
        public abstract int Characteristic { get; }
        public abstract int MarkValue { get; protected set; }
        public abstract int Met_Location { get; set; }
        public abstract int Egg_Location { get; set; }
        public abstract int OT_Friendship { get; set; }

        // Future Properties
        public virtual int Met_Year { get { return 0; } set { } }
        public virtual int Met_Month { get { return 0; } set { } }
        public virtual int Met_Day { get { return 0; } set { } }
        public virtual string HT_Name { get; set; }
        public virtual int HT_Gender { get; set; }
        public virtual int HT_Affection { get; set; }
        public virtual int HT_Friendship { get; set; }
        public virtual int HT_Memory { get; set; }
        public virtual int HT_TextVar { get; set; }
        public virtual int HT_Feeling { get; set; }
        public virtual int HT_Intensity { get; set; }
        public virtual int OT_Memory { get; set; }
        public virtual int OT_TextVar { get; set; }
        public virtual int OT_Feeling { get; set; }
        public virtual int OT_Intensity { get; set; }
        public virtual int Geo1_Region { get; set; }
        public virtual int Geo2_Region { get; set; }
        public virtual int Geo3_Region { get; set; }
        public virtual int Geo4_Region { get; set; }
        public virtual int Geo5_Region { get; set; }
        public virtual int Geo1_Country { get; set; }
        public virtual int Geo2_Country { get; set; }
        public virtual int Geo3_Country { get; set; }
        public virtual int Geo4_Country { get; set; }
        public virtual int Geo5_Country { get; set; }
        public virtual byte Enjoyment { get; set; }
        public virtual byte Fullness { get; set; }
        public virtual int AbilityNumber { get; set; }
        public virtual int Country { get; set; }
        public virtual int Region { get; set; }
        public virtual int ConsoleRegion { get; set; }

        /// <summary>
        /// The date the Pokémon was met.
        /// </summary>
        /// <returns>A DateTime representing the date the Pokémon was met, or null if either the <see cref="PKM"/> format does not support dates or the stored date is invalid.</returns>
        /// <remarks>Not all <see cref="PKM"/> types support the <see cref="MetDate"/> property.  In these cases, this property will return null.
        /// 
        /// If null is assigned to this property, it will be cleared.</remarks>
        public virtual DateTime? MetDate
        {
            get
            {
                // Check to see if date is valid
                if (!Util.IsDateValid(2000 + Met_Year, Met_Month, Met_Day))
                {
                    return null;
                }
                else
                {
                    return new DateTime(2000 + Met_Year, Met_Month, Met_Day);
                }
            }
            set
            {
                if (value.HasValue)
                {
                    // Only update the properties if a value is provided.
                    Met_Year = value.Value.Year - 2000;
                    Met_Month = value.Value.Month;
                    Met_Day = value.Value.Day;
                }
                else
                {
                    // Clear the Met Date.
                    // If code tries to access MetDate again, null will be returned.
                    Met_Year = 0;
                    Met_Month = 0;
                    Met_Day = 0;
                }
            }
        }

        public virtual int Egg_Year { get { return 0; } set { } }
        public virtual int Egg_Month { get { return 0; } set { } }
        public virtual int Egg_Day { get { return 0; } set { } }

        /// <summary>
        /// The date a Pokémon was met as an egg.
        /// </summary>
        /// <returns>A DateTime representing the date the Pokémon was met as an egg, or null if the <see cref="PKM"/> format does not support dates.</returns>
        /// <remarks>Not all <see cref="PKM"/> types support the <see cref="EggMetDate"/> property.  In these cases, this property will return null.
        /// 
        /// If null is assigned to this property, it will be cleared.</remarks>
        public virtual DateTime? EggMetDate
        {
            get
            {
                // Check to see if date is valid
                if (!Util.IsDateValid(2000 + Egg_Year, Egg_Month, Egg_Day))
                {
                    return null;
                }
                else
                {
                    return new DateTime(2000 + Egg_Year, Egg_Month, Egg_Day);
                }
            }
            set
            {
                if (value.HasValue)
                {
                    // Only update the properties if a value is provided.
                    Egg_Year = value.Value.Year - 2000;
                    Egg_Month = value.Value.Month;
                    Egg_Day = value.Value.Day;
                }
                else
                {
                    // Clear the Met Date.
                    // If code tries to access MetDate again, null will be returned.
                    Egg_Year = 0;
                    Egg_Month = 0;
                    Egg_Day = 0;
                }
            }
        }

        public virtual int OT_Affection { get { return 0; } set { } }
        public virtual int RelearnMove1 { get { return 0; } set { } }
        public virtual int RelearnMove2 { get { return 0; } set { } }
        public virtual int RelearnMove3 { get { return 0; } set { } }
        public virtual int RelearnMove4 { get { return 0; } set { } }
        public virtual int EncounterType { get { return 0; } set { } }

        // Exposed but not Present in all
        public abstract int CurrentHandler { get; set; }

        // Derived
        public virtual int SpriteItem => HeldItem;
        public virtual bool IsShiny => TSV == PSV;
        public virtual bool Locked { get { return false; } set { } }
        public int TrainerID7 => (int)((uint)(TID | (SID << 16)) % 1000000);
        public bool VC2 => Version >= 39 && Version <= 41;
        public bool VC1 => Version >= 35 && Version <= 38;
        public bool Horohoro => Version == 34;
        public bool XY => Version == (int)GameVersion.X || Version == (int)GameVersion.Y;
        public bool AO => Version == (int)GameVersion.AS || Version == (int)GameVersion.OR;
        public bool SM => Version == (int)GameVersion.SN || Version == (int)GameVersion.MN;
        protected bool PtHGSS => GameVersion.Pt == (GameVersion)Version || HGSS;
        public bool HGSS => new[] {GameVersion.HG, GameVersion.SS}.Contains((GameVersion)Version);
        public bool VC => VC1 || VC2;
        public bool Gen7 => Version >= 30 && Version <= 33;
        public bool Gen6 => Version >= 24 && Version <= 29;
        public bool Gen5 => Version >= 20 && Version <= 23;
        public bool Gen4 => Version >= 7 && Version <= 12 && Version != 9;
        public bool Gen3 => Version >= 1 && Version <= 5 || Version == 15;
        public bool Gen2 => Version == (int)GameVersion.GSC;
        public bool Gen1 => Version == (int)GameVersion.RBY;
        public bool GenU => !(Gen7 || Gen6 || Gen5 || Gen4 || Gen3 || Gen2 || Gen1 || VC);
        public int GenNumber
        {
            get
            {
                if (Gen7) return 7;
                if (Gen6) return 6;
                if (Gen5) return 5;
                if (Gen4) return 4;
                if (Gen3) return 3;
                if (Gen2) return Format; // 2
                if (Gen1) return Format; // 1
                if (VC) return 1;
                return -1;
            } 
        }
        public bool PKRS_Infected => PKRS_Strain > 0;
        public bool PKRS_Cured => PKRS_Days == 0 && PKRS_Strain > 0;
        public virtual bool ChecksumValid => Checksum == CalculateChecksum();
        public int CurrentLevel { get { return PKX.getLevel(Species, EXP); } set { EXP = PKX.getEXP(value, Species); } }
        public int MarkCircle      { get { return Markings[0]; } set { var marks = Markings; marks[0] = value; Markings = marks; } }
        public int MarkTriangle    { get { return Markings[1]; } set { var marks = Markings; marks[1] = value; Markings = marks; } }
        public int MarkSquare      { get { return Markings[2]; } set { var marks = Markings; marks[2] = value; Markings = marks; } }
        public int MarkHeart       { get { return Markings[3]; } set { var marks = Markings; marks[3] = value; Markings = marks; } }
        public int MarkStar        { get { return Markings[4]; } set { var marks = Markings; marks[4] = value; Markings = marks; } }
        public int MarkDiamond     { get { return Markings[5]; } set { var marks = Markings; marks[5] = value; Markings = marks; } }
        public string ShowdownText => ShowdownSet.getShowdownText(this);
        public string[] QRText => this.getQRText();

        public virtual string FileName
        {
            get
            {
                string form = AltForm > 0 ? $"-{AltForm:00}" : "";
                string star = IsShiny ? " ★" : "";
                return $"{Species:000}{form}{star} - {Nickname} - {Checksum:X4}{EncryptionConstant:X8}.{Extension}";
            }
        }
        public int[] IVs
        {
            get { return new[] { IV_HP, IV_ATK, IV_DEF, IV_SPE, IV_SPA, IV_SPD }; }
            set
            {
                if (value?.Length != 6) return;
                IV_HP = value[0]; IV_ATK = value[1]; IV_DEF = value[2];
                IV_SPE = value[3]; IV_SPA = value[4]; IV_SPD = value[5];
            }
        }
        public int[] EVs
        {
            get { return new[] { EV_HP, EV_ATK, EV_DEF, EV_SPE, EV_SPA, EV_SPD }; }
            set
            {
                if (value?.Length != 6) return;
                EV_HP = value[0]; EV_ATK = value[1]; EV_DEF = value[2];
                EV_SPE = value[3]; EV_SPA = value[4]; EV_SPD = value[5];
            }
        }
        public int[] Moves
        {
            get { return new[] { Move1, Move2, Move3, Move4 }; }
            set { if (value?.Length != 4) return; Move1 = value[0]; Move2 = value[1]; Move3 = value[2]; Move4 = value[3]; }
        }
        public int[] RelearnMoves
        {
            get { return new[] { RelearnMove1, RelearnMove2, RelearnMove3, RelearnMove4 }; }
            set
            {
                if (value.Length > 0) RelearnMove1 = value[0];
                if (value.Length > 1) RelearnMove2 = value[1];
                if (value.Length > 2) RelearnMove3 = value[2];
                if (value.Length > 3) RelearnMove4 = value[3];
            }
        }
        public int PIDAbility
        {
            get
            {
                if (GenNumber > 5 || Format > 5)
                    return -1;
                if (GenNumber == 5)
                    return (int)((PID >> 16) & 1);
                return (int)(PID & 1);
            }
        }

        public virtual int[] Markings
        {
            get
            {
                int[] mark = new int[8];
                for (int i = 0; i < 8; i++)
                    mark[i] = (MarkValue >> i) & 1;
                return mark;
            }
            set
            {
                if (value.Length > 8)
                    return;
                byte b = 0;
                for (int i = 0; i < value.Length; i++)
                    b |= (byte)(Math.Min(value[i], 1) << i);
                MarkValue = b;
            }
        }

        public int[] CNTs
        {
            get { return new[] { CNT_Cool, CNT_Beauty, CNT_Cute, CNT_Smart, CNT_Tough, CNT_Sheen }; }
            set { if (value?.Length != 6) return; CNT_Cool = value[0]; CNT_Beauty = value[1]; CNT_Cute = value[2]; CNT_Smart = value[3]; CNT_Tough = value[4]; CNT_Sheen = value[5]; }
        }
        public virtual int HPType
        {
            get { return 0xF * ((IV_HP & 1) << 0 | (IV_ATK & 1) << 1 | (IV_DEF & 1) << 2 | (IV_SPE & 1) << 3 | (IV_SPA & 1) << 4 | (IV_SPD & 1) << 5) / 0x3F; }
            set
            {
                IV_HP = (IV_HP & ~1) + PKX.hpivs[value, 0];
                IV_ATK = (IV_ATK & ~1) + PKX.hpivs[value, 1];
                IV_DEF = (IV_DEF & ~1) + PKX.hpivs[value, 2];
                IV_SPE = (IV_SPE & ~1) + PKX.hpivs[value, 3];
                IV_SPA = (IV_SPA & ~1) + PKX.hpivs[value, 4];
                IV_SPD = (IV_SPD & ~1) + PKX.hpivs[value, 5];
            }
        }

        // Legality Extensions
        public virtual bool WasLink => false;
        private bool _WasEgg;
        public virtual bool WasEgg
        {
            get
            {
                if (HasOriginalMetLocation)
                    return Egg_Location > 0;
                return _WasEgg;
            } set { _WasEgg = value; }
        }
        public virtual bool WasEvent => Met_Location > 40000 && Met_Location < 50000 || FatefulEncounter;
        public virtual bool WasEventEgg => ((Egg_Location > 40000 && Egg_Location < 50000) || (FatefulEncounter && Egg_Location > 0)) && Met_Level == 1;
        public virtual bool WasTradedEgg => Egg_Location == 30002;
        public virtual bool WasIngameTrade => Met_Location == 30001;
        public virtual bool IsUntraded => Format >= 6 && string.IsNullOrWhiteSpace(HT_Name) && GenNumber == Format;
        public virtual bool IsNative => GenNumber == Format;
        public virtual bool IsOriginValid => Species <= Legal.getMaxSpeciesOrigin(Format);

        public virtual bool SecretSuperTrainingUnlocked { get { return false; } set { } }
        public virtual bool SecretSuperTrainingComplete { get { return false; } set { } }

        public virtual int HyperTrainFlags { get { return 0; } set { } }
        public virtual bool HT_HP { get { return false; } set { } }
        public virtual bool HT_ATK { get { return false; } set { } }
        public virtual bool HT_DEF { get { return false; } set { } }
        public virtual bool HT_SPA { get { return false; } set { } }
        public virtual bool HT_SPD { get { return false; } set { } }
        public virtual bool HT_SPE { get { return false; } set { } }

        /// <summary>
        /// Toggles the Hyper Training flag for a given stat.
        /// </summary>
        /// <param name="stat">Battle Stat (H/A/B/S/C/D)</param>
        public void HyperTrainInvert(int stat)
        {
            switch (stat)
            {
                case 0: HT_HP ^= true; break;
                case 1: HT_ATK ^= true; break;
                case 2: HT_DEF ^= true; break;
                case 3: HT_SPA ^= true; break;
                case 4: HT_SPD ^= true; break;
                case 5: HT_SPE ^= true; break;
            }
        }

        /// <summary>
        /// Checks if the <see cref="PKM"/> could inhabit a set of games.
        /// </summary>
        /// <param name="Generation">Set of games.</param>
        /// <param name="species"></param>
        /// <returns>True if could inhabit, False if not.</returns>
        public bool InhabitedGeneration(int Generation, int species = -1)
        {
            if (species < 0)
                species = Species;

            if (Format == Generation)
                return true;

            if (Format < Generation)
                return false; // Future

            if (!IsOriginValid)
                return false;

            // Sanity Check Species ID
            if (Legal.getMaxSpeciesOrigin(GenNumber) < species && !Legal.getFutureGenEvolutions(GenNumber).Contains(species))
                return false;

            int gen = GenNumber;
            switch (Generation)
            {
                case 1:
                case 2: return Format <= 2 || VC;
                case 3: return Gen3;
                case 4: return 3 <= gen && gen <= 4;
                case 5: return 3 <= gen && gen <= 5;
                case 6: return 3 <= gen && gen <= 6;
                case 7: return VC || 3 <= gen && gen <= 7;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the PKM has its original met location.
        /// </summary>
        /// <returns>Returns false if the Met Location has been overwritten via generational transfer.</returns>
        public bool HasOriginalMetLocation => !(Format < 3 || VC || GenNumber <= 4 && Format != GenNumber);

        /// <summary>
        /// Checks if the current <see cref="Gender"/> is valid.
        /// </summary>
        /// <returns>True if valid, False if invalid.</returns>
        public abstract bool getGenderIsValid();

        /// <summary>
        /// Updates the checksum of the <see cref="PKM"/>.
        /// </summary>
        public void RefreshChecksum() { Checksum = CalculateChecksum(); }

        /// <summary>
        /// Reorders moves and fixes PP if necessary.
        /// </summary>
        public void FixMoves()
        {
            ReorderMoves();

            if (Move1 == 0) { Move1_PP = 0; Move1_PPUps = 0; }
            if (Move2 == 0) { Move2_PP = 0; Move2_PPUps = 0; }
            if (Move3 == 0) { Move3_PP = 0; Move3_PPUps = 0; }
            if (Move4 == 0) { Move4_PP = 0; Move4_PPUps = 0; }
        }

        /// <summary>
        /// Reorders moves to put Empty entries last.
        /// </summary>
        private void ReorderMoves()
        {
            if (Move4 != 0 && Move3 == 0)
            {
                Move3 = Move4;
                Move3_PP = Move4_PP;
                Move3_PPUps = Move4_PPUps;
                Move4 = 0;
            }
            if (Move3 != 0 && Move2 == 0)
            {
                Move2 = Move3;
                Move2_PP = Move3_PP;
                Move2_PPUps = Move3_PPUps;
                Move3 = 0;
                ReorderMoves();
            }
            if (Move2 != 0 && Move1 == 0)
            {
                Move1 = Move2;
                Move1_PP = Move2_PP;
                Move1_PPUps = Move2_PPUps;
                Move2 = 0;
                ReorderMoves();
            }
        }

        /// <summary>
        /// Applies the desired Ability option.
        /// </summary>
        /// <param name="n">Ability Number (0/1/2)</param>
        public void RefreshAbility(int n)
        {
            AbilityNumber = 1 << n;
            int[] abilities = PersonalInfo.Abilities;
            if (n < abilities.Length)
                Ability = abilities[n];
        }

        /// <summary>
        /// Gets the IV Judge Rating value.
        /// </summary>
        /// <remarks>IV Judge scales his response 0 (worst) to 3 (best).</remarks>
        public int PotentialRating
        {
            get
            {
                int ivTotal = IVs.Sum();
                if (ivTotal <= 90)
                    return 0;
                if (ivTotal <= 120)
                    return 1;
                return ivTotal <= 150 ? 2 : 3;
            }
        }

        /// <summary>
        /// Gets the current Battle Stats.
        /// </summary>
        /// <param name="p"><see cref="PersonalInfo"/> entry containing Base Stat Info</param>
        /// <returns>Battle Stats (H/A/B/S/C/D)</returns>
        public virtual ushort[] getStats(PersonalInfo p)
        {
            int level = CurrentLevel;
            ushort[] Stats = new ushort[6];
            Stats[0] = (ushort)(p.HP == 1 ? 1 : ((HT_HP ? 31 : IV_HP) + 2 * p.HP + EV_HP / 4 + 100) * level / 100 + 10);
            Stats[1] = (ushort)(((HT_ATK ? 31 : IV_ATK) + 2 * p.ATK + EV_ATK / 4) * level / 100 + 5);
            Stats[2] = (ushort)(((HT_DEF ? 31 : IV_DEF) + 2 * p.DEF + EV_DEF / 4) * level / 100 + 5);
            Stats[4] = (ushort)(((HT_SPA ? 31 : IV_SPA) + 2 * p.SPA + EV_SPA / 4) * level / 100 + 5);
            Stats[5] = (ushort)(((HT_SPD ? 31 : IV_SPD) + 2 * p.SPD + EV_SPD / 4) * level / 100 + 5);
            Stats[3] = (ushort)(((HT_SPE ? 31 : IV_SPE) + 2 * p.SPE + EV_SPE / 4) * level / 100 + 5);

            // Account for nature
            int incr = Nature / 5 + 1;
            int decr = Nature % 5 + 1;
            if (incr == decr) return Stats;
            Stats[incr] *= 11; Stats[incr] /= 10;
            Stats[decr] *= 9; Stats[decr] /= 10;
            return Stats;
        }
        /// <summary>
        /// Applies the specified stats to the <see cref="PKM"/>.
        /// </summary>
        /// <param name="Stats">Battle Stats (H/A/B/S/C/D)</param>
        public void setStats(ushort[] Stats)
        {
            Stat_HPMax = Stat_HPCurrent = Stats[0];
            Stat_ATK = Stats[1];
            Stat_DEF = Stats[2];
            Stat_SPE = Stats[3];
            Stat_SPA = Stats[4];
            Stat_SPD = Stats[5];
        }

        /// <summary>
        /// Checks if the <see cref="PKM"/> can hold its <see cref="HeldItem"/>.
        /// </summary>
        /// <param name="ValidArray">Items that the <see cref="PKM"/> can hold.</param>
        /// <returns>True/False if the <see cref="PKM"/> can hold its <see cref="HeldItem"/>.</returns>
        public virtual bool CanHoldItem(ushort[] ValidArray)
        {
            return ValidArray.Contains((ushort)HeldItem);
        }

        /// <summary>
        /// Deep clones the <see cref="PKM"/> object. The clone will not have any shared resources with the source.
        /// </summary>
        /// <returns>Cloned <see cref="PKM"/> object</returns>
        public abstract PKM Clone();

        /// <summary>
        /// Gets the PP of a Move ID with consideration of the amount of PP Ups applied.
        /// </summary>
        /// <param name="move">Move ID</param>
        /// <param name="ppup">PP Ups count</param>
        /// <returns>Current PP for the move.</returns>
        public int getMovePP(int move, int ppup)
        {
            return getBasePP(move) * (5 + ppup) / 5;
        }

        /// <summary>
        /// Gets the base PP of a move ID depending on the <see cref="PKM"/>'s format.
        /// </summary>
        /// <param name="move">Move ID</param>
        /// <returns>Amount of PP the move has by default (no PP Ups).</returns>
        private int getBasePP(int move)
        {
            int[] pptable;
            switch (Format)
            {
                case 1: pptable = Legal.MovePP_RBY; break;
                case 2: pptable = Legal.MovePP_GSC; break;
                case 3: pptable = Legal.MovePP_RS; break;
                case 4: pptable = Legal.MovePP_DP; break;
                case 5: pptable = Legal.MovePP_BW; break;
                case 6: pptable = Legal.MovePP_XY; break;
                case 7: pptable = Legal.MovePP_SM; break;
                default: pptable = new int[1]; break;
            }
            if (move >= pptable.Length)
                move = 0;
            return pptable[move];
        }

        /// <summary>
        /// Applies a shiny PID to the <see cref="PKM"/>.
        /// </summary>
        /// <remarks>
        /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
        /// </remarks>
        public void setShinyPID()
        {
            do PID = PKX.getRandomPID(Species, Gender, Version, Nature, AltForm, PID); while (!IsShiny);
            if (GenNumber < 6)
                EncryptionConstant = PID;
        }
        /// <summary>
        /// Applies a PID to the <see cref="PKM"/> according to the specified <see cref="Gender"/>.
        /// </summary>
        /// <remarks>
        /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
        /// </remarks>
        public void setPIDGender(int gender)
        {
            do PID = PKX.getRandomPID(Species, gender, Version, Nature, AltForm, PID); while (IsShiny);
            if (GenNumber < 6)
                EncryptionConstant = PID;
        }
        /// <summary>
        /// Applies a PID to the <see cref="PKM"/> according to the specified <see cref="Gender"/>.
        /// </summary>
        /// <remarks>
        /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
        /// </remarks>
        public void setPIDNature(int nature)
        {
            do PID = PKX.getRandomPID(Species, Gender, Version, nature, AltForm, PID); while (IsShiny);
            if (GenNumber < 6)
                EncryptionConstant = PID;
        }
        /// <summary>
        /// Applies a PID to the <see cref="PKM"/> according to the specified <see cref="AltForm"/>.
        /// </summary>
        /// <remarks>
        /// This method should only be used for Unown originating in Generation 3 games.
        /// If a <see cref="PKM"/> originated in a generation prior to Generation 6, the <see cref="EncryptionConstant"/> is updated.
        /// </remarks>
        public void setPIDUnown3(int form)
        {
            do PID = Util.rnd32(); while (PKX.getUnownForm(PID) != form);
        }
        
        /// <summary>
        /// Converts a <see cref="XK3"/> or <see cref="PK3"/> to <see cref="CK3"/>.
        /// </summary>
        /// <returns><see cref="CK3"/> format <see cref="PKM"/></returns>
        public PKM convertToCK3()
        {
            if (Format != 3)
                return null;
            if (GetType() == typeof(CK3))
                return this;
            var pk = new CK3();
            TransferPropertiesWithReflection(this, pk);
            pk.setStats(getStats(PersonalTable.RS[pk.Species]));
            return pk;
        }
        /// <summary>
        /// Converts a <see cref="PK3"/> or <see cref="CK3"/> to <see cref="XK3"/>.
        /// </summary>
        /// <returns><see cref="XK3"/> format <see cref="PKM"/></returns>
        public PKM convertToXK3()
        {
            if (Format != 3)
                return null;
            if (GetType() == typeof(XK3))
                return this;
            var pk = new XK3();
            TransferPropertiesWithReflection(this, pk);
            pk.setStats(getStats(PersonalTable.RS[pk.Species]));
            return pk;
        }
        /// <summary>
        /// Converts a <see cref="CK3"/> or <see cref="XK3"/> to <see cref="PK3"/>.
        /// </summary>
        /// <returns><see cref="PK3"/> format <see cref="PKM"/></returns>
        public PKM convertToPK3()
        {
            if (Format != 3)
                return null;
            if (GetType() == typeof(PK3))
                return this;
            var pk = new PK3();
            TransferPropertiesWithReflection(this, pk);
            pk.RefreshChecksum();
            return pk;
        }

        /// <summary>
        /// Applies all shared properties from <see cref="Source"/> to <see cref="Destination"/>.
        /// </summary>
        /// <param name="Source"><see cref="PKM"/> that supplies property values.</param>
        /// <param name="Destination"><see cref="PKM"/> that receives property values.</param>
        protected void TransferPropertiesWithReflection(PKM Source, PKM Destination)
        {
            var SourceProperties = ReflectUtil.getPropertiesCanWritePublic(Source.GetType());
            var DestinationProperties = ReflectUtil.getPropertiesCanWritePublic(Destination.GetType());

            // Skip Data property when applying all individual properties. Let the setters do the updates for Data.
            foreach (string property in SourceProperties.Intersect(DestinationProperties).Where(prop => prop != nameof(Data)))
            {
                var prop = ReflectUtil.GetValue(this, property);
                if (prop != null)
                    ReflectUtil.SetValue(Destination, property, prop);
            }
        }
    }
}
