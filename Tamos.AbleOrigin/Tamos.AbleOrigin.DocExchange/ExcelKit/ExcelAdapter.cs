using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.Record;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using Tamos.AbleOrigin.Common;

namespace Tamos.AbleOrigin.DocExchange.ExcelKit
{
    /// <summary>
    /// Excel 文档的适配器，提供与Excel文档间的读写等功能
    /// </summary>
    public class ExcelAdapter
    {
        /// <summary>
        /// 目前默认适配格式为Excel 2007
        /// </summary>
        public static ExcelAdapter GetAdapter()
        {
            return new ExcelAdapter();
        }

        private short _custColorIndex;

        private ExcelAdapter()
        {
            _custColorIndex = PaletteRecord.FIRST_COLOR_INDEX;
        }

        #region ExportExcel

        public string ExportExcel(string path, params SheetPage[] sheets)
        {
            if (string.IsNullOrEmpty(path) || sheets == null || sheets.Length <= 0) return "导出参数为空";

            //检查存储目录
            var filePath = Path.GetDirectoryName(path);
            if (filePath != null && !Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            var workbook = new HSSFWorkbook();
            foreach (var sheetPage in sheets)
            {
                //Excel里SheetName不允许“[]\/?”等特殊字符
                var sheet = workbook.CreateSheet(sheetPage.SheetName ?? "Sheet" + (workbook.NumberOfSheets + 1));
                if (sheetPage.Sections == null || sheetPage.Sections.Count < 1) continue;

                //解析SheetPage中样式为NPOI样式
                ParseSheetStyles(sheetPage.SheetStyles, workbook);

                //循环Sheet的每个部分
                var curRowIndex = 0;
                foreach (var section in sheetPage.Sections)
                {
                    var secColumnCount = section.ColumnCount;
                    //写入headers
                    if (section.Headers?.Count > 0)
                    {
                        WriteHeaderOrFooter(section.Headers, sheet, ref curRowIndex, secColumnCount);
                    }
                    //write section body data
                    if (section.BodyWriteAction != null)
                    {
                        var writer = new NPOIContentWriter(sheet, curRowIndex);
                        section.BodyWriteAction(writer);
                        curRowIndex = writer.CurRowIndex;
                    }
                    //写入footers
                    if (section.Footers?.Count > 0)
                    {
                        WriteHeaderOrFooter(section.Footers, sheet, ref curRowIndex, secColumnCount);
                    }

                    //every section has 1 row span
                    curRowIndex += 1;
                }
            }

            //输出到文件
            using (var stream = new FileStream(path, FileMode.Create))
            {
                workbook.Write(stream);
            }
            return null;
        }

        private void WriteHeaderOrFooter(List<SheetRow> rows, ISheet sheet, ref int curRowIndex, int secColumnCount)
        {
            foreach (var header in rows)
            {
                var curColumnIndex = 0;
                var row = sheet.CreateRow(curRowIndex);
                //loop header cell
                foreach (var celObj in header.Cells)
                {
                    if (celObj.ChildrenCount < 1)
                    {
                        //single cell
                        var cell = row.CreateCell(curColumnIndex);
                        ApplyCellValueAndLayout(cell, celObj, curRowIndex, curColumnIndex, secColumnCount);
                    }
                    else
                    {
                        //with children cells
                        int rowOffset = 0, columnOffset = 0;
                        IRow tpRow = null;
                        foreach (var subCel in celObj.ChildrenCells)
                        {
                            var celRowNum = curRowIndex + rowOffset;
                            if (tpRow == null) //set row of subCell
                            {
                                if (rowOffset == 0) tpRow = row;
                                else tpRow = sheet.GetRow(celRowNum) ?? sheet.CreateRow(celRowNum);
                            }

                            var cell = tpRow.CreateCell(curColumnIndex + columnOffset);
                            ApplyCellValueAndLayout(cell, subCel, celRowNum, curColumnIndex + columnOffset, secColumnCount);

                            //check offset
                            if (columnOffset + subCel.ColumnSpanOrDefault >= celObj.ColumnSpanOrDefault) //new row
                            {
                                rowOffset++;
                                columnOffset = 0;
                                tpRow = null;
                            }
                            else
                            {
                                columnOffset += subCel.ColumnSpanOrDefault;
                            }
                        }
                    }
                    curColumnIndex += celObj.ColumnSpanOrDefault;
                }
                curRowIndex += header.RowSpan;
            }
        }

        #endregion

        #region Cell layout style set

        private void ParseSheetStyles(IEnumerable<LayoutStyle> styles, HSSFWorkbook workbook)
        {
            //var res = new List<ICellStyle>();
            if (styles == null) return;
            foreach (var style in styles)
            {
                var cst = workbook.CreateCellStyle();
                style.StoreIndex = cst.Index;
                //字体
                if (style.FontPoints > 0 || style.FontBold)
                {
                    var font = workbook.CreateFont();
                    if (style.FontPoints > 0) font.FontHeightInPoints = style.FontPoints;
                    if (style.FontBold) font.IsBold = true;
                    cst.SetFont(font);
                }
                //对齐
                if (style.Alignment != AlignmentMode.NotSet)
                {
                    switch (style.Alignment)
                    {
                        case AlignmentMode.Center:
                            cst.Alignment = HorizontalAlignment.Center;
                            break;
                        case AlignmentMode.Left:
                            cst.Alignment = HorizontalAlignment.Left;
                            break;
                        case AlignmentMode.Right:
                            cst.Alignment = HorizontalAlignment.Right;
                            break;
                    }
                }
                if (style.VerticalAlignment != AlignmentMode.NotSet)
                {
                    switch (style.VerticalAlignment)
                    {
                        case AlignmentMode.Center:
                            cst.VerticalAlignment = VerticalAlignment.Center;
                            break;
                        case AlignmentMode.Top:
                            cst.VerticalAlignment = VerticalAlignment.Top;
                            break;
                        case AlignmentMode.Bottom:
                            cst.VerticalAlignment = VerticalAlignment.Bottom;
                            break;
                    }
                }
                //set bgcolor
                if (!string.IsNullOrEmpty(style.BackgroundColor))
                {
                    SetStyleBgColor(cst, style.BackgroundColor, workbook);
                }
                //set border
                if (style.HasBorder)
                {
                    cst.BorderLeft = cst.BorderTop = cst.BorderRight = cst.BorderBottom = BorderStyle.Thin;
                }

                //res.Add(cst);
            }
        }

        private void SetStyleBgColor(ICellStyle style, string colorStr, HSSFWorkbook workbook)
        {
            if (style == null || colorStr.Length < 6) return;
            var bytes = new byte[3];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(colorStr.Substring(i*2, 2), 16);
            }

            //NPOI2.2.1 设置FillForegroundColor，背景色才生效
            style.FillPattern = FillPattern.SolidForeground;
            var palette = workbook.GetCustomPalette();
            var color = palette.FindColor(bytes[0], bytes[1], bytes[2]);
            if (color == null) //set custom color
            {
                if (_custColorIndex + 1 < PaletteRecord.STANDARD_PALETTE_SIZE + PaletteRecord.FIRST_COLOR_INDEX) _custColorIndex++;
                palette.SetColorAtIndex(_custColorIndex, bytes[0], bytes[1], bytes[2]);
                style.FillForegroundColor = _custColorIndex;
            }
            else
            {
                style.FillForegroundColor = color.Indexed;
            }
        }

        private void ApplyCellValueAndLayout(ICell cell, BaseCell celObj, int curRow, int curColumn, int sectionColumnCount)
        {
            //set cell value
            if (!string.IsNullOrEmpty(celObj.Value)) cell.SetCellValue(celObj.Value);

            //call layout set
            var layoutSet = celObj.LayoutSet;
            CellRangeAddress mergeRange = null;
            if (layoutSet != null)
            {
                if (layoutSet.RowSpan > 1 || layoutSet.ColumnSpan > 1 || layoutSet.HorizontalStretch)
                {
                    var lastRow = layoutSet.RowSpan > 1 ? curRow + layoutSet.RowSpan - 1 : curRow;
                    var lastColumn = layoutSet.HorizontalStretch
                        ? curColumn + sectionColumnCount - 1
                        : (layoutSet.ColumnSpan > 1 ? curColumn + layoutSet.ColumnSpan - 1 : curColumn);
                    if (curRow != lastRow || curColumn != lastColumn)
                    {
                        mergeRange = new CellRangeAddress(curRow, lastRow, curColumn, lastColumn);
                        cell.Sheet.AddMergedRegion(mergeRange);
                    }
                }
                if (layoutSet.ColumnWidth > 0)
                {
                    cell.Sheet.SetColumnWidth(cell.ColumnIndex, celObj.LayoutSet.ColumnWidth * 256);
                }
                if (layoutSet.RowHeight > 0)
                {
                    cell.Row.HeightInPoints = layoutSet.RowHeight;
                }
            }

            //cell style set
            var styleSet = celObj.LayoutStyle;
            if (styleSet != null)
            {
                cell.CellStyle = cell.Sheet.Workbook.GetCellStyleAt(styleSet.StoreIndex);
                if (styleSet.ApplyToColumn && cell.CellStyle != null)
                {
                    var aplStyle = cell.CellStyle;
                    if (styleSet.HasBorder || !string.IsNullOrEmpty(styleSet.BackgroundColor)) //边框和颜色不应用到列
                    {
                        aplStyle = cell.Sheet.Workbook.CreateCellStyle();
                        aplStyle.CloneStyleFrom(cell.CellStyle);
                        aplStyle.FillPattern = FillPattern.NoFill;
                        aplStyle.BorderLeft = aplStyle.BorderTop = aplStyle.BorderRight = aplStyle.BorderBottom = BorderStyle.None;
                    }
                    cell.Sheet.SetDefaultColumnStyle(cell.ColumnIndex, aplStyle);
                }

                //修复合并单元格样式
                if (mergeRange != null) SetMergeCellStyle(cell, mergeRange);
            }
        }

        //NPOI 合并单元格后，边框会有缺失，这里执行恢复设置
        internal static void SetMergeCellStyle(ICell mergeCell, CellRangeAddress range)
        {
            //(Sheet as HSSFSheet).SetEnclosedBorderOfRegion(); 效率较低
            var style = mergeCell.CellStyle;
            var sheet = mergeCell.Sheet;
            for (var ri = range.FirstRow; ri <= range.LastRow; ri++)
            {
                var row = sheet.GetRow(ri) ?? sheet.CreateRow(ri);
                for (var ci = range.FirstColumn; ci <= range.LastColumn; ci++)
                {
                    var cell = row.GetCell(ci) ?? row.CreateCell(ci, mergeCell.CellType);
                    cell.CellStyle = style;
                }
            }
        }

        #endregion

		#region ReadExcel

		public IContentReader ReadExcel(string filePath)
		{
			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;

			var fileStream = File.OpenRead(filePath);
			var workbook = new HSSFWorkbook(fileStream);
			if (workbook.NumberOfSheets < 1)
			{
				fileStream.Dispose();
				return null;
			}

			return new NPOIContentReader(workbook.GetSheetAt(0), fileStream);
		}

		#endregion
    }
}