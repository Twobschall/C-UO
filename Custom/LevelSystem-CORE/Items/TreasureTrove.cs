// Add content Menu (info) that opens a gump that allows the user to transorm the item into other armor parts. 

using Server;
using System;
using Server.Accounting;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using System.Collections.Generic;
using Server.Engines.XmlSpawnerExtMod;

namespace Server.Items 
{
	public class TreasureTrove : Backpack
	{
		private double m_Redux;	
		private bool m_UseStatsforMaxItems = true;
		private int m_BoundToMobile = 0;
		private int m_MultiplierMax = 12;
		private int m_DivideVarMax = 2;
	
	
		[CommandProperty(AccessLevel.GameMaster)]
        public bool UseStatsforMaxItems
        {
            get { return m_UseStatsforMaxItems; }
            set { m_UseStatsforMaxItems = value; }
        }
		[CommandProperty(AccessLevel.GameMaster)]
        public int DivideVarMax
        {
            get { return m_DivideVarMax; }
            set { m_DivideVarMax = value; }
        }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public int MultiplierMax
        {
            get { return m_MultiplierMax; }
            set { m_MultiplierMax = value; }
        }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public int BoundToMobile
        {
            get { return m_BoundToMobile; }
            set { m_BoundToMobile = value; }
        }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int ReduxPercent
		{
			get
			{
				return (int)(m_Redux*100);
			}
			set
			{
				if(value<0)
				{
					value=0;
				}
				if(value>100)
				{
					value=100;
				}
				m_Redux=((double)value)/100;
				if ( Parent is Item )
				{
					( Parent as Item ).UpdateTotals();
					( Parent as Item ).InvalidateProperties();
				}
				else if ( Parent is Mobile )
				{
					( Parent as Mobile ).UpdateTotals();
				}
				else
					UpdateTotals();
				InvalidateProperties();
			}
		}
		public override string DefaultName 
		{ 
			get 
			{ 
				return "Treasure Trove"; 
			}
		}
		
		[Constructable]
		public TreasureTrove() : this(100) 
		{
			MaxItems = 925000;
			Visible = true;
			LootType = LootType.Blessed;
			Layer = Layer.Talisman;
			ItemID = 0x2F5B;
			Movable = true;
			GumpID = 60;
		//	GumpID = 10913;
		}
		
		[Constructable]
		public TreasureTrove(int redux) : base() 
		{
			ReduxPercent=redux;
		}
		public TreasureTrove(Serial serial) : base(serial) 
		{
			
		}
		
		public override bool OnEquip( Mobile from )
		{
			if (from is PlayerMobile)
			{
				PlayerMobile pm = from as PlayerMobile;
				LevelSheet xmlplayer = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
				
				if (xmlplayer == null)
				{
					from.SendMessage( "This wont work for you!" );
					return false;
				}
				else if (xmlplayer.Levell >= 100)
				{
					return true;
				}
				else
				{
					from.SendMessage( "Your level is not high enough!" );
					return false;
				}
			}
			return false;
		}
		
		public override int DefaultMaxWeight
		{ 
			get
			{ 
				return 9000; 
			} 
		}
		
		public override bool OnDragLift( Mobile from )
		{
				return true;
		}
			
		public override void OnDoubleClick( Mobile m )
		{
			this.GumpID = 60;
			
			int MaxItemsStats = m.RawStr + m.RawDex + m.RawInt * MultiplierMax / DivideVarMax;
			
			if(BoundToMobile == 0)
			{ 
      			BoundToMobile = m.Serial;
                this.Name = m.Name.ToString() + "'s Treasure Trove";
				m.SendMessage( "Your storage is now active!" );
      		}
			if (BoundToMobile != m.Serial)
			{
				m.SendMessage( "This does not belong to you!" );
				return;
			}
			
			if (this.UseStatsforMaxItems == true && m.AccessLevel == AccessLevel.Player)
			{
				if (this.MaxItems != MaxItemsStats)
				{
					m.SendMessage( "Your storage has adjusted based on your new stats!" );
				}
				this.MaxItems = MaxItemsStats;
			}
			if (m.AccessLevel >= AccessLevel.GameMaster)
			{
				this.MaxItems = 925000;
			}
			
			this.InvalidateProperties();
			base.OnDoubleClick(m);
		}
		
		private static int MaxItemsAbilityInt (Mobile m, Item item)
		{
			PlayerMobile pm = m as PlayerMobile;
			
			double maxitem;
			maxitem = ((pm.Int * pm.Mana)) / 5;
			if (pm.Skills.Total >= 500)
			{
				maxitem += pm.Skills.Total;
			}
			
			return (int)maxitem / 10;  
		}

		public override void UpdateTotal(Item sender, TotalType type, int delta)
		{
			base.UpdateTotal(sender,type,delta);
			if(type==TotalType.Weight)
			{
				if ( Parent is Item )
					( Parent as Item ).UpdateTotal( sender, type, (int)(delta*m_Redux)*-1 );
				else if ( Parent is Mobile )
					( Parent as Mobile ).UpdateTotal( sender, type, (int)(delta*m_Redux)*-1 );
			}
		}
		
		public override int GetTotal(TotalType type)
		{
			if(type==TotalType.Weight)
				return (int)(base.GetTotal(type)*(1.0-m_Redux));
			return base.GetTotal(type);
		}
		
        public virtual bool Accepted
        {
            get
            {
                return this.Deleted;
            }
        }
		
        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool ret = base.DropToWorld(from, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.AccessLevel > AccessLevel.Player)
                {
                    return true;
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "You feel silly for wanting to drop something so useful...");
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {	
            bool ret = base.DropToMobile(from, target, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.AccessLevel > AccessLevel.Player)
                {
                    return true;
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "This cannot be traded!");
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            bool ret = base.DropToItem(from, target, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.AccessLevel > AccessLevel.Player)
                {
                    return true;
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "This can only exist on the top level of the backpack!");
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
		
		public override void Serialize( GenericWriter writer ) 
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write((double)m_Redux);
			writer.Write((int)m_BoundToMobile);
			writer.Write((int)m_MultiplierMax);
			writer.Write((int)m_DivideVarMax);
			writer.Write((bool)m_UseStatsforMaxItems);
		}
		public override void Deserialize( GenericReader reader ) 
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Redux = reader.ReadDouble();
			m_BoundToMobile = reader.ReadInt();
			m_MultiplierMax = reader.ReadInt();
			m_DivideVarMax = reader.ReadInt();
			m_UseStatsforMaxItems = reader.ReadBool();
		}
	}
 
}