<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeliveryPriceChoose.aspx.cs" Inherits="RFD.FMS.WEB.COD.DeliveryPriceChoose" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>价格维护</title>
    <script src="../Scripts/base.js" type="text/javascript" charset="gb2312"></script>
    <script src="../Scripts/import/common.js" type="text/javascript"></script>
    <script type="text/javascript">

        $(function () {
            var emsArr = new Array();
            emsArr[emsArr.length] = '((<select id="ems5" onblur="fnShowPriceResult()"><option value="不取整">不取整</option><option value="向上取整">向上取整</option><option value="取整">向下取整</option></select>';
            emsArr[emsArr.length] = '(重量/<input type="text" id="ems0" size=6 value="0.50001" onblur="fnShowPriceResult()" />';
            emsArr[emsArr.length] = ')';
            emsArr[emsArr.length] = '*<input type="text" id="ems1" size=4 onblur="fnShowPriceResult()" />';
            emsArr[emsArr.length] = '+<input type="text" id="ems2" size=4 onblur="fnShowPriceResult()" />)';
            emsArr[emsArr.length] = '*<input type="text" id="ems3" size=4 onblur="fnShowPriceResult()" />)';
            emsArr[emsArr.length] = '+<input type="text" id="ems4" size=4 onblur="fnShowPriceResult()" />';
            $("#txtEMS").html(emsArr.join(''));
        })

        $(function () {
            var zjsArr = new Array();
            zjsArr[zjsArr.length] = '<select id="zjs4" onblur="fnShowPriceResult()"><option value="不取整">不取整</option><option value="向上取整">向上取整</option><option value="取整">向下取整</option></select>'
            zjsArr[zjsArr.length] = '(负数取零(重量-<input type="text" id="zjs1" size=4 onblur="fnShowPriceResult()" />)/';
            zjsArr[zjsArr.length] = '<input type="text" id="zjs0" size=6 value="1" onblur="fnShowPriceResult()" />))';
            zjsArr[zjsArr.length] = '*<input type="text" id="zjs2" size=4 onblur="fnShowPriceResult()" />';
            zjsArr[zjsArr.length] = '+<input type="text" id="zjs3" size=4 onblur="fnShowPriceResult()" />';
            $("#txtZJS").html(zjsArr.join(''));
        })

    	$(function() {
    		fnShowChoose(document.getElementById("rbPrice"));
    	})

        function fnOK() {
            //if (!fnJudgeInput()) return;
    		if (!fnJudgePrice()) return;
    		window.returnValue = $("#priceResult").val();
    		window.close();
    	}

    	function fnJudgePrice() {
    		var rb = document.getElementsByName("rbChoose");
    		if (rb[0].checked) {
    			if (fnVerify("price")) return true;
    		}
    		if (rb[1].checked) {
    			if (fnVerify("ems1") &&
    				fnVerify("ems2") &&
    				fnVerify("ems3") &&
                    fnVerify("ems4")) return true;
    		}

    		if (rb[2].checked) {
    			if (fnVerify("zjs1") &&
    				fnVerify("zjs2") &&
    				fnVerify("zjs3")) return true;
    		}
    	}

    	function fnVerify(id) {
    		var str = '输入金额不规范';
    		return isMoney(document.getElementById(id), str);
        }

        function isEmpty(theValue, strMsg) {
            //alert(theValue);
            if (theValue == "") {
                alert("不能存在空输入!");
                return false;
            }
            return true;
        }

    	function isMoney(pObj, errMsg) {
    		var obj = eval(pObj);
    		strRef = "1234567890.";
    		if (!isEmpty(obj.value, errMsg)) return false;
    		for (i = 0; i < obj.value.length; i++) {
    			tempChar = obj.value.substring(i, i + 1);
    			if (strRef.indexOf(tempChar, 0) == -1) {
    				if (errMsg == null || errMsg == "")
    					alert("数据不符合要求,请检查");
    				else
    					alert(errMsg);
    				if (obj.type == "text")
    					obj.focus();
    				return false;
    			} else {
    				tempLen = obj.value.indexOf(".");
    				if (tempLen != -1) {
    					strLen = obj.value.substring(tempLen + 1, obj.value.length);
    					if (strLen.length > 2) {
    						if (errMsg == null || errMsg == "")
    							alert("数据不符合要求,请检查");
    						else
    							alert(errMsg);
    						if (obj.type == "text")
    							obj.focus();
    						return false;
    					}
    				}
    			}
    		}
    		return true;
    	}

    	function fnShowPriceResult() {
    		var rb = document.getElementsByName("rbChoose");
    		var r = "";
    		if (rb[0].checked) {
    			r = $("#price").val();
    		}

    		if (rb[1].checked) {
    		    r = "((" + $("#ems5").val() + "(重量/" + $("#ems0").val() + ")*" + $("#ems1").val() + "+" + $("#ems2").val() + ")*" + $("#ems3").val() + ")+" + $("#ems4").val();
    		}

    		if (rb[2].checked) {
    		    r = $("#zjs4").val()+"(负数取零(重量-" + $("#zjs1").val() + ")/" + $("#zjs0").val() + ")*" + $("#zjs2").val() + "+" + $("#zjs3").val();
    		}
    		$("#priceResult").val(r)
    	}

    	function fnShowChoose(E) {
    		switch (E.id) {
    			case "rbPrice":
    				$("#txtEMS").css("color", "#cccccc");
    				$("#txtZJS").css("color", "#cccccc");
    				$("#price").removeAttr("disabled");
    				$("#ems0").attr("disabled", "disabled");
    				$("#ems1").attr("disabled","disabled");
    				$("#ems2").attr("disabled", "disabled");
    				$("#ems3").attr("disabled", "disabled");
    				$("#ems4").attr("disabled", "disabled");
    				$("#ems5").attr("disabled", "disabled");
    				$("#zjs0").attr("disabled", "disabled");
    				$("#zjs1").attr("disabled", "disabled");
    				$("#zjs2").attr("disabled", "disabled");
    				$("#zjs3").attr("disabled", "disabled");
    				$("#zjs4").attr("disabled", "disabled");
    				break;
    			case "rbEMS":
    				$("#txtZJS").css("color", "#cccccc");
    				$("#txtEMS").css("color", "#000000");
    				$("#price").attr("disabled", "disabled");
    				$("#zjs0").attr("disabled", "disabled");
    				$("#zjs1").attr("disabled", "disabled");
    				$("#zjs2").attr("disabled", "disabled");
    				$("#zjs3").attr("disabled", "disabled");
    				$("#zjs4").attr("disabled", "disabled");
    				$("#ems0").removeAttr("disabled");
    				$("#ems1").removeAttr("disabled");
    				$("#ems2").removeAttr("disabled");
    				$("#ems3").removeAttr("disabled");
    				$("#ems4").removeAttr("disabled");
    				$("#ems5").removeAttr("disabled"); 
    				break;
                case "rbZJS":
                    $("#txtZJS").css("color", "#000000");
                    $("#txtEMS").css("color", "#cccccc");
                    $("#price").attr("disabled", "disabled");
                    $("#ems0").attr("disabled", "disabled");
                    $("#ems1").attr("disabled", "disabled");
                    $("#ems2").attr("disabled", "disabled");
                    $("#ems3").attr("disabled", "disabled");
                    $("#ems4").attr("disabled", "disabled");
                    $("#ems5").attr("disabled", "disabled");
                    $("#zjs0").removeAttr("disabled");
                    $("#zjs1").removeAttr("disabled");
                    $("#zjs2").removeAttr("disabled");
                    $("#zjs3").removeAttr("disabled");
                    $("#zjs4").removeAttr("disabled");
                    break;
    			default:
    				break;
    		}
    	}
	</script>

</head>
<body>
    <form id="form1" runat="server">
    <table style="padding:5px; line-height:30px;">
		<tr>
			<td><input type="radio" id="rbPrice" name="rbChoose" checked="checked" onclick="fnShowChoose(this)" /></td>
			<td>金额</td>
			<td></td>
		</tr>
		<tr>
			<td></td>
			<td colspan="2"><input type="text" id="price" onblur="fnShowPriceResult()" /></td>
		</tr>
		<tr>
			<td><input type="radio" id="rbEMS" name="rbChoose" onclick="fnShowChoose(this)" /></td>
			<td>邮政<span style="color:Red">(取整默认为向下取整)</span></td>
			<td></td>
		</tr>
		<tr>
			<td></td>
			<td colspan="2" id="txtEMS">
			
			</td>
		</tr>
		<tr>
			<td><input type="radio" id="rbZJS" name="rbChoose" onclick="fnShowChoose(this)" /></td>
			<td>宅急送<span style="color:Red">(取整默认为向下取整)</span></td>
			<td></td>
		</tr>
		<tr>
			<td></td>
			<td colspan="2" id="txtZJS"></td>
		</tr>
    </table>
    <div style=" text-align:right; padding-right:30px;">
		<input type="text" id="priceResult" readonly="readonly" size=60 />
		<input type="button" id="btnOK" value="确定" onclick="fnOK()" />
    </div>
    </form>
</body>
</html>
