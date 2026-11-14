# secret-base-world

Cluster向けワールド制作のための開発拠点です。UnityとBlenderのMCP構成、制作Skill、汎用的な制作・検証手順をまとめています。

## 方針

- Unityプロジェクトは `UnityProject/` に配置します。初回は[開発環境ガイド](./docs/development-setup.md)のセットアップスクリプトを実行してください。
- `UnityProject/` にはCluster Creator Kitの公式テンプレートとサンプルシーンが含まれます。テンプレートの`LICENSE.md`と`README.md`も保持してください。
- `.mcp.json` はUnityプロジェクトが `UnityProject/` にある構成を想定しています。
- リポジトリ全体のライセンスは未設定です。`UnityProject/` 内のテンプレート部分は配布元の [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) が適用されます。

## 開発環境

- Unity Editor / Cluster Creator Kit: 利用するテンプレートとCCKが要求するバージョン
- Unity CLI: Unity Editor連携MCP用
- Node.js: 20以上
- Blender: FBX Export APIを利用できるバージョン

セットアップの詳細は[開発環境ガイド](./docs/development-setup.md)を参照してください。

## 含まれるもの

- `.agents/skills/`: Clusterワールド、Blender、ClusterScript向けのAgent Skills
- `.github/instructions/`: Unity描画設定とConsole検証の共通指示
- `UnityProject/`: Cluster Creator Kitテンプレートプロジェクトとサンプルシーン
- `scripts/blender-mcp.mjs`: 基本形状から `.blend` と `.fbx` を生成するBlender MCPサーバー
- `scripts/setup-cluster-template.ps1`: 初回用テンプレート取得・配置スクリプト
- `.mcp.json`: UnityとBlenderのMCP接続設定

Blender MCPは新しいシーンを基本形状から生成するもので、既存のBlenderシーンを編集するツールではありません。
