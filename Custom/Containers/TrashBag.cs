using System;
using System.Collections.Generic;
using Server.Multis;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Engines.Points;
using System.Linq;
using Server.Items;
using Daat99MasterLooterSystem;

namespace Server.Custom
{
    public class TrashBag : Container
    {
        public class CleanupArray
        {
            public Mobile mobiles { get; set; }
            public Item items { get; set; }
            public double points { get; set; }
            public bool confirm { get; set; }
            public Serial serials { get; set; }
        }

        private Timer m_Timer;
        private List<CleanupArray> m_Cleanup;

        public virtual bool AddCleanupItem(Mobile from, Item item)
        {
            double checkbagpoint;
            bool added = false;

            if (item is BaseContainer)
            {
                Container c = (Container)item;

                List<Item> list = c.FindItemsByType<Item>();

                for (int i = list.Count - 1; i >= 0; --i)
                {
                    checkbagpoint = CleanUpBritanniaData.GetPoints(list[i]);

                    if (checkbagpoint > 0 && m_Cleanup.Find(x => x.serials == list[i].Serial) == null)
                    {
                        m_Cleanup.Add(new CleanupArray { mobiles = from, items = list[i], points = checkbagpoint, serials = list[i].Serial });

                        if (!added)
                            added = true;
                    }
                }
            }
            else
            {
                checkbagpoint = CleanUpBritanniaData.GetPoints(item);

                if (checkbagpoint > 0 && m_Cleanup.Find(x => x.serials == item.Serial) == null)
                {
                    m_Cleanup.Add(new CleanupArray { mobiles = from, items = item, points = checkbagpoint, serials = item.Serial });
                    added = true;
                }
            }

            return added;
        }

        public void ConfirmCleanupItem(Item item)
        {
            if (item is BaseContainer)
            {
                Container c = (Container)item;

                List<Item> list = c.FindItemsByType<Item>();

                m_Cleanup.Where(r => list.Select(k => k.Serial).Contains(r.items.Serial)).ToList().ForEach(k => k.confirm = true);
            }
            else
            {
                m_Cleanup.Where(r => r.items.Serial == item.Serial).ToList().ForEach(k => k.confirm = true);
            }
        }

        [Constructable]
        public TrashBag()
            : base(0x9b2)
        {
            Hue = 0x3B2;            
            Name = "Trash Bag";
            Movable = true;
            m_Cleanup = new List<CleanupArray>();
        }

        public TrashBag(Serial serial)
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

            if (Items.Count > 0)
            {
                m_Timer = new EmptyTimer(this);
                m_Timer.Start();
            }

            m_Cleanup = new List<CleanupArray>();
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            AddCleanupItem(from, dropped);

            if (TotalItems >= 50)
            {
                Empty(501478); // The trash is full!  Emptying!
            }
            else
            {
                SendLocalizedMessageTo(from, 1010442); // The item will be deleted in three minutes

                if (m_Timer != null)
                    m_Timer.Stop();
                else
                    m_Timer = new EmptyTimer(this);

                m_Timer.Start();
            }

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            AddCleanupItem(from, item);

            if (TotalItems >= 50)
            {
                Empty(501478); // The trash is full!  Emptying!
            }
            else
            {
                SendLocalizedMessageTo(from, 1010442); // The item will be deleted in three minutes

                if (m_Timer != null)
                    m_Timer.Stop();
                else
                    m_Timer = new EmptyTimer(this);

                m_Timer.Start();
            }

            return true;
        }

        public void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && house.IsCoOwner(from))
            {
                Effects.PlaySound(Location, Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.
                Destroy();
            }
        }

        public void Empty(int message)
        {
            List<Item> items = Items;

            if (items.Count > 0)
            {
                PublicOverheadMessage(Network.MessageType.Regular, 0x3B2, message, "");

                int totalGold = 0;

                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (i >= items.Count)
                        continue;

                    ConfirmCleanupItem(items[i]);

                    #region SA
                    if (.01 > Utility.RandomDouble())
                        DropToCavernOfDiscarded(items[i]);
                    else
                    {
                        items[i].Delete();
                        totalGold += 10; // Award 10 gold per item deleted
                    }
                    #endregion
                }

                if (totalGold > 0)
                {
                    // Award gold to the player
                    if (RootParent is PlayerMobile)
                    {
                        PlayerMobile mobile = (PlayerMobile)RootParent;
                        Daat99MasterLooterSystem.Daat99MasterLootersUtils.GivePlayerGold(mobile, totalGold);

                        // You can customize the message or logic for awarding gold here if needed
                        Console.WriteLine("Awarded " + totalGold + " gold to the player for cleaning up the server!");
                    }
                }
            }

            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        private class EmptyTimer : Timer
        {
            private readonly TrashBag m_Barrel;
            public EmptyTimer(TrashBag barrel)
                : base(TimeSpan.FromMinutes(3.0))
            {
                m_Barrel = barrel;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                m_Barrel.Empty(501479); // Emptying the trashcan!
            }
        }

        #region SA
        public static void DropToCavernOfDiscarded(Item item)
        {
            if (item == null || item.Deleted)
                return;

            Rectangle2D rec = new Rectangle2D(901, 482, 40, 27);
            Map map = Map.TerMur;

            for (int i = 0; i < 50; i++)
            {
                int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
                int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
                int z = map.GetAverageZ(x, y);

                Point3D p = new Point3D(x, y, z);

                if (map.CanSpawnMobile(p))
                {
                    item.MoveToWorld(p, map);
                    return;
                }
            }

            item.Delete();
        }
        #endregion
    }
}
