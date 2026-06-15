<!-- Language switch -->
**简体中文** | [English](README.en.md)

<div align="center">

<img src="docs/assets/hero.png" alt="冰与火之舞修改器 ADOFAI Trainer — 按 F3 呼出的游戏内图形修改器，集成 Autoplay、隐藏 HUD、变速、放宽判定、无敌与解锁全部关卡" width="100%">

<sub> <a href="docs/assets/promo.mp4">观看 30 秒宣传片</a></sub>

# 冰与火之舞修改器 · ADOFAI Trainer

**按下 F3，得到一次满分通关——这就是游戏自带的 Autoplay，只是被亮了出来。**

一个《冰与火之舞》(A Dance of Fire and Ice) 的 **游戏内图形修改器**，基于 BepInEx 5。集成帧级满分 Autoplay、无 HUD 干净录制、变速、无敌、放宽判定与解锁全部关卡——**只翻动游戏自己已有的开关，不做内存扫描**。

[![release](https://img.shields.io/github/v/release/Cohenjikan/ADOFAITrainer?label=release&sort=semver&color=ff6b6b)](https://github.com/Cohenjikan/ADOFAITrainer/releases)
[![license](https://img.shields.io/github/license/Cohenjikan/ADOFAITrainer?color=4ecdc4)](LICENSE)
[![BepInEx](https://img.shields.io/badge/BepInEx-5.4.23.x-7048e8)](https://github.com/BepInEx/BepInEx)
[![game](https://img.shields.io/badge/ADOFAI-Steam%20977950-1b2838?logo=steam)](https://store.steampowered.com/app/977950/)
[![price](https://img.shields.io/badge/%E5%85%8D%E8%B4%B9%E5%BC%80%E6%BA%90-FREE-2ea44f)](https://github.com/Cohenjikan/ADOFAITrainer)
[![stars](https://img.shields.io/github/stars/Cohenjikan/ADOFAITrainer?style=social)](https://github.com/Cohenjikan/ADOFAITrainer/stargazers)

[安装](#安装) · [功能](#功能) · [使用](#使用) · [取舍与注意](#取舍与注意必读) · [从源码构建](#从源码构建) · [English](README.en.md)

</div>

> [!NOTE]
> **仅供单机自娱与录制使用**（例如制作完美通关视频）。请勿用于排行榜、对战等任何在线场景。本项目与 7th Beat Games **无任何关联**。

> [!IMPORTANT]
> **本工具完全免费、开源，严禁倒卖。** 标题栏、菜单顶部与加载日志均带有本项目地址水印，并内置完整性校验——**删除或篡改该水印 / 项目地址会使修改器直接禁用**（连唯一的 Harmony 补丁也不会加载）。如果你是付费拿到的，说明被人倒卖了，请到 [本仓库](https://github.com/Cohenjikan/ADOFAITrainer) 免费获取。

---

## 安装

### macOS / Linux（原生，无需 BepInEx）

打开「终端」，粘贴运行一行即可（自动装 .NET SDK → 编译 → 织入游戏启动钩子）：

```bash
curl -fsSL https://raw.githubusercontent.com/Cohenjikan/ADOFAITrainer/refs/heads/main/install.sh | bash
```

然后正常用 Steam 启动游戏，进任意画面按 **F3** 开 / 关菜单。卸载同样一行：

```bash
curl -fsSL https://raw.githubusercontent.com/Cohenjikan/ADOFAITrainer/refs/heads/main/uninstall.sh | bash
```
> 游戏更新或 Steam「验证文件完整性」后，重跑一次安装命令即可。

### Windows

打开 **PowerShell**，粘贴运行一行即可（自动定位游戏 → 没装 BepInEx 就装上 → 下载最新 DLL；老用户跑这行也会平滑升级）：

```powershell
irm https://raw.githubusercontent.com/Cohenjikan/ADOFAITrainer/refs/heads/main/win_install.ps1 | iex
```

进入游戏按 **F3** 呼出菜单。卸载同理：

```powershell
irm https://raw.githubusercontent.com/Cohenjikan/ADOFAITrainer/refs/heads/main/win_uninstall.ps1 | iex
```
> 也可到右侧 **[Releases](https://github.com/Cohenjikan/ADOFAITrainer/releases)** 手动下载；详细步骤见下方[「Windows 安装详解」](#windows-安装详解bepinex)。

---

## 为什么用它

录制《冰与火之舞》完美通关的最大障碍，是**手速对点**——游戏核心是「踩着节拍点击」，全程零失误几乎不可能靠手打做到。

与其练到手抽筋或写键盘宏，本修改器直接借用引擎**自带的 autoplay**：它按谱面浮点拍点触发，没有输入延迟，天然帧级满分，画面与真人手打完全一致，而且 **主游戏里没有任何 autoplay 水印**。

它和市面上的「内存修改器」做法根本不同：

| | 普通内存修改器 | 本修改器 |
|---|---|---|
| 实现方式 | 扫描 / 改写内存偏移 | 只翻动游戏**自己已有**的标志（`RDC` / `GCS` / `scrController` / `Persistence`）|
| 游戏更新后 | 偏移一变就失效 | 调用的是游戏自身逻辑，**通常不受影响** |
| 自定义补丁数量 | 多处 hook | **全项目仅一个 Harmony 补丁**（放宽判定），其余全是普通字段赋值 |
| 界面 | 多为盲改配置文件 | **游戏内 IMGUI 浮层**，带中文字体、三个分页 |

> 与作者的姊妹项目「节奏医生修改器」同源同法——两款游戏同为 7th Beat Games 出品，引擎与代码高度同源。（作者其它项目见 [GitHub 主页](https://github.com/Cohenjikan)。）

---

## 功能

按 **F3** 呼出浮层菜单，三个分页：**普通** / **开发者** / **关于**。

### 普通玩家

#### Autoplay — 全程满分自动演奏（主游戏无水印）
引擎按谱面帧级满分自动演奏，画面与真人手打无异；**主游戏里没有 autoplay 水印**，可在关卡内随时开关。
> 原理：仅在关卡内把 `RDC.auto` 设为开关状态——这正是引擎自带 autoplay 的原生标志（编辑器里的 Otto 吉祥物才会显示水印，所以只在关卡内启用）。

<img src="docs/assets/feature-1.png" alt="普通页：Autoplay 与隐藏 HUD 开关，下方为说明文字" width="70%">

#### 隐藏 HUD（干净录制）
去掉屏幕上的 HUD，OBS 录出来干干净净；配合 Autoplay 就复刻了游戏自带的「开发者录制模式」。
> 原理：设置 `RDC.noHud`；游戏的 dev 录制模式正是 `RDC.auto = true; RDC.noHud = true`。

#### 游戏变速 0.5×–3×（含音高）
慢放练习或加速。滑块连续可调，并有 `0.75×` / `1×` / `1.5×` / `2×` 快捷按钮。
> 原理：预置 `GCS.speedTrialMode` + `GCS.nextSpeedRun`（在关卡 Start 时读取），点「应用变速并重开本关」调用 `scrController.instance.Restart(true)` 生效。

<img src="docs/assets/feature-2.png" alt="变速分区：变速开关、滑块、0.75x/1x/1.5x/2x 快捷按钮，以及应用变速并重开本关按钮" width="70%">

#### 无敌 No-Fail
即游戏自带的「无判定」练习模式，永不失败、不被打断，关卡内开启立即生效。
> 原理：设置 `GCS.useNoFail`，并把 `scrController.instance.noFail` 一并推到当前关卡。

#### 放宽判定（含完美窗口）
本项目**唯一的 Harmony 补丁**。手打也能轻松全 Perfect——倍数 `1.0–5.0` 可调，连 Perfect/Pure 窗口一起放宽。
> 原理：`JudgeWindowPatch` 后缀 `scrMisc.GetAdjustedAngleBoundaryInDeg`，把返回值乘以 `max(1.0, judgeMult)`，同时加宽 Counted / Perfect / Pure 三个命中窗口。开 Autoplay 时本就满分，此项专为手动练习。

<img src="docs/assets/feature-3.png" alt="便利/玩法分区：无敌 No-Fail 开关与放宽判定倍数滑块" width="70%">

### 开发者

| 功能 | 说明 |
|---|---|
| **解锁全部关卡**（临时，不写存档）+ **前往选关** | 二者配合 = 关卡直达；关闭即恢复，不污染存档（`RDC.forceUnlockAllLevels`）|
| **永久解锁全部关卡** | 写入游戏自身的 `Persistence.unlockAllLevels` 并保存，**可随时取消，不删任何已有进度** |
| **跳过过场动画** / **显示 FPS** / **固定星球颜色** | 录制更稳（`RDC.skipCutscenes` / `GCS.showFPS` / `GCS.staticPlanetColors`）|
| **打开存档目录** | 在 `Persistence.DataPath` 处打开资源管理器 |

---

## Windows 安装详解（BepInEx）

> 适用于 **Steam 正式版**（Unity 6 / x64 / Mono）。需要先装 BepInEx 5。

**① 装 BepInEx 5（x64, Mono）**

1. 到 [BepInEx Releases](https://github.com/BepInEx/BepInEx/releases) 下载 **`BepInEx_win_x64_5.4.23.x.zip`**。
2. 把压缩包内容**解压到游戏根目录**（与 `A Dance of Fire and Ice.exe` 同一层；解压后该目录会多出 `winhttp.dll`、`BepInEx/` 等）。
   > 游戏目录怎么找：Steam → 右键《A Dance of Fire and Ice》→ 管理 → 浏览本地文件。
3. **启动一次游戏再退出**，让 BepInEx 生成 `BepInEx/plugins`、`BepInEx/config` 等文件夹。

**② 放入本修改器**

- **方法 A（手动，推荐）**：下载 [`dist/ADOFAITrainer.dll`](dist/ADOFAITrainer.dll)，放进 `游戏目录\BepInEx\plugins\`。
- **方法 B（脚本）**：下载本仓库 → 编辑 [`tools/install.bat`](tools/install.bat) 里的 `GAME=` 路径 → 双击运行，它会把 `dist\ADOFAITrainer.dll` 拷到 `BepInEx\plugins`。

**③ 验证**

启动游戏后打开 `游戏目录\BepInEx\LogOutput.log`，看到包含这串文字的一行即成功（前缀与结尾会更长）：

```text
ADOFAI Trainer (冰与火之舞修改器) v1.41 · 免费开源 FREE · github.com/Cohenjikan/ADOFAITrainer · loaded. Menu key = F3.
```

进入任意关卡，按 **F3** 呼出菜单。

---

## 使用

1. 进入任意关卡，按 **F3** 开 / 关菜单。
2. **录制完美通关**：在「普通」页打开 **Autoplay**（可再开「隐藏 HUD」）→ 进关卡 → 用 OBS 等录屏。
3. **录被锁住的关卡**：在「开发者」页开「解锁全部关卡」→「前往选关」直接进入。
4. **变速**：拖动滑块（或点快捷按钮）后，点 **「应用变速并重开本关」** 才生效（引擎不支持中途变速）；点「恢复原速并重开」可还原。
5. **想自己手打**：把 Autoplay 关掉即可；想手打也满分，就开「放宽判定」。

---

## 取舍与注意（必读）

- **变速不保留音高**：引擎用 `song.pitch` 直接缩放，没有时间拉伸，所以快 / 慢的同时音调也会升 / 降。这是已知取舍。
- **变速只在关卡开始 / 重开时生效**：引擎无法中途变速，改完务必点「应用变速并重开本关」。
- **「速度试炼」会自动 +0.1**：游戏机制会在赢一关后把速度自动加 0.1，**录定速前请重新设置**。
- **需先手动安装 BepInEx 5**（x64, Mono）：这不是一键安装器。
- **版本绑定当前引擎**：目标为 Unity 6（6000.3.x）、BepInEx 5.4.23.x、`netstandard2.1`；游戏大版本更新后可能需要适配。
- **完整性 / 防倒卖校验不是 DRM**：它只是同一 DLL 内的字符串常量比对，能劝退顺手倒卖，但**重新编译即可绕过**，并非真正的防破解。
- **严格单机 / 离线**：请勿用于在线、排行榜或对战；修改游戏可能违反其 EULA，**使用风险自负**。

---

## 从源码构建

需要 .NET SDK（构建 `netstandard2.1`）、以及一份**已安装 BepInEx 的游戏副本**（用于引用 DLL，包含 `RDTools.dll`）。本仓库**不含任何游戏素材**。

```bash
# 默认读取 D:\steam\steamapps\common\A Dance of Fire and Ice
# 其它路径用 -p:GameDir=... 覆盖
dotnet build src/ADOFAITrainer.csproj -c Release -p:GameDir="你的\A Dance of Fire and Ice"
```

产物在 `src/bin/Release/ADOFAITrainer.dll`。

---

## 卸载

- **只移除修改器**：删除 `游戏目录\BepInEx\plugins\ADOFAITrainer.dll`（或运行 [`tools/uninstall.bat`](tools/uninstall.bat)）。
- **连 BepInEx 一起移除 / 恢复原版**：删除游戏根目录的 `winhttp.dll`（最快的「禁用 BepInEx」方式），或一并删除 `winhttp.dll` + `BepInEx/` + `doorstop_config.ini`。
- 也可在 Steam 里「验证游戏文件完整性」一键还原。

> 配置文件在 `游戏目录\BepInEx\config\com.cohen.adofaitrainer.cfg`（可改菜单热键），卸载后可一并删除。

---

## 兼容性

| 项 | 值 |
|---|---|
| 游戏 | A Dance of Fire and Ice（Steam 正式版，appid 977950） |
| 引擎 | Unity 6（6000.3.x）/ x64 / Mono |
| 加载器 | BepInEx 5.4.23.x |
| 目标框架 | netstandard2.1 |

> 游戏大版本更新后可能需要适配；若加载失败，先确认 BepInEx 版本与本说明一致。

---

## 免责声明

- **非官方**：本项目是粉丝制作的非官方第三方工具，与游戏开发商 [7th Beat Games](https://7thbeat.com/) **无任何关联**，亦未获其授权或认可。《A Dance of Fire and Ice》及其名称、商标、美术与音乐等素材的一切权利归 7th Beat Games 所有。
- **不含游戏内容**：本仓库**仅包含作者自行编写的插件代码**，不含也不分发游戏的任何源代码、DLL、音频、图像或其它素材；运行时只通过 BepInEx / HarmonyX 调用游戏**自身已存在**的公开函数，不做内存扫描。
- **仅限单机**：本工具仅供**离线单机**的自娱、练习与录制。请**勿**用于在线、排行榜、对战或任何会影响其他玩家公平性的场景。
- **遵守 EULA**：修改游戏可能违反其最终用户许可协议（EULA）/ 服务条款。是否使用由你自行决定，并须自行遵守相关条款；由此产生的一切后果（如账号处罚、存档损坏等）均由使用者自行承担。
- **风险自负**：本工具按「现状」提供，不附带任何明示或暗示的担保；作者不对使用本工具造成的任何直接或间接损失负责。
- **完全免费**：本工具免费、开源（[MIT](LICENSE)），**严禁倒卖**；若你是付费获得的，说明被人倒卖了，请到本仓库免费获取。
- **版权方异议**：如相关版权方认为本项目有不当之处，可通过 GitHub Issue 联系，作者将配合下架或调整。

---

## 致谢

- 模组框架 [BepInEx](https://github.com/BepInEx/BepInEx) / [HarmonyX](https://github.com/BepInEx/HarmonyX)。
- 与姊妹项目「节奏医生修改器」同源同法（同为 7th Beat Games 出品）。

本项目代码以 [MIT](LICENSE) 许可证开源。Made by Cohenjikan.