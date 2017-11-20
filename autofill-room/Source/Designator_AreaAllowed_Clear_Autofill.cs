using Verse;
using UnityEngine;

namespace RimWorld
{
    public class Designator_AreaAllowedClear_Autofill : Designator_AreaAllowedClear
    {
        public Designator_AreaAllowedClear_Autofill() : base() { }
        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            return (AcceptanceReport)(c.InBounds(this.Map) && Designator_AreaAllowed.SelectedArea != null);
        }
        public override void DesignateSingleCell(IntVec3 c)
        {
            base.DesignateSingleCell(c);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Room room = RegionAndRoomQuery.RoomAt(c, Map);
                if (room != null)
                {
                    foreach (IntVec3 vec in room.Cells)
                        base.DesignateSingleCell(vec);
                    if (!room.IsHuge)
                        foreach (IntVec3 vec in room.BorderCells)
                            base.DesignateSingleCell(vec);
                }
            }
        }
    }
}
