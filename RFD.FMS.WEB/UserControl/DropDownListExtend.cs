using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI.Design;
using System.Collections;
using System.Collections.Generic;

namespace RFD.FMS.WEB.UserControl
{
    /// <summary>
    /// DropDownList 的摘要说明。
    /// </summary>
    [ToolboxData("<{0}:DropDownListExtend runat=\"server\" />")]
    public class DropDownListExtend : System.Web.UI.WebControls.TextBox
    {
        public DropDownList _DropDownList;

        public DropDownListExtend()
        {
            _DropDownList = new DropDownList();
        }

        private IDictionary<object, object> Values
        {
            get 
            {
                if (ViewState["DropDownValues"] == null)
                {
                    ViewState["DropDownValues"] = new Dictionary<object, object>();
                }

                return ViewState["DropDownValues"] as IDictionary<object, object>;
            }
            set
            { 
                if (ViewState["DropDownValues"] == null)
                {
                    ViewState["DropDownValues"] = new Dictionary<object, object>();
                }

                ViewState["DropDownValues"] = value;
            }
        }

        public string TextValue
        {
            get { return Text; }
            set { Text = value; }
        }

        public int SelectedIndex
        {
            get { return _DropDownList.SelectedIndex; }
            set { _DropDownList.SelectedIndex = value; }
        }

        public void Add(string key, string value)
        {
            IDictionary<object,object> dicValues = Values;

            dicValues.Add(key, value);

            Values = dicValues;
        }

        public void Clear()
        {
            IDictionary<object, object> dicValues = Values;

            dicValues.Clear();

            Values = dicValues;
        }

        public string SelectedValue
        {
            get { return _DropDownList.SelectedValue; }
        }

        /**/
        /// <summary> 
        /// 将此控件呈现给指定的输出参数。
        /// </summary>
        /// <param name="output"> 要写出到的 HTML 编写器 </param>
        protected override void Render(HtmlTextWriter output)
        {
            int iWidth = Convert.ToInt32(base.Width.Value);

            if (iWidth == 0)
            {
                iWidth = 100;

                base.Width = Unit.Parse("100px");
            }

            int sWidth = iWidth + 22;
            int spanWidth = sWidth - 18;

            output.Write("<div style=\"POSITION:relative\">");
            output.Write("<span style=\"MARGIN-LEFT:" + spanWidth.ToString() + "px;OVERFLOW:hidden;WIDTH:18px\">");

            _DropDownList.Width = Unit.Parse(sWidth.ToString() + "px");
            _DropDownList.Style.Add("MARGIN-LEFT", "-" + spanWidth.ToString() + "px");
            _DropDownList.Attributes.Add("onchange", "this.parentNode.nextSibling.value = $(this).find(':selected').text()");

            if (Values.Count > 0)
            {
                foreach (string key in Values.Keys)
                {
                    ListItem item = new ListItem();
                    item.Value = key;
                    item.Text = Values[key].ToString();
                    
                    _DropDownList.Items.Add(item);
                }
            }

            _DropDownList.RenderControl(output);

            output.Write("</span>");

            base.Style.Clear();
            base.Width = Unit.Parse(iWidth.ToString() + "px");
            base.Style.Add("left", "0px");
            base.Style.Add("POSITION", "absolute");

            base.Render(output);

            output.Write("</div>");
        }
    }
}


