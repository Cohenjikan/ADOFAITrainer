# 冰与火之舞修改器 — macOS / Linux 原生版（无需 BepInEx）

ADOFAI 修改器的 **macOS 原生移植**（Linux 顺带支持，见下）。功能与 Windows 版一致
（Autoplay、变速、放宽判定、无敌、解锁全部关卡、跳过过场、固定星球色等），
但**不使用 BepInEx / Doorstop**——因为 Unity 的 macOS 播放器用 `dlopen`/`dlsym` 动态加载 Mono，
Doorstop 在 macOS 上无法挂钩。与「节奏医生修改器 Mac 版」同源同法（同为 7th Beat Games）。

## 原理

这游戏是 **Mono** 后端，`Assembly-CSharp.dll` 是可改写的 IL。本方案用 **Mono.Cecil 静态织入**：
在游戏自身的启动函数 `ADOStartup.Startup` 开头插入一句 `ADOFAITrainerMac.Loader.Init()`
（它是 ADOFAI 的引导例程：设语言 / 初始化 Steam / 取平台 / 载入存档 / 修分辨率，正好对应
节奏医生的 `RDStartup.Setup`）。游戏一启动就自动把修改器带起来——零注入器、原生运行。

- 修改器逻辑：`ADOFAITrainerMac.dll`（由 `../src/` 的 Windows 源码移植，去掉 BepInEx 外壳，改为普通 MonoBehaviour）
- 运行时补丁库：`0Harmony.dll`（pardeike Lib.Harmony 2.4.2，自包含；2.4 起原生支持 Apple Silicon arm64）
- 织入器：`Patcher/`（Mono.Cecil 控制台程序，幂等：总是从纯净备份 `*.adofaitrainer-backup` 重新打补丁，绝不重复注入）

## 安装

在**仓库根目录**直接运行（脚本会自动装 .NET SDK → 编译 → 备份 → 织入）：

```bash
./install.sh
```

然后**正常通过 Steam 启动游戏**，进任意关卡按 **F3** 开/关菜单。

> 自定义游戏路径：`MANAGED="/path/to/.../Managed" ./install.sh`

## 验证是否加载

```bash
# macOS
grep ADOFAITrainerMac "$HOME/Library/Logs/7th Beat Games/A Dance of Fire and Ice/Player.log"
# 期望： ... v1.4.0-mac loaded · ... · Menu key = F3 · patches ok=1 fail=0
```

## 卸载 / 还原

```bash
./uninstall.sh
```

从备份还原 `Assembly-CSharp.dll`，删除两个 DLL。或用 Steam「验证游戏文件完整性」一键还原。

## Linux（顺带支持，未经真机验证）

织入方案是平台无关的托管代码，Linux 上同样成立；`install.sh` / `uninstall.sh` 已用 `uname`
做了 Linux 分支（`A Dance of Fire and Ice_Data/Managed`、`~/.config/unity3d/...` 日志、Steam 多库查找）。

**但作者没有 Linux 真机，以下两点未确认**，Linux 用户请留意：

- **是否存在原生 Linux 版**：ADOFAI 的 Steam 商店页主要列 Windows / macOS。若你是用 **Proton 跑 Windows 版**，
  目录布局/进程名/日志路径都不一样，本脚本不适用（强制改用原生版后再试）。
- **原生可执行名**：脚本会自动从游戏目录里找 `*.x86_64`（Unity 与 `*_Data` 同级的可执行）并据此判断
  「游戏是否在跑」，无需手填;找不到时退回默认猜测 `ADanceOfFireAndIce.x86_64`。即便判断失准也只影响
  「运行中」的拦截,不影响织入本身。

## 注意

- **游戏更新或 Steam「验证文件完整性」会还原 `Assembly-CSharp.dll`，补丁随之失效**——重新跑一次 `install.sh` 即可。
- 仅单机；写存档的功能（永久解锁）与 Windows 版一致，谨慎使用。
- 保留了原作者的免费/防倒卖水印（删改水印会触发自校验、修改器自动失效）。
