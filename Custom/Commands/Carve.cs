using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Items;

namespace Server.Commands
{
    public class CarveCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("carve", AccessLevel.Player, new CommandEventHandler(OnCarveCommand));
        }

        [Usage("carve")]
        [Description("Carves all uncarved corpses within 10 spaces of the player.")]
        private static void OnCarveCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Iterate through all items on the map
            foreach (Item item in from.GetItemsInRange(10))
            {
                // Check if the item is a corpse
                if (item is Corpse)
                {
                    Corpse corpse = (Corpse)item;  // Explicitly cast to Corpse

                    // Check if the owner of the corpse is a BaseCreature and the corpse is not carved
                    BaseCreature creature = corpse.Owner as BaseCreature;  // Explicit cast to BaseCreature
                    if (creature != null && !corpse.Carved)
                    {
                        // Replace 'someItem' with the actual item you want to use in the OnCarve method
                        Item actualItem = new ButcherKnife();  // ButcherKnife keeps the yield on the corpse, SkinningKnife puts it into the players backpack.

                        // Call the OnCarve method
                        creature.OnCarve(from, corpse, actualItem);

                        // Set the carved attribute to true
                        corpse.Carved = true;

                        // Optionally, you can notify the player about the successful carve
                        from.SendMessage("You successfully carve items from the corpse.");
                    }
                }
            }
            from.SendMessage("Carving complete.");
        }
    }
}
