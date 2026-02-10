# ScratchCard CLI 生成 xls 为空：优化方案

## 背景与现象
CLI 项目在输入奖品与数量后，会生成一个 `.xls` 文件，但打开后内容为空（或上传到 MinIO 的文件为空）。

## 结论摘要（最可能根因）
1. **生成路径不一致**（高概率、直接导致“看到/上传的文件为空”）
   - CLI 侧先生成并打印/上传了 `filePath`。
   - 但 `GenerateExcelFile(card)` 内部又重新生成了另一个路径并写入。
   - 结果：**写入的是 B 文件，而你查看/上传的可能是 A 文件**（A 文件未写入/0 字节/旧文件）。

2. **返回已关闭的 FileStream**（隐患/潜在问题）
   - `GenerateExcelFile` 在 `wb.Write(fs)` 后立即 `Close/Dispose`，但仍 `return fs`。
   - 当前调用方虽然没用这个 `stream` 再做事，但这是典型的资源生命周期 bug。

3. **奖品坐标随机生成不可靠**（导致“看起来空白/被覆盖”）
   - `AwardPosition` 每次都 `new Random()`，高频创建会重复种子，坐标大量重复。
   - 重复会造成奖品写入同一单元格，被后写覆盖，最终表格看起来像“几乎没填”。

4. **输入校验不足**（次要）
   - `GetNumber` 对负数提示但不强制重新输入；数量允许 0 也会产生“无填充”。

> 备注：第 1 点单独就足以解释“生成/上传的是空文件”。

---

## 目标
- **不改变现有交互 UX**（仍然通过 Console 输入长度/宽度/奖项/数量）。
- 确保生成的 `.xls`：
  - 写入路径与上传路径一致；
  - 文件落盘成功且非 0 字节；
  - 奖品分布可控、尽量不重复覆盖。

---

## 最小改动修复方案（优先推荐）

### 方案 A：统一 filePath 的单一来源（最小侵入，优先做）
**核心原则：路径只生成一次。**

#### 改动点
1. CLI 侧：只调用一次 `FileUtil.GetFilePath()`，并把它用于：
   - 控制台打印
   - `GenerateExcelFile` 写入
   - MinIO 上传

2. FileUtil 侧：把 `GenerateExcelFile` 改成接收 `filePath` 参数，并写入该路径。

#### 建议的接口形态（示例，不要求照抄）
- `public static void GenerateExcelFile(Card card, string filePath)`
  - 内部使用 `FileMode.Create` 打开文件
  - `using var fs = new FileStream(...)`
  - `wb.Write(fs)` 后自然释放

#### 为什么有效
- 彻底消除“写入 B 文件、上传 A 文件”的错位。

---

## 文件写入与资源生命周期优化（与方案 A 可一起做）

### 1) 不返回已 Dispose 的 Stream
- 若生成方法的职责是“写文件”，建议返回 `string filePath` 或 `FileInfo`，而不是 `FileStream`。

### 2) 使用 `FileMode.Create`
- 替换 `OpenOrCreate`，确保每次生成都是新内容，避免旧文件残留或长度截断问题。

### 3) 写完自检（强烈建议）
- 写完后检查：
  - `File.Exists(filePath)`
  - `new FileInfo(filePath).Length > 0`
- 若失败：直接提示错误并停止上传。

---

## 奖品坐标生成优化（解决“看起来空白/覆盖”）

### 方案 B：共享随机源（小改动，但仍可能重复）
- 用 `Random.Shared`（.NET 6+）替代每次 `new Random()`。
- 或在外部创建单例 `Random` 并传入。

### 方案 C：全坐标洗牌分配（推荐，正确性最好）
**思路：先生成所有格子坐标列表，整体 shuffle，然后按奖品数量顺序分配。**

#### 优点
- 坐标不重复，不会覆盖
- 数量精确
- 结果更符合用户直觉

#### 实施建议
- 生成 `List<(x,y)> allCells`
- Fisher–Yates shuffle
- 按奖项数量切片分配给每个 `Award` 的 positions

> 如需保持 `AwardPosition` 类型：可以让 `AwardPosition` 支持直接指定 `x,y` 构造，而不是内部随机。

---

## 输入校验建议（低成本提升体验）
- `GetNumber` 建议改成：
  - 必须 `> 0`（尤其是“奖品数量”）
  - 若输入非法（非数字/负数/0），循环重试
- 在生成前输出汇总：
  - 总格子数、总奖品数、剩余空格数

---

## 验证清单（不依赖复杂测试）
1. 输入最小案例：长度 2、宽度 2、奖项 1、数量 1
   - 生成文件应非 0 字节
   - `Answer` 表至少有 1 个格子写入奖品名称
2. 多奖项：长度 5、宽度 5、奖项 3，各数量若干
   - 不应出现“几乎全空”的情况
3. 路径一致性：控制台输出路径 == 实际写入路径 == 上传路径

---

## 实施顺序建议
1. **先做方案 A（路径统一）**：直接解决“空文件/上传错文件”。
2. 再做“写入/资源生命周期优化”：避免潜在流使用 bug。
3. 最后做“坐标分配优化”：提升内容填充质量与可解释性。

---

## 关联代码位置（便于定位）
- CLI 入口与上传：`ScratchCard.CLI/Program.cs`
- 文件生成：`ScratchCard.File/FileUtil.cs`
- MinIO 上传：`ScratchCard.File/MinioUtil.cs`
- 坐标生成：`ScratchCard.Model/AwardPosition.cs`
- 输入校验：`ScratchCard.CLI/CheckUtils.cs`
