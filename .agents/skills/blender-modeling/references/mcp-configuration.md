# Blender MCP configuration

This repository includes `scripts/blender-mcp.mjs`, a Node.js MCP server that creates a new Blender scene from primitive meshes and exports `.blend` and `.fbx` files.

## Capabilities

- Shapes: `cube`, `sphere`, `ico_sphere`, `cylinder`, `cone`, `torus`, `plane`
- Create 1–200 objects per call, with name, location, rotation, scale, and optional basic material.
- Creates a fresh factory scene; does not edit existing scenes or objects.
- Does not provide arbitrary Blender Python execution, custom meshes, UV editing, or rendering.
- Output names are unique: existing `.blend` or `.fbx` files are not overwritten.

## Output and executable

`.mcp.json` sets `BLENDER_MCP_OUTPUT_DIR` to `UnityProject/Assets/Generated/Blender`. Set `BLENDER_EXECUTABLE` in the environment if Blender is not on PATH or in a detected Windows installation directory.

Unity creates `.meta` files for imported assets. Do not generate or edit those files manually.

## FBX settings

The included exporter uses Metric units with a unit scale of 1, applies object rotation and scale while keeping location, exports selected meshes, and uses `-Z` Forward / `Y` Up, FBX unit scaling, and Apply Transform. It does not export animation. The primitive origin remains at the shape center; a bottom-center origin is not supported.

These settings are specific to this exporter. Check the receiving application's import result and actual bounds; successful export alone does not verify scale, orientation, materials, or Cluster compatibility. Blender's FBX API may change across versions, so check the installed Blender version and API.

For general MCP safeguards, see [MCP operations](./mcp-operations.md). For asset handoff and validation, see the [authoring workflow](./authoring-workflow.md) and [validation checklist](./validation-checklist.md).
