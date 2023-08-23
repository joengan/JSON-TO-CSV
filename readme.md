# JSON to CSV Converter

這是一個工具，專為將 JSON 檔案轉換成 CSV 檔案而設計，但僅限於表格 (Table) 格式的資料。

## 用途

- 快速將表格格式的 JSON 數據轉換為 CSV。
- 無需額外安裝任何依賴或工具。
- 自動在 JSON 檔案的目錄下生成對應的 CSV 檔案。
- 生成的 CSV 檔案使用 Big5 編碼，以便於在 Excel 中直接開啟和查看。

## 下載和安裝

如果你不想從源代碼開始，我們提供了預先打包好的 `.exe` 版本供下載：

[點擊此處下載最新版本](https://github.com/joengan/JSON-TO-CSV/releases)

下載後，直接運行該 `.exe` 文件即可。

## 用法

1. 確保你的 JSON 檔案是表格格式，例如:

```json
[
    {"name": "張三", "age": 30, "city": "台北"},
    {"name": "李四", "age": 25, "city": "高雄"}
]
