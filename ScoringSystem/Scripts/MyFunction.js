
    var pagename;
    function GotoPage(sender, e) {
        var name = e["item"].name;
    pagename = name;
        location.href = '/' + name;
        
}



//让左边栏进入相应页面时，也选择对应的项目
function GetLeftNavBarItem(name) {
    return LeftNavBar.GetItemByName(name);
}

function GetLeftNavBarGroup(name) {
    return LeftNavBar.GetGroupByName(name);
}

function SetLeftNarBarItemSelect(group, item) {
    GetLeftNavBarGroup(group).SetExpanded(true);
    LeftNavBar.SetSelectedItem(GetLeftNavBarItem(item));
}


function GetDateEditValueToCFormat(DateEdit) {
    if (DateEdit.GetValue() != null) {
        var endtime = new Date(DateEdit.GetValue());

        var timestring = endtime.getFullYear() + "-" + (endtime.getMonth() + 1) + "-" + endtime.getDate() + " " + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds();

        return timestring;
    }
    else {
        return null;
    }
}