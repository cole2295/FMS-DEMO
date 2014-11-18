/*

<TreeView ID="TreeView1"
Width="200" Height="500" Theme="Vista" BackColor="#EEEEEE" BorderWidth="1px" BorderStyle="solid" BorderColor="FF0000"		 
NodeStyle="Style..." HoverNodeStyle="Style..." SelectedNodeStyle="Style..."
NodeClass="CssClass" HoverNodeClass="CssClass" SelectedNodeClass="CssClass"
NodeIndent="Integer" NodeSpacing="Integer" NodePadding="Integer" ChildNodesPadding="Integer"
		
ShowImages="true" ShowCheckBoxes="Boolean" ShowDebugNode="Boolean" AllowNodeEditing="Boolean"	
ExpandOnSelect="Boolean" CollapseOnSelect="Boolean"
SelectedPath="templates.3" CheckedPaths="" ExpandDepth="0"
			 		 		 		 
OnLoad="Function"
OnNodeExpand="Function" OnNodeCollapse="Function"
OnNodeSelect="Function" OnCheck="Function"
OnContextMenu="Function"
OnNodeNavigate="Function"
		
Xml="Xml Path">		
		
<TreeNode Name="IdentityKey" Text="String" Value="String" Title="String" Image="ImagePath" Href="Url|Path" Target="_blank|_parent|frameName" Xml="Xml Path"></TreeNode>
	
</TreeView>
	
* <TreeView>标签请单独放到某个元素中
* 需要读取的属性放到构造函数里, 以方便遍历
* 所有事件均初始化为Function, 以方便遍历
* 读取配置, 使用TreeView标签转化的<DIV>
* 如果节点或根同时配置了节点项和Xml, 将在节点项后显示Xml
* TreeView对应<DIV>, TreeNode对应<SPAN>
* TreeView事件如果需要传递TreeNode作为参数
* 事件没有返回值
* TreeNode的节点属性 如firstChild当临近节点改变时改变
* TreeNode没有事件
	
TreeView声明可以通过两种方式: 通过标签名是DIV还是TREEVIEW区分两种方式而且只支持这两种方式
1. <TreeView>标签配置
2. TreeView对象实例化, 实现化时不读取标签属性, 只把div当作显示TreeView的容器
	
* 因为childNodes.length不能判断未展开的节点, 所以通过TreeNode.hasChildNodes判断是否有子节点
* 通过TreeNode.loaded指明子节点是否已经加载完成
* 通过TreeNode.__dataElement保存子节点数据信息
* 展开时再加载子节点信息
	
* 实现XML节点中不区分大小写的读取
* 默认右键菜单, 右键菜单的空白区域的显示
* TreeNode的getAttribute()方法
* TreeNode的setAttribute()方法
	
* expandDepth 属性
* selectedPath 属性
* checkedPaths 属性

nodeStyle, hoverNodeStyle, selectedNodeStyle, nodeClass hoverNodeClass selectedNodeClass 下一步实现
* FireFox不缩进的问题
* __parseBoolean, __parsePixel 的问题, 方法名是否必须依赖于对象, 如果必须, 是否能写得更简单些
* 各个属性的默认值问题, FireFox下不支持没有单位的数值设定
		
FireFox键盘事件
# 模块化 CheckBox, ContextMenu, TextEditor, DragDrop - 可以实现
节点编辑
拖放
HtmlParser
	
TreeNode.expandImage, 当未设置时为null
*/

document.treeViews = new Array();

TreeView = function(id) {
    /// <summary>构造函数</summary>

    /// <value type="String">显示TreeView或TreeView标签的元素ID</value>
    this.id = id;
    /// <value type="String">源数据XML地址</value>
    this.xml = null;

    /// <value type="String|Integer">宽度</value>
    this.width = '100%';
    /// <value type="String|Integer">高度, 因为使用DIV布局, 所以高度请尽量使用固定值, 如果使用百分比, 也应保证TreeView的父级元素设定了高度</value>
    this.height = '100%'; //'500px';

    /// <value type="String">样式主题</value>
    this.theme = 'vista';
    /// <value type="String">背景颜色</value>
    this.backColor = null || '#FFFFFF';
    /// <value type="String|Integer">边框宽度</value>
    this.borderWidth = null;
    /// <value type="String">边框样式</value>
    this.borderStyle = null;
    /// <value type="String">边框颜色</value>
    this.borderColor = null;

    /// <value type="String|Integer">每级TreeNode的缩进距离</value>
    this.nodeIndent = '16px';
    /// <value type="String|Integer">节点内对象(如文本、图标)与节点外框之间的距离</value>
    this.nodePadding = '2px';
    /// <value type="String|Integer">两个同级节点之间的间距</value>
    this.nodeSpacing = '0px';
    /// <value type="String|Integer">父节点与子节点之间的距离</value>
    this.childNodesPadding = '0px';

    /// <value type="String">普通状态下的节点样式</value>
    //this.nodeStyle = null;
    /// <value type="String">鼠标划过状态下的节点样式</value>
    //this.hoverNodeStyle = null;
    /// <value type="String">被选择状态下的节点样式</value>
    //this.selectedNodeStyle = null;

    /// <value type="String">普通状态下的节点样式</value>
    //this.nodeClass = null
    /// <value type="String">鼠标划过状态下的节点样式</value>
    //this.hoverNodeClass = null;
    /// <value type="String">被选择状态下的节点样式</value>
    //this.selectedNodeClass = null;

    /// <value type="Boolean">是否显示节点图标</value>
    this.showImages = true;
    /// <value type="Boolean|String">是否显示复选框, 可选值'All|Leaf|None|Branch|Root'<value>
    this.showCheckBoxes = false;
    /// <value type="Boolean">是否在选择节点时展开子节点</value>
    this.expandOnSelect = false;
    /// <value type="Boolean">是否在选择节点时关闭子节点</value>
    //this.collapseOnSelect = false;
    /// <value type="Boolean">是否显示debug节点</value>
    this.showDebugNode = false;
    /// <value type="Boolean">是否可以编辑节点文本</value>
    this.allowNodeEditing = false;
    /// <value type="Boolean">是否作为HtmlParser</value>
    //this.htmlParser = false;

    /// <value type="Integer">默认展开的TreeNode深度, 0为根节点</value>
    this.expandDepth = 0;

    /// <value type="String">默认选择的项, 格式 1.2.3</value>
    this.selectedPath = null;
    /// <value type="String">默认选中的项, 格式 1.2.3,1.2.4,...</value>
    this.checkedPaths = null;

    /// <value type="Array" elementType="TreeNode">所有根节点的集合</value>
    this.childNodes = new Array();
}

// 全局变量, 脚本保存路径
TreeView.$root = function() {
    var scripts = document.getElementsByTagName('SCRIPT');
    var path = scripts[scripts.length - 1].src;
    path = path.substring(0, path.lastIndexOf('/') + 1);
    return path;
} ();

// 全局变量, 相关图片保存路径
TreeView.$images = function() {
    return TreeView.$root + 'images/';
} ();

//TreeView.__Modules = {};
//TreeView.__Modeles.CheckBox = false;
//TreeView.__Modeles.ContextMenu = false;
//TreeView.__Modules.TextEditor = false;
//TreeView.__Modules.DragDrop = false;
//TreeView.__Modules.HtmlParser = false;

/// <value type="Array" elementType="TreeNode">所有根节点</value>
//TreeView.prototype.childNodes = [];
/// <value type="TreeNode">第一个根节点</value>
TreeView.prototype.firstChild = null;
/// <value type="TreeNode">最后一个根节点</value>
TreeView.prototype.lastChild = null;

/// <value type="Element">显示TreeView的HTML元素的引用(div或其他),用于读取或设置属性值</value>
TreeView.prototype.div = null;

/// <value type="Boolean">标识TreeView是否已经被加载</value>
TreeView.prototype.loaded = false;

/// <value type="TreeNode">选择的节点项</value>
TreeView.prototype.selectedNode = null;

/// <value type="Array" elementType="TreeNode">在TreeView.getCheckedNodes, TreeNode.__getCheckedNodes方法中使用的临时变量</value>
TreeView.prototype.__checkedNodes = null;
/// <value type="Event">用户事件临时变量</value>
TreeView.prototype.__event = { toggle: null, contextMenu: null };
/// <value type="Object" elementType="Boolean">__initialize中使用到的对象临时变量</value>
TreeView.prototype.__actions = { expanding: null, selecting: null, checking: null };

TreeView.prototype.expandAll = function() {
    /// <summary>展开所有</summary>
    for (var i = 0; i < this.childNodes.length; i++) {
        this.childNodes[i].__expandAll();
    }
};

TreeView.prototype.collapseAll = function () {
    /// <summary>关闭所有</summary>
    for (var i = 0; i < this.childNodes.length; i++) {
        this.childNodes[i].__collapseAll();
    }
};

TreeView.prototype.loadAll = function() {
    /// <summary>加载所有</summary>
    if (this.firstChild != null) {
        this.firstChild.__loadAll();
    }
};

TreeView.prototype.checkAll = function() {
    /// <summary>选中所有</summary>

    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i].checked != 1) this.childNodes[i].check();
    }
};

TreeView.prototype.uncheckAll = function() {
    /// <summary>取消选中所有</summary>

    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i].checked != 0) this.childNodes[i].uncheck();
    }
};

TreeView.prototype.appendChild = function(treeNode) {
    /// <summary>添加根节点</summary>

    //name
    if (treeNode.name == null || treeNode.name == '') {
        treeNode.name = this.id + '_' + this.childNodes.length;
    }

    //parentNode
    treeNode.parentNode = null;
    //treeView
    treeNode.treeView = this;
    //depth
    treeNode.depth = 0;
    //path
    treeNode.path = treeNode.name;
    //checked

    var length = this.childNodes.length;

    // childNodes
    this.childNodes.push(treeNode);

    // render	
    this.childNodes[length].populate();
    this.div.appendChild(this.childNodes[length].element);
    this.div.appendChild(this.childNodes[length].div);

    // firstChild & lastChild
    if (length == 0) this.firstChild = this.childNodes[length];
    this.lastChild = this.childNodes[length];

    //previousSibling & nextSibling
    if (length > 0) {
        this.childNodes[length].previousSibling = this.childNodes[length - 1];
        this.childNodes[length - 1].nextSibling = this.childNodes[length];
        this.childNodes[length].nextSibling = null;
    }
};

TreeView.prototype.insertBefore = function(treeNode, referenceNode) {
    /// <summary>在referenceNode之前插入根节点</summary>
    /// <param name="treeNode" type="TreeNode">要添加的节点</param>
    /// <param name="referenceNode" type="TreeNode">参考节点</param>

    //name
    if (treeNode.name == null || treeNode.name == '') {
        treeNode.name = this.id + '_' + this.childNodes.length;
    }

    //parentNode
    treeNode.parentNode = null;
    //treeView
    treeNode.treeView = this;
    //depth
    treeNode.depth = 0;
    //path
    treeNode.path = treeNode.name;

    //index
    var index = -1;
    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i] == referenceNode) {
            index = i;
            break;
        }
    }

    if (index > -1) {
        // childNodes
        this.childNodes.splice(index, 0, treeNode);

        // render	
        this.childNodes[index].populate();
        this.div.insertBefore(this.childNodes[index].element, referenceNode.element);
        this.div.insertBefore(this.childNodes[index].div, referenceNode.element);

        // firstChild
        if (index == 0) this.firstChild = this.childNodes[0];

        //previousSibling & nextSibling
        var previousNode = referenceNode.previousSibling;
        this.childNodes[index].previousSibling = previousNode;
        if (previousNode != null) previousNode.nextSibling = this.childNodes[i];
        this.childNodes[index].nextSibling = referenceNode;
        referenceNode.previousSibling = this.childNodes[index];
    }
    else {
        //如果没有找不到参考节点, 就直接附加
        this.appendChild(treeNode);
    }
};

TreeView.prototype.removeChild = function(treeNode) {
    /// <summary>删除根节点</summary>
    /// <param name="treeNode" type="TreeNode">要删除的节点</param>

    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i] == treeNode) {
            this.div.removeChild(treeNode.element);
            this.div.removeChild(treeNode.div);

            this.childNodes.splice(i, 1);

            // firstChild, lastChild, nextSibling, previousSibling
            if (this.childNodes.length == 0) {
                //一项不剩下
                this.firstChild = null;
                this.lastChild = null;
            }
            else if (this.childNodes.length == 1) {
                //只剩下一项
                this.firstChild = this.childNodes[0];
                this.lastChild = this.childNodes[0];
                this.childNodes[0].previousSibling = null;
                this.childNodes[0].nextSibling = null;
            }
            else {
                if (i == 0) {
                    //是第一项
                    this.firstChild = this.childNodes[0];
                    this.childNodes[0].previousSibling = null;
                }
                else if (i == this.childNodes.length) {
                    //是最后一项
                    this.lastChild = this.childNodes[i - 1];
                    this.childNodes[i - 1].nextSibling = null;
                }
                else {
                    //在中间
                    this.childNodes[i].previousSibling = this.childNodes[i - 1];
                    this.childNodes[i - 1].nextSibling = this.childNodes[i];
                }
            }
            break;
        }
    }
};

TreeView.prototype.removeAll = function() {
    /// <summary>删除所有根节点</summary>
    //清除子节点
    this.childNodes.length = 0;
    //删除元素节点
    for (var i = this.div.childNodes.length - 1; i >= 0; i--) {
        if (this.div.childNodes[i].nodeType == 1 && this.div.childNodes[i].nodeName != 'SPAN') {
            this.div.removeChild(this.div.childNodes[i]);
        }
    }
    //firstChild & lastChild
    this.firstChild = null;
    this.lastChild = null;
}

TreeView.prototype.load = function() {
    /// <summary>加载TreeView的根节点</summary>

    //id
    if (this.id == null) this.id = '__TreeView_' + document.treeViews.length;
    //element
    if (this.div == null) this.div = document.getElementById(this.id);

    var treeView = this;

    // 右键事件, 在空白处点击将要显示的菜单
    this.div.oncontextmenu = function(ev) {
        ev = ev || window.event;
        var target = ev.srcElement || ev.target;

        while (target.nodeName != 'TD' && target != this) {
            target = target.parentNode;
        }
        if (target == this) {
            treeView.__event.contextMenu = ev;
            treeView.__executeEvent('oncontextmenu', null);
            treeView.__event.contextMenu = null;
            return false;
        }
    };

    // 键盘事件	
    this.div.onkeydown = function(ev) {
        var broken = true;
        if (treeView.selectedNode != null) {
            //←↑→↓		37,38,39,40
            //esc 27
            //enter 13
            //→ 展开
            //← 闭合
            //↑ 上一个节点
            //↓ 下一个节点
            //Enter 展开
            //Esc 取消
            ev = ev || window.event;
            var treeNode = treeView.selectedNode;
            if (ev.keyCode == 37) //←
            {
                if (treeNode.expanded) {
                    treeNode.collapse();
                }
            }
            else if (ev.keyCode == 38) //↑
            {
                if (treeNode.previousSibling != null) {
                    if (treeNode.previousSibling.hasChildNodes && treeNode.previousSibling.expanded) {
                        treeNode.previousSibling.lastChild.select();
                    }
                    else {
                        treeNode.previousSibling.select();
                    }
                }
                else if (treeNode.parentNode != null) {
                    treeNode.parentNode.select();
                }
            }
            else if (ev.keyCode == 39) //→
            {
                if (!treeNode.expanded) {
                    treeNode.expand();
                }
            }
            else if (ev.keyCode == 40) //↓
            {
                if (treeNode.hasChildNodes && treeNode.expanded) {
                    treeNode.firstChild.select();
                }
                else if (treeNode.nextSibling != null) {
                    treeNode.nextSibling.select();
                }
                else if (treeNode.parentNode != null && treeNode.parentNode.nextSibling != null) {
                    treeNode.parentNode.nextSibling.select();
                }
            }
            else if (ev.keyCode == 13) //enter
            {
                if (!treeNode.expanded) {
                    treeNode.expand();
                }
            }
            else if (ev.keyCode == 27) //esc
            {
                treeNode.unselect();
            }
            else {
                broken = false;
            }
        }
        else {
            broken = false;
        }

        return !broken;
    }

    // 应用TreeView样式
    // width
    this.__parsePixel('width', '100%');
    this.div.style.width = this.width;
    // height
    this.__parsePixel('height', '100%')
    this.div.style.height = this.height;
    // theme
    this.theme = this.theme.toLowerCase();
    if (this.theme != 'vista' || this.theme != 'xp') this.theme = 'vista';
    // backColor
    if (this.backColor == null) this.backColor = '#FFFFFF';
    this.div.style.backgroundColor = this.backColor;
    // borderWidth
    if (this.borderWidth != null) {
        this.__parsePixel('borderWidth', '0px')
        this.div.style.borderWidth = this.borderWidth;
    }
    // borderStyle
    if (this.borderStyle != null) this.div.style.borderStyle = this.borderStyle;
    // borderColor
    if (this.borderColor != null) this.div.style.borderColor = this.borderColor;

    // 超过范围出现滚动条
    this.div.style.overflow = 'auto';

    /// nodeIndent
    this.__parsePixel('nodeIndent', '16px')
    /// nodePadding
    this.__parsePixel('nodePadding', '2px');
    /// nodeSpacing
    this.__parsePixel('nodeSpacing', '0px');
    /// childNodesPadding
    this.__parsePixel('childNodesPadding', '0px');

    // showImages
    this.__parseBoolean('showImages', true);
    // showCheckBoxes
    this.__parseBoolean('showCheckBoxes', false);
    // expandOnSelect
    this.__parseBoolean('expandOnSelect', false);
    // collapseOnSelect
    //this.__parseBoolean('collapseOnSelect', false);
    // showDebugNode
    this.__parseBoolean('showDebugNode', false);
    // allowNodeEditing
    this.__parseBoolean('allowNodeEditing', false);

    // expandDepth
    if (this.expandDepth == null) this.expandDepth = 1;
    if (typeof (this.expandDepth) != 'number') this.expandDepth = parseInt(this.expandDepth);
    if (isNaN(this.expandDepth)) this.expandDepth = 1;

    // selectedPath
    if (this.selectedPath == '') this.selectedPath = null;
    // checkedPaths
    if (this.checkedPaths == '') this.checkedPaths = null;

    //从配置获得节点
    this.__getChildNodes(this.div);

    //从xml加载数据
    if (this.xml != null && !this.loaded) {
        this.div.appendChild(this.__getLoadingNode());
        Xml.Request(this.xml, null, this, '__getChildNodesFromXml');
    }

    //保存到全局对象
    document.treeViews[this.id] = this;

    //onload事件
    if (this.xml == null && !this.loaded) {
        this.__initialize();
    }
};

TreeView.prototype.reload = function() {
    /// <summary>重新加载所有节点</summary>

    this.removeAll();

    this.loaded = false;
    this.load();
}

TreeView.prototype.expandTo = function(depth) {
    /// <summary>展开节点到指定的深度</summary>

    if (depth > 1) {
        if (this.firstChild != null) {
            this.firstChild.__expandTo(depth);
        }
    }
}

TreeView.prototype.selectNodeByPath = function(path, triEvent) {
    /// <summary>根据路径选择一个节点</summary>
    /// <param name="path" type="String">节点完整路径</param>
    /// <param name="triEvent" type="Boolean">是否触发onnodeselect事件</param>

    var names = path.split('.');
    var index = -1;
    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i].name == names[0]) {
            index = i;
            break;
        }
    }

    //是否查找完成
    var finished = true;

    //如果找到节点
    if (index > -1) {
        if (names.length > 1) {
            names.splice(0, 1);
            if (this.childNodes[index].loaded) {
                if (!this.childNodes[index].expanded) this.childNodes[index].expand();
                this.childNodes[index].__selectNodeByPath(names, triEvent);
            }
            else {
                this.childNodes[index].__onexpand = function() {
                    this.__selectNodeByPath(names, triEvent);
                }
                this.childNodes[index].expand();
            }
            finished = false;
        }
        else {
            //在这个方法里默认不触发事件
            if (triEvent != true) triEvent = false;
            this.childNodes[index].select(triEvent);
        }
    }

    if (finished) {
        if (this.__actions.selecting) {
            //表示选择已经停止	
            this.__actions.selecting = false;
            //检查是否可以完成load
            this.__complete();
        }
    }
}

TreeView.prototype.checkNodesByPaths = function(paths, triEvent) {
    /// <summary>根据paths集合选中项</summary>
    /// <param name="paths" type="Array" elementType="String">path数组, 格式['1.2', '1.3.4', ...]</param>
    /// <param name="triEvent" type="Boolean">是否触发onnodecheckedchanged事件</param>

    if (typeof (paths) == 'string') paths = paths.split(',');

    this.__checkNodeByPath(paths, paths[0], triEvent);
}

TreeView.prototype.getNodeByName = function(nodeName) {
    /// <summary>根据name获得节点, 要求节点已经被载入, 否则返回null</summary
    var element = document.getElementById('__node_' + nodeName);
    if (element != null) {
        var names = element.getAttribute('path').split('.');
        var rootNode = null;
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].name == names[0]) {
                rootNode = this.childNodes[i];
                break;
            }
        }
        if (rootNode != null && names.length > 1) {
            names.splice(0, 1);
            return rootNode.__getNodeByName(names);
        }
        else {
            return rootNode;
        }
    }
    else {
        return null;
    }
}

TreeView.prototype.getCheckedNodes = function() {
    /// <summary>得到所有被checked的节点</summary>

    if (this.showCheckBoxes) {
        this.__checkedNodes = new Array();
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].checked == 1) {
                this.__checkedNodes.push(this.childNodes[i]);
            }
            else if (this.childNodes[i].checked == 2) {
                this.childNodes[i].__getCheckedNodes();
            }
        }
        return this.__checkedNodes;
    }
    else {
        return null;
    }
}

TreeView.prototype.showMenu = function(menuName) {
    /// <summary>显示右键菜单</summary>
    if (this.__event.contextMenu != null) document.menus[menuName].show(this.__event.contextMenu);
}

TreeView.prototype.debug = function() {
    /// <summary>显示综合调试器</summary>
};

/*-----
支持 xml: 标记如
xml:/root/123.aspx?id=#name&name=#text 返回XML文件根节点文本
或事件
window.open('/root/123.aspx?id=#name&name=#text', '', 'width=200,height=100');
------*/

TreeView.prototype.onload = function() {
    /// <summary>加载完成后触发</summary>
};

TreeView.prototype.onnodeexpand = function(expandedNode) {
    /// <summary type="Function">节点展开后触发</summary>
    /// <param name="expandedNode" type="TreeNode">刚刚被展开的节点</summary>
};

TreeView.prototype.onnodecollapse = function(colapsedNode) {
    /// <summary type="Function">节点关闭后触发</summary>
    /// <param name="colapsedNode" type="TreeNode">刚刚被闭合的节点</summary>
};

TreeView.prototype.onnodeselect = function(selectedNode) {
    /// <summary type="Function">当选择节点改变后触发</summary>
    /// <param name="selectedNode" type="TreeNode">刚刚被选择的那个节点</summary>
};

TreeView.prototype.onnodecheckedchanged = function(node) {
    /// <summary type="Function">当某个节点选中状态改变后触发</summary>
    /// <param name="node" type="TreeNode">选中状态变化的那个节点</summary>
};

TreeView.prototype.onnodetextchanged = function(node) {
    /// <summary type="Funciton">节点文本更改后触发, 事件名称再定</summary>
    /// <param name="node" type="TreeNode">文本改变的那个节点</summary>
};

TreeView.prototype.oncontextmenu = function(node) {
    /// <summary type="Funciton">在节点上点击右键时触发</summary>	
    /// <param name="node" type="TreeNode">目标节点</summary>	
};

TreeView.prototype.onnodenavigate = function(node) {
    /// <summary type="Funciton">点击节点上的链接时触发, "javascript:"类不算链接</summary>	
    /// <param name="node" type="TreeNode">目标节点</summary>	
};

TreeView.prototype.__initialize = function() {
    /// <summary>在TreeView根节点载入后触发, 用来初始化 expandDepth, selectedPath, checkedPath属性</summary>

    // expandDepth & expandTo
    if (this.expandDepth > 1) {
        this.__actions.expanding = true;
        this.expandTo(this.expandDepth);
    }
    else {
        this.__actions.expanding = false;
    }

    // selectedPath & selectNodeByPath
    if (this.selectedPath != null) {
        this.__actions.selecting = true;
        this.selectNodeByPath(this.selectedPath, false);
    }
    else {
        this.__actions.selecting = false;
    }

    // checkedPaths & checkNodesByPaths
    if (this.checkedPaths != null) {
        this.__actions.checking = true;
        this.checkNodesByPaths(this.checkedPaths.split(','), false);
    }
    else {
        this.__actions.checking = false;
    }

    //检查是否能够完成加载
    this.__complete();
}

TreeView.prototype.__complete = function() {
    /// <summary>完成load</summary>
    if (this.__actions.expanding == false && this.__actions.selecting == false && this.__actions.checking == false) {
        this.loaded = true;
        this.__executeEvent('onload');
    }
}

TreeView.prototype.__getChildNodesFromXml = function(xmlRequest) {
    /// <summary>读取数据源后执行的函数</summary>

    //移除loading节点
    this.div.removeChild(this.div.lastChild);

    //获取子节点
    this.__getChildNodes(xmlRequest.responseXML.lastChild);

    //loaded
    if (!this.loaded) {
        this.__initialize();

        //this.loaded = true;
        //this.__executeEvent('onload');
    }
}

TreeView.prototype.__getChildNodes = function(element) {
    /// <summary>从element配置获取TreeView的根节点</summary>
    /// <param name="element" type="Element">从element的子节点获取根节点信息</param>
    var treeNodes = TreeView.__GetTreeNodes(element);
    for (var i = 0; i < treeNodes.length; i++) {
        this.appendChild(treeNodes[i]);
    }
}

TreeView.prototype.__getLoadingNode = function() {
    /// <summary>获得"正在载入..."节点</summary>

    var table, tbody, tr, td;

    table = document.createElement('TABLE');
    table.setAttribute('sign', 'loading');
    table.style.marginTop = this.nodeSpacing;
    table.style.marginBottom = this.nodeSpacing;
    table.cellPadding = this.nodePadding;
    tbody = document.createElement('TBODY');
    tr = document.createElement('TR');

    td = document.createElement('TD');
    td.innerHTML = '<img src="' + TreeView.$images + 'blank.gif" width="16" height="16" />';
    tr.appendChild(td);
    td = document.createElement('TD');
    td.innerHTML = '<img src="' + TreeView.$images + 'spinner.gif" width="16" height="16" />';
    tr.appendChild(td);
    td = document.createElement('TD');
    td.innerHTML = 'Loading...';
    tr.appendChild(td);

    tbody.appendChild(tr);
    table.appendChild(tbody);

    return table;
};

TreeView.prototype.__checkNodeByPath = function(paths, path, triEvent) {
    /// <summary>根据path选中项</summary>
    /// <param name="paths" type="Array" elementType="string">path数组</param>
    /// <param name="path" type="String">单个path</param>
    /// <param name="triEvent" type="Boolean">是否触发onnodecheckedchanged事件</param>

    var names = path.split('.');
    var index = -1;
    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i].name == names[0]) {
            index = i;
            break;
        }
    }

    //是否查找完成
    var finished = false;

    //如果找到节点
    if (index > -1) {
        if (names.length > 1) {
            names.splice(0, 1);
            if (this.childNodes[index].loaded) {
                this.childNodes[index].__checkNodeByPath(paths, names, triEvent);
            }
            else {
                this.childNodes[index].__onload = function() {
                    this.__checkNodeByPath(paths, names, triEvent);
                }
                this.childNodes[index].load();
            }
        }
        else {
            //在这个方法里默认不触发事件
            if (triEvent != true) triEvent = false;
            this.childNodes[index].check(triEvent);

            //查找下一个节点
            paths.splice(0, 1);
            if (paths.length > 0) {
                this.__checkNodeByPath(paths, paths[0], triEvent);
            }
            else {
                finished = true;
            }
        }
    }

    if (finished) {
        if (this.__actions.checking) {
            //表示选中已经停止	
            this.__actions.checking = false;
            //检查是否可以完成load
            this.__complete();
        }
    }
}

TreeView.prototype.__parseBoolean = function(propertyName, defaultValue) {
    /// <summary>计算布尔属性的值</summary>
    /// <param name="propertyName" type="String">属性名</param>
    /// <param name="defaultValue" type="Boolean">默认值</param>
    var propertyValue = this[propertyName];
    if (propertyValue == null) propertyValue = defaultValue;
    if (typeof (propertyValue) == 'string') {
        if (/^true$/i.test(propertyValue) || /^false$/i.test(propertyValue)) {
            propertyValue = propertyValue.toLowerCase();
        }
        propertyValue = eval(propertyValue);
    }
    if (propertyValue != true && propertyValue != false) propertyValue = defaultValue;
    this[propertyName] = propertyValue;
}

TreeView.prototype.__parsePixel = function(propertyName, defaultValue) {
    /// <summary>计算像素属性的值, 如宽高坐标等</summary>
    /// <param name="propertyName" type="String">属性名</param>
    /// <param name="defaultValue" type="Boolean">默认值</param

    var propertyValue = this[propertyName];
    if (propertyValue == null) propertyValue = defaultValue;
    if (typeof (propertyValue) == 'number') {
        propertyValue += 'px';
    }
    else if (typeof (propertyValue) == 'string') {
        if (/^\d+$/.test(propertyValue)) propertyValue += 'px';
        if (propertyValue == '') propertyValue = defaultValue;
    }
    this[propertyName] = propertyValue;
}

TreeView.prototype.__executeEvent = function(eventName, argument) {
    /// <summary>执行事件</summary>
    /// <param name="eventName" type="String">事件名</param>
    /// <param name="argument" type="object">事件参数</param>

    if (this[eventName] != null) {
        if (typeof (this[eventName]) == 'function') {
            this[eventName](argument);
        }
        else if (typeof (this[eventName]) == 'string') {
            var ev;
            eval('ev = function(){' + this[eventName] + '}');
            ev.call(this, argument);
        }
    }
}

/**********
	
TreeNode 树形节点
	
**********/

TreeNode = function(name, text, value, image, expandImage, title, href, target, xml) {
    /// <summary>构造函数</summary>

    /// <value type="String">节点的唯一标识, 传递节点引用</value>
    this.name = name;
    /// <value type="String">节点的文本</value>
    this.text = text;
    /// <value type="String">节点的值</value>
    this.value = value;
    /// <value type="String">节点的注释, 鼠标划过节点时显示</value>
    this.title = title || '';

    /// <value type="String">节点的默认图标</value>
    this.image = image;
    /// <value type="String">节点展开时的图标</value>
    this.expandImage = expandImage;

    /// <value type="String">鼠标点击节点时的链接路径</value>
    this.href = href;
    /// <value type="String">节点链接目标</value>
    this.target = target;

    /// <value type="String|Element">子节点数据源</value>
    this.xml = xml;

    /// <value type="Array" elementType="TreeNode">获取当前节点子节点的集合</value>
    this.childNodes = new Array();

    /// <value type="Array" elementType="String">属性数组, 用于初始化节点和getAttribute和setAttribute方法</value>
    this.attributes = new Array();
}

/// <value type="TreeView">指定TreeNode所属的TreeView</value>
TreeNode.prototype.treeView = null;

/// <value type="Element">显示TreeNode的HTML元素的引用(table), 用于节点更新</value>
TreeNode.prototype.element = null;
/// <value type="Element">显示TreeNode的子节点的HTML元素的引用(div), 用于子节点更新</value>
TreeNode.prototype.div = null;

/// <value type="Element">在子节点未加载时保存数据的元素, 加载完成后为null</value>
TreeNode.prototype.__dataElement = null;
/// <value type="Array" elementType="String">属性数组, 用于初始化节点和getAttribute和setAttribute方法</value>
//TreeNode.prototype.attributes = new Array();

/// <value type="Boolean">指示节点是否有子节点</value>
TreeNode.prototype.hasChildNodes = false;
/// <value type="Array" elementType="TreeNode">获取当前节点子节点的集合</value>
//TreeNode.prototype.childNodes = [];	
/// <value type="TreeNode">获取子节点中的第一个子节点</value>
TreeNode.prototype.firstChild = null;
/// <value type="TreeNode">获取子节点中的最后一个子节点</value>
TreeNode.prototype.lastChild = null;
/// <value type="TreeNode">获取上一个同辈节点</value>
TreeNode.prototype.previousSibling = null;
/// <value type="TreeNode">获取下一个同辈节点</value>
TreeNode.prototype.nextSibling = null;
/// <value type="TreeNode">获取当前节点的父节点, null 为一级节点</value>
TreeNode.prototype.parentNode = null;

/// <value type="String">从根TreeNode到当前节点的路径</value>
TreeNode.prototype.path = '';
/// <value type="Integer">获得节点的当前深度, 0为根节点</value>
TreeNode.prototype.depth = 0;

/// <value type="Boolean">判断节点是否正在加载</value>
TreeNode.prototype.loading = false;
/// <value type="Boolean">判断子节点是否已经加载完成</value>
TreeNode.prototype.loaded = false;
/// <value type="Boolean">判断节点是否正在展开</value>
TreeNode.prototype.expanding = false;
/// <value type="Boolean">判断节点是否处于展开状态</value>
TreeNode.prototype.expanded = false;
/// <value type="Boolean">判断节点是否处于选择状态</value>
TreeNode.prototype.selected = false;
/// <value type="Integer">判断节点是否处于选中状态 0 未选中 1 选中 2 不确定</value>
TreeNode.prototype.checked = 0;

TreeNode.prototype.populate = function() {
    /// <summary>装配节点</summary>

    //hasChildNodes
    if ((this.xml != null && this.xml != '') || this.childNodes.length > 0) this.hasChildNodes = true;

    var treeNode = this;

    var table, tbody, tr, td, img, a, div;

    table = document.createElement('TABLE');
    table.id = '__node_' + this.name;
    table.setAttribute('sign', 'node');
    table.setAttribute('path', this.path);
    table.style.marginTop = treeNode.treeView.nodeSpacing;
    table.style.marginBottom = treeNode.treeView.nodeSpacing;
    table.cellPadding = treeNode.treeView.nodePadding;
    tbody = document.createElement('TBODY');
    tr = document.createElement('TR');

    //+-
    td = document.createElement('TD');
    td.setAttribute('sign', 'burl');
    td.align = 'center';
    img = document.createElement('IMG');
    img.align = 'absmiddle';
    if (treeNode.hasChildNodes) {
        img.setAttribute('c', TreeView.$images + treeNode.treeView.theme + '/burl_0a.gif');
        img.setAttribute('e', TreeView.$images + treeNode.treeView.theme + '/burl_1a.gif');
        img.src = img.getAttribute('c');
        img.onmouseover = function() {
            this.src = this.src.replace('a.', 'b.');
        }
        img.onmouseout = function() {
            this.src = this.src.replace('b.', 'a.');
        }
        img.onclick = function(ev) {
            treeNode.treeView.__event.toggle = ev || window.event;
            treeNode.toggle();
        }
    }
    else {
        img.src = TreeView.$images + 'blank.gif';
    }
    img.width = 16;
    img.height = 16;
    td.appendChild(img);
    tr.appendChild(td);

    //checkbox
    if (treeNode.treeView.showCheckBoxes) {
        treeNode.checked = ((treeNode.parentNode != null && treeNode.parentNode.checked == 1) ? 1 : 0);

        td = document.createElement('TD');
        td.setAttribute('sign', 'checkbox');
        td.align = 'center';
        td.onmouseover = function() {
            this.firstChild.src = this.firstChild.src.replace('a.', 'b.');
        }
        td.onmouseout = function() {
            this.firstChild.src = this.firstChild.src.replace(/(b|c)\./i, 'a.');
        }
        td.onmousedown = function(ev) {
            ev = ev || window.event;
            if (ev.button == 1 || ev.button == 0) {
                this.firstChild.src = this.firstChild.src.replace('b.', 'c.');
            }
        }
        td.onmouseup = function(ev) {
            ev = ev || window.event;
            if (ev.button == 1 || ev.button == 0) {
                this.firstChild.src = this.firstChild.src.replace('c.', 'b.');

                if (treeNode.checked == 0) {
                    treeNode.check();
                }
                else {
                    treeNode.uncheck();
                }
            }
        }
        img = document.createElement('IMG');
        img.align = 'absmiddle';
        img.src = TreeView.$images + treeNode.treeView.theme + '/checkbox_' + treeNode.checked + 'a.gif';
        img.width = 16;
        img.height = 16;
        td.appendChild(img);
        tr.appendChild(td);
    }

    //image
    if (treeNode.treeView.showImages) {
        td = document.createElement('TD');
        td.align = 'center';
        td.setAttribute('sign', 'image');
        img = document.createElement('IMG');
        img.src = treeNode.image;
        img.width = 16;
        img.height = 16;
        td.appendChild(img);
        tr.appendChild(td);
    }

    //text
    td = document.createElement('TD');
    td.setAttribute('sign', 'text');
    td.style.cursor = 'default';
    td.style.whiteSpace = 'nowrap';
    td.title = treeNode.title;
    td.style.borderWidth = '1px';
    td.style.borderStyle = 'solid';
    td.style.borderColor = treeNode.treeView.backColor;
    td.onmouseover = function() {
        if (treeNode.treeView.selectedNode != treeNode) {
            this.style.borderColor = '#999999';
            this.style.backgroundColor = '#F8F8F8';
        }
    }
    td.onmouseout = function() {
        if (treeNode.treeView.selectedNode != treeNode) {
            this.style.borderColor = treeNode.treeView.backColor;
            this.style.backgroundColor = '';
        }
    }
    td.onmouseup = function(ev) {
        //选中
        if (treeNode.treeView.selectedNode != treeNode) {
            treeNode.select();
        }
    }
    td.oncontextmenu = function(ev) {
        treeNode.treeView.__event.contextMenu = ev || window.event;
        //显示右键菜单
        treeNode.treeView.__executeEvent('oncontextmenu', treeNode);
        treeNode.treeView.__event.contextMenu = null;
        return false;
    }
    td.ondblclick = function(ev) {
        if (treeNode.hasChildNodes) {
            treeNode.treeView.__event.toggle = ev || window.event;
            treeNode.toggle();
        }
    }
    a = document.createElement('A');
    if (treeNode.href != null && treeNode.href != '') a.href = treeNode.href;
    if (treeNode.target != null && treeNode.target != '') a.target = treeNode.target;
    a.innerHTML = treeNode.text;
    a.onclick = function() {
        //onnodenavigate事件
        if (this.href != '' && this.href.indexOf('javascript:') == -1) {
            treeNode.treeView.__executeEvent('onnodenavigate', treeNode);
        }
    }
    td.appendChild(a);
    tr.appendChild(td);

    tbody.appendChild(tr);
    table.appendChild(tbody);
    div = document.createElement('DIV');
    div.style.marginLeft = treeNode.treeView.nodeIndent;
    div.style.paddingTop = treeNode.treeView.childNodesPadding;
    div.style.paddingBottom = treeNode.treeView.childNodesPadding;
    div.style.display = 'none';

    treeNode.element = table;
    treeNode.div = div;
}

TreeNode.prototype.load = function() {
    /// <summary>加载子节点项</summary>
    this.loading = true;

    if (this.__dataElement != null) {
        //从配置获得节点
        this.__getChildNodes(this.__dataElement);
    }

    //从xml加载数据
    if (this.xml != null && !this.loaded) {
        // 如果div的子节点数为0或者不存在loading节点, 表明不是正在加载, 加载子节点
        if (this.div.childNodes.length == 0 || this.div.lastChild.getAttribute('sign') != 'loading') {
            this.div.appendChild(this.treeView.__getLoadingNode());

            //处理xml中的#name,#text,#value, 占位符区分大小写
            var url = this.xml;
            while (url.indexOf('#name') > -1) url = url.replace('#name', escape(this.name));
            while (url.indexOf('#text') > -1) url = url.replace('#text', escape(this.text));
            while (url.indexOf('#value') > -1) url = url.replace('#value', escape(this.value));
            this.xml = url;

            Xml.Request(this.xml, null, this, '__getChildNodesFromXml');
        }
    }

    //load
    if (this.xml == null && !this.loaded) {
        //当没有子节点时, 处理burl为blank
        if (this.childNodes.length == 0) {
            this.unburl();
            this.hadChildNodes = false;
        }

        this.loading = false;
        this.loaded = true;
        //执行TreeNode事件
        //if (this.treeView.__actions.checking)
        this.__executeEvent('__onload');

        //如果是展开动作引发的load, 处理expand事件
        if (this.expanding) {
            this.__completeExpanding();
        }
    }
};

TreeNode.prototype.reload = function() {
    /// <summary>重新加载</summary>
    this.removeAll();

    this.loaded = false;
    this.load();
}

TreeNode.prototype.toggle = function () {
    /// <summary>将节点切换为展开或闭合状态</summary>
    if (!this.expanded) {
        this.treeView.collapseAll();
        this.expand();
    }
    else {
        //闭合
        this.collapse();
    }
};

TreeNode.prototype.expand = function() {
    /// <summary>展开节点</summary>
    /// <param name="triEvent" type="Boolean">是否触发事件, 默认触发</param>

    if (this.treeView != null) {
        this.expanding = true;

        //+-
        var burl = this.element.rows[0].cells[0].firstChild;
        burl.src = burl.getAttribute('e');
        //image
        this.element.rows[0].cells[this.treeView.showCheckBoxes ? 2 : 1].firstChild.src = this.expandImage;
        //div
        this.div.style.display = '';

        //如果数据没有加载就加载		
        if (!this.loaded) {
            this.load();
        }
        else {
            this.__completeExpanding();
        }
    }
};

TreeNode.prototype.collapse = function() {
    /// <summary>闭合节点</summary>
    if (this.treeView != null) {
        //+-
        var burl = this.element.rows[0].cells[0].firstChild;
        burl.src = burl.getAttribute('c');
        //image
        this.element.rows[0].cells[this.treeView.showCheckBoxes ? 2 : 1].firstChild.src = this.image;
        //div
        this.div.style.display = 'none';

        this.expanded = false;
        this.treeView.__executeEvent('onnodecollapse', this);
    }
};

TreeNode.prototype.unselect = function() {
    /// <summary>取消选择当前节点</summary>
    if (this.treeView != null) {
        if (this.selected) {
            var cells;
            //恢复样式
            cells = this.element.rows[0].cells;
            cells[cells.length - 1].style.borderColor = this.treeView.backColor;
            cells[cells.length - 1].style.backgroundColor = '';
            this.selected = false;

            this.treeView.selectedNode = null;
        }
    }
};

TreeNode.prototype.select = function(triEvent) {
    /// <summary>选择当前节点</summary>
    /// <param name="triEvent" type="Boolean">是否触发事件, 默认触发</param>

    if (this.treeView != null) {
        if (this.treeView.selectedNode != null && this != this.treeView.selectedNode) {
            this.treeView.selectedNode.unselect();
        }

        // 改变样式
        var cells = this.element.rows[0].cells;
        cells[cells.length - 1].style.borderColor = '#999999';
        cells[cells.length - 1].style.backgroundColor = '#FFDDAA';

        this.selected = true;

        // selectedNode
        this.treeView.selectedNode = this;
        // onnodeselect
        if (triEvent != false) this.treeView.__executeEvent('onnodeselect', this);

        // expandOnSelect
        if (this.hasChildNodes && this.treeView.expandOnSelect && !this.expanded) {
            this.treeView.collapseAll();
            this.expand();
        }
        // collapseOnSelect
        //if (this.hasChildNodes && this.collapseOnSelect && this.expanded)
        //{
        //	this.collapse();	
        //}
    }
};

TreeNode.prototype.burl = function() {
    /// <summary>恢复节点的+-</summary>

    // 给当前节点添加burl
    var img = this.element.rows[0].cells[0].firstChild;
    img.setAttribute('c', TreeView.$images + this.treeView.theme + '/burl_0a.gif');
    img.setAttribute('e', TreeView.$images + this.treeView.theme + '/burl_1a.gif');
    img.src = img.getAttribute(this.expanded ? 'e' : 'c');
    img.onmouseover = function() {
        this.src = this.src.replace('a.', 'b.');
    }
    img.onmouseout = function() {
        this.src = this.src.replace('b.', 'a.');
    }
    var treeNode = this;
    img.onclick = function(ev) {
        treeNode.treeView.__event.toggle = ev || window.event;
        treeNode.toggle();
    }
}

TreeNode.prototype.unburl = function() {
    /// <summary>去掉节点的+-</summary>
    // 去掉当前节点的burl
    var img = this.element.rows[0].cells[0].firstChild;
    img.src = TreeView.$images + 'blank.gif';
    img.onmouseover = null;
    img.onmouseout = null;
    img.onclick = null;
    // 恢复节点图标
    var image = this.element.rows[0].cells[this.element.rows[0].cells.length - 2].firstChild;
    image.src = this.image;
    // 恢复展开默认值
    this.expanded = false;
};

TreeNode.prototype.check = function(triEvent) {
    /// <summary>选中当前节点</summary>
    /// <param name="triEvent" type="Boolean">是否触发事件, 默认触发</param>

    this.__toggleCheckBox(1);

    //检查子项
    this.__traverseChildNodes();

    //检查父级项
    this.__traverseParentNodes();

    //执行事件
    if (triEvent != false) this.treeView.__executeEvent('onnodecheckedchanged', this);
};

TreeNode.prototype.uncheck = function(triEvent) {
    /// <summary>取消选中当前节点</summary>
    this.__toggleCheckBox(0);

    //检查子项数
    this.__traverseChildNodes();

    //检查父级项 - 需要递归函数
    this.__traverseParentNodes();

    //执行事件
    if (triEvent != false) this.treeView.__executeEvent('onnodecheckedchanged', this);
}

TreeNode.prototype.getAttribute = function(attributeName) {
    /// <summary>得到自定义的属性值</summary>

    return this.attributes[attributeName.toLowerCase()];
}

TreeNode.prototype.setAttribute = function(attributeName, attributeValue) {
    /// <summary>设置自定义的属性值</summary>

    this.attributes[attributeName.toLowerCase()] = attributeValue;
}

TreeNode.prototype.appendChild = function(treeNode) {
    /// <summary>添加子节点</summary>

    //name
    if (treeNode.name == null || treeNode.name == '') {
        treeNode.name = this.name + '_' + this.childNodes.length;
    }

    //parentNode
    treeNode.parentNode = this;
    //treeView
    treeNode.treeView = this.treeView;
    //depth
    treeNode.depth = this.depth + 1;
    //path
    treeNode.path = this.path + '.' + treeNode.name;

    var length = this.childNodes.length;

    // childNodes
    this.childNodes.push(treeNode);

    // render
    this.childNodes[length].populate();
    this.div.appendChild(this.childNodes[length].element);
    this.div.appendChild(this.childNodes[length].div);


    this.childNodes[length].element.onclick = function() {
        SetTabPage(treeNode.name, treeNode.text, treeNode.value);
    }

    // firstChild & lastChild
    if (length == 0) this.firstChild = this.childNodes[length];
    this.lastChild = this.childNodes[length];

    //previousSibling & nextSibling
    if (length > 0) {
        this.childNodes[length].previousSibling = this.childNodes[length - 1];
        this.childNodes[length - 1].nextSibling = this.childNodes[length];
    }
    // 如果是第一个节点, 处理burl为+-
    if (length == 0 && !this.hasChildNodes) {
        this.burl();
        this.hasChildNodes = true;
        this.loaded = true;
        this.expand(false);
    }
    //选择/选中
    //if (treeNode.selected) treeNode.select();
    //if (treeNode.checked) treeNode.check();
    //if (this.expanded) treeNode.expand();
};

TreeNode.prototype.insertBefore = function(treeNode, referenceNode) {
    /// <summary>在referenceNode之前插入节点</summary>
    /// <param name="treeNode" type="TreeNode">要添加的节点</param>
    /// <param name="referenceNode" type="TreeNode">参考节点</param>

    //name
    if (treeNode.name == null || treeNode.name == '') {
        treeNode.name = this.id + '_' + this.childNodes.length;
    }

    //parentNode
    treeNode.parentNode = this;
    //treeView
    treeNode.treeView = this.treeView;
    //depth
    treeNode.depth = this.depth + 1;
    //path
    treeNode.path = this.path + '.' + treeNode.name;

    //index
    var index = -1;
    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i] == referenceNode) {
            index = i;
            break;
        }
    }

    if (index > -1) {
        // childNodes
        this.childNodes.splice(index, 0, treeNode);

        // render	
        this.childNodes[index].populate();
        this.div.insertBefore(this.childNodes[index].element, referenceNode.element);
        this.div.insertBefore(this.childNodes[index].div, referenceNode.element);

        // firstChild
        if (index == 0) this.firstChild = this.childNodes[0];

        //previousSibling & nextSibling
        var previousNode = referenceNode.previousSibling;
        this.childNodes[index].previousSibling = previousNode;
        if (previousNode != null) previousNode.nextSibling = this.childNodes[i];
        this.childNodes[index].nextSibling = referenceNode;
        referenceNode.previousSibling = this.childNodes[index];
    }
    else {
        //如果找不到参考节点, 直接附加
        this.appendChild(treeNode);
    }
};

TreeNode.prototype.removeChild = function(treeNode) {
    /// <summary>删除子节点</summary>

    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i] == treeNode) {
            this.div.removeChild(treeNode.element);
            this.div.removeChild(treeNode.div);

            //checked
            if (this.treeView.showCheckBoxes) {
                //只有当前节点的选中状态为2时才影响选中状态的改变
                if (this.checked == 2) {
                    var checked = 0;
                    //如果除了treeNode全选中, 则treeNode选中, 如果除了treeNode全未选中, 则treeNode未选中, 
                    //treeNode.checked = 0;
                    treeNode.__traverseParentNodes();
                }
            }

            this.childNodes.splice(i, 1);

            //checked
            if (this.treeView.showCheckBoxes) {
                if (this.checked == 2) {
                    if (this.childNodes.length > 0) {
                        this.childNodes[0].__traverseParentNodes();
                    }
                }
            }

            // firstChild, lastChild, nextSibling, previousSibling
            if (this.childNodes.length == 0) {
                //一项不剩下
                this.firstChild = null;
                this.lastChild = null;
            }
            else if (this.childNodes.length == 1) {
                //只剩下一项
                this.firstChild = this.childNodes[0];
                this.lastChild = this.childNodes[0];
                this.childNodes[0].previousSibling = null;
                this.childNodes[0].nextSibling = null;
            }
            else {
                if (i == 0) {
                    //是第一项
                    this.firstChild = this.childNodes[0];
                    this.childNodes[0].previousSibling = null;
                }
                else if (i == this.childNodes.length) {
                    //是最后一项
                    this.lastChild = this.childNodes[i - 1];
                    this.childNodes[i - 1].nextSibling = null;
                }
                else {
                    //在中间
                    this.childNodes[i].previousSibling = this.childNodes[i - 1];
                    this.childNodes[i - 1].nextSibling = this.childNodes[i];
                }
            }

            break;
        }
    }
    //+-
    if (this.childNodes.length == 0) this.unburl();
};

TreeNode.prototype.removeAll = function() {
    /// <summary>删除所有子节点</summary>

    this.childNodes.length = 0;
    this.div.innerHTML = '';
    this.div.style.display = 'none';
    this.unburl();

    //firstChild & lastChild
    this.firstChild = null;
    this.lastChild = null;

    //checkbox
    if (this.treeView.showCheckBoxes) {
        if (this.checked == 2) {
            this.uncheck(false);
        }
    }
};

TreeNode.prototype.edit = function() {
    /// <summary>编辑子节点</summary>
};

TreeNode.prototype.update = function() {
    /// <summary>根据节点配置更新TreeNode节点的显示</summary>
    //name, text, value, image, expandImage, title, href, target, xml

    if (this.treeView != null) {
        var cells = this.element.rows[0].cells;
        for (var i = 0; i < cells.length; i++) {
            switch (cells[i].getAttribute('sign')) {
                case 'image':
                    cells[i].firstChild.src = (this.expanded ? this.expandImage : this.image);
                    break;
                case 'text':
                    cells[i].title = this.title;
                    if (this.href != null && this.href != '') cells[i].firstChild.href = this.href;
                    if (this.target != null && this.target != '') cells[i].firstChild.target = this.target;
                    cells[i].firstChild.innerHTML = this.text;
                    break;
            }
        }
    }
};

TreeNode.prototype.__completeExpanding = function(triEvent) {
    /// <summary>完成展开</summary>
    /// <param name="triEvent" type="Boolean">是否触发事件, 默认触发</param>

    this.expanding = false;
    this.expanded = true;

    //如果行为来自用户, 执行TreeView事件
    if (this.treeView.__event.toggle != null) {
        this.treeView.__executeEvent('onnodeexpand', this);
        this.treeView.__event.toggle = null;
    }
    //执行TreeNode事件
    //if (this.treeView.__actinos.expanding)
    this.__executeEvent('__onrender');
    //if (this.treeView.__actinos.selecting)
    this.__executeEvent('__onexpand');
};

TreeNode.prototype.__toggleCheckBox = function(checkedState) {
    /// <summary>切换checkbox状态和checked</summary>
    /// <param name="checkedState" valueType="Integer">要切换到的状态</param>

    var checkbox = this.element.rows[0].cells[1].firstChild;
    switch (checkedState) {
        case 0:
            checkbox.src = checkbox.src.replace(/_(1|2)/, '_0');
            break;
        case 1:
            checkbox.src = checkbox.src.replace(/_(0|2)/, '_1');
            break;
        case 2:
            checkbox.src = checkbox.src.replace(/_(0|1)/, '_2');
            break;
    }
    this.checked = checkedState;
};

TreeNode.prototype.__traverseChildNodes = function() {
    /// <summary>遍历子节点, 让子节点的选中状态和父节点一致, 适用于当前节点选中状态为0和1的时候</summary>

    if (this.loaded && this.hasChildNodes) {
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].checked != this.checked) {
                this.childNodes[i].__toggleCheckBox(this.checked);
                this.childNodes[i].__traverseChildNodes();
            }
        }
    }
};

TreeNode.prototype.__traverseParentNodes = function() {
    /// <summary>遍历父节点, 改变父级节点的选中状态</summary>
    if (this.parentNode != null) {
        var checkState = this.checked;
        for (var i = 0; i < this.parentNode.childNodes.length; i++) {
            if (this.parentNode.childNodes[i].checked != checkState) {
                checkState = 2;
                break;
            }
        }
        if (this.parentNode.checked != checkState) {
            this.parentNode.__toggleCheckBox(checkState);
        }

        this.parentNode.__traverseParentNodes();
    }
};

TreeNode.prototype.__getCheckedNodes = function() {
    /// <summary>获得当前节点子节点被checked的项, 被自己调用或在TreeView.getCheckedNodes中调用</summary>
    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i].checked == 1) {
            this.treeView.__checkedNodes.push(this.childNodes[i]);
        }
        else if (this.childNodes[i].checked == 2) {
            this.childNodes[i].__getCheckedNodes();
        }
    }
};

TreeNode.prototype.__getNodeByName = function(names) {
    /// <summary>根据name获得节点</summary
    var node = null;
    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i].name == names[0]) {
            node = this.childNodes[i];
            break;
        }
    }

    if (node != null && names.length > 1) {
        names.splice(0, 1);
        return node.__getNodeByName(names);
    }
    else {
        return node;
    }
};

TreeNode.prototype.__expandAll = function() {
    /// <summary>展开所有子节点, 在TreeView.expandAll中使用</summary>
    if (this.hasChildNodes) {
        if (this.expanded) {
            this.firstChild.__expandAll();
        }
        else {
            this.__onrender = function() {
                if (this.firstChild != null) {
                    this.firstChild.__expandAll();
                }
            }
            this.expand();
        }
    }
    else {

        //查找下一个节点, 如果没有下一个相临节点, 就查找父级节点
        var node = this;
        while (node.nextSibling == null) {
            if (node.parentNode == null) {
                node = null;
                break;
            }
            else {
                node = node.parentNode;
            }
        }
        if (node != null) {
            node = node.nextSibling;
            node.__expandAll();
        }
    }
};

TreeNode.prototype.__collapseAll = function() {
    /// <summary>闭合所有子节点, 在TreeView.collapseAll中使用</summary>
    if (this.hasChildNodes) {
        if (this.loaded) {
            this.firstChild.__collapseAll();
        }
        if (this.expanded) {
            this.collapse();
        }
    }
    else {
        //查找下一个节点, 如果没有下一个相临节点, 就查找父级节点
        var node = this;
        while (node.nextSibling == null) {
            if (node.parentNode == null) {
                node = null;
                break;
            }
            else {
                node = node.parentNode;
            }
        }
        if (node != null) {
            node = node.nextSibling;
            node.__collapseAll();
        }
    }
};

TreeNode.prototype.__loadAll = function() {
    /// <summary>展开所有子节点, 在TreeView.loadAll中使用</summary>
    if (this.hasChildNodes) {
        if (this.loaded) {
            this.firstChild.__loadAll();
        }
        else {
            this.__onload = function() {
                if (this.firstChild != null) {
                    this.firstChild.__loadAll();
                }
            }
            this.load();
        }
    }
    else {

        //查找下一个节点, 如果没有下一个相临节点, 就查找父级节点
        var node = this;
        while (node.nextSibling == null) {
            if (node.parentNode == null) {
                node = null;
                break;
            }
            else {
                node = node.parentNode;
            }
        }
        if (node != null) {
            node = node.nextSibling;
            node.__loadAll();
        }
    }
};

TreeNode.prototype.__expandTo = function(depth) {
    /// <summary>依次展开节点到指定的深度, 在TreeView.expandTo方法中被调用</summary>

    if (depth > this.depth + 1 && this.hasChildNodes) {
        if (this.expanded) {
            this.firstChild.__expandTo(depth);
        }
        else {
            this.__onrender = function() {
                if (this.firstChild != null) {
                    this.firstChild.__expandTo(depth);
                }
            }
            this.expand();
        }
    }
    else {
        //查找下一个节点, 如果没有下一个相临节点, 就查找父级节点
        var node = this;
        while (node.nextSibling == null) {
            if (node.parentNode == null) {
                node = null;
                break;
            }
            else {
                node = node.parentNode;
            }
        }
        if (node != null) {
            node = node.nextSibling;
            node.__expandTo(depth);
        }
        else {
            if (this.treeView.__actions.expanding) {
                //表示展开已经停止	
                this.treeView.__actions.expanding = false;
                //检查是否可以完成load
                this.treeView.__complete();
            }
        }
    }
};

TreeNode.prototype.__selectNodeByPath = function(names, triEvent) {
    /// <summary>根据路径选择一个节点, 在TreeView.selectNodeByPath中被调用</summary>	
    /// <param name="names" type="String">节点name数组</param>
    /// <param name="triEvent" type="Boolean">是否触发onnodeselect事件</param

    var index = -1;
    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i].name == names[0]) {
            index = i;
            break;
        }
    }

    //是否查找完成
    var finished = true;

    //如果找到节点
    if (index > -1) {
        if (names.length > 1) {
            names.splice(0, 1);
            if (this.childNodes[index].loaded) {
                if (!this.childNodes[index].expanded) this.childNodes[index].expand();
                this.childNodes[index].__selectNodeByPath(names, triEvent);
            }
            else {
                this.childNodes[index].__onexpand = function() {
                    this.__selectNodeByPath(names, triEvent);
                }
                this.childNodes[index].expand();
            }
            finished = false;
        }
        else {
            //在这个方法里默认不触发事件
            if (triEvent != true) triEvent = false;
            this.childNodes[index].select(triEvent);
        }
    }

    if (finished) {
        if (this.treeView.__actions.selecting) {
            //表示展开已经停止	
            this.treeView.__actions.selecting = false;
            //检查是否可以完成load
            this.treeView.__complete();
        }
    }
};

TreeNode.prototype.__checkNodeByPath = function(paths, names, triEvent) {
    /// <summary>根据路径选中一个节点, 在TreeView.__checkNodeByPath中被调用</summary>
    /// <param name="paths" type="Array" elementType="String">节点path数组</param>
    /// <param name="names" type="String">节点name数组</param>
    /// <param name="triEvent" type="Boolean">是否触发onnodeselect事件</param

    var index = -1;
    for (var i = 0; i < this.childNodes.length; i++) {
        if (this.childNodes[i].name == names[0]) {
            index = i;
            break;
        }
    }

    //是否查找完成
    var finished = false;

    //如果找到节点
    if (index > -1) {
        if (names.length > 1) {
            names.splice(0, 1);
            if (this.childNodes[index].loaded) {
                this.childNodes[index].__checkNodeByPath(paths, names, triEvent);
            }
            else {
                this.childNodes[index].__onload = function() {
                    this.__checkNodeByPath(paths, names, triEvent);
                }
                this.childNodes[index].load();
            }
        }
        else {
            //在这个方法里默认不触发事件
            if (triEvent != true) triEvent = false;
            this.childNodes[index].check(triEvent);

            //查找下一个节点
            paths.splice(0, 1);
            if (paths.length > 0) {
                this.treeView.__checkNodeByPath(paths, paths[0], triEvent);
            }
            else {
                //查找完成
                finished = true;
            }
        }
    }

    if (finished) {
        if (this.treeView.__actions.checking) {
            //表示命中已经停止	
            this.treeView.__actions.checking = false;
            //检查是否可以完成load
            this.treeView.__complete();
        }
    }
};

TreeNode.prototype.__getChildNodesFromXml = function(xmlRequest) {
    /// <summary>读取数据源后执行的函数</summary>

    //移除loading节点	
    if (this.div.lastChild.getAttribute('sign') == 'loading') this.div.removeChild(this.div.lastChild);

    //获取子节点
    this.__getChildNodes(xmlRequest.responseXML.lastChild);

    //loaded
    if (!this.loaded) {
        //当没有子节点时, 处理burl为blank
        if (this.childNodes.length == 0) {
            this.unburl();
            this.hasChildNodes = false;
        }

        this.loading = false;
        this.loaded = true;

        //执行__onload事件
        this.__executeEvent('__onload');

        //如果是expand引发的load
        if (this.expanding) {
            this.__completeExpanding();
        }
    }
};

TreeNode.prototype.__getChildNodes = function(element) {
    /// <summary>获取TreeNode子节点</summary>
    /// <param name="element" type="Element">从element的子节点获取节点信息</param>
    var treeNodes = TreeView.__GetTreeNodes(element);
    for (var i = 0; i < treeNodes.length; i++) {
        this.appendChild(treeNodes[i]);
    }
};

TreeNode.prototype.__executeEvent = function(eventName, argument) {
    /// <summary>执行事件</summary>
    /// <param name="eventName" type="String">事件名</param>
    /// <param name="argument" type="object">事件参数</param>

    if (this[eventName] != null) {
        if (typeof (this[eventName]) == 'function') {
            this[eventName](argument);
        }
        else if (typeof (this[eventName]) == 'string') {
            var ev;
            eval('ev = function(){' + this[eventName] + '}');
            ev.call(this, argument);
        }

        //如果是私有事件, 执行一次后清除
        if (eventName.indexOf('__') == 0) {
            this[eventName] = null;
        }
    }
};

TreeNode.prototype.__onrender = function() {
    /// <summary>TreeNode呈现后执行, 在expandTo方法中使用</summary>	
};

TreeNode.prototype.__onexpand = function() {
    /// <summary>TreeNode展开后执行, 在selectNodeByPath方法中使用</summary>	
};

TreeNode.prototype.__onload = function() {
    /// <summary>TreeNode载入完成后执行, 在checkNodesByPaths方法中使用</summary>	
};
/**********
	
全局方法
	
**********/

TreeView.__GetTreeNodes = function(element) {
    /// <summary>从元素得到TreeNodes</summary>

    var treeNodes = [];
    var treeNode;

    var nodes = element.childNodes;
    var property, propertyValue;
    var images; //image & expandImage	
    for (var i = 0; i < nodes.length; i++) {
        //SPAN标记将被解释为TreeNode
        if (nodes[i].nodeType == 1 && (nodes[i].nodeName == 'SPAN' || nodes[i].nodeName.toUpperCase() == 'TREENODE')) {
            treeNode = new TreeNode();

            //遍历节点的属性, 将[属性名-属性值]保存到node
            for (var j = 0; j < nodes[i].attributes.length; j++) {
                treeNode.attributes[nodes[i].attributes[j].name.toLowerCase()] = nodes[i].attributes[j].value;
            }

            //ownProperties			
            for (property in treeNode) {
                if (treeNode.hasOwnProperty(property) && !(treeNode[property] instanceof Array)) {
                    propertyValue = treeNode.attributes[property.toLowerCase()];
                    if (propertyValue != null) {
                        treeNode[property] = propertyValue;
                    }
                }
            }
            //xml
            if (treeNode.xml == '') treeNode.xml = null;
            //image|expandImage
            if (treeNode.image != null && treeNode.expandImage == null) {
                images = treeNode.image.split('|');
                images[0] = images[0].replace('$root', TreeView.$root);
                images[0] = images[0].replace('$images', TreeView.$images);
                treeNode.image = images[0];
                if (images.length > 1) {
                    images[1] = images[1].replace('$root', TreeView.$root);
                    images[1] = images[1].replace('$images', TreeView.$images);
                    treeNode.expandImage = images[1];
                }
                else {
                    treeNode.expandImage = images[0];
                }
            }
            //childNodes
            if (nodes[i].childNodes.length > 0) {
                treeNode.__dataElement = nodes[i];
            }
            //hasChildNodes
            if (treeNode.__dataElement != null || treeNode.xml != null) treeNode.hasChildNodes = true;

            treeNodes.push(treeNode);
        }
    }
    return treeNodes;
};

TreeView.__LoadModule = function(moduleName) {
    /// <summary>加载TreeView模块</summary>

    var script = document.createElement('SCRIPT');
    script.type = 'text/javascript';
    script.src = TreeView.$root + 'Root.TreeView.' + moduleName + '.js';
    document.body.previousSibling.appendChild(script);
}

function __InitializeTreeViews() {
    /// <summary>初始化document.treeViews并呈现页面上所有TreeView</summary>

    // 替换页面上所有的TreeView标签
    var treeViews = document.getElementsByTagName('TreeView');

    // id
    var ids = new Array(treeViews.length);
    for (var i = 0; i < treeViews.length; i++) {
        if (treeViews[i].id == '') treeViews[i].id = '__TreeView_' + i;
        ids[i] = treeViews[i].id;
    }
    var parentNode, html;
    for (var i = 0; i < treeViews.length; i++) {
        parentNode = treeViews[i].parentNode;
        html = parentNode.innerHTML;
        html = html.replace(/<treeview/i, '<div');
        html = html.replace(/<\/treeview>/i, '</div>');
        while (/<treenode/i.test(html)) {
            html = html.replace(/<treenode/i, '<span');
        }
        while (/<\/treenode>/i.test(html)) {
            html = html.replace(/<\/treenode>/i, '</span>');
        }
        parentNode.innerHTML = html;
    }

    var treeView;
    var property, propertyValue;
    //遍历可获取的属性, 将属性读入TreeView对象
    for (var i = 0; i < ids.length; i++) {
        treeView = new TreeView(ids[i]);
        treeView.div = document.getElementById(ids[i]);
        for (property in treeView) {
            if ((property != 'childNodes' && treeView.hasOwnProperty(property)) || property.indexOf('on') == 0) {
                propertyValue = treeView.div.getAttribute(property);
                if (propertyValue != null) {
                    treeView[property] = propertyValue;
                }
            }
        }
        if (treeView.xml == '') treeView.xml = null;
        treeView.load();
    }
}
function SetTabPage(id, pageTitle, pageHref) {
    var main = window.parent.frames["main"];
    if (main && main.SetPage) {
        main.SetPage(id, pageTitle, pageHref, true);
    }
}
__InitializeTreeViews();