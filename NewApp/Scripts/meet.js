$(function () {
    FetchMeeting();
    $('#dialog').dialog({
        autoOpen: false,
        width: '350px',
        show: {
            effect: 'drop',
            duration: 500
        },
        hide: {
            effect: 'clip',
            duration: 500
        }
    })

    $("#pushOn").tinyToggle({
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

    $(".js-example-basic-multiple").select2({
        closeOnSelect: false,

        allowClear: true,
        backgroundColor: '#fff',

    });


    $('.datepicker').datetimepicker({
        dateFormat: 'dd-mm-yy',
        timeFormat: 'HH:mm'
    })

    addContact.onclick = function (event) {

        var name = "MeetId";
        var val = "0";
        document.cookie = encodeURIComponent(name) + '=' + encodeURIComponent(val);
       
        
        $('#title').val('');
        $('#start').val('');
        $('#place').val('');
        $("#description").val('');
        $("#participants").val('');
        $('#contacts').val(null).trigger('change');
        $('#pushOn').tinyToggle('uncheck')
        $('#dialog').dialog('open');
      
        getContacts();
    }

    searchLine.onkeyup = function (event) {
        if ($('#searchLine').val() == "") {
            FetchMeeting();
        }
    }
    searchButton.onclick = function (event) {
        if ($('#searchLine').val() == "") {
            FetchMeeting();
        }
        else {
            SearchMeeting($('#searchLine').val());
        }
    }

    btnSave.onclick = function (event) {
       
        if (($('#title').val().trim() == "") ) {
            $("#error").text("Опис зустрічі обов'язковий");
            return;
        }
        else if ($('#start').val().trim() == "") {
            $("#error").text("Вкажіть початок зустрічі");

            return;
        }
        else if ($('#place').val().trim() == "") {
            $("#error").text("Вкажіть місце зустрічі");

            return;
        }
        else if ($('#participants').val().trim() == "") {
            $("#error").text("Вкажіть учасників зустрічі");

            return;
        }
       
       
            var data = {

                MeetId: parseInt(GetCookie("MeetId"), 10),
                title: $('#title').val().trim(),
                dateM: $('#start').val().trim(),
                place: $('#place').val().trim(),
                participants: $('#participants').val().trim(),
                notes: $("#description").val().trim(),
                notif: (($('#pushOn').is(":checked") == true)? 1 : 0 )
        }
     
      SaveMeet(data);

        contList = $('.js-example-basic-multiple').val();
       
        var cont = "";
        if (contList != null) {
            $.each(contList, function (i, v) {
                cont = cont + v.toString() + "/";
            })
        }
        else {
            cont = "-1";
        }
       
        var contData = {
            forCont: cont.toString(),
        }
        SaveContact(contData);
            $('#dialog').dialog('close');
    }


    function SaveContact(data) {

        $.ajax({
            type: "POST",
            url: '/Meeting/SaveContacts',
            data: data,
            success: function (data) {
                if (data.status) {
                    FetchMeeting();
             }
            },
            error: function () {
                alert('Failed');
            }
        })
    }

    
})
function getContacts() {
    $.ajax({
        type: "GET",
        url: 'http://localhost:52396/Meeting/getContList',
    
        success: function (data) {
            $.each(data, function (i, v) {


                if ($('#contacts').find("option[value='" + v.ContId + "']").length) {
                    // $('#contacts').val(v.ContId).trigger('change');
                } else {
                    // Create a DOM Option and pre-select by default
                    var newOption = new Option(v.surname, v.ContId, false, false);
                    $('#contacts').append(newOption).trigger('change');
                }
            })
        },
        error: function () {
            alert('Failed');
        }
    })
}
function setSelectedCont(id) {
    $('#contacts').val(null).trigger('change');
    $.ajax({
        type: "GET",
        url: 'http://localhost:52396/Meeting/getSelected',
        data: { 'MeetId': id },
        success: function (data) {
            $.each(data, function (i, v) {

                data.push(v.ContId);
            })

            $('#contacts').val(data);
            $('#contacts').trigger('change');
        },
        error: function () {
            alert('Failed');
        }
    })

}
function SaveMeet(data) {
    $.ajax({
        type: "POST",
        url: '/Meeting/SaveMeeting',
        data: data,
        success: function (data) {
            if (data.status) {
                FetchMeeting();
            }
        },
        error: function () {
            alert('Failed');
        }
    })
}

function FetchMeeting() {
    $.ajax({
        url: 'http://localhost:52396/Meeting/listMeet',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html'
    })
        .success(function (result) {
            $('#part').html(result);

        })
        .error(function (xhr, status) {
            alert('error');
        })
}
function red(data) {

    var name = "MeetId";
    var val = data.toString();
    document.cookie = encodeURIComponent(name) + '=' + encodeURIComponent(val);
    getMeeting(data);
    getContacts();
    setSelectedCont(data);
    $('#dialog').dialog('open');

}
function del(data) {

    var name = "MeetId";
    var val = data.toString();
    document.cookie = encodeURIComponent(name) + '=' + encodeURIComponent(val);
    delMeeting();
    FetchMeeting();
}

function delMeeting() {
    var id = parseInt(GetCookie("MeetId"), 10);

    if (parseInt(GetCookie("MeetId"), 10) != 0 && confirm('Видалити цю зустріч?')) {
        $.ajax({
            type: "POST",
            url: '/Meeting/DeleteMeeting',
            data: { 'MeetId': id },
            success: function (data) {
                if (data.status) {
                    FetchMeeting();
                    
                }

            },
            error: function () {
                alert('Failed');
            }
        })
    } else {
       
        $('#title').val(''),
        $('#start').val(''),
        $('#place').val(''),
        $('#participants').val(''),
        $("#description").val(''),
        $('#contacts').val(null).trigger('change');
        $('#pushOn').tinyToggle("uncheck");
    }
}

function getMeeting(id) {
    
    cont = [];
    $.ajax({
        type: "GET",
        url: 'http://localhost:52396/Meeting/getMeeting',
        data: { 'MeetId': id },
        success: function (data) {
           
            $.each(data, function (i, v) {
                
                     $('#title').val(v.title),
                    $('#start').val(v.dateM),
                    $('#place').val(v.place),
                    $('#participants').val(v.participants),
                    $("#description").val(v.notes)
                $('#pushOn').tinyToggle(v.notif? "check":"uncheck")
            })
            $("#error").text('');
            $('#dialog').dialog('open');

        },
        error: function () {
            alert('Failed');
        }
    })
    } 


function GetCookie(name) {

    var cookie = " " + document.cookie;
    var search = " " + name + "=";
    var setStr = null;
    var offset = 0;
    var end = 0;
    if (cookie.length > 0) {
        offset = cookie.indexOf(search);
        if (offset != -1) {
            offset += search.length;
            end = cookie.indexOf(";", offset)
            if (end == -1) {
                end = cookie.length;
            }
            setStr = unescape(cookie.substring(offset, end));
        }
    }

    return (setStr);
}

function SearchMeeting(data) {
    $.ajax({
        url: 'http://localhost:52396/Meeting/searchMeeting',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        data: { 'searchResult': data },
        dataType: 'html'
    })
        .success(function (result) {
            $('#part').html(result);

        })
        .error(function (xhr, status) {
            alert('error');
        })


}
