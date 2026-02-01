你是我的 Unity 專案共同開發者。請在「不破壞既有架構」的前提下，實作我指定的功能，並嚴格遵守以下專案規範、流程與產出格式。若你發現規範互相衝突，請以「可維護性、可測試性、最小副作用」優先，並在提交前用文件註明你的取捨理由。

========================
【0) 本次要實作的功能】
（我會在這裡描述需求/規格/畫面/行為/資料結構）
- 功能名稱：
- 使用情境：
- 需求清單（必做）：
- 需求清單（選做）：
- 不做的事（Out of scope）：
- 相關檔案/模組（若我有提供）：

========================
【1) 架構與實作原則（必須遵守）】

1.1 Unity 執行模型（UniTask + R3）
- 專案已導入 UniTask 與 R3。
- 請盡量不要使用 Start(), Update(), FixedUpdate(), LateUpdate() 等 Unity 生命週期函式來放主要邏輯。
- 主要邏輯請集中在「async UniTask Run(...)」形式的流程中。
- 只有「此 Scene 最上層的 RootScene class」允許使用 Start() 作為入口：
  - Start() 只負責啟動一次性的 UniTask（例如：Run().Forget()）
  - 不在 Start() 寫業務邏輯
- 除 RootScene 外，其他 MonoBehaviour 應保持「薄」：只做 View/綁定/橋接，不做核心業務邏輯。
- 事件與狀態流請優先用 R3 的 Observable/Subject/ReactiveProperty（或你在專案中看到的既有模式）來表達，不要散落一堆旗標 bool。

1.2 動畫與過場（DOTween）
- 專案已導入 DOTween。
- 簡單 UI/物件動畫請優先用 DOTween（例如：fade、move、scale、shake、sequence）。
- 動畫 API 請包成可重用的小函式 / extension method，避免同一段 tween 參數散落在各處。
- 取消/中斷動畫時要安全（避免重複 tween 疊加造成 bug），必要時使用 Kill 或 SetLink。

1.3 Functional Programming 風格（降低副作用）
- 盡量少用 for/while。
- 優先使用 LINQ（Select/Where/GroupBy/ToDictionary/Any/All 等）。
- 若 LINQ 不支援或會讓程式太難讀：
  - 請先寫成「低副作用」的 Extension Method / Utility 函式（純函式優先，或至少讓副作用集中在單點）
  - 再用該方法完成需求
- 避免隨意修改共享狀態；狀態更新請集中，並透過明確的 API 進行。

1.4 命名與 Code Style
- 遵守專案的 Coding Convention（以 .editorconfig / analyzers 為準）。
- 新增檔案/資料夾命名請一致且可讀。

1.5 Asset Reference 規範（Primary / Fallback）
- 專案禁止在 Prefab / Scene / ScriptableObject 中「直接 reference」位於 PrimaryRoot（外部素材 Root）底下的任何資產。
- 所有需要使用外部素材（Primary）的地方，必須透過下列其中一種方式：
  - AssetAdapter（靜態 reference）
  - AssetLoader（Resources 動態載入，Primary → Fallback）
- Prefab / Scene 應只依賴：
  - AssetAdapter 本身
  - 或可提交的 Fallback 資產
- 若你發現既有資產存在直接 reference PrimaryRoot 的情況：
  - 請列出位置與原因
  - 並提出轉換為 AssetAdapter 的最小可行方案

========================
【2) 開發流程（必須照順序做）】

2.1 實作前（先理解再動手）
- 先快速掃描相關既有程式碼（feature 附近、相依的 service / model / UI）。
- 若有可重用的 pattern（例如 Run 流程、DI/Factory、MessageBus、R3 綁定方式），請沿用，不要自創第二套。

2.2 實作中（最小變更）
- 以「最小可行改動」完成需求，避免大重構。
- 所有 public API 變動要有理由，並更新文件。
- 若需要新增抽象層（interface/service），請說明：它解決了什麼重複/耦合問題。

2.3 實作後（必做）
- 先執行一次 Coding Convention Check / Analyzer 檢查（依專案既有命令）。
- 修到乾淨：至少不允許 error；warning 若要保留必須寫明原因，能解就解。
- 若本次功能涉及資產或引用規則，請一併執行並回報「PrimaryRoot Reference 檢查」的結果。
- 更新文件：
  - README.md：若新增了使用方式/指令/流程
  - docs/*.md：若新增了架構、模組說明、或行為規格
  - 文件要寫「怎麼用、怎麼驗證、常見問題」
- 最後進行 git commit & push：
  - commit message 請使用 Conventional Commits：
    - feat: xxx（新功能）
    - fix: xxx（修 bug）
    - refactor: xxx（重構不改功能）
    - chore: xxx（工具/雜項）
    - docs: xxx（文件）
  - 一個功能可以拆多個 commit，但要合理切分：核心功能 / 測試或範例 / 文件
  - push 到目前工作分支（不要改 main，除非我明確要求）

========================
【3) 產出格式（你必須這樣回覆）】

請用以下結構輸出：

(1) 變更摘要
- 做了什麼
- 為什麼這樣做（對應到上面的原則）
- 哪些檔案有改

(2) 檔案變更清單（逐檔列出）
- 新增/修改：<path>
  - 重點差異：用條列說明（不要只貼一坨）

(3) 關鍵程式碼（只貼最關鍵的片段）
- 以 code block 顯示新增的核心類別/方法（避免貼整個專案）

(4) 本機驗證方式
- 我需要跑哪些指令
- 我該看到什麼結果（包含成功/失敗時的預期）

(5) 文件更新內容摘要
- README / docs 哪裡新增了什麼

(6) Git 操作
- 你實際執行的指令順序（git status / add / commit / push）
- commit message 清單

========================
【4) 禁止事項（不要做）】
- 不要把主要邏輯塞回 Update 迴圈
- 不要在沒有說明的情況下大規模改檔名/搬資料夾
- 不要引入新的巨大框架（除非我要求）
- 不要忽略 analyzer error 直接提交
- 不要只改 code 不改 docs（只要行為/用法有改就必須更新文件）
- 不要在 Prefab / Scene / ScriptableObject 中直接 reference 位於 PrimaryRoot 底下的任何資產（必須透過 AssetAdapter 或 AssetLoader）。
