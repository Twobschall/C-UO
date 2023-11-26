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
    public static Dictionary<Mobile, Timer> BuffTimers = new Dictionary<Mobile, Timer>();

    public static void ApplyFoodBuff(Mobile mobile)
    {
        if (mobile.Alive && !HasBuff(mobile))
        {
            ApplyBuff(mobile);
        }
    }

    private static void ApplyBuff(Mobile mobile)
    {
        TimeSpan duration = TimeSpan.FromMinutes(60);

        double percentageIncrease = 0.1;

        int increasedStr = (int)(mobile.RawStr * percentageIncrease);
        int increasedDex = (int)(mobile.RawDex * percentageIncrease);
        int increasedInt = (int)(mobile.RawInt * percentageIncrease);

        mobile.AddStatMod(new StatMod(StatType.Str, "FoodBuff_Str", increasedStr, duration));
        mobile.AddStatMod(new StatMod(StatType.Dex, "FoodBuff_Dex", increasedDex, duration));
        mobile.AddStatMod(new StatMod(StatType.Int, "FoodBuff_Int", increasedInt, duration));

        FoodBuffApplied[mobile] = true;
        FoodBuffStartTime[mobile] = DateTime.UtcNow;

        mobile.SendMessage("You have received the Food Buff for one hour!");

        // Schedule a cleanup task to remove the entry after one hour
        Timer buffTimer = new BuffTimer(mobile);
        buffTimer.Start();
        BuffTimers[mobile] = buffTimer;
    }

    private static void RemoveBuffEntry(Mobile mobile)
    {
        if (FoodBuffApplied.ContainsKey(mobile) && FoodBuffApplied[mobile])
        {
            FoodBuffApplied[mobile] = false;
            FoodBuffStartTime.Remove(mobile);
            BuffTimers.Remove(mobile);
        }
    }

    public static TimeSpan GetRemainingBuffDuration(Mobile mobile)
    {
        if (FoodBuffApplied.ContainsKey(mobile) && FoodBuffApplied[mobile])
        {
            DateTime startTime = FoodBuffStartTime[mobile];
            TimeSpan elapsed = DateTime.UtcNow - startTime;
            TimeSpan remaining = TimeSpan.FromMinutes(60) - elapsed;

            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        return TimeSpan.Zero;
    }

    private static bool HasBuff(Mobile mobile)
    {
        return FoodBuffApplied.ContainsKey(mobile) && FoodBuffApplied[mobile];
    }

    private class BuffTimer : Timer
    {
        private readonly Mobile _mobile;

        public BuffTimer(Mobile mobile) : base(TimeSpan.FromMinutes(60), TimeSpan.FromMinutes(60))
        {
            _mobile = mobile;
        }

        protected override void OnTick()
        {
            if (_mobile != null && !_mobile.Deleted)
            {
                if (GetRemainingBuffDuration(_mobile) == TimeSpan.Zero)
                {
                    RemoveBuffEntry(_mobile);
                    Stop();
                }
            }
            else
            {
                Stop();
            }
        }
    }
}
}