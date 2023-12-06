using System;
using Server.Items;

namespace Server.Custom
{
    public class ArrowBag : Bag
    {
        [Constructable]
        public ArrowBag() : this(24) // create a 24 item bag by default
        {
        }

        [Constructable]
        public ArrowBag(int maxitems) : base()
        {
            Weight = 1.0;
            MaxItems = maxitems;
            Name = "Arrow Bag";
            Hue = 1923;
            LootType = LootType.Blessed;
        }

        public ArrowBag(Serial serial) : base(serial)
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

        // Override the OnDragDrop method to handle the item dropping logic.
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            // Check if the dropped item is an arrow or bolt.
            if (dropped is Arrow || dropped is Bolt)
            {
                // Call the base implementation to perform the default behavior.
                return base.OnDragDrop(from, dropped);
            }
            else
            {
                // Reject the item if it's not an arrow or bolt.
                from.SendLocalizedMessage(500647); // That is not something you can put in there.
                return false;
            }
        }

        // Override the OnDragDropInto method to allow arrows and bolts to be dropped into the bag.
        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            // Check if the dropped item is an arrow or bolt.
            if (item is Arrow || item is Bolt)
            {
                // Call the base implementation to perform the default behavior.
                return base.OnDragDropInto(from, item, p);
            }
            else
            {
                // Reject the item if it's not an arrow or bolt.
                from.SendLocalizedMessage(500647); // That is not something you can put in there.
                return false;
            }
        }
    }
}
