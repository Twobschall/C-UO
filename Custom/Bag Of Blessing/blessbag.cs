using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    public class BlessBag : Container
    {
        [Constructable]
        public BlessBag() : base(0xE76)
        {
            Name = "A Bag of Blessing";
            Weight = 0.0;
            Hue = 1153; // Frosty White
            LootType = LootType.Blessed;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            if (item.LootType == LootType.Blessed)
            {
                // Unbless the item
                item.LootType = LootType.Regular;
                from.SendMessage("The item is no longer blessed.");
            }
            else
            {
                // Bless the item
                item.LootType = LootType.Blessed;
                from.SendMessage("Your stuff is now blessed.");
            }

            return true;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            if (dropped.LootType == LootType.Blessed)
            {
                // Unbless the item
                dropped.LootType = LootType.Regular;
                from.SendMessage("The item is no longer blessed.");
            }
            else
            {
                // Bless the item
                dropped.LootType = LootType.Blessed;
                from.SendMessage("The item is now blessed.");
            }

            return true;
        }

        public BlessBag(Serial serial) : base(serial)
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
    }
}
