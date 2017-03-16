using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class Command_ToggleDoorMode : Command
    {
        public SoundDef turnOnSound = SoundDefOf.CheckboxTurnedOn;
        public SoundDef turnOffSound = SoundDefOf.CheckboxTurnedOff;
        public Func<DoorMode> getMode;
        public Action toggleAction;

        public override SoundDef CurActivateSound
        {
            get
            {
                if (getMode() == DoorMode.HOLD_OPEN_ALWAYS)
                    return this.turnOffSound;
                return this.turnOnSound;
            }
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            this.toggleAction();
        }

        public override GizmoResult GizmoOnGUI(Vector2 loc)
        {
            GizmoResult gizmoResult = base.GizmoOnGUI(loc);
            Rect rect = new Rect(loc.x, loc.y, this.Width, 75f);
            Texture texToUse = (Texture)Widgets.CheckboxOffTex;
            if (getMode() == DoorMode.HOLD_OPEN_CONDITIONAL)
            {
                texToUse = (Texture)Widgets.CheckboxPartialTex;
            } else if (getMode() == DoorMode.HOLD_OPEN_ALWAYS)
            {
                texToUse = (Texture)Widgets.CheckboxOnTex;
            }
            GUI.DrawTexture(new Rect((float)((double)rect.x + (double)rect.width - 24.0), rect.y, 24f, 24f), texToUse);
            return gizmoResult;
        }
        
        public override bool InheritInteractionsFrom(Gizmo other)
        {
            Command_ToggleDoorMode commandToggle = other as Command_ToggleDoorMode;
            if (commandToggle != null)
                return commandToggle.getMode() == this.getMode();
            return false;
        }
    }
}