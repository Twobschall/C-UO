using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Server.Custom
{
    public class FoodBuff
    {
        public static Dictionary<Mobile, bool> FoodBuffApplied = new Dictionary<Mobile, bool>();
        public static Dictionary<Mobile, DateTime> FoodBuffStartTime = new Dictionary<Mobile, DateTime>();

        // Make the method public
        public static void ApplyFoodBuff(Mobile mobile)
        {
            // Check if the mobile is alive and doesn't already have the buff
            if (mobile.Alive && !HasBuff(mobile))
            {
                // Apply the custom buff
                ApplyBuff(mobile);
            }
        }

        private static void ApplyBuff(Mobile mobile)
        {
            // Set the duration of the buff
            TimeSpan duration = TimeSpan.FromHours(1); // Adjusted to 1 hour

            // Calculate the percentage increase in stats (10% in this case)
            double percentageIncrease = 0.1;

            // Calculate the increased stats based on the current stats
            int increasedStr = (int)(mobile.RawStr * percentageIncrease);
            int increasedDex = (int)(mobile.RawDex * percentageIncrease);
            int increasedInt = (int)(mobile.RawInt * percentageIncrease);

            // Add the stat modifications with unique keys
            mobile.AddStatMod(new StatMod(StatType.Str, "FoodBuff_Str", increasedStr, duration));
            mobile.AddStatMod(new StatMod(StatType.Dex, "FoodBuff_Dex", increasedDex, duration));
            mobile.AddStatMod(new StatMod(StatType.Int, "FoodBuff_Int", increasedInt, duration));

            // Mark the buff as applied for the mobile
            FoodBuffApplied[mobile] = true;
            FoodBuffStartTime[mobile] = DateTime.UtcNow;

            // Optionally, you can notify the player about the buff
            mobile.SendMessage("You have received the Food Buff for one hour!");

            // You may also add additional effects or visuals here
        }

        public static TimeSpan GetRemainingBuffDuration(Mobile mobile)
        {
            if (FoodBuffApplied.ContainsKey(mobile) && FoodBuffApplied[mobile])
            {
                DateTime startTime = FoodBuffStartTime[mobile];
                TimeSpan elapsed = DateTime.UtcNow - startTime;
                TimeSpan remaining = TimeSpan.FromHours(1) - elapsed;

                return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
            }

            return TimeSpan.Zero;
        }

        private static bool HasBuff(Mobile mobile)
        {
            // Check if the buff is applied to the mobile
            return FoodBuffApplied.ContainsKey(mobile) && FoodBuffApplied[mobile];
        }
    }
}
