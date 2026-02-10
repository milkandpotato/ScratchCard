# ScratchCard Web UI：表头中文 + 底部按钮固定（技术方案）

> 项目隔离说明：本文档仅覆盖 `ScratchCard.Web` 的页面 [ScratchCard.Web/Components/Pages/NewCard.razor](../ScratchCard.Web/Components/Pages/NewCard.razor) UI 问题修复，不影响 CLI/文件生成/MinIO 逻辑。

## 1. 问题描述
1) “设置奖品”步骤的表格列头显示为英文 `Name` / `Number`，期望改为中文显示。
2) “上一步/下一步”按钮区域当前随内容滚动，期望固定在页面底部（始终可见）。

## 2. 目标
- 列头显示中文：`奖品名称`、`奖品数量`。
- 底部按钮固定：在步骤 0/1 表单较长时，按钮始终固定在视口底部居中显示。
- 最小改动：不新增页面、不引入新组件、不增加新主题/颜色（使用 Ant Design CSS 变量）。

## 3. 方案设计

### 3.1 表头中文
- 在 `PropertyColumn` 显式设置列标题（如 `Title`），避免默认从属性表达式推断英文。

### 3.2 底部按钮固定
- 将按钮容器增加一个专用 class（例如 `.steps-footer`），使用 `position: fixed; left: 0; right: 0; bottom: 0;` 固定到底部。
- 为避免遮挡表单内容：给页面根容器 `.page-content` 增加 `padding-bottom`，高度约等于 footer 高度。
- footer 视觉分隔：用 Ant Design CSS 变量（如 `--ant-color-split`、`--ant-color-bg-container`）设置顶部边框/背景（不硬编码颜色）。

## 4. 验证点
- 进入 `/card`，步骤 2（设置奖品）表头显示中文。
- 下拉滚动表单区域时，“上一步/下一步”始终在底部可见且不遮挡输入。

## 5. 任务与完成标识
- [x] 修改表头为中文
- [x] 底部按钮固定在页面底部
- [x] 本地构建验证 `dotnet build ScratchCard.sln -c Release`
