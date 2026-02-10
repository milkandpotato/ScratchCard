# ScratchCard Web UI：页面只显示上半部分（技术方案）

> 项目隔离说明：本文档仅覆盖 `ScratchCard.Web` 页面 [ScratchCard.Web/Components/Pages/NewCard.razor](../ScratchCard.Web/Components/Pages/NewCard.razor) 的布局/高度问题修复，不涉及 CLI、文件生成、MinIO、Docker Compose。

## 1. 问题描述
- 在“设置奖品”步骤，页面内容看起来只占上半部分，下半部分出现大块空白（影响填写体验）。

## 2. 初步原因判断
- 页面中 `.form-content` 使用了固定高度 `height: 300px;`。
- 当视口高度较大时，会导致表单/表格区域只显示上部，剩余空间为空白。
- 同时底部按钮为固定 footer，页面需要为 footer 留出空间，进一步放大了空白观感。

## 3. 目标
- 表单区域高度随视口自适应，尽量占满 footer 之上的可用高度。
- 内容过长时在表单区域内滚动，底部按钮保持可见。
- 最小改动：仅调整 CSS，不引入新组件/新主题。

## 4. 方案
- 将 `.form-content` 的 `height: 300px` 改为 `height: calc(100vh - Xpx)`，让其随视口高度变化。
- 增加 `min-height`，避免小屏时过小。
- footer 增加 `z-index`，避免被表格遮挡。
- 适当调整 `.page-content` 的 `padding-bottom`，与 footer 高度匹配，避免内容被遮挡。

## 5. 验证点
- 大屏：表格区域占满 footer 上方空间，不再出现大块空白。
- 小屏：可滚动填写，按钮始终可见。

## 6. 任务与完成标识
- [x] 调整 `.form-content` 为视口自适应高度
- [x] 确保 footer 不遮挡内容（z-index/padding）
- [x] 本地构建验证 `dotnet build ScratchCard.sln -c Release`
- [x] 更新到 Docker（`docker compose up -d --build`）
