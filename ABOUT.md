# 关于

> A free, open-source in-game BepInEx trainer for *A Dance of Fire and Ice* (autoplay & recording tools). The details below are in Chinese.

## 这是什么

**冰与火之舞修改器 (ADOFAI Trainer)** 是一个为单机节奏游戏《A Dance of Fire and Ice》制作的游戏内修改器（trainer）。它以一个 BepInEx 插件的形式运行，在游戏里叠加一个图形菜单（按 `F3` 呼出），把游戏里原本隐藏的能力暴露成可一键开关的选项。

## 为什么

最初的目标只是**录制完美通关视频**：游戏核心是「按节拍点击」，手速对点很难做到全程满分。与其练手速或写键盘宏，不如直接借用游戏引擎**自带的 autoplay**——它按谱面的浮点拍点触发，没有输入延迟，天然帧级满分，画面和真人手打完全一致。后来顺手把扫描游戏文件时发现的其它隐藏能力（变速、解锁、放宽判定等）也一并做进了菜单。

## 工作原理

修改器**不做内存偏移扫描**，而是直接调用游戏自身已存在的开关和函数（通过 BepInEx + HarmonyX）。例如：

- **Autoplay** = 把 `RDC.auto` 设为 `true`——这正是游戏自带「录制模式」（`GCS.d_recording`）所用的同一个原生标志（同时执行 `RDC.auto = true; RDC.noHud = true`），主游戏里**没有 autoplay 水印**。
- **变速** = 设置 `GCS.speedTrialMode` + `GCS.nextSpeedRun`，引擎在关卡开始时据此同时缩放 BPM 与音源音高。
- **永久解锁** = 写入 `Persistence.unlockAllLevels`（游戏自身的设置项），它会驱动 `RDC.forceUnlockAllLevels`。
- **放宽判定** = 唯一的 Harmony 补丁：后缀 `scrMisc.GetAdjustedAngleBoundaryInDeg`，按倍数放大 Counted/Perfect/Pure 三个命中窗口。

因为只是「调用游戏自己的逻辑」，所以比内存修改稳定得多，游戏小更新通常也不易失效。本工具与「节奏医生修改器」同源同法——两款游戏同为 7th Beat Games 出品、引擎与代码高度同源。

## 免费声明

本工具**完全免费、开源，严禁倒卖**。标题栏、菜单顶部与加载日志显示的项目地址即为完整性水印；删除或篡改会触发校验，使修改器**直接禁用**。请始终从官方地址获取：**github.com/Cohenjikan/ADOFAITrainer**

## 作者

Cohenjikan · 以 MIT 许可证开源。与 7th Beat Games 无关联。
