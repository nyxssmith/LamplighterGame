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
- ] forward time
- scroll wheel or , . to zoom in/out the camera
- left click to use held item
- right click TODO (mybe make use belt item)
- walk into items to pick up / hold (if they are marked as "pickup")




# Game Layout

Status's
- `*` means implimented and working
- resource cost `n` is TBD

## Towns

Each has its own resource generation and economy

Can set the faction of all its residents when they move in/ generate

### Professions

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
- Warehouse
  - Controlls the amount of each resource that at town can store
  - Wood and food excess decay over time
  - More can be built to increase town storage
    - resource decay always the same
    - only needs to be built more when generation is high enough
  - Building costs:
    - Wood n
    - Stone n
- Lamplighter House
  - Same building as house, but is used as a start and stop point for lampligher routes
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
- all houses are actual houses
- create all buildings in list
- split all building types into own classes from main building controller
- add more items
- make buildings "unlockable" in build tool instead of all available at start
- make the controls / key inputs configurable
- fix bugs with the entering town/switching characters
- do all TODO in the code

