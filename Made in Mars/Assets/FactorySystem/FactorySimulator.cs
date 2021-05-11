using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorySimulator : MonoBehaviour {

    public static FactorySimulator s;

    private void Awake() {
        if (s != null) {
            Debug.LogError(string.Format("More than one singleton copy of {0} is detected! this shouldn't happen.", this.ToString()));
        }

        s = this;
    }


    public void StartSimulation() {
        StartCoroutine(UpdateLoop());
    }


    float energyProduced = 100000;
    float energyUsed = 1;
    float efficiency;

    public int currentSimTick = 0;
    IEnumerator UpdateLoop() {
        yield return new WaitForSeconds(0.5f);
        while (true) {
            
            efficiency = energyProduced / energyUsed;
            if (efficiency > 1)
                efficiency = 1;
            energyUsed = 1;
            energyProduced = 100000;

            for (int i = 0; i < FactoryMaster.s.GetBuildings().Count; i++) {
                if (FactoryMaster.s.GetBuildings()[i] != null) {
                    if (FactoryMaster.s.GetBuildings()[i].craftController.isActive) {
                        energyUsed += UpdateBuilding(FactoryMaster.s.GetBuildings()[i], efficiency);
                    }
                }
            }

            for (int i = 0; i < FactoryMaster.s.GetConnectors().Count; i++) {
                if (FactoryMaster.s.GetConnectors()[i] != null) {
                    UpdateConnector(FactoryMaster.s.GetConnectors()[i]);
                }
            }

            FactoryVisuals.s.BeltVisualsUpdate();
            FactoryVisuals.s.ConnectorVisualUpdate();

            for (int i = 0; i < FactoryMaster.s.GetBelts().Count; i++) {
                if (FactoryMaster.s.GetBelts()[i] != null) {
                    UpdateBelt(FactoryMaster.s.GetBelts()[i]);
                }
            }


            if (currentSimTick % FactoryDrones.DronesSlowFactor == 0) {
                FactoryDrones.UpdateTasks();

                for (int i = 0; i < FactoryMaster.s.GetDrones().Count; i++) {
                    if (FactoryMaster.s.GetDrones()[i] != null) {
                        FactoryDrones.UpdateDrone(FactoryMaster.s.GetDrones()[i]);
                    }
                }
            }

            currentSimTick++;
            yield return new WaitForSeconds(1f / FactoryMaster.SimUpdatePerSecond);
        }
    }



    /// <summary>
    /// In each sim step each connect will find the next input slot item, and put it into the next free output slot.
    /// </summary>
    /// <param name="connector"></param>
    public static void UpdateConnector(Connector connector) {

        for (int job = 0; job < connector.itemTransportJobs.Length; job++) {
            var curJob = connector.itemTransportJobs[job];
            // If we are not moving an item, then take it.
            if (!curJob.isMovingItem) {
                // Move over every possible input source once
                for (int i = 0; i < connector.inputs.Count; i++) {
                    var n = (i + connector.inputCounter) % connector.inputs.Count;
                    // Try to take an item from a belt or a building
                    if (connector.inputs[n].CheckTakeItem(out Item curItem)) {
                        connector.inputCounter = n + 1;

                        if (ConnectorTryToPlaceItem(connector, curItem, out Connector.Connection target, connector.inputs[n])) {
                            curJob.isMovingItem = true;
                            curJob.isTakingFromInput = true;
                            curJob.startConnection = connector.inputs[n];
                            curJob.endConnection = target;
                            curJob.movingItem = curItem;
                            curJob.itemCurPos = Position.MoveCardinalDirection(curJob.startConnection.position, curJob.startConnection.direction, 1);
                            curJob.itemDir = Position.CardinalDirection(curJob.itemCurPos, curJob.startConnection.position);
                            connector.inputs[n].TryTakeItem(out curItem);
                            break;
                        }
                    }
                }
            } else {
                // The first move from belt/building ONTO connector
                if (curJob.isTakingFromInput) {
                    curJob.itemCurPos = curJob.startConnection.position;
                    curJob.itemDir = Position.CardinalDirection(curJob.startConnection.position, curJob.endConnection.position);
                    curJob.isTakingFromInput = false;


                    // Subsequent moves that are on top of the connector
                } else if (curJob.itemCurPos != curJob.endConnection.position) {
                    curJob.itemCurPos = Position.MoveTowards(curJob.itemCurPos, curJob.endConnection.position, 1);

                    if (curJob.itemCurPos == curJob.endConnection.position) {
                        curJob.itemDir = curJob.endConnection.direction;
                    }

                    // movement complete, try inserting to the slot. If cannot insert just stay here.
                } else {
                    if (curJob.endConnection.TryInsertItem(curJob.movingItem)) {
                        curJob.isMovingItem = false;
                    }
                }
            }
        }
    }

    static bool ConnectorTryToPlaceItem(Connector connector, Item curItem, out Connector.Connection target, Connector.Connection source) {
        // Move over every possible output source once to try to find an empty slot
        for (int i = 0; i < connector.outputs.Count; i++) {
            var n = (i + connector.outputCounter) % connector.outputs.Count;
            // Try to insert to a belt or a building
            if (connector.outputs[n] != source && connector.outputs[n].CheckInsertItem(curItem)) {
                connector.outputCounter = n + 1;
                target = connector.outputs[n];
                return true;
            }
        }

        target = null;
        return false;
    }






    /// <summary>
    /// Find the last empty spot in the belt and reduce it by 1.
    /// The Factorio belt optimization
    /// </summary>
    /// <param name="belt"></param>
    public static void UpdateBelt(Belt belt) {
        bool thereIsEmptySlot = false;


        if (belt.items.Count == 0) {
            Debug.LogError("Belt doesn't have belt segments! " + belt.startPos + " - " + belt.endPos);
            return;
        }

        // Edge case where the belt is perfectly uniform, hence we cannot move anything
        if (belt.items.Count == 1) {
            return;
        }


        var beltsLastSlot = belt.items.Count - 1;

        for (int i = beltsLastSlot; i >= 0; i--) {
            if (belt.items[i].item.isEmpty()) {
                var count = belt.items[i].count;
                count -= 1;
                bool slotDestroyed;
                if (count > 0) {
                    belt.items[i] = new Belt.BeltSegment() {count = count, item = belt.items[i].item};
                    slotDestroyed = false;
                } else {
                    belt.items.RemoveAt(i);
                    slotDestroyed = true;
                }

                //If we destroyed a slot, me might need to merge the new touching slots
                if (slotDestroyed) {
                    // We cannot merge if the destroyed slot was at the very end or at the very start
                    if (i != beltsLastSlot) {
                        if (i != 0) {
                            // Check if the current slot (right after the empty slot) and the previous slot have the same item
                            if (belt.items[i].item == belt.items[i - 1].item) {
                                belt.items[i - 1] = new Belt.BeltSegment() {
                                    count = belt.items[i].count + belt.items[i - 1].count,
                                    item = belt.items[i].item
                                };

                                belt.items.RemoveAt(i);
                            }
                        }
                    }
                }

                thereIsEmptySlot = true;
                break;
            }
        }

        if (thereIsEmptySlot) {
            if (belt.items[0].item.isEmpty()) {
                var count = belt.items[0].count;
                count += 1;
                belt.items[0] = new Belt.BeltSegment() {count = count, item = belt.items[0].item};
            } else {
                belt.items.Insert(0, new Belt.BeltSegment() {count = 1, item = Item.GetEmpty()});
            }
        }
    }





    /// <summary>
    /// Updates the building - handled by building crafting controller
    /// </summary>
    /// <param name="building"></param>
    public static float UpdateBuilding(Building building, float efficiency) {
        return building.UpdateCraftingProcess(efficiency);
    }
}
