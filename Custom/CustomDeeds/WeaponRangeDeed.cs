using System;
using Server.Network;
using Server.Prompts;
using Server.Targeting;
using Server.Items;

namespace Server.Custom
{
    public class WeaponRangeDeed : Item
    {
        [Constructable]
        public WeaponRangeDeed() : base(0x14F0)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 1161;
            Name = "Weapon Range Deed +3";
        }

        public WeaponRangeDeed(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                from.Target = new WeaponRangeDeedTarget(this);
            }
        }
    }

    public class WeaponRangeDeedTarget : Target
    {
        private WeaponRangeDeed m_WeaponRangeDeed;

        public WeaponRangeDeedTarget(WeaponRangeDeed weaponRangeDeed) : base(1, false, TargetFlags.None)
        {
            m_WeaponRangeDeed = weaponRangeDeed;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            Item deedInBackpack = from.Backpack.FindItemByType(typeof(WeaponRangeDeed));

            if (target is BaseWeapon)
            {
                BaseWeapon weapon = target as BaseWeapon;

                if (weapon is BaseRanged)
                {
                    from.SendMessage(38, "Bows and crossbows cannot have their range modified.");
                    return;
                }

                // Check if the weapon range is not already 3
                if (weapon.Layer != Layer.OneHanded && weapon.MaxRange != 3)
                {
                    weapon.Layer = Layer.OneHanded;
                    weapon.MaxRange = 3; // Set the weapon range to 3
                    deedInBackpack.Delete(); // Delete the deed only if found
                    from.SendMessage(38, "The weapon range is now 3.");
                }
                else
                {
                    from.SendMessage(38, "The selected weapon already has a range of 3.");
                }
            }
            else
            {
                from.SendMessage(38, "You can only enhance weapons that are not bows or crossbows.");
            }
        }
    }
}
