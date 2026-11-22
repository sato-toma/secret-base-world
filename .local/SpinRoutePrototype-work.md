# SpinRoutePrototype work checklist

- [x] Locate the lower-gate route marker, south-west return corridor, TempleOrnaments, and entrance-return stop.
- [x] Add a traversable second-to-first-floor connection that reaches `Detour_73_LowerGate_Start_Direction`.
- [x] Place a monument or route cue in the +Z space beyond `ReturnCorridor_SouthWest_Lower`, without moving the BGM or anything under `LOCKED_`.
- [x] Resolve the `TempleOrnaments` / `Detour_25_EntranceReturn_Stop` overlap while preserving the original ornament mesh.
- [x] Save the scene and verify route geometry, clearances, transforms, and Unity Console/compiler state.

## Inspection notes

- `Detour_73_LowerGate_Start_Direction` is at world position `(18, -3, 9)`.
- The second-floor platform is continuous above this route; no named stair, ramp, or ladder exists. A direct staircase needs a floor opening, while an outside/perimeter route would be longer.
- The `BGM` object is under `LOCKED_/System` at the origin; it was not moved or edited.
- The stop trigger was moved +0.1 m in Z (from -4.0 to -3.9). Its previous mesh reference was preserved as `TempleEntranceOrnaments_EntranceStopClearance`; the original ornament asset is unchanged.
- `ReturnCorridor_SouthWest_Lower` is at `(5.5, -2.46, 15.5)`; the +Z side has a limited placement gap before the next boundary geometry.
- Chosen connection: a direct, temple-appropriate staircase through a floor opening (confirmed with the user).

## Implementation and verification

- Cut a 1 x 1 m opening centered at `(17, 9)` in `SecondFloor_East_WestRim`, using matching floor panels and preserving adjacent maze walls.
- Added a 20-step, 360-degree stone spiral stair at `(17, -3, 9)` with a static MeshCollider and center spindle. All 20 tread raycasts hit the stair collider; physics overlap queries found no actual intersections with nearby BoxColliders.
- Added a 1.96 m stone-and-gold marker at `(8.4, -3, 18.5)` under `1stFloor`, beyond the return corridor on its +Z side. Its decorative parts have no colliders. The `BGM` and `LOCKED_` hierarchy remain untouched.
- The clearance copy retains all three material submeshes. Triangle-bounds checks found six original `MinimalTrim` triangles intersecting the stop volume and zero intersecting triangles in the clearance copy. The stop bounds no longer intersect nearby BoxCollider bounds.
- Scene saved. Unity reports compilation succeeded with zero current Console errors or warnings. Play Mode and Cluster runtime traversal were not tested.

## Follow-up request (2026-11-22)

- [x] Leave the undersized stair inactive; any replacement must provide at least a 4 x 4 m footprint.
- [x] Add a temple-like focal object on the first floor near the BGM area without moving or editing `LOCKED_`.
- [x] Inspect the 4 x 4 m stair footprint and its effect on the upper-floor maze; no clear location exists at the BGM point.
- [x] Survey other 4 x 4 m stair locations while preserving the existing maze layout.
- [x] Remove `TempleOrnaments` render-mesh triangles from the conservative `Detour_24_EntranceReturn_Start` spin corridor while preserving the original mesh.
- [x] Save the scene and recheck object paths, collision clearances, and Unity Console/compiler state.

### Follow-up inspection

- `BGM` is at world `(0, 0, 0)` under `/LOCKED_/System`; the first-floor platform is at `y=-3`.
- A 4 x 4 m opening centered at the BGM point is not currently clear on the upper floor: maze walls, triggers, and gate objects occupy that footprint. Do not remove them without resolving the route-layout choice.
- `Detour_24_EntranceReturn_Start` is at `(2, 0.54, -4)` and faces `-X`; the TempleOrnaments render mesh overlaps a conservative spin corridor, but TempleOrnaments itself has no Collider. Inspect the adjacent wall colliders before selecting a surgical fix.

### Follow-up implementation status

- Added collider-free `1stFloor/BGMTempleShrine` at `(0, -3, 0)`, using existing Built-in-compatible stone and gold materials. It has 13 visual parts, a 2.4 m x 2.2 m footprint, and a 2.53 m height; its top remains below the upper-floor slab.
- The clearance copy `TempleEntranceOrnaments_Detour24Clearance` has zero triangle bounds intersecting the conservative spin volume. `TempleOrnaments` has no Collider, and no mesh triangles remain in that visual clearance volume.
- A `FloorJointLines` mesh has a small bounds overlap near the shrine footprint; it is decorative and the shrine has no Collider. Confirm appearance in the saved scene.
- No 4 x 4 m stair was added. Relocating or removing upper-floor maze walls, gates, or triggers requires a layout decision first.
- The user chose to keep the maze intact and search for another stair location.
- A bounds-based survey found 17 upper-floor 4 x 4 m footprints with no above-floor collider obstruction, but every matching first-floor vertical footprint intersects existing maze colliders. The old `(17, 9)` stair location also intersects upper maze walls and the lower spin-wall route.
- Two perimeter candidates need further approval before construction: near `(-5, 4)`, the stair footprint is partly outside both floor edges and crosses one upper-maze wall; near `(13, 24)`, it extends beyond the north floor edge and crosses the lower north boundary wall. Neither is a clear, no-layout-change placement.
- The scene was saved. The `Stop_Bottom` and `Detour_50_LowerAnnexNorth_Start_2` objects remain at their original hierarchy paths and positions.
- The Unity Console had one error from an Editor evaluation that scanned large unrelated meshes and exceeded the 5 s main-thread timeout; narrow follow-up checks succeeded and script compilation remained successful. The other eight warnings were pre-existing: four empty-assembly-definition warnings and four mobile pixel-light performance warnings. No license warning was present.
- After documenting those diagnostics, the stale Console buffer was cleared and rechecked: 0 errors, 0 warnings, and `compilationFailed=false`. No Cluster runtime traversal test was performed.

### North-edge stair implementation (2026-11-22)

- [x] Use the user-approved north-edge location near `(13, 24)`, opening the north boundary wall and extending the first-floor landing by approximately 1 m.
- [x] Add a 4 x 4 m, 20-step spiral stair under `2ndFloor`, keeping the old undersized stair inactive.
- [x] Create a 4.04 m wide upper-floor opening and split the lower boundary-wall colliders/visual mesh while preserving the original combined mesh asset.
- [x] Confirm no other collider AABBs overlap the stair bounds or upper opening; all 20 tread raycasts hit the stair collider.
- [x] Confirm the upper-maze visual mesh has no triangles overlapping the stair volume, and the clipped lower-wall visual mesh has no triangle centroids in the opening.
- [x] Save `SpinRoutePrototype.unity`; remove the temporary `NorthStairBuilder.cs`.

The stair bounds are `(13.00, -1.49, 24.00)` with size `(4.00, 2.98, 4.00)`. The landing extension is centered at `(13, -3.1, 25.5)` with size `(4, 0.2, 1)`; the lower wall opening is 5 m wide. The saved Scene View showed the stair and opening, though the surrounding structure makes the view visually busy. Unity compilation succeeded with zero current Console errors/warnings. Play Mode and Cluster runtime traversal were not tested.
