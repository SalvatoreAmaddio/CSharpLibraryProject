﻿using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using System.Runtime.InteropServices;
using XL = Microsoft.Office.Interop.Excel;

namespace Backend.Office
{
    public class Worksheet : IDestroyable
    {
        XL._Worksheet wrksheet;

        public Worksheet(XL._Worksheet wrksheet) => this.wrksheet = wrksheet;

        /// <summary>
        /// Sets the name of the worksheet.
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name) => this.wrksheet.Name = name;

        /// <summary>
        /// Prints a header with a default style. By default, the header will be at the first row.
        /// </summary>
        /// <param name="headers">the headers</param>
        /// <param name="row">the row index where the header will be printed at</param>
        public void PrintHeader(string[] headers, int row = 1)
        {
            int column = 1;
            foreach(string header in headers) 
            {
                SetValue(header, row, column);
                column++;
            }

            Range range = GetRange(1, row, headers.Length, row);
            range.HorizontalAlignment(XlAlign.Center);
            range.VerticalAlignment(XlAlign.Center);
            range.Bold(true);
            range.Destroy();
        }

        /// <summary>
        /// Prints a <see cref="IEnumerable{T}"/> in a tabular format. By default, the print starts at the second row.
        /// </summary>
        /// <param name="records"></param>
        /// <param name="row"></param>
        public void PrintData(IEnumerable<ISQLModel> records, int row = 2)
        {
            int column = 1;
            foreach (ISQLModel record in records) 
            {
                foreach(ITableField tableField in record.GetAllTableFields()) 
                {
                    if (tableField is FKField fk) 
                    {
                        ISQLModel? value = DatabaseManager.Find(fk.ClassName)?.Records.FirstOrDefault(s=>s.Equals(fk?.GetValue()));
                        SetValue(value, row, column);
                    }
                    else 
                        SetValue(tableField?.GetValue(), row, column);
                    column++;
                }
                column = 1;
                row++;
            }
        }

        /// <summary>
        /// Sets the value of a given cell. The cell is represented by the row and column index. 
        /// <para/>
        /// For Example:
        /// <code>
        ///   excel.Worksheet?.SetValue("ciao", 1, 1); //prints the word 'ciao' in cell A1.
        /// </code>
        /// </summary>
        /// <param name="value">the value to print</param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <exception cref="WorksheetException"></exception>
        public void SetValue(object? value, int row = 1, int col = 1)
        {
            if (row <= 0 || col <= 0) throw new WorksheetException("In Excel, indexes starts from 1.");
            this.wrksheet.Cells[row, col] = (value == null) ? string.Empty : value;
        }

        /// <summary>
        /// Sets the value of a given cell. The cell is represented by the row and column index. 
        /// <para/>
        /// For Example:
        /// <code>
        ///   excel.Worksheet?.SetValue("ciao", 1, "A"); //prints the word 'ciao' in cell A1.
        /// </code>
        /// </summary>
        /// <param name="value">the value to print</param>
        /// <param name="row">the row index</param>
        /// <param name="col">the column label</param>
        /// <exception cref="WorksheetException"></exception>
        public void SetValue(object? value, int row=1, string col="A")
        {
            if (row <= 0) throw new WorksheetException("In Excel, indexes starts from 1.");
            this.wrksheet.Cells[row, col] = (value == null) ? string.Empty : value;
        }

        /// <summary>
        /// Gets a Range based on indexes. For Example:
        /// <code>
        ///     Range range = GetRange(1, 1, 2, 1); //gets the Range from A1 to B1.
        /// </code>
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="row1"></param>
        /// <param name="col2"></param>
        /// <param name="row2"></param>
        /// <returns>A Range</returns>
        /// <exception cref="WorksheetException"></exception>
        public Range GetRange(int col1 = 1, int row1 = 1, int col2 = 1, int row2 = 1)
        {
            if (row1 <= 0 || col1<= 0  || col2 <= 0 || row2 <= 0) throw new WorksheetException("In Excel, indexes starts from 1.");
            return new Range(wrksheet, col1, row1, col2, row2);
        }

        /// <summary>
        /// Gets a Range based on cells' names. For Example:
        /// <code>
        ///     Range range = GetRange("A1", "H1"); //gets the Range from A1 to H1.
        /// </code>
        /// </summary>
        /// <param name="cell1"></param>
        /// <param name="cell2"></param>
        /// <returns>A Range</returns>
        public Range GetRange(string cell1, string cell2)
        {
            return new Range(wrksheet, cell1, cell2);
        }

        public void Destroy() => Marshal.ReleaseComObject(this.wrksheet);

    }
}
