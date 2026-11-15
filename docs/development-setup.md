# 開発環境のセットアップ

## Unityプロジェクト

このリポジトリはCluster Creator Kitテンプレートを `UnityProject/` にGit管理しています。クローン後、Unityを開く前にリポジトリルートからサブモジュールを初期化してください。

```powershell
git submodule update --init --recursive
npm ci
```

新規クローン時は `git clone --recurse-submodules <repository-url>` でも取得できます。サブモジュールは親リポジトリで指定されたコミットに固定されます。Unity ExporterのPackage Manager参照は `tools/cck-scenegraph-tools/` を使用するため、未初期化の状態でUnityを開かないでください。

公式[テンプレートアーカイブ](https://github.com/ClusterVR/ClusterCreatorKitTemplate/archive/master.zip)からの初期配置には `.\scripts\setup-cluster-template.ps1` も利用できます。通常のクローンではプロジェクトは取得済みなので不要です。スクリプトは既存プロジェクトを上書きしません。テンプレートの `master` は更新されるため、このスクリプトで再取得した内容はGit管理中のものと同一とは限りません。Asset、`.meta`、`Packages/`、`ProjectSettings/` はGit管理し、Unityの生成物はルートの `.gitignore` で除外します。

テンプレートには複数のサンプルが含まれます。最小構成から始める場合は `Assets/ClusterCreatorKitTemplate/Minimal/Scenes/MinimalSample.unity` を開いてください。シーンの依存Assetを保つため、`UnityProject/Assets/ClusterCreatorKitTemplate/`以下を部分的に移動・削除しないでください。

テンプレート配布元の`README.md`と`LICENSE.md`も `UnityProject/` に保持します。テンプレートは[CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)で提供されています。公式の導入記事は[テンプレートワールドの導入方法](https://creator.cluster.mu/2021/10/15/template-sample/)です。Unity EditorとCCKのバージョン要件は、取得したテンプレートの`README.md`を確認してください。

`.mcp.json` のUnity MCP設定は `UnityProject/` をUnityプロジェクトのルートとして起動します。別の配置にする場合は、設定の `--project-path` を変更してください。

## Blender MCP

`.mcp.json` は `scripts/blender-mcp.mjs` を起動します。Blender実行ファイルを自動検出できない場合は `BLENDER_EXECUTABLE` を設定してください。MCP出力先は既定で `UnityProject/Assets/Generated/Blender` です。変更する場合は、`.mcp.json` の `BLENDER_MCP_OUTPUT_DIR` を編集します。

`create_fbx` は立方体、球、正二十面体球、円柱、円錐、トーラス、平面を作成し、`.blend` と `.fbx` を出力します。同名の出力がある場合は上書きしません。入力制約とFBX設定は[`blender-modeling` Skill](../.agents/skills/blender-modeling/SKILL.md)と、その[`Blender MCP設定資料`](../.agents/skills/blender-modeling/references/mcp-configuration.md)を参照してください。

## 再利用ツールとSceneGraph解析

- [`tools/cck-scenegraph-tools`](../tools/cck-scenegraph-tools/) はUnity Exporter、TypeScript解析器、サンプル、Skillを含むサブモジュールです。Unityの `com.azuki.unity-semantic-graph` はこのサブモジュール内のパッケージを参照します。手元の [`cluster-script-authoring` Skill](../.agents/skills/cluster-script-authoring/SKILL.md) はこのプロジェクト向けに出力先や型定義の扱いを調整しています。
- [`tools/blender-modeling-tools`](../tools/blender-modeling-tools/) はBlender Skillと技術資料の参照元です。手元の [`blender-modeling` Skill](../.agents/skills/blender-modeling/SKILL.md) はこのプロジェクトのMCP設定を優先します。参照元に記載されたroom固有の出力先をそのまま使わないでください。
- サブモジュールの更新は各ツールの互換性を確認した上で明示的に行い、親リポジトリのポインター変更も記録してください。外部リポジトリには独自の利用条件があります。

Dependabotは毎週、ルートのnpm依存関係とサブモジュール、GitHub Actionsを確認し、更新があればプルリクエストを作成します。サブモジュールのプルリクエストではUnity Exporterと解析器・Skillの互換性を確認してください。各サブモジュール内のnpm依存関係はそれぞれのリポジトリで管理します。この設定はNode.js本体や `package.json` のNode.jsバージョン要件を自動更新するものではありません。

SceneGraph解析器はNode.js 26.xを使用し、初回に次を実行します。

```powershell
Push-Location tools\cck-scenegraph-tools\packages\ts-analyzer
npm ci
npm run build
Pop-Location
```

Unity Editorを閉じた状態で、リポジトリルートから対象シーンとClusterScriptの実際のパスを指定して実行します。

```powershell
.\tools\cck-scenegraph-tools\scripts\export-and-analyze.ps1 `
  --project-path (Resolve-Path .\UnityProject).Path `
  --scene-path "Assets/ClusterCreatorKitTemplate/Minimal/Scenes/MinimalSample.unity" `
  --script-path "C:\path\to\your-script.ts" `
  --log-path "Library/scenegraph-export.log" `
  --fail-on-issues --fail-on-unverified
```

Windowsの標準Unity Hub配置以外では `--unity-path` または `UNITY_PATH` を指定します。出力 `UnityProject/SceneGraph.generated.toml` は生成物でGit管理しません。解析結果は静的な診断であり、Play ModeやClusterでの動作を保証しません。パスや判定オプションの詳細はサブモジュールの [`docs/quickstart.md`](../tools/cck-scenegraph-tools/docs/quickstart.md) を参照してください。

## ClusterScriptのビルド

TypeScriptソースを `UnityProject/Assets/Project/Script/src/` に配置してから、リポジトリルートで次を実行します。

```powershell
npm run build:clusterscripts
```

生成JavaScriptはGit管理外なので、クローン後やソース変更後はUnity Editorを開く前にビルドしてください。

`package.json` の `build:clusterscripts` が次の処理を順に実行します。

1. `fetch:cluster-script-api`: 公式の型定義を `node_modules/@types/cluster-script/` に取得します。
2. `compile:clusterscripts`: ルートの `tsconfig.json`（`scripts/cluster-script-tsconfig.json` を継承）を使い、TypeScriptを `UnityProject/Assets/Project/Script/dist/` にコンパイルします。この設定はエディターでも自動検出されます。
3. `minify:clusterscripts`: 出力されたJavaScriptを縮小します。

各工程は個別にも実行できます。型定義はビルドごとに公式サイトから取得されるため、取得時点でのAPIに依存します。生成された `.js` はGit管理せず、`.gitignore` で除外します。一方、Unityの `ScriptableItem` が参照するGUIDを維持するため、対応する `.js.meta` はGit管理します。TypeScriptソースと `.ts.meta` もGit管理します。ビルド後にUnityでアセットをインポートし、`.meta` が未作成の場合は生成・確認してコミットしてください。ソースがない状態ではビルドは失敗し、削除済みソースの古い出力は自動削除されません。

エディターの型エラーを解消するためだけに型定義が必要な場合は、リポジトリルートで `npm run fetch:cluster-script-api` を実行してください。

Spinのスクリプトを変更した場合は、ビルド後に `node --test scripts\spin.test.cjs` を実行します。このテストはメッセージと移動計算の模擬検証であり、Cluster上の衝突やアバターの動作は別途試遊してください。

これらの型定義取得・縮小処理および `LOCKED_` 保護ルールは [sato-toma/room](https://github.com/sato-toma/room) の機能をこのプロジェクトの配置に合わせて改変したものです。原著作者は Cluster, Inc. と azuki、元プロジェクトの条件は [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) です。room固有のシーンやBGMは移植していません。

## Agent customization

VS Codeでリポジトリを開くと、`.agents/skills/` 配下のSkillsと `.github/instructions/` 配下のInstructionsを利用できます。MCPサーバーを使う場合はVS Codeから接続を有効にし、Unityプロジェクトが開ける状態であることを確認してください。

## 制作物の管理

Unityが管理する `.meta` とAsset GUIDを保持してください。中間生成物、一時ファイル、完成したUnityアセットの保存先は[`Temporary Assets` instruction](../.github/instructions/temporary-assets.instructions.md)に従います。
