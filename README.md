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
has squad, 1 member must be lighter with stick, others can do anything
each has primary, secondard, and artifact items

difficulty done by length of map and complexity of missions

only can save at end of mission, just saves squad character data nothing else

each mission rewards gold depending on how they did as well as a fixed ammount for upkeep depending on difficulty (less = harder)

push escape to see squad overview of who has what stats and items

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
 1. swap/give/take primary
 2. swap/give/take artifact
 3. part ways
 
 

## map gen

map is premade terrain with rocks and trees and lakes etc

each level picks 1 point on the map to start
(depending on difficulty distnance to end is changed)
then a random point for end is picked, it must be n distance from start (if fails after 5 times, pick new start pint)

then a road is generated between the 2 points (if failed, pick new points)

the road then clears the terrain around it

start and end each generate a premade town
 - town gate only open if its a defend town

## Items

no items are ever dropped, can only swap items at start or end, or market

all items, like sword, trinket etc are subclasses of the item class

each character has 3 slots, primary and secondary are on body and can be swapped with a key
if character is lighter their secondary item still exists on them, but cannot be swapped to as ligth stick ont their back

items have a dynamic characteristic system
 - name: effect on weapon ; effect of artifact
 - vampiric: life steal on hit ; any costs or downsides go to others 
 - shielding: invulnerability when lands a hit ;others around them get hit, redirects to them
 - energizing: increase movement speed on hit; all in zone get movement speed buff
 - flaming: lights hit on fire ; chance to ignite attacker if hit
 - chilling; chance to freeze hit; chance to freeze attacker if hit
 - reaching; longer attack range; all other artifact effects in its zone are increased
 - strong; more damage of hit; stronger atrifact effect
 - weak; less damage ; weaker artifact effect
 - sluggish; slower attack speed; slower movement speed for all in its effect
 - sharing; hits do a portion of their damage to any enemy in range even if not hit direct; shares artifact effect in radius
 - giving; damage to enemy is inverted to heal them; effects given to others but not self
 - costly; lowers potency of item on hit ; makes effect triggering cost mana

 
 any weapon or artifact can have an optional effect
 
 ### weapons
 
 sword
 
 axe
 
 bow?
 
 magic wand (fireball, ice wave, healing, speed boost, flame blast (lights lamps and sets all on fire))
 
 shield
 
 ### artifacts
 
 
 trinket
  - just any item but can have effect
  - doesnt do anything on its own
  - guarneteed to have an effect
 
 charm of shielding
   - absorbs % of damage for holder
   - costs mana to do so

charm of healing
 - slow health regen over time
 
 charm of harming
  - slow health decay over time
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


