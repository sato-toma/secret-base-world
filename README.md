# secret-base-world

Cluster向けワールド制作のための開発拠点です。UnityとBlenderのMCP構成、制作Skill、汎用的な制作・検証手順をまとめています。

## 方針

- このリポジトリに `room` のシーン、GameObject、モデル、マテリアル等は移植していません。
- Unityプロジェクトは含めていません。`UnityProject/` に利用するCluster Creator Kitテンプレート等を各自で用意してください。
- `.mcp.json` はUnityプロジェクトが `UnityProject/` にある構成を想定しています。
- 公開時点で `LICENSE` は設定していません。再利用条件を明示する場合は、適切なライセンスを別途選択してください。

## 開発環境

- Unity Editor / Cluster Creator Kit: 利用するテンプレートとCCKが要求するバージョン
- Unity CLI: Unity Editor連携MCP用
- Node.js: 20以上
- Blender: FBX Export APIを利用できるバージョン

セットアップの詳細は[開発環境ガイド](./docs/development-setup.md)を参照してください。

## 含まれるもの

- `.agents/skills/`: Clusterワールド、Blender、ClusterScript向けのAgent Skills
- `.github/instructions/`: Unity描画設定とConsole検証の共通指示
- `scripts/blender-mcp.mjs`: 基本形状から `.blend` と `.fbx` を生成するBlender MCPサーバー
- `.mcp.json`: UnityとBlenderのMCP接続設定

Blender MCPは新しいシーンを基本形状から生成するもので、既存のBlenderシーンを編集するツールではありません。
