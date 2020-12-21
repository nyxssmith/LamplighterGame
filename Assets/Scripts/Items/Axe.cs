// Instantiates 10 copies of Prefab each 2 units apart from each other
using System.Collections;
using UnityEngine;

public class Axe : MonoBehaviour
{
    //Parent of the potion
    private CharacterController Character;

    private ItemController BaseItem;

    private bool CheckStillHeld = false;

    // which action to trigger on
    private float ActionToFunctionOn = 1.0f;

    void Start()
    {
        BaseItem = this.gameObject.GetComponent<ItemController>();
    }

    public void SetCharacter(CharacterController CurrentCharacter)
    {
        Character = CurrentCharacter;
    }

    public void Update()
    {
        if (BaseItem.CanDoAction == ActionToFunctionOn)
        {
            BaseItem.CanDoAction = 0.0f;
            SetCharacter(BaseItem.ActionTargetCharacterController);
            BaseItem.SetActionTargetCharacterController(null);

            DoHit(BaseItem.GetDamage());
        }
        else if (Input.GetMouseButtonDown(0) && BaseItem.GetCoolDown() >= 0.0f && BaseItem.isPickedUp && BaseItem.GetHoldingCharacterController().GetIsPlayer())
        {
            // if not fighting, try to chop a tree
            DoChop();
        }
    }

    public void DoHit(float Damage)
    {
        Character.AddValueToHealth(-1.0f * Damage);
    }

    public void DoChop()
    {
        TreeInstance[] trees = Terrain.activeTerrain.terrainData.treeInstances;

        //TerrainData terrain = Terrain.activeTerrain.terrainData;
        ArrayList newTrees = new ArrayList();
        Vector3 terrainDataSize = Terrain.activeTerrain.terrainData.size;
        Vector3 activeTerrainPosition = Terrain.activeTerrain.GetPosition();
        float distance;

        foreach (TreeInstance tree in trees)
        {
            distance =
                Vector3
                    .Distance(Vector3.Scale(tree.position, terrainDataSize) +
                    activeTerrainPosition,
                    BaseItem.transform.position);

            if (distance > 1)
            {
                newTrees.Add (tree);
            }
        }
        Terrain.activeTerrain.terrainData.treeInstances =
            (TreeInstance[]) newTrees.ToArray(typeof (TreeInstance));

        // Now refresh the terrain, getting rid of the darn collider
        float[,] heights =
            Terrain.activeTerrain.terrainData.GetHeights(0, 0, 0, 0);
        Terrain.activeTerrain.terrainData.SetHeights(0, 0, heights);

        // TODO maybe back up all chars positions, so they dont fall out of world
        Terrain.activeTerrain.GetComponent<TerrainCollider>().enabled = false;
        Debug.Log("BRIEF PAUSE IN THE RUN LINE FOR TREE CHOPPING");
        // TODO maybe put town stuff here as the pause
        Terrain.activeTerrain.GetComponent<TerrainCollider>().enabled = true;

        // TODO add wood value to town
        
    }

    // todo GENERATE fire color from holding charatcer controlelrs factions
    // normal color = lamploghter (standard time)
    // green = tech (1.25 time)
    // blue = magic (2x time)
    // purple = ??? somehting else? (infinte maybe?)
}
