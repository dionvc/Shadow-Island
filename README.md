# Shadow-Island

Under about there is proper credits for any material used.  Otherwise the rest was authored by me.  Most art was drawn using the Piskel or Aesprite software.
Some effects were rendered using blender.

Controls:

All controls except the debug controls are listed in the settings
There are a few hotkey commands
Press L to advance to next wave
Press K to receive all items to your inventory
Click + shift will craft 5 items
Click + ctrl will craft as much as possible

Goal:

Collect resources and build defenses to protect the beacon against enemies.  
Build machinery like drills and assemblers to help with automating defense.

Bugs/Kinda bugs/frustrations:

Mining a building destroys everything in it
Switching a recipe for a building destroys everything in it
Mining a belt leaves behind the items (should be picked up to inventory)
Blowing up a belt element can cause some weird bugs (but does not crash the game)
Can link a loader to yourself and unload your inventory directly into a chest while moving
Can use loaders/splitters to basically teleport items
Splitter filter screen will open with left filter unchecked even if left filter was previously applied.  Right filter works fine.  I'm sure I made a silly mistake somewhere in the UI code as the underlying values seem fine (something to do with toggle groups, probably should have used toggle groups)

Statistics:

~280 pieces of art (sometimes consisting of multiple frames)
~71 scripts
~75 prefabs which are used to instantiate most everything in the game
~7 particle types with a particle pooling system

AI:

Turret uses a rudimentary sort of distance checking AI with state
EnemyAI uses a more advanced AI which is built on top of A* using a custom designed distance to obstacle style A* map.
  The A* algorithm prebakes a map of values which reflect the distance to the nearest static obstacle in the map
  The pathing algorithm takes into account agent size when pathing (although I only finished art for one enemy so the actual usage of this system is not directly apparent)
  The EnemyAI has a double state of action (moving, attacking) and aistate (idle, pursuing, and formation)
  
Some Info:

Beacon:

![alt text](https://github.com/dionvc/Shadow-Island/blob/main/beacon.PNG?raw=true)

Defend at all costs.

Belt usage:

![alt text](https://github.com/dionvc/Shadow-Island/blob/main/beltusage.PNG?raw=true)

Loaders can be placed next to machines and chests and will load or unload items

Splitters will split items from both inputs to outputs by item

Filter usage:

![alt text](https://github.com/dionvc/Shadow-Island/blob/main/filterusage.PNG?raw=true)

Splitters and loaders can filter items
