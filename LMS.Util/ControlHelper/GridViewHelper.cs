using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Collections.Specialized;

namespace RFD.FMS.Util.ControlHelper
{
    public static class GridViewHelper
    {
        /// <summary>
        /// 查找绑定字段的Cell index
        /// </summary>
        /// <param name="gridView"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static int FindColumnIndex(this GridView gridView, string fieldName)
        {
            for (int i = 0; i < gridView.Columns.Count; i++)
            {
                var column = gridView.Columns[i] as BoundField;
                if (column != null && column.DataField == fieldName)
                {
                    return i;
                }
            }
            return 0;
        }

        public static string GetCellText(TableCell cell)
        {
            string text = cell.Text;
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
            foreach (Control control in cell.Controls)
            {
                if (control != null && control is IButtonControl)
                {
                    IButtonControl btn = control as IButtonControl;
                    text = btn.Text.Replace("\r\n", "").Trim();
                    break;
                }
                if (control != null && control is ITextControl)
                {
                    LiteralControl lc = control as LiteralControl;
                    if (lc != null)
                    {
                        continue;
                    }
                    ITextControl l = control as ITextControl;

                    text = l.Text.Replace("\r\n", "").Trim();
                    break;
                }
            }
            return text;
        }
        /// <summary>
        /// 从GridView的数据生成DataTable
        /// </summary>
        /// <param name="gv">GridView对象</param>
        public static DataTable GridView2DataTable(GridView gv)
        {
            DataTable table = new DataTable();
            int rowIndex = 0;
            List<string> cols = new List<string>();
            if (!gv.ShowHeader && gv.Columns.Count == 0)
            {
                return table;
            }
            GridViewRow headerRow = gv.HeaderRow;
            if (headerRow != null)
            {
                int columnCount = headerRow.Cells.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    string text = GetCellText(headerRow.Cells[i]);
                    cols.Add(text);
                }
                foreach (GridViewRow r in gv.Rows)
                {
                    if (r.RowType == DataControlRowType.DataRow)
                    {
                        DataRow row = table.NewRow();
                        int j = 0;
                        for (int i = 0; i < columnCount; i++)
                        {
                            string text = GetCellText(r.Cells[i]);
                            if (!String.IsNullOrEmpty(text))
                            {
                                if (rowIndex == 0)
                                {
                                    string columnName = cols[i];
                                    if (String.IsNullOrEmpty(columnName))
                                    {
                                        continue;
                                    }
                                    if (table.Columns.Contains(columnName))
                                    {
                                        continue;
                                    }
                                    DataColumn dc = table.Columns.Add();
                                    dc.ColumnName = columnName;
                                    dc.DataType = typeof(string);
                                }
                                row[j] = text;
                                j++;
                            }
                        }
                        rowIndex++;
                        table.Rows.Add(row);
                    }
                }
            }
            return table;
        }

        public static void Check(GridView gridView, string checkUiID, bool isCheck)
        {
            foreach (GridViewRow row in gridView.Rows)
            {
                ((CheckBox)row.FindControl(checkUiID)).Checked = isCheck;
            }
        }

        /// <summary>
        /// 获取所有选中行的所有Key值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gridView"></param>
        /// <param name="checkUiID"></param>
        /// <param name="valueIndex"></param>
        /// <returns></returns>
        public static IList<KeyValuePair<DataKey, GridViewRow>> GetSelectedRows(GridView gridView, string checkUiID)
        {
            IList<KeyValuePair<DataKey, GridViewRow>> keyPairValues = new List<KeyValuePair<DataKey, GridViewRow>>();
            KeyValuePair<DataKey, GridViewRow> keyPair;

            foreach (GridViewRow row in gridView.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    if (((CheckBox)row.FindControl(checkUiID)).Checked)
                    {
                        DataKey dataKey = gridView.DataKeys[row.RowIndex];

                        keyPair = new KeyValuePair<DataKey, GridViewRow>(dataKey, row);

                        keyPairValues.Add(keyPair);
                    }
                }
            }

            return keyPairValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gridView"></param>
        /// <param name="checkUiID"></param>
        /// <param name="valueIndex"></param>
        /// <param name="keyValueIndex">唯一键的index</param>
        /// <returns></returns>
        public static IList<KeyValuePair<T, string>> GetSelectedRows<T>(GridView gridView, string checkUiID, int valueIndex,int keyValueIndex)
        {
            IList<KeyValuePair<T, string>> keyPairValues = new List<KeyValuePair<T, string>>();

            KeyValuePair<T, string> keyPair;

            string id;
            string status;

            foreach (GridViewRow row in gridView.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    if (((CheckBox)row.FindControl(checkUiID)).Checked)
                    {
                        DataKey dataKey = gridView.DataKeys[row.RowIndex];

                        id = dataKey.Values[keyValueIndex].ToString();
                        status = row.Cells[valueIndex].Text;

                        keyPair = new KeyValuePair<T, string>(DataConvert.ToValue<T>(id), status);

                        keyPairValues.Add(keyPair);
                    }
                }
            }

            return keyPairValues;
        }

        public static IList<KeyValuePair<T, string>> GetSelectedRows<T>(GridView gridView, string checkUiID, int valueIndex)
        {
            IList<KeyValuePair<T, string>> keyPairValues = new List<KeyValuePair<T, string>>();

            KeyValuePair<T, string> keyPair;

            string id;
            string status;

            foreach (GridViewRow row in gridView.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    if (((CheckBox)row.FindControl(checkUiID)).Checked)
                    {
                        DataKey dataKey = gridView.DataKeys[row.RowIndex];

                        id = dataKey.Value.ToString();
                        status = row.Cells[valueIndex].Text;

                        keyPair = new KeyValuePair<T, string>(DataConvert.ToValue<T>(id), status);

                        keyPairValues.Add(keyPair);
                    }
                }
            }

            return keyPairValues;
        }
        /// <summary>
        /// 无数据时显示表头
        /// </summary>
        /// <param name="gridView">绑定控件</param>
        /// <param name="data">数据源</param>
        public static void ShowEmptyGridHeader(this GridView gridView, DataTable data)
        {
            ShowEmptyGridHeader(gridView, data, "查找不到任何数据，请重新设定条件查询！", 0);
        }
        /// <summary>
        /// 无数据时显示表头
        /// </summary>
        /// <param name="gridView">绑定控件</param>
        /// <param name="data">数据源</param>
        public static void ShowEmptyGridHeader(this GridView gridView, DataTable data, string error)
        {
            ShowEmptyGridHeader(gridView, data, error, 0);
        }
        /// <summary>
        /// 无数据时显示表头
        /// </summary>
        /// <param name="gridView">绑定控件</param>
        /// <param name="data">数据源</param>
        public static void ShowEmptyGridHeader(this GridView gridView, DataTable data, string error, int colspans)
        {
            data.Rows.Add(data.NewRow());
            gridView.DataSource = data;
            gridView.DataBind();
            gridView.Rows[0].Cells.Clear();
            gridView.Rows[0].Cells.Add(new TableCell());
            gridView.Rows[0].Cells[0].ColumnSpan = data.Columns.Count + colspans;
            gridView.Rows[0].Cells[0].Text = error;
            gridView.Rows[0].Cells[0].ForeColor = Color.Red;
            gridView.RowStyle.HorizontalAlign = HorizontalAlign.Center;
        }
        /// <summary>
        /// 绑定数据(支持显示空表头)
        /// </summary>
        /// <param name="gridView">绑定控件</param>
        /// <param name="dataSource">数据源</param>
        public static void BindData(this GridView gridView, object dataSource, string error, int colspans)
        {
            var data = dataSource as DataTable;
            if (data.IsEmpty())
            {
                gridView.ShowEmptyGridHeader(data, error, colspans);
                return;
            }
            gridView.DataSource = data;
            gridView.DataBind();
        }
        /// <summary>
        /// 绑定数据(支持显示空表头)
        /// </summary>
        /// <param name="gridView">绑定控件</param>
        /// <param name="dataSource">数据源</param>
        public static void BindData(this GridView gridView, object dataSource, int colspan)
        {
            BindData(gridView, dataSource, "查找不到任何数据，请重新设定条件查询！", colspan);
        }
        /// <summary>
        /// 获取GirdView的列表头(不含隐藏列)
        /// </summary>
        /// <param name="gridView">绑定控件</param>
        /// <returns></returns>
        public static string[] GetGridViewHeaders(GridView gridView)
        {
            var columns = new List<string>();

            if (gridView != null && gridView.HeaderRow != null)
            {
                for (var i = 0; i < gridView.HeaderRow.Cells.Count; i++)
                {
                    var cell = gridView.HeaderRow.Cells[i];

                    var column = gridView.Columns[i];

                    if (cell.Visible || column.Visible)
                    {
                        var text = cell.Text;

                        if (cell.HasControls() && cell.Controls[0] is LinkButton)
                        {
                            text = ((LinkButton)cell.Controls[0]).Text;
                        }

                        columns.Add(text);
                    }
                }
            }

            return columns.ToArray<string>();
        }
        /// <summary>
        /// 获取GirdView的列表头(不含隐藏列)
        /// </summary>
        /// <param name="gridView">绑定控件</param>
        /// <param name="ignoreColumns">忽略不显示的列</param>
        /// <param name="addColumns">需要新增显示的列</param>
        /// <returns></returns>
        public static string[] GetGridViewHeaders(GridView gridView, string[] ignoreColumns, params string[] addColumns)
        {
            var columns = GetGridViewHeaders(gridView);

            var temp = new List<string>();

            if (columns != null && columns.Length > 0)
            {
                foreach (var col in addColumns)
                {
                    if (columns.Contains(col)) continue;

                    temp.Add(col);
                }

                foreach (var col in columns)
                {
                    if (ignoreColumns.Contains(col)) continue;

                    temp.Add(col);
                }
            }

            return temp.ToArray<string>();
        }

        /// <summary>
        /// 合并普通列
        /// </summary>
        /// <param name="gv">GridView对象</param>
        /// <param name="columnIndex">合并列的名称</param>
        public static void UnitCell(GridView gv, int columnIndex)        
        {            
            int i = 0;                  //当前行数            
            string lastType = string.Empty;        //当前判断是否合并行对应列的值            
            int lastCell = 0;           //判断最后一个相同值的行的索引            
            
            if (gv.Rows.Count > 0)            
            {                
                lastType = gv.Rows[0].Cells[columnIndex].Text.ToString();                
                gv.Rows[0].Cells[columnIndex].RowSpan = 1;                
                lastCell = 0;            
            }            
            
            for (i = 1; i < gv.Rows.Count; i++)            
            {                
                if (gv.Rows[i].Cells[columnIndex].Text == lastType)                
                {                    
                    gv.Rows[i].Cells[columnIndex].Visible = false;                    
                    gv.Rows[lastCell].Cells[columnIndex].RowSpan++;                
                }                
                else                
                {                    
                    lastType = gv.Rows[i].Cells[columnIndex].Text.ToString();                    
                    lastCell = i;                    
                    gv.Rows[i].Cells[columnIndex].RowSpan = 1;                
                }            
            }        
        }

        /// <summary>        
        /// Gridview列的合并（模板列）        
        /// </summary>        
        /// <param name="gv">需要合并的GridView对象</param>        
        /// <param name="columnIndex">所要合并列的索引</param>        
        /// <param name="lblName">模板列对象的ID</param>        
        public static void UnitCell(GridView gv, int columnIndex, string lblName)        
        {            
            int i = 0;    //当前行数            
            string lastType = string.Empty;  //当前判断是否合并行对应列的值            
            int lastCell = 0;  //判断最后一个相同值的行的索引            
            
            if (gv.Rows.Count > 0)            
            {                
                lastType = (gv.Rows[0].Cells[columnIndex].FindControl(lblName) as Label).Text;                
                gv.Rows[0].Cells[columnIndex].RowSpan = 1;                
                lastCell = 0;            
            }            
            
            for (i = 1; i < gv.Rows.Count; i++)            
            {                
                if ((gv.Rows[i].Cells[columnIndex].FindControl(lblName) as Label).Text == lastType)                
                {                    
                    gv.Rows[i].Cells[columnIndex].Visible = false;                    
                    gv.Rows[lastCell].Cells[columnIndex].RowSpan++;                
                }                
                else                
                {                    
                    lastType = (gv.Rows[i].Cells[columnIndex].FindControl(lblName) as Label).Text.ToString();                    
                    lastCell = i;                    
                    gv.Rows[i].Cells[columnIndex].RowSpan = 1;                
                }            
            }       
        }
    }
}
