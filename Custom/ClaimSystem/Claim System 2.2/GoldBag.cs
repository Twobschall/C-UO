#region AuthorHeader
//
//	Claim System version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Server.Items;

namespace Xanthos.Claim
{
    public class GoldBag : Bag
    {		
		[Constructable]
		public GoldBag() : base()
		{
			Weight = 0.0;
			Hue = 1174;
			Name = "gold bag";
			LootType = ClaimConfig.GoldBagBlessed? LootType.Blessed : LootType.Regular;
		}

		public GoldBag( Serial serial ) : base( serial )
		{
		}

		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
		}
		

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
       public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!(item is Gold))
            {
                from.SendMessage("It seems you can't place that item in this bag.");
                return false;
            }
            return base.OnDragDropInto(from, item, p);
        }
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!(dropped is Gold))
            {
                from.SendMessage("It seems you can't place that item in this bag.");
                return false;
            }
            return base.OnDragDrop(from, dropped);
        }
        
		 public override int GetTotal(TotalType type)
        {
            if (type != TotalType.Weight)
                return base.GetTotal(type);
            else
            {
                return (int)(TotalItemWeights() * (0.0));
            }
        } 
		      public override void UpdateTotal(Item sender, TotalType type, int delta)
        {
            if (type != TotalType.Weight)
                base.UpdateTotal(sender, type, delta);
            else
                base.UpdateTotal(sender, type, (int)(delta * (0.0)));
        }
		private double TotalItemWeights()
        {
            double weight = 0.0;

            foreach (Item item in Items)
                weight += (item.Weight * (double)(item.Amount));

            return weight;
        }
    }
}
