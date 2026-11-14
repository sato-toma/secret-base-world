---
description: "Use when editing Unity scenes or prefabs; preserve GameObjects marked as approved."
applyTo:
  - "UnityProject/Assets/**/*.unity"
  - "UnityProject/Assets/**/*.prefab"
---
# Approved Unity content

- Treat a GameObject whose name starts with `LOCKED_` as approved and read-only. Its children and referenced components/assets are included.
- Do not rename, move, reparent, delete, or change the Transform, components, children, or asset references of approved content. Do not add children beneath an approved GameObject.
- Add or revise content beside approved objects, or in a separate preview scene or prefab variant. Do not move existing objects under a new marker parent just to protect them; that can change hierarchy-dependent behavior.
- If a requested change requires editing approved content, stop before making that edit and ask for explicit permission. Once authorized, keep the change limited to the requested object and report what changed.
- A name marker is an agent convention, not a Unity-enforced lock. Unity's Static setting is for engine behavior, not edit protection. Use version-control commits for recoverable milestones and review scene/prefab diffs before accepting changes.
