---
description: "Use when creating or editing Cluster Unity worlds, scenes, prefabs, materials, shaders, or render-pipeline settings. Requires Built-in Render Pipeline and Built-in-compatible shaders."
applyTo:
  - "UnityProject/Assets/**/*.unity"
  - "UnityProject/Assets/**/*.prefab"
  - "UnityProject/Assets/**/*.mat"
  - "UnityProject/Assets/**/*.shader"
  - "UnityProject/ProjectSettings/GraphicsSettings.asset"
  - "UnityProject/Packages/manifest.json"
---
# Cluster world rendering

- Verify the render-pipeline support of the target Cluster Creator Kit version and project before authoring materials. For a Built-in Render Pipeline target, do not add URP/HDRP packages or assign their pipeline assets.
- Keep `GraphicsSettings.m_CustomRenderPipeline` unset (`{fileID: 0}`).
- Before using a material, verify its shader is Built-in-compatible. Replace URP/HDRP shaders with an appropriate Built-in shader or a Built-in-compatible custom shader, preserving the intended appearance and material properties.
- Do not replace shaders blindly or suppress missing-shader warnings with fallback settings. Check the affected materials and their rendered appearance after conversion.
- After editing render settings, materials, or shaders, re-import or open the project and check that no active scene/material references URP or HDRP shaders.
- Unity validation must have no unresolved license-related warnings. Check that the Unity license is active and resolve licensing-client or Editor-installation problems at their source; do not hide or suppress the warning.
