---
description: "Use when creating Blender models, FBX exports, previews, screenshots, or other temporary Unity project assets."
applyTo:
  - "UnityProject/Assets/Generated/Blender/**"
  - "UnityProject/Assets/Temp/**"
---
# Temporary Unity assets

- Keep scratch models, intermediate exports, renders, screenshots, and validation output under `UnityProject/Assets/Temp/`, grouped by feature when useful.
- Promote only approved deliverables to the appropriate tracked asset folder, preserving their Unity `.meta` files.
- Do not force-add files ignored by Git. Check the project `.gitignore` and repository asset policy before adding generated or third-party assets.
