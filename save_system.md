# Save System

## Saves File/Folder Layout
Saves
  - Save-Timestamp
    - Towns
      - `town_uuid.json`
        - Saves all resources
        - Saves UUID
        - Saves position
    - Characters
      - CharacterUUID
        - `uuid.json`
          - saves same as character data
          - Postion
          - Buildings list (as UUIDS)
        - `quest.json`
          - Saves same as quest data
          - Current step
        - Items
          - `item_uuid.json`
            - Saves same as itemdata
            - Saves holder
            - Saves position
            - Saves rotation
    - Items
      - `item_uuid.json`
        - Saves same as itemdata
        - Saves holder
        - Saves position
        - Saves rotation
    - Buildings
      - `building_uuid.json`
        - Saves UUID
        - Saves type
        - Saves "has done work" value
        - Saves List of characterUUIDs
        - Saves position
        - Saves rotation(s)

All Saves are saved as json into each save folder.

"Save" objects are each structured like above describes, with each `.json` file being a sub object that is serializable to be output to file.
"Save" objects are serializable and can be output to file (or each part is TODO)

#### Saving

1. Create blank "Save" object
1. Loop through "all x in scene" to create lists of characterSave etc to populate save object
1. Save objects are output to files

#### Loading

1. A Save object is created from serialized saved copy
1. All existing towns, buildings etc are destroyed
1. Convesion from Save object to other objects happens
  1. Towns are populated first (position only)
  1. Buidling are created (position and town ref only)
  1. All characters are created and placed
    - And all items that they have
    - and all quests they have
  1. All items are created and placed (that were not already made)
1. All buildings are repopuldated with data such as type etc
1. All characters are repopulated with data such as buildings etc









# Save Manager .cs

## Save towns

## Save buildings

## Save Characters

## Save Items


