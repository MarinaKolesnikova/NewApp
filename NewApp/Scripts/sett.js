$(function () {
    getSettings();
    getPush()
    $("#spinnerH").spinner({
        max: 10,
         min:0,
         });
    $("#spinnerM").spinner({
        max: 59,
        min: 0,
       
    });
    $("#spinnerH").spinner("value", 0);
    $("#spinnerM").spinner("value", 0);
    $("#pushOn").tinyToggle({
        type: "toggle",
        palette: "blue",
        size: "big", // you can also specify defined custom sizes Ex. "mysize"
        value: true,
        onReady: function () { /* do something... */ },
        onClick: function () {
           
            if ($("#pushOn").is(':checked')) {

                $('#box*').removeClass("disabledcont"); SetPush(1);
            }
            else { $('#box*').addClass("disabledcont"); SetPush(0);}
        },
        onChange: function () { },
        onCheck: function () { },
        onUncheck: function () {},
        onDisabled: function () { /* do something... */ },
        onEnabled: function () { /* do something... */ }
    }); 
    $("#allOn").tinyToggle({
        type: "toggle",
        palette: "blue",
        size: "big", // you can also specify defined custom sizes Ex. "mysize"
        value: true,
        onReady: function () { /* do something... */ },
        onClick: function () { /* do something */ },
        onChange: function () { /* do something... */ },
        onCheck: function () { /* do something... */ },
        onUncheck: function () { /* do something... */ },
        onDisabled: function () { /* do something... */ },
        onEnabled: function () { /* do something... */ }
    });
    $("#delAcc").click(function(){
        if ( confirm('Видалити цей акаунт?')) {
            $.ajax({
                type: "POST",
                url: '/Settings/deleteAcc',
                
                success: function (data) {
                    if (data.status) {
                        window.location = "http://localhost:52396/Account/Registration";
                    }

                },
                error: function () {
                    alert('Failed');
                }
            })
        } else {

            return
        }
    
    })
    $("#bSave").click(function () {
        if (emailCheck()==true) {
            
            var timer = $("#spinnerH").spinner("value") * 60 + $("#spinnerM").spinner("value");
           
            var data = {
                timer: timer,
                allM: (($('#allOn').is(":checked") == true) ? 1 : 0),
                email: $('#emailT').val().trim(),

            }
            SaveSettings(data);
           
            SetPush(($('#pushOn').is(":checked") == true) ? 1 : 0);
}
})
 ;
})
function SaveSettings(data) {
    
    $.ajax({
        type: "POST",
        url: '/Settings/setSettings',
        data: data,
        success: function (data) {
            if (data.status) {
                alert('Success!');
            }
        },
        error: function () {
            alert('Failed');
        }
    })
}

function SetPush(data) {
    $.ajax({
        type: "POST",
        url: '/Settings/setPush',
        data: { e: data },
        success: function (data) {
            if (data.status) {
                
            }
        },
        error: function () {
            alert('Failed');
        }
    })
}

function getSettings() {
   
    $.ajax({
        type: "GET",
        url: 'http://localhost:52396/Settings/getSettings',
        
        success: function (data) {
            $.each(data, function (i, v) {
                $('#emailT').val(v.email);
                $('#spinnerH').val(v.hour);
                $('#spinnerM').val(v.minute);
                $('#allOn').tinyToggle(v.allM==1 ? "check" : "uncheck")

            })
                  
        },
        error: function () {
            alert('Failed');
        }
    })

}

function getPush() {
   
    $.ajax({
        type: "GET",
        url: 'http://localhost:52396/Settings/getPush',

        success: function (data) {
          
            $('#pushOn').tinyToggle(data.pushState == 1 ? "check" : "uncheck");
                if(data.pushState==1) {
                $('#box*').removeClass("disabledcont");
            }
 
        },
        error: function () {
            alert('Failed');
        }
    })

}
function emailCheck() {
   
    var strmail = $('#emailT').val();
    if ((strmail.indexOf("@") < 0) || (strmail.indexOf(".") < 0) || (strmail.lastIndexOf(".") < strmail.indexOf("@"))) {
        alert("Введіть правильний e-mail!");
        return false;
    }
    return true;
}