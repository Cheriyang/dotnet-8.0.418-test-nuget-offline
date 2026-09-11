# .NET 8.0.418 离线测试依赖合集

面向 **.NET SDK 8.0.418 / net8.0**，包含以下三个指定版本以及 NuGet 实际解析的完整传递依赖：

| 指定包 | 版本 |
| --- | --- |
| Microsoft.NET.Test.Sdk | 17.8.0 |
| xunit | 2.5.3 |
| xunit.runner.visualstudio | 2.5.3 |

共 **88 个原始 `.nupkg` 文件**：3 个直接依赖、85 个传递依赖。每个文件均独立存放于 [`packages/`](packages/)，合集 ZIP 在本仓库 Releases 中提供。

## 使用方法

先安装 .NET SDK **8.0.418**。下载 Release ZIP 并完整解压，或克隆本仓库。以下命令在解压后的根目录执行：

```powershell
dotnet restore ./sample/OfflineTests.csproj --configfile ./NuGet.Config --locked-mode -p:NuGetAudit=false
dotnet test ./sample/OfflineTests.csproj --no-restore
```

`NuGet.Config` 清除在线包源，仅使用相邻的 `packages` 目录；根目录和示例目录的 `global.json` 精确指定 SDK 8.0.418。`NuGetAudit=false` 用于避免离线还原尝试查询在线漏洞数据库，不代表包已通过漏洞审计。

在自己的 `net8.0` 项目中使用：

```powershell
dotnet restore C:/path/to/YourTests.csproj --configfile C:/path/to/unpacked/NuGet.Config -p:NuGetAudit=false
dotnet test C:/path/to/YourTests.csproj --no-restore
```

项目应引用上述指定版本。若另有包引用，需补齐它们的依赖；本合集不包含 SDK、其他目标框架的引用包或自包含发布所需的运行时包。

## 来源与校验

所有包由 SDK 8.0.418 从官方 NuGet 源 `https://api.nuget.org/v3/index.json` 下载，并按真实 `net8.0` 依赖图收集，没有修改包内容。

- `packages-manifest.json`：每个包的 ID、版本、直接/传递类型、依赖边、源地址、大小、SHA-256 和 SHA-512。
- `SHA256SUMS.txt`：各 `.nupkg` 文件的校验值。
- `sample/packages.lock.json`：SDK 生成的精确依赖锁文件及 NuGet 内容哈希。
- `VALIDATION.txt`：离线还原和示例测试的验证记录。

其中早期 .NET Standard 依赖由 xUnit 2.5.3 的依赖链引入，因此包数较多。这里只覆盖 NuGet 为本示例 `net8.0` 项目解析的依赖闭包，不宣称覆盖所有目标框架。

各包的版权、许可证和使用条款归原发布者所有，请查阅对应 `.nupkg` 中的 `.nuspec`、许可证文件及官方 NuGet 页面。
