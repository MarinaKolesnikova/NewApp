

$(function () {
   FetchEvent();
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
    searchLine.onkeyup = function (event) {
        if ($('#searchLine').val() == "") {
            FetchEvent();
        }
        else {
            Search($('#searchLine').val());
        }
    }
    btnSave.onclick = function (event) {
        
        if (($('#contName').val().trim() == "") && ($('#contSurname').val().trim() == "")) {
            $("#error").text("Введіть ім'я чи прізвище");
            return;
        }
        else if (($('#contName').val().trim().length > 30)) {
            $("#error").text("Ім'я має бути менше за 30 символів");
            return
        }
        else if (($('#contSurname').val().trim().length > 30)) {
            $("#error").text("Прізвище має бути менше за 30 символів");
            return
        }
        else if ($('#phNumber').val().trim().length> 20) {
            $("#error").text("Номер телефону має бути до 20 символів");

            return;
        }
        else if ($('#phNumber').val().trim() == "") {
            $("#error").text("Номер телефону обов'язковий");

             return;
        }
        else {
            var data = {

                ContId: parseInt(getCookie("idContact"), 10),
                contName: $('#contName').val().trim(),
                contSurname: $('#contSurname').val().trim(),
                phNumber: $('#phNumber').val().trim(),

            }
           
            SaveContact(data);
            $('#dialog').dialog('close');
           // FetchEvent()
        }
    }

    btnDelete.onclick = function (event) {
           
        delContact();
    }


        addContact.onclick = function (event) {

            var name = "idContact";
            var val = "0";
            document.cookie = encodeURIComponent(name) + '=' + encodeURIComponent(val);

            $('#contName').val('');
            $('#contSurname').val('');
            $('#phNumber').val('');
            $("#error").text('');
        $('#dialog').dialog('open');
        }

    function SaveContact(data) {
        
        $.ajax({
            type: "POST",
            url: 'http://localhost:52396/Contact/SaveContact',
            data: data,
            success: function (data) {
                if (data.status) {
                    //Refresh the calender
                    FetchEvent();
                   
                                 }
            },
            error: function () {
                alert('Failed');
            }
        })
    }

})

function FetchEvent() {
    $.ajax({
        url: 'http://localhost:52396/Contact/listCont',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        dataType: 'html'
    })
        .success(function (result) {
            $('#flo').html(result);

        })
        .error(function (xhr, status) {
            alert('error');
        })


}

function Search(data) {
    $.ajax({
        url: 'http://localhost:52396/Contact/searchCont',
        contentType: 'application/html; charset=utf-8',
        type: 'GET',
        data: {'searchResult': data },
        dataType: 'html'
    })
        .success(function (result) {
            $('#flo').html(result);

        })
        .error(function (xhr, status) {
            alert('error');
        })


}


function getCookie(name) {

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

function red(data) {
   
   
    var name = "idContact";
    var val = data.toString();
    document.cookie = encodeURIComponent(name) + '=' + encodeURIComponent(val); 
    getContact(data)

}
function del(data) {
   
    var name = "idContact";
    var val = data.toString();
    document.cookie = encodeURIComponent(name) + '=' + encodeURIComponent(val);
    delContact();
}

function delContact() {
   
    if (parseInt(getCookie("idContact"), 10) != 0 && confirm('Видалити цей контакт?')) {
        $.ajax({
            type: "POST",
            url: '/Contact/DeleteContact',
            data: { 'ContId': parseInt(getCookie('idContact'), 10) },
            success: function (data) {
                if (data.status) {
                    FetchEvent();
                    $('#dialog').dialog('close');
                }
               
            },
            error: function () {
                alert('Failed');
            }
        })
    } else {
        $('#contName').val('');
        $('#contSurname').val('');
        $('#phNumber').val('');
   
    }
}

function getContact(id) {
    
    cont = [];
    $.ajax({
        type: "GET",
        url: 'http://localhost:52396/Contact/getContact',
        data: { 'ContId': id },
        success: function (data) {

            $.each(data, function (i, v) {
                $('#contName').val(v.name);
                $('#contSurname').val(v.surname);
                $('#phNumber').val(v.phNumber);
            })
        },
        error: function () {
            alert('Failed');
        }
    })
    $("#error").text('');
    $('#dialog').dialog('open');
} 

