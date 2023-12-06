using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Commands
{
    public class DumpCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("Dump", AccessLevel.Player, new CommandEventHandler(DumpCommand_OnCommand));
        }

        [Usage("Dump")]
        [Description("Move all items from one container to another.")]
        public static void DumpCommand_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new DumpTarget(e.Mobile);
            e.Mobile.SendMessage("Target the source container to move items from.");
        }

        private class DumpTarget : Target
        {
            private readonly Mobile m_From;

            public DumpTarget(Mobile from)
                : base(18, false, TargetFlags.None)
            {
                m_From = from;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Container destContainer = targeted as Container;
                if (destContainer != null)
                {
                    from.Target = new DumpTarget2(m_From, destContainer);
                    from.SendMessage("Target the destination container to move items to.");
                }
                else
                {
                    from.SendMessage("Invalid target. Please target a container.");
                }
            }
        }

        private class DumpTarget2 : Target
        {
            private readonly Mobile m_From;
            private readonly Container m_SourceContainer;

            public DumpTarget2(Mobile from, Container sourceContainer)
                : base(18, false, TargetFlags.None)
            {
                m_From = from;
                m_SourceContainer = sourceContainer;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                Container sourceContainer = targeted as Container;
                if (sourceContainer != null)
                {
                    MoveItems(m_SourceContainer, sourceContainer);
                    from.SendMessage("Items moved successfully.");
                }
                else
                {
                    from.SendMessage("Invalid target. Please target a container.");
                }
                }
            private void MoveItems(Container source, Container destination)
            {
                // Create a separate list to store items
                List<Item> itemsToMove = new List<Item>();

                // Add items to the list without modifying the source container during iteration
                foreach (Item item in source.Items)
                {
                    itemsToMove.Add(item);
                }

                // Move items from the list to the destination container
                foreach (Item item in itemsToMove)
                {
                    destination.AddItem(item);
                    source.Items.Remove(item); // Remove the item after moving it
                }
            }
        }    
    }
}
