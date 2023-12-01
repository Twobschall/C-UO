using System;
using Server;
using Server.Mobiles;

namespace Server.Custom
{
    public class IncreaseFollowersDeed : Item
    {
        [Constructable]
        public IncreaseFollowersDeed()
            : base(0x14F0) // You can change the graphic ID as needed
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Name = "Follower Increase Deed";
        }

        public IncreaseFollowersDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
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
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                // Increase FollowerMax by 1, up to a maximum of 6
                if (from.Followers < 6)
                {
                    from.Followers++;
                    from.SendMessage("Your follower limit is now " + from.Followers + ". The max is 6");
                    Consume();
                }
                else
                {
                    from.SendMessage("You already have the maximum follower limit.");
                }
            }
        }
    }
}
