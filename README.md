# secret-base-world

Cluster向けワールド制作のための開発拠点です。UnityとBlenderのMCP構成、制作Skill、汎用的な制作・検証手順をまとめています。

## 方針

- Unityプロジェクトは `UnityProject/` にあります。Unityを開く前に[開発環境ガイド](./docs/development-setup.md)に従いサブモジュールを初期化してください。
- `UnityProject/` にはCluster Creator Kitの公式テンプレートとサンプルシーンが含まれます。テンプレートの`LICENSE.md`と`README.md`も保持してください。
- `.mcp.json` はUnityプロジェクトが `UnityProject/` にある構成を想定しています。
- リポジトリ全体のライセンスは未設定です。`UnityProject/` 内のテンプレート部分とroom由来の機能には、各配布元の [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) が適用されます。サブモジュールにはそれぞれの利用条件が適用されます。

## 開発環境

- Unity Editor / Cluster Creator Kit: 利用するテンプレートとCCKが要求するバージョン
- Unity CLI: Unity Editor連携MCP用
- Node.js: 20以上（SceneGraph解析器は26.x）
- Blender: FBX Export APIを利用できるバージョン

セットアップの詳細は[開発環境ガイド](./docs/development-setup.md)を参照してください。

## 含まれるもの

- `.agents/skills/`: Clusterワールド、Blender、ClusterScript向けのAgent Skills
- `.github/instructions/`: Unity描画設定とConsole検証の共通指示
- `UnityProject/`: Cluster Creator Kitテンプレートプロジェクトとサンプルシーン
- `scripts/blender-mcp.mjs`: 基本形状から `.blend` と `.fbx` を生成するBlender MCPサーバー
- `scripts/setup-cluster-template.ps1`: 初回用テンプレート取得・配置スクリプト
- `tools/cck-scenegraph-tools/`: SceneGraph Exporter、解析器とClusterScript関連Skillの固定参照
- `tools/blender-modeling-tools/`: Blender Skillと技術資料の固定参照
- `.mcp.json`: UnityとBlenderのMCP接続設定

Blender MCPは新しいシーンを基本形状から生成するもので、既存のBlenderシーンを編集するツールではありません。

## 神殿の背景

`SpinRoutePrototype` はBuilt-inの `Skybox/Panoramic` と静止した全天画像を使用します。浮遊島・水鏡・雲海は背景画像であり、地形やリアルタイム反射ではありません。画像の生成元は `UnityProject/AgentScripts/DreamTempleSky.cs` です。Unity Editorを開いた状態でUnity MCPの `run_script` に `AgentScripts/DreamTempleSky.cs` とエントリーポイント `DreamTempleSky.Generate` を指定すると、追跡対象の `DreamTempleSky.png` を上書き生成します。既存の `.meta` とマテリアルの参照は維持してください。

全天画像は縦の継ぎ目を防ぐため、横方向のRepeatを有効にし、ミップマップを無効にしています。
