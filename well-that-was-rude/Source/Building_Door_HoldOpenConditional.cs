using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Verse;
using Verse.Sound;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    public class Building_Door_HoldOpenConditional : Building_Door
    {
        public int maxCellsToCheck = 6;                         //Max distance to hold a door for a pawn

        private DoorMode        doorMode = DoorMode.DEFAULT;    //Takes control of "holdOpenInt"
        private Command_Toggle  holdOpenToggle;                 //Reference to HoldOpen gizmo

        public override void SpawnSetup(Map map)
        {
            base.SpawnSetup(map);
            this.GetGizmos(); //Instantiate the new toggle
        }

        public bool ShouldHoldOpenNow
        {
            get
            {
                //Log.Message("Should I hold the door open?");
                foreach (Pawn p in PawnsFinder.AllMaps_Spawned)
                {
                    if (p.Position != Position && PawnCanOpen(p) && (Position - p.Position).LengthManhattan < 2*maxCellsToCheck)
                    {
                        //Log.Message(" " + p.Name + " sure is close to it");
                        PawnPath path = p.pather.curPath;
                        if (path != null && path.inUse)
                        {
                            float moveSpeed = p.GetStatValue(StatDefOf.MoveSpeed);
                            for (int lookAhead = 1; lookAhead < (float)maxCellsToCheck * moveSpeed; lookAhead++)
                            {
                                if (lookAhead < path.NodesLeftCount && path.Peek(lookAhead).GetDoor(Find.VisibleMap) == this)
                                {
                                    //Log.Message(" Hold the door for " + p.Name);
                                    return true;
                                }
                            }
                        }
                    }
                }
                //Log.Message(" No reason to hold it open");
                return false;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue(ref doorMode, "doorMode", DoorMode.DEFAULT, true); //Save doorMode
        }

        public override void Tick()
        {
            if (doorMode == DoorMode.HOLD_OPEN_CONDITIONAL)
            {
                if (Open && Map.thingGrid.CellContains(Position, ThingCategory.Pawn))
                {
                    HoldOpenInt = ShouldHoldOpenNow;
                }
            }
            base.Tick();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos())
            {
                Command_Toggle ct = g as Command_Toggle;
                if (ct != null && ct.icon == TexCommand.HoldOpen)
                {
                    ct.turnOffSound = null;
                    ct.turnOnSound = null;
                    holdOpenToggle = ct;
                    yield return ToggleDoorMode; //Replace HoldOpen gizmo
                }
                else
                {
                    yield return g;
                }
            }
        }

        private Command_ToggleDoorMode ToggleDoorMode
        {
            get
            {
                //Replace gizmo with HoldOpenConditional gizmo
                Command_ToggleDoorMode ro = new Command_ToggleDoorMode();
                ro.defaultLabel = "CommandToggleDoorHoldOpen".Translate();
                ro.defaultDesc = "CommandToggleDoorHoldOpenDesc".Translate();
                ro.hotKey = KeyBindingDefOf.Misc3;
                ro.icon = TexCommand.HoldOpen;
                ro.getMode = () => doorMode;
                ro.toggleAction = () =>
                {
                    if (ro.getMode() == DoorMode.DEFAULT)
                    {
                        doorMode = DoorMode.HOLD_OPEN_CONDITIONAL;
                    }
                    else if (ro.getMode() == DoorMode.HOLD_OPEN_CONDITIONAL)
                    {
                        doorMode = DoorMode.HOLD_OPEN_ALWAYS;
                        HoldOpenInt = true;
                    }
                    else
                    {
                        doorMode = DoorMode.DEFAULT;
                        HoldOpenInt = false;
                    }
                };
                return ro;
            }
        }

        private bool HoldOpenInt
        {
            set
            {
                if (holdOpenToggle != null && holdOpenToggle.isActive() != value)
                {
                    holdOpenToggle.toggleAction();
                    Map.reachability.ClearCache();
                }
                //Log.Message("HoldOpenInt set to " + value);
            }
            get
            {
                return holdOpenToggle.isActive();
            }
        }
    }
}


