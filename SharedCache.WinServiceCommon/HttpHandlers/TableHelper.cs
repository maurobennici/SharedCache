#region Copyright (c) Roni Schuetz - All Rights Reserved
// * --------------------------------------------------------------------- *
// *                              Roni Schuetz                             *
// *              Copyright (c) 2010 All Rights reserved                   *
// *                                                                       *
// * Shared Cache high-performance, distributed caching and    *
// * replicated caching system, generic in nature, but intended to         *
// * speeding up dynamic web and / or win applications by alleviating      *
// * database load.                                                        *
// *                                                                       *
// * This Software is written by Roni Schuetz (schuetz AT gmail DOT com)   *
// *                                                                       *
// * This library is free software; you can redistribute it and/or         *
// * modify it under the terms of the GNU Lesser General Public License    *
// * as published by the Free Software Foundation; either version 2.1      *
// * of the License, or (at your option) any later version.                *
// *                                                                       *
// * This library is distributed in the hope that it will be useful,       *
// * but WITHOUT ANY WARRANTY; without even the implied warranty of        *
// * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU      *
// * Lesser General Public License for more details.                       *
// *                                                                       *
// * You should have received a copy of the GNU Lesser General Public      *
// * License along with this library; if not, write to the Free            *
// * Software Foundation, Inc., 59 Temple Place, Suite 330,                *
// * Boston, MA 02111-1307 USA                                             *
// *                                                                       *
// *       THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE.        *
// * --------------------------------------------------------------------- *
#endregion

// *************************************************************************
//
// Name:      TableHelper.cs
// 
// Modified:  28-01-2010 SharedCache.com, chrisme  : clean up code
// ************************************************************************* 

namespace SharedCache.WinServiceCommon.HttpHandlers
{
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;

	/// <summary>
	/// Helper class used when building up the Html tables for display
	/// </summary>
	internal class TableHelper
	{
        /// <summary>
        /// Adds the cell.
        /// </summary>
        /// <param name="rowToAddCellTo">The row to add cell to.</param>
        /// <param name="cellText">The cell text.</param>
        /// <returns>A TableCell</returns>
		public static TableCell AddCell(TableRow rowToAddCellTo, string cellText)
		{
			var cell = new TableCell();

			cell.Text = cellText;
			rowToAddCellTo.Cells.Add(cell);

			return cell;
		}

        /// <summary>
        /// Adds the cell.
        /// </summary>
        /// <param name="rowToAddCellTo">The row to add cell to.</param>
        /// <param name="cellText">The cell text.</param>
        /// <param name="hyperLink">The hyper link.</param>
        /// <returns>A TableCell</returns>
		public static TableCell AddCell(TableRow rowToAddCellTo, string cellText, string hyperLink)
		{
			var cell = new TableCell();
			var anchor = new HtmlAnchor();

			anchor.HRef = hyperLink;
			anchor.InnerText = cellText;
			cell.Controls.Add(anchor);

			rowToAddCellTo.Cells.Add(cell);

			return cell;
		}


        /// <summary>
        /// Adds the header cell.
        /// </summary>
        /// <param name="rowToAddCellTo">The row to add cell to.</param>
        /// <param name="cellText">The cell text.</param>
        /// <returns>A TableCell</returns>
		public static TableCell AddHeaderCell(TableRow rowToAddCellTo, string cellText)
		{
			var cell = new TableHeaderCell();

			cell.Text = cellText;
			rowToAddCellTo.Cells.Add(cell);
			cell.HorizontalAlign = HorizontalAlign.Left;
			return cell;
		}

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <returns>A Table</returns>
		public static Table CreateTable()
		{
			var table = new Table();

			table.BorderStyle = BorderStyle.Solid;
			table.BorderWidth = Unit.Pixel(1);
			table.Width = Unit.Percentage(100);
			table.CellPadding = 0;
			table.CellSpacing = 0;
			return table;
		}
	}
}
