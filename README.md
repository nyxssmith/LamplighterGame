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

Status's
- `*` means implimented and working
- resource cost `n` is TBD

## Victory Conditions

n can be user set, defaults to 5

must get n points to win
points can come and go based on world status

points are calculated at the end of each day or when a vote is called


town having 3+ greater population than all other towns = 1 pt
 - towns can start with different populations, some migth have an advantage to get to this

town mayor having ownership of a town (1 point each)
 - game starts with all mayors/players having 1 point
 - can get more points by taking over another town in any way
     - can be bought out
     - can be fighted out
     - can be churched out (negotiate and put church in town)
     - can be a trade partner and vote on which mayor is main one

town is voted as current capitol of the island = 1pt
 - determined by town "score" which is made up of wealth and population and size
 - every x days a town can pay to call a vote, this then picks the capitol (they can loose the vote)

town has 1 of all possible buildings built and worked in it = 1pt
 - if town is on water, must have a dock
 - if town is by mines, all mines must be worked
 - must have alchemist
 - must have blacksmith
 - must have shop
 - must have houses
 - must have shops

town has trade route with 2 other towns = 1pt each
 - if town a has route with b,c to get next point, must have routes with d,e
 - some towns can start with a trade route as an advantage

town has monument built 1 point each
 - monument is super expensive building that buffs all townsfolk in an area

town has most of a stat of all towns (1 point per two stats met) (.5 point per stat met, but 2.5 is rounded down to 2 points given)
 - most mana in the world and magic buildings (floating lights built etc)
 - most wealth in world (town has most gold)
 - most ore
 - most metal
 - most wood
 - most farms



## Towns

Each has its own resource generation and economy

Can set the faction of all its residents when they move in/ generate

### Professions

- Mayor
  - is the player or npc owns a town, if dies title passes to another townsperson
  - wanders the town center and is the leader to talk to
- Farmer
  - Mans farms
- Woodcutter
  - chops trees
  - works lumbermill
- Blacksmith
  - Works blacksmith
- Miner
  - Works mines
- Gaurd
  - Gaurds Posts
  - Gaurds Traders
- Shopkeeper
  - Works shops
- Trader
  - Goes on trade routes
- Bandit
  - Bandits shit
- Lampligher
  - Lights roads
- Alchemist
  - Works alchemist

### Building List

- House
  - Ideal state: 1 house per resident
  - Cannot recruit new people to town unless houses > residents in the town
  - If house is destroyed, residents will share
  - Building costs:
    - wood n
    - stone n
    - metal 1
- Farm
  - Location for farmers to work
  - Max 3 farmers per farm
  - Makes `1.5` food resource per person working the farm, up to max of `3x1.5=4.5`
  - Food resource is required for a residents work to be done
    - Residents will still work and act normally, but each needs 1 food per day do do work / work to be turned into resources at end of day
    - Farms get priority for food resource > resources made conversion, then calculated for other buildings
    - Farms cost 1 food to run, but if food = 0 they will still run
  - Building costs:
    - wood n
- Shop
  - Location for Shopkeepers to work
  - Makes money resource for the town per shop that is open
  - Can buy town resources at shop for trade route
  - Max 1 trade route per shop 
  - Generates up to 3 items depending on other buildings in town (if they are worked)
    - Sells potions if potion maker in town 
    - Sells weapons if blacksmith in town
    - Sells magic items if both alchmist and blacksmith in town
  - Building costs:
    - wood n
    - metal 1
- Blacksmith
  - Refines Ore resource into metal resouce
  - If worked, allows shops to sell weapons and tools
  - Building costs:
    - stone n
    - metal n
- Lumbermill
  - Is worked by lumberjack
    - Lumberjack will go cut tree, then return to mill to preduce 1 wood unit per day per lumberjack
  - 2 Lumberjacks can work 1 lumbermill
  - Building costs:
    - stone n
    - wood n
    - metal n
- Alchomist
  - Worked by alchomists
  - If worked allows shops to sell potions
  - Building costs:
    - wood n
    - stone n
    - metal 1
- Mine
  - Generates Stone and Ore resources
    - 1:.25 ratio of stone to ore
    - only 1 is produced per work unit, with 75% being stone, 25% being ore
  - N miners can work the mine
    - Each miner working generates 1 unit of work
  - Has chance to injure miners per day
  - Building costs:
    - N/A cannot be built, all mines will be premade
- Warehouse (might skip it)
  - Controlls the amount of each resource that at town can store
  - Wood and food excess decay over time
  - More can be built to increase town storage
    - resource decay always the same
    - only needs to be built more when generation is high enough
  - Building costs:
    - Wood n
    - Stone n
- Lamplighter House
  - Same building as house, but is home of existing lamplighters, spawns a new one if their owner is killed
  - Ambient building
  - Building costs:
    - N/A All will be preplaced and cannot be deleted, as they are owned by lamplighters
- Guard Post
  - Location that Guards can be assigned to
  - When assigned they will wait there and be on lookout for enemies
  - Building costs:
    - wood n
- Fences and Walls
  - Are barriers that cost wood to build
  - Can be burnt down by bandit raids (walls harder to do so)
- Docks
  - New characters arrive at docks and either go on thier own quests or join villages
  - source of new residents
  - Building costs TBD if allowed to be built
- Church
  - can be visited to replenish mana
  - Building costs:
    - Wood n


# TODO LIST

`*` = started

- better dialog and HUD (add background to text)
- right click to use belt item
- quests for npcs to do complex tasks
- split charactercontroller tasks/professions into their own classes
- Shop dialog generation
- town resources*
- create all buildings in list
- split all building types into own classes from main building controller
- add more items
- make buildings "unlockable" in build tool instead of all available at start
- make the controls / key inputs configurable
- fix bugs with the entering town/switching characters
- do all TODO in the code
- raids on towns
- bandits on roads
- lampligher jobs based on quests

