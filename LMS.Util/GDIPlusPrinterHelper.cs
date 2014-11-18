using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace RFD.FMS.Util
{
    public class GDIPlusPrinterHelper : PrintDocument//, IDisposable
    {
        private readonly IList<IPrintable> _printableList = new List<IPrintable>();
        private int Width { set; get; }
        private int Height { set; get; }
        public Margins TableMargins { set; get; }
        private Graphics _graphics { set; get; }
        public GDIPlusPrinterHelper()
        {
            this.TableMargins = new Margins(0, 0, 0, 0);
            this.PrintPage += new PrintPageEventHandler(OrderThirdPartyPrinter_PrintPage);
        }

        public Graphics GetGraphics()
        {
            return this._graphics;
        }

        /// <summary>
        /// 打印设置对话框
        /// </summary>
        /// <param name="printDocument">打印的文档</param>
        /// <returns></returns>
        public PrinterSettings ShowPrintSetupDialog()
        {
            PrinterSettings ps = new PrinterSettings();
            PrintDialog pDlg = new PrintDialog();
            try
            {
                pDlg.AllowSomePages = true;
                pDlg.Document = this;
                var reslut = pDlg.ShowDialog();
                if (reslut == DialogResult.OK)
                {
                    ps = pDlg.PrinterSettings;
                    this.Print();
                }
            }
            catch (InvalidPrinterException ex)
            {
            }
            catch (Exception ex)
            {
            }
            finally
            {
                pDlg.Dispose();
            }
            return ps;
        }

        void OrderThirdPartyPrinter_PrintPage(object sender, PrintPageEventArgs e)
        {
            this._graphics = e.Graphics;
            _printableList.ToList().ForEach(printable =>
            {
                if (printable is AbsolutePrintRectangle)
                {
                    var rect = (AbsolutePrintRectangle)printable;
                    this._graphics.DrawRectangle(rect.RectanglePen,
                                                 rect.LeftTopX,
                                                 rect.LeftTopY,
                                                 rect.Width,
                                                 rect.Height);
                }
                if (printable is PrintLabel)
                {
                    var label = (PrintLabel)printable;
                    this._graphics.DrawString(label.Text,
                                              label.LabelFont, label.LabelBrush,
                                              label.LeftTopX,
                                              label.LeftTopY);
                }
                if (printable is PrintArea)
                {
                    var area = (PrintArea)printable;
                    this._graphics.DrawString(area.Text, area.AreaFont, area.AreaBrush, area.LayoutRectangle, area.TextFormat);
                }
                if (printable is PrintPicture)
                {
                    var pic = (PrintPicture)printable;
                    this._graphics.DrawImage(pic.PictureImage, (int)pic.LeftTopX, (int)pic.LeftTopY, pic.Width, pic.Height);
                }
                if (printable is PrintLine)
                {
                    var line = (PrintLine)printable;
                    this._graphics.DrawLine(line.LinePen, line.X1, line.Y1, line.X2, line.Y2);
                }
            });
            var currentRowIndex = -1;
            var currentColumnIndex = -1;
            var lastRowMaxHeight = Single.MinValue;
            var currentX = Convert.ToSingle(this.TableMargins.Left);
            var currentY = Convert.ToSingle(this.TableMargins.Top);
            _printableList.Where(printable => printable is FlowPrintRectangle).ToList()
                .ConvertAll<FlowPrintRectangle>(item => (FlowPrintRectangle)item)
                .OrderBy(item => item.RowIndex).ThenBy(item => item.ColumnIndex).
                ToList().ForEach(printable =>
              {
                  if (currentRowIndex == -1)
                      currentRowIndex = printable.RowIndex;
                  if (currentColumnIndex == -1)
                      currentColumnIndex = printable.ColumnIndex;
                  if (currentRowIndex != printable.RowIndex)
                  {
                      currentX = Convert.ToSingle(this.TableMargins.Left);
                      currentY += lastRowMaxHeight;
                      lastRowMaxHeight = Single.MinValue;
                      currentRowIndex = printable.RowIndex;
                  }
                  var rect = (FlowPrintRectangle)printable;
                  this._graphics.DrawRectangle(rect.RectanglePen,
                                              currentX,
                                               currentY,
                                               rect.Width,
                                               rect.Height);

                  if (rect.InnerPrintable != null)
                      rect.InnerPrintable.ToList().ForEach(innerPrintable =>
                       {
                           if (innerPrintable is PrintLabel)
                           {
                               var label = (PrintLabel)innerPrintable;
                               this._graphics.DrawString(label.Text,
                                             label.LabelFont, label.LabelBrush,
                                            (currentX + label.LeftTopX),
                                           (currentY + label.LeftTopY));
                           }
                           if (innerPrintable is PrintArea)
                           {
                               var area = (PrintArea)innerPrintable;
                               this._graphics.DrawString(area.Text, area.AreaFont, area.AreaBrush, new RectangleF
                               {
                                   X = area.LayoutRectangle.X + currentX,
                                   Y = area.LayoutRectangle.Y + currentY,
                                   Width = area.LayoutRectangle.Width,
                                   Height = area.LayoutRectangle.Height,
                               }
                               , area.TextFormat);
                           }
                       });

                  currentX += rect.Width;
                  if (rect.Height > lastRowMaxHeight)
                      lastRowMaxHeight = rect.Height;
              });
        }

        public void Add(IPrintable printable)
        {
            _printableList.Add(printable);
        }

        public new void Print()
        {
            base.Print();
        }

        #region IDisposable 成员

        //void IDisposable.Dispose()
        //{
        //    _graphics.Dispose();
        //}

        #endregion
    }

    public interface IPrintable
    { }

    public class FlowPrintRectangle : IPrintable
    {
        public int RowIndex { set; get; }
        public int ColumnIndex { get; set; }
        public float Width { set; get; }
        public float Height { set; get; }
        public Pen RectanglePen { set; get; }
        public IList<IPrintable> InnerPrintable { set; get; }
    }

    public class AbsolutePrintRectangle : IPrintable
    {
        public float LeftTopX { set; get; }
        public float LeftTopY { set; get; }
        public float Width { set; get; }
        public float Height { set; get; }
        public Pen RectanglePen { set; get; }
        public IList<IPrintable> InnerPrintable { set; get; }
    }

    public class PrintLabel : IPrintable
    {
        public float LeftTopX { set; get; }
        public float LeftTopY { set; get; }
        public Font LabelFont { set; get; }
        public Brush LabelBrush { set; get; }
        public string Text { get; set; }
    }

    public class PrintLine : IPrintable
    {
        public float X1 { set; get; }
        public float Y1 { set; get; }
        public float X2 { set; get; }
        public float Y2 { set; get; }
        public Pen LinePen { set; get; }
    }

    public class PrintPicture : IPrintable
    {
        public Image PictureImage { set; get; }
        public float LeftTopX { set; get; }
        public float LeftTopY { set; get; }
        public float Width { set; get; }
        public float Height { set; get; }
    }

    /// <summary>
    /// 打印范围字符串
    /// </summary>
    /// <remarks>
    /// add by 张本冬
    /// </remarks>
    public class PrintArea : IPrintable
    {
        public string Text { get; set; }
        public Font AreaFont { set; get; }
        public Brush AreaBrush { set; get; }
        public RectangleF LayoutRectangle { get; set; }
        public StringFormat TextFormat { get; set; }
    }
}
