# ScratchCard Web 功能对齐 CLI + Docker 运行：技术方案

> 项目隔离说明：本文档仅覆盖 `ScratchCard.Web`（以及其依赖的共享库 `ScratchCard.File` / `ScratchCard.Model` 中与 Web 功能对齐直接相关的最小改动）。不涉及 CLI 项目的交互/功能扩展。

## 1. 目标
- Web 侧实现与 CLI 相同的核心能力：
  - 输入刮刮卡：长度、宽度、奖项名称与数量
  - 校验输入（正整数、总奖品数不超过格子数、奖项不重复且非空）
  - 分配**不重复**的奖品坐标
  - 生成 `.xls`（NPOI）并落盘
  - 上传至 MinIO
  - 提供下载（从 MinIO 拉取后下载到浏览器）
- Web 可在 Docker 容器中稳定运行（默认 HTTP），并能在容器内成功生成文件（路径可写）。

## 2. 现状梳理（基于现有代码）
- Web 页面已有 [ScratchCard.Web/Components/Pages/NewCard.razor](../ScratchCard.Web/Components/Pages/NewCard.razor) ：
  - 已实现分步表单、坐标不重复分配、调用 `FileUtil.GenerateExcelFile`、调用 `MinioUtil.UploadFileAsync`、提供下载按钮。
- 主要问题/差异：
  1. 上传未 `await`：生成完成后马上进入“成功”步骤，可能导致 MinIO 还没写完就点击下载。
  2. 下载 `MemoryStream` 未复位：从 MinIO CopyTo 后 position 在末尾，下载可能变成 0 字节。
  3. Linux 容器落盘路径：`FileUtil.GetFilePath()` 在 Linux 默认写到 `/home/scratchcard/...`，在 `aspnet:8.0` + `USER app` 下可能无权限。
  4. 奖品数量允许 0（与 CLI 的“正整数”校验不一致），可能导致“看起来没生成内容”。
  5. 输入校验缺少 UI 反馈：异常会直接抛出，影响体验和可诊断性。

## 3. 设计与实现方案

### 3.1 输入校验与 UX（保持现有页面结构，不新增页面/复杂交互）
- 保持现有三步（设置卡片 / 设置奖品 / 完成）。
- 在“下一步”时做校验：
  - 长度、宽度、奖品种类数 > 0
  - 每个奖品：Name 非空、不可重复；Number > 0
  - 总奖品数 <= 总格子数
- 校验失败：使用 AntDesign 的 `Message`/`Notification`（已有 AntDesign）提示，不进入下一步。

### 3.2 坐标分配
- 沿用现有的“全坐标洗牌后切片分配”的方式（与 CLI 当前实现一致）：
  - 生成全部坐标列表
  - Fisher–Yates shuffle
  - 顺序分配给各奖项数量

### 3.3 生成 xls 与落盘路径（Docker 兼容）
- 在 `ScratchCard.File/FileUtil.GetFilePath()` 增加环境变量支持：
  - 优先使用 `SCRATCHCARD_OUTPUT_DIR`（若设置）
  - 未设置时：
    - Windows/macOS 保持原逻辑（Downloads）
    - Linux 默认改为 `/tmp/scratchcard`（可写，适配非 root 用户）
- `GenerateExcelFile` 继续接收 `filePath` 并确保目录创建（当前已实现）。

### 3.4 MinIO 上传/下载一致性与稳定性
- 上传：在生成后 **await** `UploadFileAsync(bucket, filePath)`，确保 MinIO 已可下载。
- 下载：从 MinIO 下载到 `MemoryStream` 后设置 `memoryStream.Position = 0` 再触发浏览器下载。
- Bucket 策略：
  - 为贴近 CLI 的“自动 bucket”，默认使用 `Environment.UserName`（容器内通常是 `app`）。
  - 若需要固定 bucket（如 public），通过配置 `MinioSettings:Bucket` 覆盖（默认可设为 `public`）。

### 3.5 Docker 运行
- 现有根目录 `Dockerfile` 已构建并运行 `ScratchCard.Web`。
- 补充 Docker 运行所需环境变量建议：
  - `ASPNETCORE_URLS=http://+:8080`
  - `SCRATCHCARD_OUTPUT_DIR=/tmp/scratchcard`
  - MinIO：`MinioSettings__serverAddress` / `MinioSettings__port` / `MinioSettings__accessKey` / `MinioSettings__secretKey` / `MinioSettings__secure` / `MinioSettings__bucket`
- 代码层面：默认不强制 HTTPS 重定向；通过配置 `EnableHttpsRedirection=true` 显式开启（适配反向代理/TLS 终止）。

## 4. 验证清单
- 本地运行 Web：
  - 最小样例（2x2，1 个奖项，数量 1）能生成并下载非 0 字节 `.xls`。
  - 多奖项样例总数 <= 格子数，下载内容正确。
- Docker 运行：
  - 容器启动后可访问页面
  - 生成 xls 不报权限错误
  - 上传 MinIO 完成后可正常下载

## 6. Docker 运行示例

### 6.1 构建镜像
- 在仓库根目录执行：
  - `docker build -t scratchcard-web:local .`

### 6.2 运行容器（HTTP，推荐用于本地/内网）
> 镜像内默认：`ASPNETCORE_URLS=http://+:8080`，文件输出目录默认 `/tmp/scratchcard`（可写）。

- 示例（根据你的 MinIO 信息替换）：
  - `docker run --rm -p 8080:8080 \
    -e MinioSettings__serverAddress=47.99.75.217 \
    -e MinioSettings__port=9000 \
    -e MinioSettings__accessKey=... \
    -e MinioSettings__secretKey=... \
    -e MinioSettings__secure=false \
    -e MinioSettings__bucket=public \
    scratchcard-web:local`

### 6.3 可选：开启 HTTPS 重定向
> 仅当你在容器前面有 TLS 终止（反向代理）且确认访问链路支持 HTTPS 时开启。

- `docker run ... -e EnableHttpsRedirection=true ...`

## 5. 任务拆分与完成标识

- [x] 方案已评审（本文）
- [x] Web：补齐输入校验与错误提示
- [x] Web：上传流程改为 await，避免未上传完成
- [x] Web：下载前复位 MemoryStream Position
- [x] File：Linux 下输出路径改为可写目录 + 支持 `SCRATCHCARD_OUTPUT_DIR`
- [x] Web：支持配置 `MinioSettings:Bucket`（默认值合理）
- [x] Docker：补充运行文档/示例（README 或 task 内）
- [x] 验证：本地 build + Docker build
