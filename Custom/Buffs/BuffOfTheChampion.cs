using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Events;

namespace BuffOfTheChampion
{
    public class BuffOfTheChampion
    {
        private static Dictionary<Mobile, bool> BuffsOfTheChampionApplied = new Dictionary<Mobile, bool>();

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
            double percentageIncrease = 0.05;

            // Calculate the increased stats based on the current stats
            int increasedStr = (int)(mobile.RawStr * percentageIncrease);
            int increasedDex = (int)(mobile.RawDex * percentageIncrease);
            int increasedInt = (int)(mobile.RawInt * percentageIncrease);

            // Add the stat modifications with unique keys
            mobile.AddStatMod(new StatMod(StatType.Str, "BuffOfTheChampion_Str", increasedStr, duration));
            mobile.AddStatMod(new StatMod(StatType.Dex, "BuffOfTheChampion_Dex", increasedDex, duration));
            mobile.AddStatMod(new StatMod(StatType.Int, "BuffOfTheChampion_Int", increasedInt, duration));

            // Define the custom buff icon (replace 'Berserk' with the actual icon you have)
            BuffIcon customBuffIcon = BuffIcon.BuffOfTheChampion; // Replace with your desired icon

            // Set the text that will be displayed when you hover over the buff icon
            TextDefinition buffText = "Buff Of The Champion";

            // Use your server-specific method to add a buff with an icon and custom text
            BuffInfo.AddBuff(mobile, new BuffInfo(customBuffIcon, 1070812, buffText, duration, mobile));

            // Mark the buff as applied for the mobile
            BuffsOfTheChampionApplied[mobile] = true;

            // Optionally, you can notify the player about the buff
            mobile.SendMessage("You have received the Buff of the Champion for two hours!");

            // You may also add additional effects or visuals here
        }

        private static bool HasBuff(Mobile mobile)
        {
            // Check if the buff is applied to the mobile
            return BuffsOfTheChampionApplied.ContainsKey(mobile) && BuffsOfTheChampionApplied[mobile];
        }
    }
}
