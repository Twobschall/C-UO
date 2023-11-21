using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Items;

namespace Server.Custom {
	public class BankStone: Item {

		[Constructable]
		public BankStone():base(0x1F1C) {
			Hue = 1161;
			LootType = LootType.Blessed;
			Name = "Bank Stone";
		}

		public BankStone(Serial serial): base(serial) {
		}

		public override void OnDoubleClick(Mobile from) {
			if(from != null) {
				if (!IsChildOf(from.Backpack)) {
					from.SendMessage("This item must be in your backpack to use it");
				} else {
					if (from.BankBox != null)
						from.BankBox.Open();
				}
			}
		}

		public override void Serialize(GenericWriter writer) {
			base.Serialize(writer);
			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader) {
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}
