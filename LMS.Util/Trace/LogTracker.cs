using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RFD.FMS.Util.Trace
{
    public class LogTracker
    {
        #region Property
        private static List<TrackModel> _trackModels = new List<TrackModel>();

        private const string LogFileHead =
@"<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml'>
<head><title>
</title>
<script type='text/javascript'>
function $() {
    var elements = new Array();
    
    // Find all the elements supplied as arguments
    for (var i = 0; i < arguments.length; i++) {
        var element = arguments[i];
        
        // If the argument is a string assume it's an id
        if (typeof element == 'string') {
            element = document.getElementById(element);
        }
        
        // If only one argument was supplied, return the element immediately
        if (arguments.length == 1) {
            return element;
        }
        
        // Otherwise add it to the array
        elements.push(element);
    }
    
    // Return the array of multiple requested elements
    return elements;
};

function getElementsByClassName(className, tag, parent){
    parent = parent || document;
    if(!(parent = $(parent))) return false;
    
    // Locate all the matching tags
    var allTags = (tag == '*' && parent.all) ? parent.all : parent.getElementsByTagName(tag);
    var matchingElements = new Array();
    
    // Create a regular expression to determine if the className is correct
    className = className.replace(/\-/g, '\\-');
    var regex = new RegExp('(^|\\s)' + className + '(\\s|$)');
    
    var element;
    // Check each element
    for(var i=0; i<allTags.length; i++){
        element = allTags[i];
        if(regex.test(element.className)){
            matchingElements.push(element);
        }
    }
    
    // Return any matching elements
    return matchingElements;
};

function getClassNames(element) {
    if(!(element = $(element))) return false;
    // Replace multiple spaces with one space and then
    // split the classname on spaces
    return element.className.replace(/\s+/,' ').split(' ');
};

function addClassName(element, className) {
    if(!(element = $(element))) return false;
    // Append the classname to the end of the current className
    // If there is no className, don't include the space
    element.className += (element.className ? ' ' : '') + className;
    return true;
};

function removeClassName(element, className) {
    if(!(element = $(element))) return false;
    var classes = getClassNames(element);
    var length = classes.length
    //loop through the array in reverse, deleting matching items
    // You loop in reverse as you're deleting items from 
    // the array which will shorten it.
    for (var i = length-1; i >= 0; i--) {
        if (classes[i] === className) { delete(classes[i]); }
    }
    element.className = classes.join(' ');
    return (length == classes.length ? false : true);
};

var OkDisplay = true;
var ErrorDisplay = true;
var FailDisplay = true;
var SuccessDisplay = true;
var WarningDisplay = true;

function switchDisplay(levelCode)
{
	try
	{
		var diplay = eval(levelCode +'Display');
		if(diplay)
		{
			var ems = getElementsByClassName(levelCode,'tr');
			for(var i=0;i<ems.length;i++)
			{
				var em=ems[i];
				addClassName(em,'HiddenMe');
			}
			 eval(levelCode +'Display=false;');
		}
		else
		{
			var ems = getElementsByClassName(levelCode,'tr');
			for(var i=0;i<ems.length;i++)
			{
				var em=ems[i];
				removeClassName(em,'HiddenMe');
			}
			eval(levelCode +'Display=true;');
		}
	}
	catch (e)
	{
	}
};
</script>
<style type='text/css'>
.Ok
{
    background-color:#ccff99
}
.Error
{
    background-color:#ff9966
}
.Fail
{
    background-color:#ffcc00
}
.Success
{
    background-color:#00ff00
}
.Warning
{
    background-color:#ffff33
}
.HiddenMe
{
	display:none;
}


</style>
</head>
<body>
<input type='button' value='Ok' onclick='switchDisplay(this.value);'/>
<input type='button' value='Error' onclick='switchDisplay(this.value);'/>
<input type='button' value='Fail' onclick='switchDisplay(this.value);'/>
<input type='button' value='Success' onclick='switchDisplay(this.value);'/>
<input type='button' value='Warning' onclick='switchDisplay(this.value);'/><table>
";

        private const string LogFileFoot = @"</table></body></html>";

        private const string LogFileBodyTemplate = @"<tr class='{1}'><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>";

        #endregion

        #region Method

        public static void Add(TrackModel trackModel)
        {
            _trackModels.Add(trackModel);
        }

        public static void Flush(string filePath, string fileName)
        {

            try
            {
                if (_trackModels.Count == 0)
                    return;
                var fullFilePath = Path.Combine(filePath, fileName);
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);
                var offset = File.Exists(fullFilePath) && new FileInfo(fullFilePath).Length > LogFileFoot.ToArray().Length ? -1 * (LogFileFoot.ToArray().Length + 1) : 0;
                var fileStr = new StringBuilder();
                if (!File.Exists(fullFilePath))
                    fileStr.AppendLine(LogFileHead);
                _trackModels.ForEach(item => fileStr.AppendLine(string.Format(LogFileBodyTemplate,
                                                                              !item.TrackTime.HasValue ? DateTime.Now.ToString() : item.TrackTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                              EnumHelper.GetDescription(item.Level),
                                                                              item.Name,
                                                                              item.Detail,
                                                                              item.Remark)));
                fileStr.Append(LogFileFoot);
                _trackModels.Clear();
                var fs = new FileStream(fullFilePath, FileMode.OpenOrCreate, FileAccess.Write);
                var mStreamWriter = new StreamWriter(fs);
                mStreamWriter.BaseStream.Seek(offset, SeekOrigin.End);
                mStreamWriter.Write(fileStr.ToString());
                mStreamWriter.Flush();
                mStreamWriter.Close();
                fs.Close();
            }
            catch { }
            finally
            {
                _trackModels.Clear();
            }
        }

        #endregion
    }
}
