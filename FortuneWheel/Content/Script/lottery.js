(function () {
    var startRotate = function () {
        $("#zp").rotate({
            angle: 0,
            animateTo: 360,
            callback: startRotate,
            easing: function (x, t, b, c, d) {
                // t: current time, b: begInnIng value, c: change In value, d: duration
                return c * (t / d) + b;
            }
        });
    }
    var setPosition = function (angle) {
        $("#zp").rotate({
            animateTo: 540 - angle,
            callback: function () {
                switch (angle) {
                    case 0:
                        alert("恭喜您中得了IPADMINI，我们的客服将在48小时内联系您沟通发奖事宜！");
                        break;
                    case 60:
                        alert("恭喜您获得10元话费，卡密将在48小时内发至您的手机！");
                        break;
                    case 120:
                        alert("恭喜您获得2元话费，卡密将在48小时内发至您的手机！");
                        break;
                    case 180:
                        alert("很遗憾，您没有中奖 T_T");
                        break;
                    case 240:
                        alert("恭喜您获得5元话费，卡密将在48小时内发至您的手机！");
                        break;
                    case 300:
                        alert("恭喜您获得1元话费，卡密将在48小时内发至您的手机！");
                        break;
                }
                $("#start").off("click", buttonStart);
                $("#start").on("click", buttonStart);
            },
            easing: function (x, t, b, c, d) {
                // t: current time, b: begInnIng value, c: change In value, d: duration
                return c * (t / d) + b;
            }
        });
    }
    var sleeps = function (millseconds) {
        var currentDate = new Date();
        while (new Date() - currentDate < millseconds) {

        }
    }
    var buttonStart = function () {
        $("#start").off("click", buttonStart);
        $.ajax({
            url: "/Lottery/Begin",
            data: { sPhoneNumber: getPhoneNumber() },
            dataType: "json",
            success: function (data) {
                $("#zp").stopRotate();
                refresh();
                if (data.error == "") {
                    startRotate();
                    sleep(data.result);
                }
                else {
                    alert(data.error);
                    $("#start").on("click", buttonStart);
                }
            }
        });
    }
    var refresh = function () {
        $.ajax({
            url: "/Lottery/Refresh",
            data: { sPhoneNumber: getPhoneNumber() },
            dataType: "json",
            success: function (data) {
                if (data.error == "") {
                    $("#num").empty().html(data.num);
                }
                else {
                    alert(data.error);
                }
            }
        });
    };

    var download = function () {
        $.ajax({
            url: "/Lottery/Download",
            data: { sPhoneNumber: getPhoneNumber() },
            dataType: "json",
            success: function (data) {
                if (data.error == "") {
                    $("#num").empty().html(data.num);
                }
                else {
                    alert(data.error);
                }
            }
        });
    };

    var sleep = function (result) {
        $.ajax({
            url: "/Lottery/Sleep",
            dataType: "xml",
            success: function (data) {
                setPosition(result);
            }
        });
    };
    var getPhoneNumber = function () {
        var query = location.search.toLowerCase();
//        var index = query.indexOf("phone");
        if (query.substring(1, 3) == "mo") {
            return query.substring(4, index + 15);
        }
        else if (query.substring(0, 5) == "phone") {
            return query.substring(6, index + 17);
        }
        else {
            return "";
        }
    };
    $(function () {
        $("#start").on("click", buttonStart);
        $("#refresh").on("click", refresh);
        refresh();
    });
})();