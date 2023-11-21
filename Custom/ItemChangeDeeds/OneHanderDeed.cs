using System;
using Server.Network;
using Server.Prompts;
using Server.Targeting;
using Server.Items;

namespace Server.Items
{
    public class OneHanderDeed : Item
    {
        [Constructable]
        public OneHanderDeed() : base(0x14F0)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 1161;
            Name = "One Hander Deed";
        }

        public OneHanderDeed(Serial serial) : base(serial)
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
                from.Target = new OneHanderDeedTarget(this);
            }
        }
    }

    public class OneHanderDeedTarget : Target
    {
        private OneHanderDeed m_OneHanderDeed;

        public OneHanderDeedTarget(OneHanderDeed oneHanderDeed) : base(1, false, TargetFlags.None)
        {
            m_OneHanderDeed = oneHanderDeed;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            Item deedInBackpack = from.Backpack.FindItemByType(typeof(OneHanderDeed));
            Item selectedWeapon = from.Backpack.FindItemByType(typeof(BaseWeapon));

            if (target is BaseWeapon)
            {
                BaseWeapon weapon = target as BaseWeapon;

                if (selectedWeapon != null && selectedWeapon.RootParent == from)
                {
                    weapon.Layer = Layer.OneHanded;
                    deedInBackpack.Delete(); // Delete the deed only if found
                    from.SendMessage(38, "The weapon is now one-handed.");
                }
                else
                {
                    from.SendMessage(38, "The selected weapon should be in your backpack.");
                }
            }
            else
            {
                from.SendMessage(38, "You can only enhance weapons.");
            }
        }
    }
}

