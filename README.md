# Lamplighter game


### DIALOG

- easy add lines and make dialog trees
- generate dialog tree on the fly for squad join and leave

### CONTROLS

- wasd movement
- left shift sprint
- e interact
- q drop (if holding item)
- f swap hand and back items
- g swap hand and belt items
- ] switch timescale in .5 increments up to 10x speed
- scroll wheel or , . to zoom in/out the camera
- left click to use held item
- right click TODO (mybe make use belt item)
- walk into items to pick up / hold (if they are marked as "pickup")




# Game Layout

single player, no swap

each mission/level = start and end point on map, generate road between them
 - each level = 1 chapter

has squad, 1 member must be lighter with stick, others can do anything
each has primary, secondard, and artifact items

difficulty done by length of map and complexity of missions

only can save at end of mission, just saves squad character data nothing else

each mission rewards gold depending on how they did as well as a fixed ammount for upkeep depending on difficulty (less = harder)

push escape to see squad overview of who has what stats and items

multiple save slots, once a save has pased x level, its optional to join another save for a mission
- disables the item shop
- allows saves to trade items or people between them

Each save stores history of all maps and squadmates who died/left/were dismissed
 - editor can bring back old ones, or reset their item cost
 - history lets you replay older maps/missions but with current squad
 - each map stores to time to beat/score


## planned tier system idea

 - tier 1 save
is the normal save, make character and squad

once tier 1 save has an npc tag along for N (5) perfect runs, they can be dismissed and added as a tier 2 save slot

a perfect run is a run where done in under half time, all objectives etc / very good score

- teir 2
same as tier 1 but cannot change any items or stats of the player character, since they were defined in tier 1.

if you made a character that was only support and they got to go to tier 2, then that is gonna be a harder tier 2 run

once tier 2 save can do N perfect runs, the whole squad is added to tier 3 and tier 2 save is removed

- tier 3
all unplayable, but can show up in tier 1 save missions as NPCs (max 1/3 of the options)

when they show up, they mention they are "1/N" where N is total number of tier 3 characters, so you know if you can even try the boss run or not.

their options are
 - aid for the next mission and basically 1 hit everything and make it a gaurneteed perfect, even if time is ignored
 - be added to the boss run team

boss run team is per tier 1 save, and if you add character to it, they are removed from total pool, but tier 3 pool is shared across all tier 1 saves.

if the character is used to easy-mode a mission, they are still left in the pool.
 <br>
 
 - boss run
 a special mission that is made up of a tier 1 player character (but not their existing team) and N (3) tier 3 characters as the team.
 
 boss run can be triggered instead of "next mission". Will be a special hard mission( or sequence of missions) that is the final boss of the game.
 The scores for this are the main scoreboards to beat etc.

once tier 3 are used in a boss run, no matter the outcome, they are removed from the list.


 - overview
idea is that playing the game in tier 1 will unlock a "hardmode" that is tier 2 once the teir 1 team is clearly easy-ing the levels

tier 2 is the "new game plus" since you cannot change your own weapons.

once you do tier 2 enough, youll encounter tier 3 characters alot and have the option for the boss run, while building for tier 2 again.



## squad

squad can be n people


can equip each member at the start of each mission
can buy new members at end of each mission
each member has an upkeep fee to remain as part of squad

if any member is attacked they will fight back no matter what the mode is

can toggle between 3 squad modes with tab
 - follow lead: just follow the leader
 - attack: will attack any enemy on sight
 - focus attack: will follow and all members will gang up on the players target, ignoring if they are attacked or not, until player target is dead

equiping is only primary slot
ALL npcs only use primary item
you have option to give npc a primary item this will
 - swap their primary to their secondary and use what is given
 - if given lamplighter stick, then will put that as secondary
if you have a primary and so do they, option = swap them
if you have no primary and they have yours, option = take

you have option to give them an artifact
 - if they have one you gave, it will be swapped out
 - if you have none, you will take it from them
 - if they have none, it will give them yours



## missions

each starts and ends at lamplighter outpost

can be made up of 1 or more objectives

objectives
 - light road (must light all lamps between points)
 - defend town (defend either the starting or ending town)
 - kill target (must kill a certain target in the world, usually along the road)
 - escort (must do mission with also a random person in party who doesnt fight)
 
 if light road isnt an objective (90% chance it is) then the road starts pre-lit
 
 end of mission = in tavern
 each tavern sells 3 random items and has 2 random recruits who can join
 go up to item of member to recruit or buy
 
 go up to sqaudmember to push key:
 1. give them primary or secondary to replace theirs
 2. merge/give artifact (always merge, cannot un merge or reset)
 3. part ways
 
 

## map gen

map is premade terrain with rocks and trees and lakes etc

each level picks 1 point on the map to start
(depending on difficulty distnance to end is changed)
then a random point for end is picked, it must be n distance from start (if fails after 5 times, pick new start pint)

then a road is generated between the 2 points (if failed, pick new points)

then 2 more roads are made from each point to somewhere else farther past them away from other point
- these roads are prelit and just atmosphere

the road then clears the terrain around it

start and end each generate a premade town
 - town gate only open if its a defend town

## Items

no items are ever dropped, can only swap items at start or end, or market

all items, like sword, trinket etc are subclasses of the item class

each character has 3 slots, primary and secondary are on body and can be swapped with a key
if character is lighter their secondary item still exists on them, but cannot be swapped to as ligth stick ont their back
 
 any weapon or artifact can have an optional characteristic
 
 All item effects are always active
 ex: if your primary weapon lights enemies on fire, and your secondary steals life, then both of them will have both effects
 
 This allows for choosing an item purely for its effect, to have more freedom in builds along the way.
 It also allows you to equip 2 artifacts and 1 primary to just use the effects, or use a secondary for just its effect.
 
 
 ## Characteristics
 
 All effects are also characteristics
 
 
 weak
  - weaker damage / value
 strong
  - stronger damage / value
 unwieldy
  - means is primary only, and cannot swap to other weapon
  - decided at itemClass time
  - 1/3 chance of being unwieldy
  - additive
 
 special ones
 # TODO how to impliment
 giving
  - effect is given to all except owner
 sharing
  - effect is given to all around including owner
 
 
 ## Effects
 
 each one of these is a collider bubble on the item / is a prefab effect bubble that is summoned onto the item then has its potency etc set

 all percentages or amounts of x are based on potency
 all effects take mana based on potency to function unless specified
 all ambient mana drain is <1 mana per use, so very cheap, but also not very powerful
 

 
 vampiric
  - life steal on hit
 
 
 speedy
  - increase movement speed
  - doesnt cost mana
 
 
 regeneritive
  - slow life regen over time
 
 
 decaying
  - slow life loss over time
 
 
 fading
  - slow potency decrease over time for other effects
 
 
 shielding
  - damage reduction on hit
 
 
 freezing
  - freeze target on hit
 
 
 flaming
  - ignite target on hit
 
 
 sluggish
  - slower movement speed
 
 
 clumsy
  - percent damage dealt is also given to holder
 
 
 redirecting
  - percent damage dealt to holder is redirected to others around
  - can redirect to a squadmate as well
 
 
 expanding
  - all other item effects ranges are multiplied by n, n being effected by this items potency
  - doesnt take any mana
 
 
 life-leaching
  - drains health from those around and gives to holder
  - percent lost based on potency (ex drains 10 health from A and gives 8 to B)
 
 
 mana-leaching
  - drains mana from those around and gives to holder
  - percent lost based on potency (ex drains 10 mana from A and gives 8 to B)
  - doesnt cost mana, super OP
 
 
 health-converting
  - on taking damage converts it to damage to mana instead
  - doesnt take mana to do this
 
 
 mana-converting
  - on mana drain, converts it to damage to health instead
  - doesnt take mana to do this
 
 tired
  - on hit spawns another effect bubble on hit target, that means when night happens, mana isnt regened next day
 
 ### weapons
 
 sword
 
 axe
 
 bow?
 
 magic wand (fireball, ice wave, healing, speed boost, flame blast (lights lamps and sets all on fire))
 
 shield
 
 ### artifacts
 
 
 trinket
  - just any item but can have characteristic (that must be an effect)
  - doesnt do anything on its own
  - guarneteed to have an effect
 

 charm of shielding
   - absorbs % of damage for holder
   - doesnt cost mana to do so

charm of healing
 - slow health regen over time
 - doesnt cost mana
 
 charm of harming
  - slow health decay over time
  - doesnt cost mana to do so
  - can synergize with other effects to drain enemy health, or just be junk
 
 charm of speed
   - faster movement speed
   - this is seperate from energizing characteristic, so energizing charm of speed is 2 stacked speed increases
  
  charm of mana regen
   - mana regen over time
  
  charm of mana storage
   - mana pool larger
  
  charm of ranged attacks
   - increase damage of all range attacks

charm of melee
 - increase damage of melee attacks

charm of slow time
 - rare
 - slows time before taking damage, if moved enough then dont take damage at all
 

charm of %element%
 - more potent %element% effects for user
 - just enhances any element effect of that type like fire


charm of %element% infusion
 - adds %element% damage to weapons

# TODO list

redo whole game using the charatcer controller animated one from customizable characters
rm everything and start from scratch

make a good menu/ GUI system

objects:
 - lamplighter outputs (tagged as end or start) (controller of some sort)
 - town building (any building in a town, can just be a unity tag)
 - ambient (all trees/rocks, can be a unity tag)
 - lamp post (controller)

all AI handed by a seperate object that puts each in its own thread


