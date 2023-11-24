using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Events;

namespace Server.Custom
{
    public class BuffOfTheChampion
    {
        public static Dictionary<Mobile, bool> BuffOfTheChampionApplied = new Dictionary<Mobile, bool>();
        public static Dictionary<Mobile, DateTime> BuffOfTheChampionStartTime = new Dictionary<Mobile, DateTime>();

        public static void Initialize()
        {
            // Change the event to the correct creature death event in your server
            EventSink.CreatureDeath += OnCreatureDeath;
        }

        private static void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            Mobile victim = e.Creature;

            // Check if the victim is one of the specified monsters
            if (victim is AncientWyrm || victim is Leviathan || victim is ShadowWyrm || victim is SkeletalDragon || victim is Yamandon)
            {
                // Apply the buff to all active players
                foreach (NetState state in NetState.Instances)
                {
                    Mobile player = state.Mobile;

                    // Check if the player is alive and not hidden
                    if (player != null && player.Alive && !player.Hidden)
                    {
                        // Apply the buff only if the player doesn't already have it
                        if (!HasBuff(player))
                        {
                            BuffChampion(player);
                        }
                    }
                }
            }
        }

        private static void BuffChampion(Mobile mobile)
        {
            // Apply the custom buff
            ApplyBuff(mobile);
        }

        private static void ApplyBuff(Mobile mobile)
        {
            // Set the duration of the buff
            TimeSpan duration = TimeSpan.FromHours(2);

            // Calculate the percentage increase in stats (5% in this case)
            double percentageIncrease = 0.1;

            // Calculate the increased stats based on the current stats
            int increasedStr = (int)(mobile.RawStr * percentageIncrease);
            int increasedDex = (int)(mobile.RawDex * percentageIncrease);
            int increasedInt = (int)(mobile.RawInt * percentageIncrease);

            // Add the stat modifications with unique keys
            mobile.AddStatMod(new StatMod(StatType.Str, "BuffOfTheChampion_Str", increasedStr, duration));
            mobile.AddStatMod(new StatMod(StatType.Dex, "BuffOfTheChampion_Dex", increasedDex, duration));
            mobile.AddStatMod(new StatMod(StatType.Int, "BuffOfTheChampion_Int", increasedInt, duration));

            // Mark the buff as applied for the mobile
            BuffOfTheChampionApplied[mobile] = true;
            BuffOfTheChampionStartTime[mobile] = DateTime.UtcNow;

            // Optionally, you can notify the player about the buff
            mobile.SendMessage("You have received the Buff of the Champion for two hours!");

            // You may also add additional effects or visuals here
        }

        public static TimeSpan GetRemainingBuffDuration(Mobile mobile)
        {
            if (BuffOfTheChampionApplied.ContainsKey(mobile) && BuffOfTheChampionApplied[mobile])
            {
                DateTime startTime = BuffOfTheChampionStartTime[mobile];
                TimeSpan elapsed = DateTime.UtcNow - startTime;
                TimeSpan remaining = TimeSpan.FromHours(2) - elapsed;

                return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
            }

            return TimeSpan.Zero;
        }

        private static bool HasBuff(Mobile mobile)
        {
            // Check if the buff is applied to the mobile
            return BuffOfTheChampionApplied.ContainsKey(mobile) && BuffOfTheChampionApplied[mobile];
        }
    }
}
