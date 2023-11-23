This was adapted from the XMLSpawner system created and or updated by ArteGordon.
Upon the release of the new direction for ServUO the attachment system was not
adapted and serviced into the new direction.  So with that I have pulled the
attachment system into its own unique instance.  Everything has been renamed so even
if something already exist, this should not cause any conflicts. 

IMPORTANT NOTE: Since all the distro edits in regards to the attachment system
were removed from the distro, the features that normally would work will not! 
OnWeaponHit, IsEnemy As an example.  Features that were based in event syncs 
should be fine. 

I have no intention of including the distro edits needed to make certain
aspects of the spawner work unless it makes sense to the level system.  

Confimed Working without Distro edits: 
Attachments can be used as data storage and value references.
Attachments Onmovement methods do work. 
Attachments with Speech Handlers do work.

The Unique serial Save location is what is used by default.  When the attachments save it goes to a unique folder
instead of sharing the same folder that XMLSpawner2 would use, this prevents conflicts.  