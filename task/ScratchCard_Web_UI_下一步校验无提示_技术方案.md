# ScratchCard Web UI：点击“下一步”校验无提示（技术方案）

> 项目隔离说明：本文档仅覆盖 `ScratchCard.Web` 页面 [ScratchCard.Web/Components/Pages/NewCard.razor](../ScratchCard.Web/Components/Pages/NewCard.razor) 的校验提示可见性问题，不涉及 CLI/文件生成/MinIO。

## 1. 问题描述
- 在“设置奖品”步骤（Step 2），当奖品名称为空、数量为 0 等非法输入时，点击“下一步”没有明显报错提示，用户不清楚为何无法进入下一步。

## 2. 可能原因
- 当前校验提示依赖 `MessageService` 弹层，用户在滚动或注意力不在顶部时容易错过；或在某些布局/容器情况下弹层不可见。
- 新建奖品行 `Number` 初始值为 0，与 UI `Min=1` 不一致，导致“看似可填但默认非法”。

## 3. 目标
- 校验失败时，页面底部（按钮上方）始终显示一条红色错误文案，确保提示可见。
- 奖品数量默认值与最小值一致（初始化为 1），减少“默认非法”。
- 保持最小改动：不新增页面，不引入新组件库。

## 4. 方案
- 增加 `validationError` 状态字段，校验失败时写入错误信息。
- 在固定 footer 中渲染该错误信息（按钮上方一行文本），样式使用 AntDesign CSS 变量：`--ant-color-error`。
- 在 `AwardTypeNumberOnChange` 初始化奖品时设置：`award.Name = string.Empty`、`award.Number = 1`。

## 5. 验证点
- Step 2 全空/名称空：点击“下一步”，footer 上方出现错误提示（同时可保留 Message 弹层）。
- 数量为 0 不再作为默认值出现。

## 6. 任务与完成标识
- [x] 增加 footer 内联错误提示（validationError）
- [x] 初始化奖品默认值（Name 为空字符串，Number=1）
- [x] 本地构建验证 `dotnet build ScratchCard.sln -c Release`
- [x] 更新到 Docker（`docker compose up -d --build`）
