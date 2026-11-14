# 開発環境のセットアップ

## Unityプロジェクト

このリポジトリにはCluster Creator Kitテンプレートを `UnityProject/` に配置します。初回セットアップでは、リポジトリルートからPowerShellで次を実行してください。

```powershell
.\scripts\setup-cluster-template.ps1
```

このスクリプトは公式の[テンプレートアーカイブ](https://github.com/ClusterVR/ClusterCreatorKitTemplate/archive/master.zip)をダウンロード・展開し、Unityプロジェクト一式を `UnityProject/` にコピーします。シーンだけでなく、参照されるAsset、`.meta`、`Packages/`、`ProjectSettings/`も含めます。Unityの生成物はルートの`.gitignore`で除外し、プロジェクトのソースAssetと設定はGit管理します。

テンプレートには複数のサンプルが含まれます。最小構成から始める場合は `Assets/ClusterCreatorKitTemplate/Minimal/Scenes/MinimalSample.unity` を開いてください。シーンの依存Assetを保つため、`UnityProject/Assets/ClusterCreatorKitTemplate/`以下を部分的に移動・削除しないでください。

テンプレート配布元の`README.md`と`LICENSE.md`も `UnityProject/` に保持します。テンプレートは[CC BY 4.0](https://creativecommons.org/licenses/by/4.0/)で提供されています。公式の導入記事は[テンプレートワールドの導入方法](https://creator.cluster.mu/2021/10/15/template-sample/)です。Unity EditorとCCKのバージョン要件は、取得したテンプレートの`README.md`を確認してください。

`.mcp.json` のUnity MCP設定は `UnityProject/` をUnityプロジェクトのルートとして起動します。別の配置にする場合は、設定の `--project-path` を変更してください。

## Blender MCP

Node.js依存関係を復元します。

```powershell
npm ci
```

`.mcp.json` は `scripts/blender-mcp.mjs` を起動します。Blender実行ファイルを自動検出できない場合は `BLENDER_EXECUTABLE` を設定してください。MCP出力先は既定で `UnityProject/Assets/Generated/Blender` です。変更する場合は、`.mcp.json` の `BLENDER_MCP_OUTPUT_DIR` を編集します。

`create_fbx` は立方体、球、正二十面体球、円柱、円錐、トーラス、平面を作成し、`.blend` と `.fbx` を出力します。同名の出力がある場合は上書きしません。入力制約とFBX設定は[`blender-modeling` Skill](../.agents/skills/blender-modeling/SKILL.md)と、その[`Blender MCP設定資料`](../.agents/skills/blender-modeling/references/mcp-configuration.md)を参照してください。

## Agent customization

VS Codeでリポジトリを開くと、`.agents/skills/` 配下のSkillsと `.github/instructions/` 配下のInstructionsを利用できます。MCPサーバーを使う場合はVS Codeから接続を有効にし、Unityプロジェクトが開ける状態であることを確認してください。

## 制作物の管理

Unityが管理する `.meta` とAsset GUIDを保持してください。中間生成物、一時ファイル、完成したUnityアセットの保存先は[`Temporary Assets` instruction](../.github/instructions/temporary-assets.instructions.md)に従います。
