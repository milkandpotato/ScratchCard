# ScratchCard Web UI：校验提示使用 AntDesign 组件（技术方案）

> 项目隔离说明：本文档仅覆盖 `ScratchCard.Web` 页面 [ScratchCard.Web/Components/Pages/NewCard.razor](../ScratchCard.Web/Components/Pages/NewCard.razor) 的校验提示展示方式调整，不涉及业务逻辑与 Docker。

## 1. 问题描述
- 当前校验失败时，页面底部使用自定义 `<div>` 红字提示（非 AntDesign 组件）。
- 期望：提示使用 AntDesign 组件呈现（例如 Alert/Message）。

## 2. 目标
- 校验失败时，使用 AntDesign 组件展示错误提示，保证一致的 UI 风格。
- 保留现有 `MessageService.Error(...)`（即时反馈），同时用 AntDesign 组件在页面内“常驻可见”。

## 3. 方案
- 将 footer 内的纯文本提示替换为 AntDesign 的 `<Alert>` 组件：
  - `Type=Error`、`ShowIcon=true`
  - 内容绑定 `validationError`
  - 仅在 `validationError` 非空时渲染
  - 注意：部分 AntDesign 参数类型为 `bool?`，Razor 不支持空属性写法（例如 `ShowIcon`），需要显式赋值：`ShowIcon="true"`。
- 移除 `.validation-error` 自定义样式（如不再使用）。

## 4. 验证点
- Step 2 校验失败时：
  - 顶部弹出 Message（已有）
  - 底部按钮上方出现 AntDesign Alert（可见且样式统一）

## 5. 任务与完成标识
- [x] 将提示替换为 AntDesign Alert
- [x] 本地构建验证 `dotnet build ScratchCard.sln -c Release`
- [x] 更新到 Docker（`docker compose up -d --build`）
