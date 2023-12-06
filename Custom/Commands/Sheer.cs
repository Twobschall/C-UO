using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Items;

namespace Server.Commands
{
    public class SheerCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("Sheer", AccessLevel.Player, new CommandEventHandler(OnSheerCommand));
        }

        [Usage("Sheer")]
        [Description("Sheers all sheep within 10 tiles of the player.")]
        private static void OnSheerCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            // Iterate through all mobiles on the map within 10 tiles
            foreach (Mobile mobile in from.GetMobilesInRange(10))
            {
                // Check if the mobile is a living sheep
                if (mobile is Sheep && mobile.Alive)
                {
                    Sheep sheep = (Sheep)mobile;  // Explicitly cast to Sheep

                    // Replace 'ButcherKnife' with the actual item you want to use in the OnSheer method
                    Item actualItem = new ButcherKnife();  

                    // Call the Carve method on the sheep
                    bool success = sheep.Carve(from, actualItem);

                    // Optionally, you can notify the player about the successful Sheer
                    if (success)
                        from.SendMessage("You successfully Sheer the sheep.");                    
                }
            }
            from.SendMessage("Sheering attempt complete.");
        }
    }
}
