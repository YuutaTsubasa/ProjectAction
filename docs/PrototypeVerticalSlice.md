# Phase 1 Parkour Core Vertical Slice v0

## 目的
- 在 Prototype 方塊關卡中驗證跑酷核心閉環（Start -> Run -> Checkpoint -> Death/Respawn -> Goal）。
- 所有流程由 `RootScene` 的 UniTask `Run()` 進行驅動。

## 安裝到現有 Scene
1. 開啟要測試的 Scene（建議使用 `Assets/Scenes/SampleScene.unity`）。
2. 在 Unity 選單執行 `Tools > Prototype > Install Parkour Vertical Slice v0`。
3. 確認場景出現以下物件：
   - `RootScene`
   - `Player`
   - `Main Camera`
   - `Checkpoint`
   - `Goal`
   - `PrototypeFloor`
   - `VirtualInputBridge`

## 操作方式
- 移動：WASD / 左搖桿
- 跳躍：Space / A (南按鈕)
- 衝刺：Shift（按住）/ RT（按住）
- 視角：滑鼠移動 / 右搖桿

## 玩法閉環
- 玩家從起點跑到終點。
- 進入 `Checkpoint` 會更新重生點。
- 低於 `RootScene` 的 `Kill Y` 視為死亡並從最近檢查點重生。
- 進入 `Goal` 會輸出完成訊息（目前為 `Debug.Log`）。

## 關卡佈置（Installer 產物）
- 起點平台 -> 跳台階梯 -> 轉角平台 -> 直線衝刺 -> 終點平台。
- `Checkpoint` 置於中段平台，`Goal` 置於終點平台。
- `Checkpoint` / `Goal` 為透明黃色圓柱，觸發後會變成綠色。

## 注意事項
- 不直接引用 PrimaryRoot 內的資產；安裝器只會建立原型方塊與必要物件。
- 觸控虛擬搖桿僅保留 `VirtualInputBridge` 介面，尚未提供 UI。
- 若需要替換場景與關卡物件，請在 Scene 中調整並保留相同觸發器邏輯。
