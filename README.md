# 贪吃蛇

RobotPilots 软件开发训练项目，使用 C# 和 .NET 10 实现终端贪吃蛇。Task6、Task7 是同一项目的两个阶段，统一使用本仓库。

## 对应任务与当前进度

- **Task6／Stage 1：**终端字符显示、方向控制、随机食物、吃食物增长、撞墙／撞自身失败，以及失败后重试和退出。当前显示蛇长。
- **Task7／Stage 2：**重构为函数式规则核心与外围应用层；已加入启动加载 JSON 配置，以及持续 5 秒的双倍速度冲刺。
- **后续要求：**多人模式、计时与胜负规则、Protobuf＋MQTT 通信、与他人实现互通及多人对打视频仍待完成。

任务书的 Stage 1 要求显示分数，当前版本以蛇长展示结果；Stage 2 的计时结算也尚未完成，不能据此视为全部验收通过。

## 结构

- `Game.Functional`：游戏状态与规则计算。
- `Game.Application`：持有当前状态并协调操作。
- `Game.Terminal`：终端输入、显示、计时循环与配置加载。
- `Game.Core.Tests`：测试代码，包含早期阶段留下的测试。

## 运行

安装 .NET 10 SDK，在仓库根目录执行：

```bash
dotnet run --project Game.Terminal/Game.Terminal.csproj
```

方向键控制，`R` 冲刺；失败后按 `A` 重试或 `Esc` 退出。场地大小、移动速度、食物刷新和冲刺参数在 `Game.Terminal/config.json` 中配置。
